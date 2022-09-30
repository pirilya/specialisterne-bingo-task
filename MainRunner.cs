using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Bingo {
    class MainRunner {
        static void MakeFile(int numSheets, string title, string filename) {
            var filepath = Path.Join(Directory.GetCurrentDirectory(), filename);
            using (StreamWriter writer = new StreamWriter(filepath)) {
                var bng = new BingoNumberGenerator();
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
        static void Main(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine("You need to call this with 3 arguments: the number of bingo plates to generate, the title you want them to have, and the filename to output them to.");
            }
            else {
                int numPlates;
                bool success = int.TryParse(args[0], out numPlates);
                if (!success || numPlates < 0) {
                    Console.WriteLine("I cannot generate {0} bingo plates, because {0} is not a positive integer.", args[0]);
                }
                var numSheets = (numPlates + 5) / 6; // divided by 6, rounded up
                Console.WriteLine("Generating {0} bingo plates ({1} sheets of 6) all titled \"{2}\" in the file location \"{3}\"", numSheets * 6, numSheets, args[1], args[2]);
                // let people know if they've picked a large number that'll fill up their disk space and take multiple hours
                Console.WriteLine("The output file will be about {0:N0} KB.", numPlates / 10); 
                Console.WriteLine("On my computer that'd take about {0} seconds.", numPlates / 100000); 
                var s = new System.Diagnostics.Stopwatch();
                s.Start();
                MakeFile(numSheets, args[1], args[2]);
                s.Stop();
                Console.WriteLine("Done! It in fact took {0} ms", s.ElapsedMilliseconds);
            }
        }
    }
}