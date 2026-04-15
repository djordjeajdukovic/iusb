
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ElectricVehicleCms.Helpers;
using ElectricVehicleCms.Models;
using ElectricVehicleCms.Pages;

namespace ElectricVehicleCms
{
    public partial class MainWindow : Window
    {
        private readonly DataIO _dataIO;

        public ObservableCollection<User> Users { get; set; }

        public ObservableCollection<ElectricVehicle> Vehicles { get; set; }

        public User CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _dataIO = new DataIO();

            PathHelper.EnsureDataFolders();
            LoadData();
            NavigateToLoginPage();
        }

        public string UsersFilePath
        {
            get
            {
                return PathHelper.GetAbsolutePath(System.IO.Path.Combine("Data", "users.xml"));
            }
        }

        public string VehiclesFilePath
        {
            get
            {
                return PathHelper.GetAbsolutePath(System.IO.Path.Combine("Data", "vehicles.xml"));
            }
        }

        private void LoadData()
        {
            Users = _dataIO.DeSerializeObject<ObservableCollection<User>>(UsersFilePath);
            Vehicles = _dataIO.DeSerializeObject<ObservableCollection<ElectricVehicle>>(VehiclesFilePath);

            if (Users == null)
            {
                Users = new ObservableCollection<User>();
            }

            if (Vehicles == null)
            {
                Vehicles = new ObservableCollection<ElectricVehicle>();
            }
        }

        public void SaveUsers()
        {
            _dataIO.SerializeObject(Users, UsersFilePath);
        }

        public void SaveVehicles()
        {
            _dataIO.SerializeObject(Vehicles, VehiclesFilePath);
        }

        public void NavigateToLoginPage()
        {
            MainFrame.Navigate(new LoginPage());
        }

        public void NavigateToVehiclesPage()
        {
            MainFrame.Navigate(new VehiclesPage());
        }

        public void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
        }

        private void HeaderDockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
