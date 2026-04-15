
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ElectricVehicleCms.Helpers;
using ElectricVehicleCms.Models;

namespace ElectricVehicleCms.Pages
{
    public partial class VehicleDetailsPage : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly ElectricVehicle _vehicle;

        public VehicleDetailsPage(ElectricVehicle vehicle)
        {
            InitializeComponent();

            _mainWindow = (MainWindow)Application.Current.MainWindow;
            _vehicle = vehicle;

            LoadVehicleData();
        }

        private void LoadVehicleData()
        {
            VehicleNameTextBlock.Text = _vehicle.Name;
            ManufacturerTextBlock.Text = "Manufacturer: " + _vehicle.Manufacturer;
            RangeTextBlock.Text = "Range: " + _vehicle.RangeKm + " km";
            DateAddedTextBlock.Text = "Date added: " + _vehicle.DateAdded.ToString("yyyy-MM-dd HH:mm");

            string absoluteImagePath = PathHelper.GetAbsolutePath(_vehicle.ImagePath);

            if (!string.IsNullOrWhiteSpace(absoluteImagePath))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(absoluteImagePath, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                VehicleImage.Source = bitmapImage;
            }

            DescriptionViewer.Document = RtfHelper.CreateFlowDocumentFromRtf(_vehicle.RtfPath);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateToVehiclesPage();
        }
    }
}
