using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
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
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentFilePath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Открыть текстовый файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    currentFilePath = openFileDialog.FileName;
                    string fileContent = File.ReadAllText(currentFilePath);
                    InputRichTextBox.Document.Blocks.Clear();
                    InputRichTextBox.AppendText(fileContent);
                    TextBlock.Text = $"Открыт файл: {System.IO.Path.GetFileName(currentFilePath)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при открытии файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void CreateFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Создать новый файл"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;

                try
                {
                    File.WriteAllText(currentFilePath, string.Empty);
                    InputRichTextBox.Document.Blocks.Clear();
                    TextBlock.Text = $"Создан файл: {System.IO.Path.GetFileName(currentFilePath)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    Title = "Сохранить файл как"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    currentFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                string fileContent = new TextRange(InputRichTextBox.Document.ContentStart, InputRichTextBox.Document.ContentEnd).Text;
                File.WriteAllText(currentFilePath, fileContent);
                MessageBox.Show("Файл сохранён успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InputRichTextBox == null)
                {
                    MessageBox.Show("RichTextBox не найден.");
                    return;
                }

                if (InputRichTextBox.CanUndo)
                {
                    InputRichTextBox.Undo();
                }
                else
                {
                    MessageBox.Show("Нет изменений для отмены.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InputRichTextBox == null)
                {
                    MessageBox.Show("RichTextBox не найден.");
                    return;
                }

                if (InputRichTextBox.CanRedo)
                {
                    InputRichTextBox.Redo();
                }
                else
                {
                    MessageBox.Show("Нет изменений для повтора.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }
        public void Cut(object sender, RoutedEventArgs e)
        {
            if (InputRichTextBox.Selection.IsEmpty)
                return;

            TextRange selectedText = new TextRange(InputRichTextBox.Selection.Start, InputRichTextBox.Selection.End);
            Clipboard.SetText(selectedText.Text);
            selectedText.Text = string.Empty;
        }
        public void Copy(object sender, RoutedEventArgs e)
        {
            if (InputRichTextBox.Selection.IsEmpty)
                return;

            TextRange selectedText = new TextRange(InputRichTextBox.Selection.Start, InputRichTextBox.Selection.End);
            Clipboard.SetText(selectedText.Text);
        }

        public void Paste(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                InputRichTextBox.AppendText(clipboardText);
            }
        }

        public void EnlargeFont(object sender, RoutedEventArgs e)
        {
            InputRichTextBox.FontSize +=1;
            OutputRichTextBox.FontSize +=1;
        }

        public void ReduceFont(object sender, RoutedEventArgs e)
        {
            InputRichTextBox.FontSize -=1;
            OutputRichTextBox.FontSize -=1;
        }

        public void Delete(object sender, RoutedEventArgs e)
        {
            if (InputRichTextBox.Selection.IsEmpty)
                return;

            TextRange selectedText = new TextRange(InputRichTextBox.Selection.Start, InputRichTextBox.Selection.End);
            selectedText.Text = string.Empty;
        }

        public void SelectAll(object sender, RoutedEventArgs e)
        {
            InputRichTextBox.SelectAll();
            InputRichTextBox.Focus();
        }

        public void HelpButton(object sender, RoutedEventArgs e)
        {
            Window HelpWindow = new Window()
            {
                Title = "Справка",
                Width = 570,
                Height = 170,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode=ResizeMode.NoResize
            };
            TextBlock instructionText = new TextBlock()
            {
                Text = "Инструкция по работе с программой:\n\n" +
                "1. Для создания нового документа нажмите 'Создать' в меню 'Файл'.\n" +
                "2. Для открытия существующего документа выберите 'Открыть'.\n" +
                "3. Используйте 'Сохранить' для сохранения изменений.\n" +
                "4. Для редактирования текста используйте функции 'Вырезать', 'Копировать', 'Вставить' и 'Удалить'.\n" +
                "5. Вы можете отменить или повторить действия с помощью 'Отменить' и 'Повторить'.\n" +
                "6. Для получения справки нажмите 'Справка' в меню."
            };
            HelpWindow.Content = instructionText;
            HelpWindow.ShowDialog();
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public void ShowAbout(object sender, RoutedEventArgs e)
        {
            Window AboutWindow = new Window()
            {
                Title = "О программе",
                Width = 350,
                Height = 90,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            TextBlock aboutText = new TextBlock
            {
                Text = "Программа версии 1.0\n\n" +
                "Разработано студентом АВТ-214 Зотовым Дмитрием\n",
                TextAlignment = TextAlignment.Center
            };
            AboutWindow.Content = aboutText;
            AboutWindow.ShowDialog();
        }

        public void ParseText()
        {
            List<string>
        }
    }
}