using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Bingo {
    class FileMethods {
        public static void MakeFile (BingoNumberGenerator bng, int numSheets, string title, string filename) {
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
        public static void LoadFile (BingoNumberGenerator bng, string filename) {
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
    }
}