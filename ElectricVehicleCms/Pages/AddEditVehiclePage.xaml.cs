
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ElectricVehicleCms.Helpers;
using ElectricVehicleCms.Models;

namespace ElectricVehicleCms.Pages
{
    public partial class AddEditVehiclePage : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly ElectricVehicle _vehicleForEdit;
        private readonly bool _isEditMode;
        private string _selectedImageSourcePath;

        public AddEditVehiclePage()
        {
            InitializeComponent();

            _mainWindow = (MainWindow)Application.Current.MainWindow;
            PrepareEditor();
            UpdateWordCount();
        }

        public AddEditVehiclePage(ElectricVehicle vehicle) : this()
        {
            _vehicleForEdit = vehicle;
            _isEditMode = true;

            PageTitleTextBlock.Text = "Edit electric vehicle";
            FillFormWithVehicleData();
        }

        private void PrepareEditor()
        {
            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(fontFamily => fontFamily.Source);

            FontSizeComboBox.ItemsSource = new List<int> { 10, 12, 14, 16, 18, 20, 24, 28, 32 };

            ColorComboBox.ItemsSource = typeof(Colors)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(property => new ColorItem
                {
                    Name = property.Name,
                    Brush = new SolidColorBrush((Color)property.GetValue(null, null))
                })
                .OrderBy(item => item.Name)
                .ToList();
        }

