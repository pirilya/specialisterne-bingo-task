using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Bingo {
    class SimpleFormat {
        public static string Serialize (Sheet<Plate> plates, int sheetNumber, string title) {
            var lines = new List<string>();
            lines.Add("####################");
            lines.Add(String.Format("# Sheet {0,-10} #", sheetNumber + 1));
            lines.Add("####################");
            for (var plateNumber = 0; plateNumber < Constants.PlatesInSheet; plateNumber++) {
                var globalPlateNumber = (sheetNumber * Constants.PlatesInSheet) + plateNumber + 1;
                lines.Add(String.Format("{0} | Plate {1}", title, globalPlateNumber));
                for (var row = 0; row < Constants.PlateHeight; row++) {
                    lines.Add(String.Join("", 
                        plates[plateNumber].GetRow(row).Select(x => String.Format("{0,3}", x))));
                }
                lines.Add("---");
            }
            return String.Join("\n", lines);
        }
        static PlateNumbers ExtractNumbers (List<string> formattedPlate) {
            var parsedPlate = new PlateNumbers();
            foreach (var line in formattedPlate) {
                for (var col = 0; col < line.Length / 3; col++) {
                    var numberStr = line.Substring(col * 3, 3).Trim();
                    if (numberStr.Length > 0) {
                        var number = Convert.ToInt32(numberStr);
                        parsedPlate.AddInColumn(number, col);
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