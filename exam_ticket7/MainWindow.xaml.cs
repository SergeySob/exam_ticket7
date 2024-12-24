using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace exam_ticket7
{
    public partial class MainWindow : Window
    {
        private List<string> _imagePaths; // Список путей к изображениям

        public MainWindow()
        {
            InitializeComponent();
            _imagePaths = new List<string>(); // Инициализация списка
        }

        // Открытие папки через диалоговое окно
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string folderPath = dialog.SelectedPath;
                    LoadImagesFromFolder(folderPath);
                }
            }
        }

        // Открытие папки по указанному пути
        private void OpenFolderByPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = FolderPathTextBox.Text;
            if (Directory.Exists(folderPath))
            {
                LoadImagesFromFolder(folderPath);
            }
            else
            {
                MessageBox.Show("Путь не существует!");
            }
        }

        // Загрузка изображений из указанной папки
        private void LoadImagesFromFolder(string folderPath)
        {
            var extensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
            _imagePaths = Directory.GetFiles(folderPath)
                                   .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
                                   .ToList();

            UpdateFileList();
        }

        // Загрузка путей из текстового файла
        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Выберите файл с названиями изображений"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string[] lines = File.ReadAllLines(openFileDialog.FileName);
                string folderPath = Path.GetDirectoryName(openFileDialog.FileName) ?? string.Empty;

                _imagePaths = lines.Select(line => Path.Combine(folderPath, line))
                                   .Where(File.Exists)
                                   .ToList();

                UpdateFileList();
            }
        }

        // Сохранение имен открытых файлов в текстовый документ
        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Сохранить список файлов"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveFileDialog.FileName, _imagePaths.Select(Path.GetFileName));
                MessageBox.Show("Список файлов сохранен!");
            }
        }

        // Обновление списка файлов
        private void UpdateFileList()
        {
            FileList.ItemsSource = _imagePaths.Select(Path.GetFileName); // Отображение только имен файлов
            FileList.SelectedIndex = -1;
            ImageViewer.Source = null; // Очистка изображения
        }

        // Обработка выбора файла из списка
        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileList.SelectedIndex >= 0)
            {
                string selectedFile = _imagePaths[FileList.SelectedIndex];
                DisplayImage(selectedFile);
            }
        }

        // Отображение выбранного изображения
        private void DisplayImage(string filePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                ImageViewer.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
            }
        }

        // Удаление выбранного элемента из списка (не из папки)
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (FileList.SelectedIndex >= 0)
            {
                _imagePaths.RemoveAt(FileList.SelectedIndex);
                UpdateFileList(); // Обновляем список после удаления
            }
        }
    }
}
