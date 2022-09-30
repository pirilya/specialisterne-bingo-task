using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Bingo {
    class MainRunner {
        static void MakeFile (BingoNumberGenerator bng, int numSheets, string title, string filename) {
            var filepath = Path.Join(Directory.GetCurrentDirectory(), filename);
            using (StreamWriter writer = new StreamWriter(filepath)) {
                for (var sheetNumber = 0; sheetNumber < numSheets; sheetNumber++) {
                    var plates = bng.NextBatch();
                    writer.WriteLine("####################");
                    writer.WriteLine("# Sheet {0,-10} #", sheetNumber + 1);
                    writer.WriteLine("####################");
                    for (var plateNumber = 0; plateNumber < 6; plateNumber++) {
                        writer.WriteLine("{0} | Plate {1}", title, (sheetNumber * 6) + plateNumber + 1);
                        for (var row = 0; row < 3; row++) {
                            for (var col = 0; col < 9; col++) {
                                writer.Write("{0,3}", plates[plateNumber][row,col]);
                            }
                            writer.Write("\n");
                        }
                        writer.WriteLine("---");
                    }
                }
            }
        }
        static List<int>[] ExtractNumbers (List<string> formattedPlate) {
            var parsedPlate = new List<int>[9];
            Array.Fill(parsedPlate, new List<int>());
            foreach (var line in formattedPlate) {
                for (var col = 0; col < line.Length / 3; col++) {
                    var numberStr = line.Substring(col * 3, 3).Trim();
                    if (numberStr.Length > 0) {
                        var number = Convert.ToInt32(numberStr);
                        parsedPlate[col].Add(number);
                    }
                }
            }
            return parsedPlate;
        }
        static void LoadFile (BingoNumberGenerator bng, string filename) {
            // this function will error if 
            //  - the file does not exist
            //  - the file seems to not be a valid bingo plate file
            // So remember to handle those in the caller!
            var filepath = Path.Join(Directory.GetCurrentDirectory(), filename);
            using (StreamReader reader = new StreamReader(filepath)) {
                string line;
                var currentPlate = new List<string>();
                while ((line = reader.ReadLine()) != null) {
                    if (line.Length == 0 || line[0] == '#') {
                        // do nothing
                    } else if (line [0] == '-') {
                        currentPlate.RemoveAt(0);
                        bng.AddPreviousPlate(ExtractNumbers(currentPlate));
                        currentPlate.Clear();
                    } else {
                        currentPlate.Add(line);
                    }
                }
                if (currentPlate.Count() != 0) {
                    // if the file doesn't have a trailing ---, it's not correctly formatted
                    throw new FormatException(String.Format("The file at {0} doesn't seem to be a valid bingo plate file", filename));
                }
            }

        }
        static void Main (string[] args) {
            if (args.Length < 3 || args.Length > 4) {
                Console.WriteLine("You need to call this with 3 or 4 arguments: the number of bingo plates to generate, the title you want them to have, the filename to output them to, and optionally, the name of an already-generated file to load.");
            }
            else {
                int numPlates;
                bool success = int.TryParse(args[0], out numPlates);
                if (!success || numPlates < 0) {
                    Console.WriteLine("I cannot generate {0} bingo plates, because {0} is not a positive integer.", args[0]);
                }
                var s = new System.Diagnostics.Stopwatch();
                var bng = new BingoNumberGenerator();
                if (args.Length == 4) {
                    s.Start();
                    Console.WriteLine("Loading file \"{0}\"...", args[3]);
                    try {
                        LoadFile(bng, args[3]);
                    }
                    catch (FileNotFoundException) {
                        Console.WriteLine("I can't load the file {0}, because it doesn't exist.", args[3]);
                        return;
                    } catch (FormatException) {
                        Console.WriteLine("I can't load the file {0}, because it doesn't seem to be a valid bingo plate file.", args[3]);
                        return;
                    }
                    s.Stop();
                    Console.WriteLine("File loaded (It took {0} ms)", s.ElapsedMilliseconds);
                }
                var numSheets = (numPlates + 5) / 6; // divided by 6, rounded up
                Console.WriteLine("Generating {0} bingo plates ({1} sheets of 6) all titled \"{2}\" in the file location \"{3}\"...", numSheets * 6, numSheets, args[1], args[2]);
                if (numPlates >= 100000) {
                    // let people know if they've picked a large number that'll fill up their disk space and take multiple hours
                    Console.WriteLine("The output file will be about {0:N0} KB.", numPlates / 10); 
                    Console.WriteLine("On my computer that'd take about {0} seconds.", numPlates / 100000); 
                }
                s.Start();
                MakeFile(bng, numSheets, args[1], args[2]);
                s.Stop();
                Console.WriteLine("Done! (It took {0} ms)", s.ElapsedMilliseconds);
            }
        }
    }
}