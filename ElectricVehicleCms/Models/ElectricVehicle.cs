
using System;
using System.Xml.Serialization;
using ElectricVehicleCms.Helpers;

namespace ElectricVehicleCms.Models
{
    public class ElectricVehicle
    {
        public string Name { get; set; }

        public string Manufacturer { get; set; }

        public int RangeKm { get; set; }

        public string ImagePath { get; set; }

        public string RtfPath { get; set; }

        public DateTime DateAdded { get; set; }

        [XmlIgnore]
        public bool IsSelected { get; set; }

        [XmlIgnore]
        public string FullImagePath
        {
            get
            {
                return PathHelper.GetAbsolutePath(ImagePath);
            }
        }

        public ElectricVehicle()
        {
        }

        public ElectricVehicle(string name, string manufacturer, int rangeKm, string imagePath, string rtfPath, DateTime dateAdded)
        {
            Name = name;
            Manufacturer = manufacturer;
            RangeKm = rangeKm;
            ImagePath = imagePath;
            RtfPath = rtfPath;
            DateAdded = dateAdded;
        }
    }
}
