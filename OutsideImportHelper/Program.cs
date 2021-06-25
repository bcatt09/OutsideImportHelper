using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EvilDICOM.Core;
using EvilDICOM.Core.IO.Writing;
using EvilDICOM.Core.Selection;

namespace OutsideImportHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var choice = 0;

                if (Path.GetFileName(args[0]).Substring(0, 2) == "RS")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Structure Set Found");
                    choice = 1;
                }
                else if (Path.GetFileName(args[0]).Substring(0, 2) == "RP")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Plan Found");
                    choice = 2;
                }
                else
                {
                    while (!(new List<int> { 1, 2, 3 }).Contains(choice))
                    {
                        Console.WriteLine("Select the type of file you would like to edit:\n1 - Structure Set\n2 - Plan\n3 - Exit");
                        int.TryParse(Console.ReadKey().KeyChar.ToString(), out int newChoice);
                        choice = newChoice;
                        Console.WriteLine();
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;

                // *************** Structure Set Edit ***************

                if (choice == 1)
                {
                    try
                    {
                        Console.WriteLine("Attempting to fix External/Body structure for import...");
                        var dcm = DICOMObject.Read(args[0]);
                        var sel = new DICOMSelector(dcm);

                        // Find any structures with RT ROI Interpreted Type of EXTERNAL
                        var externals = sel.RTROIObservationsSequence.Select(s => s.RTROIInterpretedType_.Where(t => t.Data == "EXTERNAL"));

                        foreach (var external in externals)
                        {
                            // Change the Interpreted Type to ORGAN
                            external.Data = "ORGAN";
                        }

                        // Save the new file
                        Console.ForegroundColor = ConsoleColor.Green;
                        DICOMFileWriter.Write($"NEW {Path.GetFileName(args[0])}", DICOMIOSettings.Default(), sel.ToDICOMObject());
                        Console.WriteLine($"New RS file is in {Path.GetDirectoryName(args[0])}\\NEW {Path.GetFileName(args[0])}");
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There was an error during the process, looks like you're on your own :)");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"{e.Message}\n\n{e.InnerException}\n\n{e.StackTrace}");
                    }
                }

                // *************** Plan Edit ***************
                else if (choice == 2)
                {
                    try
                    {
                        Console.WriteLine("Attempting to change machine IDs for import...");
                        var dcm = DICOMObject.Read(args[0]);
                        var sel = new DICOMSelector(dcm);

                        // Default values
                        var _sn = "";
                        var _manufacturer = "Varian Medical Systems";
                        var _model = "Varian 2100 C/D";
                        var _name = "Eclipse CAP";

                        // Find the fields we're going to change
                        var sns = sel.BeamSequence.Select(b => b.DeviceSerialNumber_);
                        var manufacturers = sel.BeamSequence.Select(b => b.Manufacturer_);
                        var models = sel.BeamSequence.Select(b => b.ManufacturerModelName_);
                        var names = sel.BeamSequence.Select(b => b.TreatmentMachineName_);

                        // Change each of the occurences of each field
                        foreach (var sn in sns)
                            sn.Data = _sn;

                        foreach (var manufacturer in manufacturers)
                            manufacturer.Data = _manufacturer;

                        foreach (var model in models)
                            model.Data = _model;

                        foreach (var name in names)
                            name.Data = _name;

                        // Save the new file
                        Console.ForegroundColor = ConsoleColor.Green;
                        DICOMFileWriter.Write($"NEW {Path.GetFileName(args[0])}", DICOMIOSettings.Default(), sel.ToDICOMObject());
                        Console.WriteLine($"New RP file is in {Path.GetDirectoryName(args[0])}\\NEW {Path.GetFileName(args[0])}");
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There was an error during the process, looks like you're on your own :)");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"{e.Message}\n\n{e.InnerException}\n\n{e.StackTrace}");
                    }
                }
                else
                    return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("To run, drag the Plan or Structure Set file onto this executable");
            }

            Console.ReadKey();
        }
    }
}
