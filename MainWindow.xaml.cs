using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Media.Imaging;

namespace WpfAppMapp
{
    public partial class MainWindow : Window
    {
        public string filePath = "C:\\Users\\Имя Пользователя\\source\\repos\\WpfAppMapp\\WpfAppMapp\\pushpins.txt";
        public string projectFolder = "C:\\Users\\Имя Пользователя\\source\\repos\\WpfAppMapp\\WpfAppMapp\\"; //Замените Имя пользователя на ваше.
        private List<Location> pushpinLocations = new List<Location>();

        public MainWindow()
        {
            InitializeComponent();
            myMap.MouseDoubleClick += MyMap_MouseDoubleClick;
            btnRemovePushpins.Click += BtnRemovePushpins_Click;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void BtnInstructions_Click(object sender, RoutedEventArgs e)
        {
            Window instructionsWindow = new Window();
            instructionsWindow.Title = "Руководство";
            instructionsWindow.Width = 1100;
            instructionsWindow.Height = 850;
            instructionsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            StackPanel stackPanel = new StackPanel();
            stackPanel.Margin = new Thickness(10);

            TextBlock textBlock1 = new TextBlock();
            textBlock1.Text = "Руководство:\n\n1. Для установки метки, нажмите на указанную вами точку два раза. ";
            textBlock1.TextWrapping = TextWrapping.Wrap;
            textBlock1.FontSize = 20; 
            textBlock1.FontWeight = FontWeights.Bold; 
            stackPanel.Children.Add(textBlock1);

            Image image1 = new Image();
            image1.Source = new BitmapImage(new Uri(projectFolder+"screen1.png"));
            image1.Margin = new Thickness(0, 10, 0, 0); 
            stackPanel.Children.Add(image1);

            TextBlock textBlock2 = new TextBlock();
            textBlock2.Text = "2. Нажмите 'Удалить метки' чтобы убрать все установленные метки. ";
            textBlock2.TextWrapping = TextWrapping.Wrap;
            textBlock2.FontSize = 16; 
            textBlock2.FontWeight = FontWeights.Bold; 
            stackPanel.Children.Add(textBlock2);

            Image image2 = new Image();
            image2.Source = new BitmapImage(new Uri(projectFolder+"screen2.png"));
            image2.Margin = new Thickness(0, 10, 0, 0); 
            stackPanel.Children.Add(image2);

            TextBlock textBlock3 = new TextBlock();
            textBlock3.Text = "3. Метки сохраняют свои позиции при закрытии приложения. ";
            textBlock3.TextWrapping = TextWrapping.Wrap;
            textBlock3.FontSize = 16; 
            textBlock3.FontWeight = FontWeights.Bold; 
            stackPanel.Children.Add(textBlock3);

            Image image3 = new Image();
            image3.Source = new BitmapImage(new Uri(projectFolder+"screen3.png"));
            image3.Margin = new Thickness(0, 10, 0, 0); 
            stackPanel.Children.Add(image3);

            Image image4 = new Image();
            image4.Source = new BitmapImage(new Uri(projectFolder+"screen4.png"));
            image4.Margin = new Thickness(0, 10, 0, 0); 
            stackPanel.Children.Add(image4);

            // Create a scroll viewer and set its content to the stack panel
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto; 
            scrollViewer.Content = stackPanel;

            // Set the scroll viewer as the content of the window
            instructionsWindow.Content = scrollViewer;

            instructionsWindow.ShowDialog();
        }

        private void MyMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(myMap);
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            pushpinLocations.Add(pinLocation);

            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;
            myMap.Children.Add(pin);
        }

        private void BtnRemovePushpins_Click(object sender, RoutedEventArgs e)
        {
            pushpinLocations.Clear();
            myMap.Children.Clear();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SavePushpinLocationsToFile();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPushpinLocationsFromFile();
        }

        private void LoadPushpinLocationsFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    myMap.Children.Clear();

                    string[] lines = File.ReadAllLines(filePath);

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(';');

                        if (parts.Length == 2 && double.TryParse(parts[0], out double latitude) && double.TryParse(parts[1], out double longitude))
                        {
                            Location location = new(latitude, longitude);
                            pushpinLocations.Add(location);

                            Pushpin pin = new();
                            pin.Location = location;
                            myMap.Children.Add(pin);
                        }
                    }

                    MessageBox.Show($"Загружено {pushpinLocations.Count} меток.");
                }
                else
                {
                    MessageBox.Show("Pushpins file does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pushpin locations: " + ex.Message);
            }
        }

        private void SavePushpinLocationsToFile()
        {
            try
            {
                Console.WriteLine($"Saving pushpin locations to file: {Path.GetFullPath(filePath)}");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (Location location in pushpinLocations)
                    {
                        writer.WriteLine($"{location.Latitude};{location.Longitude}");
                    }
                }

                MessageBox.Show($"Сохранено {pushpinLocations.Count} меток.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving pushpin locations: {ex.Message}");
            }
        }
    }
}
