using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl.WPF;

namespace WpfAppMapp
{
    public partial class MainWindow : Window
    {
        public string filePath = "C:\\Users\\Student13\\source\\repos\\WpfAppMapp\\WpfAppMapp\\pushpins.txt"; 
        private List<Location> pushpinLocations = new List<Location>(); 

        public MainWindow()
        {
            InitializeComponent();
            myMap.MouseDoubleClick += MyMap_MouseDoubleClick;
            btnRemovePushpins.Click += BtnRemovePushpins_Click;
            Loaded += MainWindow_Loaded; 
            Closing += MainWindow_Closing;
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
            // Сохраняем метки при закрытии окна приложения
            SavePushpinLocationsToFile();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Загружаем метки при запуске приложения
            LoadPushpinLocationsFromFile();
        }

        private void LoadPushpinLocationsFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // Clear existing pushpins
                    myMap.Children.Clear();

                    // Read all lines from the file
                    string[] lines = File.ReadAllLines(filePath);

                    // Iterate through each line
                    foreach (string line in lines)
                    {
                        // Split the line by comma to get latitude and longitude
                        string[] parts = line.Split(';');

                        // Ensure that the line has valid latitude and longitude
                        if (parts.Length == 2 && double.TryParse(parts[0], out double latitude) && double.TryParse(parts[1], out double longitude))
                        {
                            Location location = new(latitude, longitude);
                            pushpinLocations.Add(location);

                            Pushpin pin = new();
                            pin.Location = location;
                            myMap.Children.Add(pin);
                        }
                    }

                    MessageBox.Show($"Loaded {pushpinLocations.Count} pushpin locations from file.");
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

                MessageBox.Show($"Saved {pushpinLocations.Count} pushpin locations to file.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving pushpin locations: {ex.Message}");
            }
        }
    }
}