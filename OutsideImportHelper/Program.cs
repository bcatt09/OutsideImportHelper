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
            #if DEBUG
            var file = @"C:\Users\bcatt\source\repos\OutsideImportHelper\OutsideImportHelper\bin\Release\RS.1.2.246.352.221.4951143429611281183.3635535026647927978.dcm";
            if (true)
            {
            #else
            if (args.Length == 1)
            {
                var file = args[0];
            #endif
                var choice = 0;

                // Structure Set
                if (Path.GetFileName(file).Substring(0, 2) == "RS")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Structure Set Found");
                    choice = 1;
                }
                // Plan
                else if (Path.GetFileName(file).Substring(0, 2) == "RP")
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
                        var dcm = DICOMObject.Read(file);
                        var selector = new DICOMSelector(dcm);
                        var roiSelector = selector.RTROIObservationsSequence;
                        var roiInterpretedTypes = roiSelector.Select(s => s.RTROIInterpretedType_);
                        var externalRoiInterpretedTypes = roiInterpretedTypes.Where(x => x.Data.ToUpper() == "EXTERNAL" || x.Data.ToUpper() == "BODY");

                        // Find any structures with RT ROI Interpreted Type of EXTERNAL or BODY and change them to GTV
                        var typeChanges = 0;
                        foreach (var type in externalRoiInterpretedTypes)
                        {
                            typeChanges++;
                            type.Data = "GTV";
                        }

                        // Find any structures with Code Value of EXTERNAL or BODY and change them to GTV
                        var roiIdentificationCodes = roiSelector.Select(x => x.RTROIIdentificationCodeSequence_);

                        var codeChanges = 0;
                        foreach (var idCode in roiIdentificationCodes)
                        {
                            var bodyFlag = false;

                            foreach (var roi in idCode.Data.Elements)
                            {
                                if (roi.Tag.CompleteID == "00080100" && (roi.DData.ToString().ToUpper() == "BODY" || roi.DData.ToString().ToUpper() == "EXTERNAL"))
                                {
                                    roi.DData = "GTVp";
                                    bodyFlag = true;
                                    codeChanges++;
                                }
                                if (roi.Tag.CompleteID == "00080104" && bodyFlag)
                                {
                                    roi.DData = "Primary Gross Tumor Volume";
                                }
                            }
                        }

                        // Output results
                        if (typeChanges == 0 && codeChanges == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Did not find any Body structures to change");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine($"Changed {typeChanges} ROI Types and {codeChanges} ROI Codes");
                        }

                        // Save the new file
                        Console.ForegroundColor = ConsoleColor.Green;
                        DICOMFileWriter.Write($"NEW {Path.GetFileName(file)}", DICOMIOSettings.Default(), selector.ToDICOMObject());
                        Console.WriteLine($"New RS file is in {Path.GetDirectoryName(file)}\\NEW {Path.GetFileName(file)}");
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There was an error during the process, looks like you're on your own :(");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"{e.Message}\n\n{e.InnerException}\n\n{e.StackTrace}");
                    }
                }

                // *************** Plan Edit ***************
                else if (choice == 2)
                {
                    try
                    {
                        bool validChoice = false;
                        int selection = -1;
                        do
                        {
                            // Print out machine list and get user inupt
                            Console.WriteLine("Which machine would you like to change the plan to use?");
                            Console.WriteLine(String.Join("\n", McLarenLinacs.LinacList.Select((x, i) => $"{i + 1} - {x.Name}")));
                            var machineChoice = Console.ReadLine();

                            // Check validity of user input
                            if (Int32.TryParse(machineChoice, out selection) && selection < McLarenLinacs.LinacList.Count && selection >= 0)
                                validChoice = true;
                        } while (!validChoice);

                        Console.WriteLine("Attempting to change machine IDs for import...");
                        var dcm = DICOMObject.Read(file);
                        var sel = new DICOMSelector(dcm);

                        // Find the fields we're going to change
                        var sns = sel.BeamSequence.Select(b => b.DeviceSerialNumber_);
                        var manufacturers = sel.BeamSequence.Select(b => b.Manufacturer_);
                        var models = sel.BeamSequence.Select(b => b.ManufacturerModelName_);
                        var names = sel.BeamSequence.Select(b => b.TreatmentMachineName_);

                        // Change each of the occurences of each field
                        foreach (var sn in sns)
                            sn.Data = McLarenLinacs.LinacList[selection-1].SerialNumber;

                        foreach (var manufacturer in manufacturers)
                            manufacturer.Data = McLarenLinacs.LinacList[selection - 1].Manufacturer;

                        foreach (var model in models)
                            model.Data = McLarenLinacs.LinacList[selection - 1].Model;

                        foreach (var name in names)
                            name.Data = McLarenLinacs.LinacList[selection - 1].Name;

                        // Save the new file
                        Console.ForegroundColor = ConsoleColor.Green;
                        DICOMFileWriter.Write($"NEW {Path.GetFileName(file)}", DICOMIOSettings.Default(), sel.ToDICOMObject());
                        Console.WriteLine($"New RP file is in {Path.GetDirectoryName(file)}\\NEW {Path.GetFileName(file)}");
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There was an error during the process, looks like you're on your own :(");
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
