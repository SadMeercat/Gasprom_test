using Microsoft.Win32;
using System;
using System.Data;
//using System.IO;
//using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx|CSV Files|*.csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                DataTable dataTable = null;

                if (filePath != null)
                {

                    Importer importer = new Importer();

                    if (System.IO.Path.GetExtension(filePath).ToLower() == ".csv")
                    {
                        dataTable = importer.ImportCSV(filePath);
                    }
                    else if (System.IO.Path.GetExtension(filePath).ToLower() == ".xls" || System.IO.Path.GetExtension(filePath).ToLower() == ".xlsx")
                    {
                        dataTable = importer.ImportExcel(filePath);
                    }

                    if (dataTable != null)
                    {
                        DataGrid.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить файл");
                    }
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(DataGrid.SelectedItem is DataRowView row)
            {
                UpdateDetailsPanel(row);
                UpdateGraphicsCanvas(row);
            }
        }

        private void UpdateDetailsPanel(DataRowView row)
        {
            NameText.Text = $"Название: {row["name"]}";
            CoordinatesText.Text = $"Координаты: {row["distance"]}; {row["angle"]}";
            SizeText.Text = $"Размер: {row["width"]}; {row["hegth"]}";
            DefectText.Text = $"Дефект: {row["isdefect"]}";
        }

        private void UpdateGraphicsCanvas(DataRowView row)
        {
            try
            {
                // Очистка Canvas
                GraphicsCanvas.Children.Clear();

                // Получение данных из строки
                double x = Convert.ToDouble(row["distance"]);
                double y = Convert.ToDouble(row["angle"]);
                double width = Convert.ToDouble(row["width"]);
                double height = Convert.ToDouble(row["hegth"]);
                string defect = row["isdefect"].ToString().ToLower();

                // Преобразование координат для Canvas
                double canvasWidth = GraphicsCanvas.ActualWidth;
                double canvasHeight = GraphicsCanvas.ActualHeight;

                double scaleX = canvasWidth / 20; // 20 м
                double scaleY = canvasHeight / 12; // 12 ч

                Rectangle rect = new Rectangle
                {
                    Width = width * scaleX,
                    Height = height * scaleY,
                    Stroke = defect == "да" ? Brushes.Red : Brushes.Green,
                    Fill = defect == "да" ? Brushes.Red : Brushes.Green,
                    Opacity = 0.5,
                    StrokeThickness = 2
                };

                // Установка позиции
                Canvas.SetLeft(rect, x * scaleX);
                Canvas.SetTop(rect, y * scaleY);

                // Добавление на Canvas
                GraphicsCanvas.Children.Add(rect);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении графики:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            // Завершаем редактирование и обновляем источник данных
            DataGrid.CommitEdit();

            // Получаем текущую выбранную строку после редактирования
            if (DataGrid.SelectedItem is DataRowView row)
            {
                // Обновляем панель с информацией
                UpdateDetailsPanel(row);

                // Обновляем графическое отображение
                UpdateGraphicsCanvas(row);
            }
        }
    }
}
