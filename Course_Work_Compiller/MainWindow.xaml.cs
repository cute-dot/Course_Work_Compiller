using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Course_Work_Compiller.models;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace Course_Work_Compiller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string _currentFilePath = null;
        private bool _isModified = false;
        private readonly TextManager textManager = new TextManager();
        
        private bool _isUpdating = false;
        public static readonly RoutedCommand OpenFileCommand = new RoutedCommand();
        public static readonly RoutedCommand SaveFileCommand = new RoutedCommand();
        public static readonly RoutedCommand AnalyzeTextCommand = new RoutedCommand();
        public MainWindow()
        {
            
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(OpenFileCommand, OpenFile_Click));
            CommandBindings.Add(new CommandBinding(SaveFileCommand, SaveFile_Click));
            CommandBindings.Add(new CommandBinding(AnalyzeTextCommand, AnalyzeText_Click));
            
            
            
            TextEditor.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(RichTextBox_DragOver), true);
            TextEditor.AddHandler(RichTextBox.DropEvent, new DragEventHandler(RichTextBox_Drop), true);
            TextEditor.TextChanged += TextEditor_TextChanged;
            
            InputBindings.Add(new KeyBinding(OpenFileCommand, Key.O, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(SaveFileCommand, Key.S, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(AnalyzeTextCommand, Key.F5, ModifierKeys.None));
            
            
        }
        
         private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return; // Предотвращаем повторный вызов

            _isUpdating = true;
            HighlightSyntax(TextEditor);
            _isUpdating = false;
            
            
        }

        private void HighlightSyntax(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            string text = textRange.Text;

            // Очищаем стили только один раз
            textRange.ClearAllProperties();

            // Вызываем подсветку
            HighlightWord(richTextBox, @"\b(fun|return)\b", Brushes.Blue);
            HighlightWord(richTextBox, @"\b(Int|Double|String|Boolean)\b", Brushes.DarkCyan);
            HighlightWord(richTextBox, @"\b\d+\b", Brushes.DarkOrange);
            HighlightWord(richTextBox, @"(\"".*?\"")", Brushes.Green);
        }

        private void HighlightWord(RichTextBox richTextBox, string pattern, Brush color)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            string text = textRange.Text;

            foreach (Match match in Regex.Matches(text, pattern))
            {
                TextPointer start = richTextBox.Document.ContentStart;
                TextPointer current = start;

                while (current != null)
                {
                    if (current.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        string runText = current.GetTextInRun(LogicalDirection.Forward);

                        int index = runText.IndexOf(match.Value, StringComparison.Ordinal);
                        if (index >= 0)
                        {
                            TextPointer selectionStart = current.GetPositionAtOffset(index) ?? throw new InvalidOperationException();
                            TextPointer selectionEnd = selectionStart?.GetPositionAtOffset(match.Value.Length) ?? throw new InvalidOperationException();

                            if (selectionStart != null && selectionEnd != null)
                            {
                                TextRange selection = new TextRange(selectionStart, selectionEnd);
                                selection.ApplyPropertyValue(TextElement.ForegroundProperty, color);
                            }
                        }
                    }
                    current = current.GetNextContextPosition(LogicalDirection.Forward);
                }
            }
        }
        
        private bool ConfirmSaveChanges()
        {
            Console.WriteLine(_isModified);
            if (_isModified == false) return true; // Если изменений нет, просто продолжаем

            // Показываем MessageBox
            MessageBoxResult result = MessageBox.Show(
                "Вы хотите сохранить изменения?",
                "Сохранение",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                SaveFile_Click(null, null); // Вызываем метод сохранения
                return true; // Разрешаем продолжение работы
            }
            else if (result == MessageBoxResult.No)
            {
                return true; // Просто продолжаем без сохранения
            }

            return false; // Отмена операции
        }
        
        
        private void RichTextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = false;
        }

        private void RichTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] docPath = (string[])e.Data.GetData(DataFormats.FileDrop);

                var dataFormat = DataFormats.Rtf;

                if (e.KeyStates == DragDropKeyStates.ShiftKey)
                {
                    dataFormat = DataFormats.Text;
                }

                System.Windows.Documents.TextRange range;
                System.IO.FileStream fStream;
                if (System.IO.File.Exists(docPath[0]))
                {
                    try
                    {
                        // Open the document in the RichTextBox.
                        range = new System.Windows.Documents.TextRange(TextEditor.Document.ContentStart,
                            TextEditor.Document.ContentEnd);
                        fStream = new System.IO.FileStream(docPath[0], System.IO.FileMode.OpenOrCreate);
                        range.Load(fStream, dataFormat);
                        fStream.Close();
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("File could not be opened. Make sure the file is a text file.");
                    }
                }
            }
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveChanges()) return;
            _isModified = false;
            TextEditor.Document.Blocks.Clear();;
            // ResultsArea.D;
        }
        public class SearchResult
        {
            public string Тип { get; set; }
            public string Результат { get; set; }
        }
        private void FindEmails_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd).Text;
            ResultArea.Items.Clear();
    
            Regex regex = new Regex(@"\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}\b");
    
            MatchCollection matches = regex.Matches(text);
    
            if (matches.Count == 0)
            {
                ResultArea.Items.Add(new SearchResult { 
                    Тип = "Email", 
                    Результат = "Адреса электронной почты не найдены" 
                });
                return;
            }
    
            foreach (Match match in matches)
            {
                ResultArea.Items.Add(new SearchResult { 
                    Тип = "Email", 
                    Результат = $"{match.Value} (позиция: {match.Index})" 
                });
            }
        }
        
        private void FindEmailsWithAutomaton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd).Text;
            ResultArea.Items.Clear();
            EmailAutomation emailAutomation = new EmailAutomation();
            var matches = emailAutomation.FindEmails(text);
        
            if (matches.Count == 0)
            {
                ResultArea.Items.Add(new SearchResult { 
                    Тип = "Email (автомат)", 
                    Результат = "Адреса электронной почты не найдены" 
                });
                return;
            }
        
            foreach (var match in matches)
            {
                ResultArea.Items.Add(new SearchResult { 
                    Тип = "Email (автомат)", 
                    Результат = $"{match.Value} (позиция: {match.Index})" 
                });
            }
        }
        
        
        private void FindHexNumbers_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd).Text;
            ResultArea.Items.Clear();
    
            Regex regex = new Regex(@"\b0[xX][0-9A-Fa-f]+\b");
    
            MatchCollection matches = regex.Matches(text);
    
            if (matches.Count == 0)
            {
                ResultArea.Items.Add(new SearchResult { 
                    Тип = "Hex", 
                    Результат = "Шестнадцатеричные числа не найдены" 
                });
                return;
            }
    
            foreach (Match match in matches)
            {
                ResultArea.Items.Add(new SearchResult { 
                    Тип = "Hex", 
                    Результат = $"{match.Value} (позиция: {match.Index})" 
                });
            }
        }
        
        private void FindPostalCodes_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd).Text;
            ResultArea.Items.Clear();
            Regex regex = new Regex(@"\b\d{3}[-\s]?\d{3}\b");
            MatchCollection matches = regex.Matches(text);
            
            if (matches.Count == 0)
            {
                ResultArea.Items.Add(new SearchResult { 
                    Тип = "Индекс", 
                    Результат = "Российские почтовые индексы не найдены" 
                });
                return;
            }
            foreach (Match match in matches)
            {
                ResultArea.Items.Add(new SearchResult { 
                    Тип = "Индекс", 
                    Результат = $"{match.Value} (позиция: {match.Index})" 
                });
            }
        }


        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveChanges()) return;
            _isModified = false;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.txt)|*.txt|All Files (*.*)|*.*"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
                    _currentFilePath = openFileDialog.FileName;
                    if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".rtf")
                    {
                        range.Load(fileStream, DataFormats.Rtf);
                    }
                    else
                    {
                        range.Load(fileStream, DataFormats.Text);
                    }
                }
            }
        }

        // private void SaveFile_Click(object sender, RoutedEventArgs e)
        // {
        //     SaveFileDialog saveFileDialog = new SaveFileDialog
        //     {
        //         Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        //     };
        //
        //     if (saveFileDialog.ShowDialog() == true)
        //     {
        //         using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
        //         {
        //             TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
        //
        //             if (Path.GetExtension(saveFileDialog.FileName).ToLower() == ".rtf")
        //             {
        //                 range.Save(fileStream, DataFormats.Rtf);
        //             }
        //             else
        //             {
        //                 range.Save(fileStream, DataFormats.Text);
        //             }
        //         }
        //     }
        // }
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveFileAs();
            }
            else
            {
                SaveToFile(_currentFilePath);
            }
        }

        // Метод "Сохранить как"
        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileAs();
        }

        // Открытие диалога "Сохранить как"
        private void SaveFileAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Сохранить как"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _currentFilePath = saveFileDialog.FileName;
                SaveToFile(_currentFilePath);
            }
        }

        // Метод сохранения содержимого RichTextBox в файл
        private void SaveToFile(string filePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
                    range.Save(fileStream, DataFormats.Text); // Сохранение в текстовом формате
                }
                _isModified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Отслеживание изменений в тексте
        private void textEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            _isModified = true;
        }
        


        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.Redo();
        }
        
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.Copy();
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.Cut();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.Paste();
        }

        private void AnalyzeText_Click(object sender, RoutedEventArgs e)
        {
            var lexer = new Lexer();
            TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
            try
            {
                var tokens = lexer.Analyze(range.Text);
                // ResultArea.ItemsSource = tokens;
                
               
                tokens.RemoveAll(token => token.Code == 7);
                foreach (var VARIABLE in tokens)
                {
                    Console.WriteLine(VARIABLE.Code);
                    Console.WriteLine(VARIABLE.Lexeme);
                }
                Parser parser = new Parser(tokens);
                try
                {
                    parser.Parse();
                    foreach (var VARIABLE in parser.Errors)
                    {
                        Console.WriteLine(VARIABLE);
                    }
                    ResultArea.ItemsSource = parser.Errors;
                    Console.WriteLine("Парсинг успешно завершен!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка парсинга: {ex.Message}");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            
            
            // ResultsArea.Ite = "Синтаксический анализ завершен."; // Здесь можно добавить логику анализа текста
        }
        
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            
            // string pdfFilePath = "руководство.pdf"; 
            //
            // // Получаем полный путь к файлу, используя текущую директорию программы
            // string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pdfFilePath);
            //
            // // Проверяем, существует ли файл
            // if (File.Exists(fullPath))
            // {
            //     try
            //     {
            //         // Открываем PDF файл с помощью ассоциированного приложения (например, браузера или PDF-просмотрщика)
            //         Process.Start(new ProcessStartInfo
            //         {
            //             FileName = fullPath,
            //             UseShellExecute = true // Используем систему для открытия файла
            //         });
            //     }
            //     catch (Exception ex)
            //     {
            //         // Если возникает ошибка при открытии, выводим сообщение
            //         MessageBox.Show($"Не удалось открыть файл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //     }
            // }
            // else
            // {
            //     // Если файл не найден
            //     MessageBox.Show("PDF файл не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            // }
        }
        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                // Получаем выбранный размер шрифта из ComboBox
                double selectedFontSize;
                if (double.TryParse(selectedItem.Content.ToString(), out selectedFontSize))
                {
                    // Изменяем размер шрифта в RichTextBox и TextBox
                    TextEditor.FontSize = selectedFontSize;
                    ResultArea.FontSize = selectedFontSize;
                    LineNumbers.FontSize = selectedFontSize;
                }
            }
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            if (ConfirmSaveChanges())
            {
                // Если пользователь выбрал "Да", закрываем приложение
                base.OnClosing(e);  // Это вызовет стандартное закрытие окна
            }
            else
            {
                // Если выбрал "Нет", отменяем закрытие
                e.Cancel = true;
            }
        }
        
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Эта программа была разработана Суриковым Александром из группы АВТ-214.", "О программе");
        }
        
        
        private void InputFirst_TextChanged(object sender, TextChangedEventArgs e)
        {
            _isModified = true;
            UpdateLineNumbers();
        }
        private void UpdateLineNumbers()
        {
            var textRange = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
            var text = textRange.Text;
            var lineCount = text.Split('\n').Length;

            LineNumbers.Text = string.Join("\n", Enumerable.Range(1, lineCount));
        }

        private void InputFirst_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            LineNumberScroller.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmSaveChanges()) 
            {
                this.Close();
            }
            
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string languageCode = selectedItem.Tag.ToString();
                ChangeLanguage(languageCode);
            }
        }
        private void ChangeLanguage(string languageCode)
        {
            ResourceDictionary dict = new ResourceDictionary();
    
            switch (languageCode)
            {
                case "en":
                    dict.Source = new Uri("ResourcesEN.xaml", UriKind.Relative);
                    break;
                case "ru":
                default:
                    dict.Source = new Uri("Resources.xaml", UriKind.Relative);
                    break;
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void TextFileOpen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                switch (menuItem.Header.ToString())
                {
                    case "Поставновка задачи":
                        textManager.OpenTaskStatement();
                        break;
                    case "Грамматика":
                        textManager.OpenGrammar();
                        break;
                    case "Классификация грамматики":
                        textManager.OpenGrammarClassification();
                        break;
                    case "Метод анализа":
                        textManager.OpenAnalysisMethod();
                        break;
                    case "Диагностика и нейтрализация ошибок":
                        textManager.OpenErrorDiagnostics();
                        break;
                    case "Тестовый пример":
                        textManager.OpenTestExample();
                        break;
                    case "Список литературы":
                        textManager.OpenBibliography();
                        break;
                    case "Исходный код программы":
                        textManager.OpenSourceCode();;
                        break;
                    default:
                        break;
                }
            }
        }
        
    }
    
}