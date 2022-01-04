using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideImportHelper
{
    public class Linac
    {
        public string Name;
        public string SerialNumber;
        public string Model;
        public string Manufacturer;
        
        public Linac()
        {
            Name = "Eclipse CAP";
            SerialNumber = "";
            Model = "Varian 2100 C/D";
            Manufacturer = "Varian Medical Systems";
        }

        public Linac(string name, string sn, string model, string manufacturer)
        {
            Name = name;
            SerialNumber = sn;
            Model = model;
            Manufacturer = manufacturer;
        }

        public static Linac newTrueBeam(string name, string sn)
        {
            return new Linac(name, sn, "TDS", "Varian Medical Systems");
        }

        public static Linac new21EX(string name, string sn)
        {
            return new Linac(name, sn, "Varian 21EX", "Varian Medical Systems");
        }

        public static Linac new2300IX(string name, string sn)
        {
            return new Linac(name, sn, "2300IX", "Varian Medical Systems");
        }
    }
}
