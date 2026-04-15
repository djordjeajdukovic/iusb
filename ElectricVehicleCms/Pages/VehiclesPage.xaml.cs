
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ElectricVehicleCms.Models;

namespace ElectricVehicleCms.Pages
{
    public partial class VehiclesPage : Page
    {
        private readonly MainWindow _mainWindow;

        public ObservableCollection<ElectricVehicle> Vehicles { get; set; }

        public VehiclesPage()
        {
            InitializeComponent();

            _mainWindow = (MainWindow)Application.Current.MainWindow;
            Vehicles = _mainWindow.Vehicles;

            DataContext = this;

            bool isAdmin = _mainWindow.CurrentUser != null && _mainWindow.CurrentUser.Role == UserRole.Admin;

            CurrentRoleTextBlock.Text = "Logged in as: " + _mainWindow.CurrentUser.Role.ToString();

            AddVehicleButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            DeleteSelectedButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            SelectAllCheckBox.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            SelectColumn.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateToPage(new AddEditVehiclePage());
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            List<ElectricVehicle> selectedVehicles = Vehicles.Where(vehicle => vehicle.IsSelected).ToList();

            if (selectedVehicles.Count == 0)
            {
                MessageBox.Show("Select at least one row for deletion.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected vehicles?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            foreach (ElectricVehicle vehicle in selectedVehicles)
            {
                string rtfAbsolutePath = Helpers.PathHelper.GetAbsolutePath(vehicle.RtfPath);

                if (File.Exists(rtfAbsolutePath))
                {
                    File.Delete(rtfAbsolutePath);
                }

                Vehicles.Remove(vehicle);
            }

            SelectAllCheckBox.IsChecked = false;
            VehiclesDataGrid.Items.Refresh();

            _mainWindow.SaveVehicles();

            MessageBox.Show("Selected vehicles were deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.CurrentUser = null;
            _mainWindow.NavigateToLoginPage();
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (ElectricVehicle vehicle in Vehicles)
            {
                vehicle.IsSelected = true;
            }

            VehiclesDataGrid.Items.Refresh();
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (ElectricVehicle vehicle in Vehicles)
            {
                vehicle.IsSelected = false;
            }

            VehiclesDataGrid.Items.Refresh();
        }

        private void VehicleHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ElectricVehicle selectedVehicle = Vehicles.FirstOrDefault(vehicle => vehicle.Name == e.Uri.OriginalString);

            if (selectedVehicle == null)
            {
                return;
            }

            if (_mainWindow.CurrentUser.Role == UserRole.Admin)
            {
                _mainWindow.NavigateToPage(new AddEditVehiclePage(selectedVehicle));
            }
            else
            {
                _mainWindow.NavigateToPage(new VehicleDetailsPage(selectedVehicle));
            }

            e.Handled = true;
        }
    }
}
