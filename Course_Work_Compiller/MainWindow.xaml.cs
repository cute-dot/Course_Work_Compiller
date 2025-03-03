using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace Course_Work_Compiller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextEditor.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(RichTextBox_DragOver), true);
            TextEditor.AddHandler(RichTextBox.DropEvent, new DragEventHandler(RichTextBox_Drop), true);
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
            TextEditor.Document.Blocks.Clear();;
            ResultsArea.Clear();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);

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

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);

                    if (Path.GetExtension(saveFileDialog.FileName).ToLower() == ".rtf")
                    {
                        range.Save(fileStream, DataFormats.Rtf);
                    }
                    else
                    {
                        range.Save(fileStream, DataFormats.Text);
                    }
                }
            }
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
            ResultsArea.Text = "Синтаксический анализ завершен."; // Здесь можно добавить логику анализа текста
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Window helpWindow = new Window
            {
                Title = "Справка",
                Width = 600,
                Height = 400,
                Content = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = "Меню\n\nМеню приложения содержит следующие разделы:\n\nФайл:\n\nСоздать – создать новый файл.\n\nОткрыть – открыть существующий файл.\n\nСохранить – сохранить текущий файл.\n\nВыход – закрыть приложение.\n\nПравка:\n\nОтменить – отменить последнее действие.\n\nПовторить – повторить отменённое действие.\n\nКопировать – скопировать выделенный текст.\n\nВырезать – вырезать выделенный текст.\n\nВставить – вставить скопированный текст.\n\nИнструменты:\n\nАнализ текста – выполнить анализ текста в редакторе.\n\nСправка:\n\nРуководство пользователя – открыть данное руководство.\n\nО программе – информация о разработчике и версии приложения.\n\nПанель инструментов\n\nПанель инструментов дублирует основные функции меню в виде кнопок с иконками:\n\nСоздать (карандаш)\n\nОткрыть (папка)\n\nСохранить (дискета)\n\nОтменить (стрелка назад)\n\nПовторить (стрелка вперёд)\n\nКопировать (документ)\n\nВырезать (ножницы)\n\nВставить (документ с текстом)\n\nАнализ текста (лупа)\n\n4. Использование\n\nОткрытие файла:\n\nВыберите \"Файл\" \u2192 \"Открыть\" или нажмите кнопку \"Открыть\" на панели инструментов.\n\nВыберите файл в проводнике и нажмите \"Открыть\".\n\nРедактирование текста:\n\nВнесите изменения в текстовом поле.\n\nИспользуйте функции \"Правка\" для редактирования.\n\nСохранение файла:\n\nВыберите \"Файл\" \u2192 \"Сохранить\" или нажмите кнопку \"Сохранить\".\n\nАнализ текста:\n\nНажмите \"Инструменты\" \u2192 \"Анализ текста\".\n\nРезультаты анализа отобразятся в нижней части окна.\n\nЗавершение работы\n\nДля выхода из приложения выберите \"Файл\" \u2192 \"Выход\" или закройте окно.\n\n",
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            };
            helpWindow.Show();

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
                    ResultsArea.FontSize = selectedFontSize;
                    LineNumbers.FontSize = selectedFontSize;
                }
            }
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Текстовый редактор версии 1.0", "О программе");
        }
        
        
        private void InputFirst_TextChanged(object sender, TextChangedEventArgs e)
        {
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
            this.Close();
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

    }
}