        private void FillFormWithVehicleData()
        {
            NameTextBox.Text = _vehicleForEdit.Name;
            ManufacturerTextBox.Text = _vehicleForEdit.Manufacturer;
            RangeTextBox.Text = _vehicleForEdit.RangeKm.ToString();
            SelectedImageTextBox.Text = _vehicleForEdit.ImagePath;
            ShowImagePreview(PathHelper.GetAbsolutePath(_vehicleForEdit.ImagePath));
            RtfHelper.LoadRtfIntoRichTextBox(DescriptionRichTextBox, _vehicleForEdit.RtfPath);
            UpdateWordCount();
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImageSourcePath = openFileDialog.FileName;
                SelectedImageTextBox.Text = openFileDialog.FileName;
                ShowImagePreview(openFileDialog.FileName);
            }
        }

        private void ShowImagePreview(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                PreviewImage.Source = null;
                return;
            }

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            PreviewImage.Source = bitmapImage;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFormData())
            {
                return;
            }

            if (_isEditMode)
            {
                UpdateVehicle();
                _mainWindow.SaveVehicles();

                MessageBox.Show("Vehicle was updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ElectricVehicle newVehicle = CreateVehicleFromForm();
                _mainWindow.Vehicles.Add(newVehicle);
                _mainWindow.SaveVehicles();

                MessageBox.Show("Vehicle was added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _mainWindow.NavigateToVehiclesPage();
        }

        private ElectricVehicle CreateVehicleFromForm()
        {
            string imageRelativePath = SaveImageIfNeeded();
            string rtfRelativePath = PathHelper.BuildRtfRelativePath(NameTextBox.Text.Trim());

            RtfHelper.SaveRichTextBoxToRtf(DescriptionRichTextBox, rtfRelativePath);

            return new ElectricVehicle(
                NameTextBox.Text.Trim(),
                ManufacturerTextBox.Text.Trim(),
                int.Parse(RangeTextBox.Text.Trim()),
                imageRelativePath,
                rtfRelativePath,
                DateTime.Now);
        }

        private void UpdateVehicle()
        {
            _vehicleForEdit.Name = NameTextBox.Text.Trim();
            _vehicleForEdit.Manufacturer = ManufacturerTextBox.Text.Trim();
            _vehicleForEdit.RangeKm = int.Parse(RangeTextBox.Text.Trim());

            if (!string.IsNullOrWhiteSpace(_selectedImageSourcePath))
            {
                _vehicleForEdit.ImagePath = PathHelper.CopyImageToApplicationFolder(_selectedImageSourcePath, NameTextBox.Text.Trim());
            }

            string currentRtfRelativePath = _vehicleForEdit.RtfPath;
            if (string.IsNullOrWhiteSpace(currentRtfRelativePath))
            {
                currentRtfRelativePath = PathHelper.BuildRtfRelativePath(NameTextBox.Text.Trim());
            }

            _vehicleForEdit.RtfPath = currentRtfRelativePath;
            RtfHelper.SaveRichTextBoxToRtf(DescriptionRichTextBox, _vehicleForEdit.RtfPath);
        }

        private string SaveImageIfNeeded()
        {
            if (!string.IsNullOrWhiteSpace(_selectedImageSourcePath))
            {
                return PathHelper.CopyImageToApplicationFolder(_selectedImageSourcePath, NameTextBox.Text.Trim());
            }

            if (_isEditMode)
            {
                return _vehicleForEdit.ImagePath;
            }

            return string.Empty;
        }

        private bool ValidateFormData()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                isValid = false;
                NameErrorLabel.Content = "Vehicle name is required.";
                NameTextBox.BorderBrush = Brushes.Red;
            }
            else if (_mainWindow.Vehicles.Any(vehicle => vehicle.Name.Equals(NameTextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase) && vehicle != _vehicleForEdit))
            {
                isValid = false;
                NameErrorLabel.Content = "Vehicle with this name already exists.";
                NameTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                NameErrorLabel.Content = string.Empty;
                NameTextBox.BorderBrush = Brushes.Gray;
            }

            if (string.IsNullOrWhiteSpace(ManufacturerTextBox.Text))
            {
                isValid = false;
                ManufacturerErrorLabel.Content = "Manufacturer is required.";
                ManufacturerTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                ManufacturerErrorLabel.Content = string.Empty;
                ManufacturerTextBox.BorderBrush = Brushes.Gray;
            }

            int rangeKm;
            if (!int.TryParse(RangeTextBox.Text.Trim(), out rangeKm) || rangeKm <= 0)
            {
                isValid = false;
                RangeErrorLabel.Content = "Enter a whole number greater than zero.";
                RangeTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                RangeErrorLabel.Content = string.Empty;
                RangeTextBox.BorderBrush = Brushes.Gray;
            }

            bool imageExists = !string.IsNullOrWhiteSpace(_selectedImageSourcePath) || (_isEditMode && !string.IsNullOrWhiteSpace(_vehicleForEdit.ImagePath));
            if (!imageExists)
            {
                isValid = false;
                ImageErrorLabel.Content = "Choose an image for the vehicle.";
            }
            else
            {
                ImageErrorLabel.Content = string.Empty;
            }

            if (!RtfHelper.HasText(DescriptionRichTextBox))
            {
                isValid = false;
                DescriptionErrorLabel.Content = "Description cannot be empty.";
                DescriptionRichTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                DescriptionErrorLabel.Content = string.Empty;
                DescriptionRichTextBox.BorderBrush = Brushes.Gray;
            }

            return isValid;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateToVehiclesPage();
        }

        private void DescriptionRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWordCount();
        }

        private void UpdateWordCount()
        {
            WordCountTextBlock.Text = "Words: " + RtfHelper.CountWords(DescriptionRichTextBox);
        }

        private void DescriptionRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object fontWeight = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldToggleButton.IsChecked = (fontWeight != DependencyProperty.UnsetValue) && fontWeight.Equals(FontWeights.Bold);

            object fontStyle = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicToggleButton.IsChecked = (fontStyle != DependencyProperty.UnsetValue) && fontStyle.Equals(FontStyles.Italic);

            object textDecorations = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineToggleButton.IsChecked = (textDecorations is TextDecorationCollection) && ((TextDecorationCollection)textDecorations).Count > 0;

            object fontFamily = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            if (fontFamily != DependencyProperty.UnsetValue)
            {
                FontFamilyComboBox.SelectedItem = fontFamily;
            }

            object fontSize = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (fontSize != DependencyProperty.UnsetValue)
            {
                double selectedSize = (double)fontSize;
                FontSizeComboBox.SelectedItem = (int)Math.Round(selectedSize);
            }

            SelectedColorRectangle.Fill = RtfHelper.GetSelectionBrush(DescriptionRichTextBox);
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null && !DescriptionRichTextBox.Selection.IsEmpty)
            {
                DescriptionRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem != null && !DescriptionRichTextBox.Selection.IsEmpty)
            {
                DescriptionRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, FontSizeComboBox.SelectedItem);
            }
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ColorItem selectedColor = ColorComboBox.SelectedItem as ColorItem;

            if (selectedColor == null)
            {
                return;
            }

            SelectedColorRectangle.Fill = selectedColor.Brush;

            if (!DescriptionRichTextBox.Selection.IsEmpty)
            {
                DescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, selectedColor.Brush);
            }
        }
    }
}
