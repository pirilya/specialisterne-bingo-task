using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Bingo {
    class UI {
        static int? getNumPlates (string userInput) {
            int numPlates;
            bool success = int.TryParse(userInput, out numPlates);
            if (!success || numPlates < 0) {
                Console.WriteLine("I cannot generate {0} bingo plates, because {0} is not a positive integer.", userInput);
                return null;
            }
            return numPlates;
        }
        static bool LoadFile (BingoNumberGenerator bng, string filename) {
            var s = new System.Diagnostics.Stopwatch();
            Console.WriteLine("Loading file \"{0}\"...", filename);
            try {
                s.Start();
                SimpleFormat.LoadFile(bng, filename);
                s.Stop();
                Console.WriteLine("File loaded (It took {0} ms)", s.ElapsedMilliseconds);
                return true;
            }
            catch (System.IO.FileNotFoundException) {
                Console.WriteLine("I can't load the file {0}, because it doesn't exist.", filename);
                return false;
            } catch (FormatException) {
                Console.WriteLine("I can't load the file {0}, because it doesn't seem to be a valid bingo plate file.", filename);
                return false;
            }
        }
        static void MakeFile (BingoNumberGenerator bng, int numPlates, string title, string filename) {
            var s = new System.Diagnostics.Stopwatch();
            var numSheets = (numPlates + 5) / 6; // divided by 6, rounded up
            Console.WriteLine("Generating {0} bingo plates ({1} sheets of 6) all titled \"{2}\" in the file location \"{3}\"...", numSheets * 6, numSheets, title, filename);
            if (numPlates >= 100000) {
                // let people know if they've picked a large number that'll fill up their disk space and take multiple hours
                Console.WriteLine("The output file will be about {0:N0} KB.", numPlates / 10); 
                Console.WriteLine("On my computer that'd take about {0} seconds.", numPlates / 100000); 
            }
            s.Start();
            var filepath = Path.Join(Directory.GetCurrentDirectory(), filename);
            using (StreamWriter writer = new StreamWriter(filepath)) {
                for (var sheetNumber = 0; sheetNumber < numSheets; sheetNumber++) {
                    var plates = bng.NextBatch();
                    writer.WriteLine(SimpleFormat.Serialize(plates, sheetNumber, title));
                }
            }
            s.Stop();
            Console.WriteLine("Done! (It took {0} ms)", s.ElapsedMilliseconds);
        }
        static void Wizard () {
            Console.WriteLine("Welcome to the bingo plate generation program!");
            Console.WriteLine("(In the future, if you don't want to go through this UI, you can also specify your desired output directly in the command line arguments.)");
            int? numPlates = null;
            while (numPlates == null) {
                Console.WriteLine("How many bingo plates do you want to generate?");
                numPlates = getNumPlates(Console.ReadLine());
            }
            var actualNumPlates = (numPlates.Value + 5) / 6 * 6;
            if (numPlates != actualNumPlates) {
                Console.WriteLine("Since bingo plates are generated in sheets of 6, the program will round up and generate {0}.", actualNumPlates);
            }
            Console.WriteLine("What title do you want to print on every plate?");
            var title = Console.ReadLine();
            Console.WriteLine("What filename do you want to output to?");
            var filename = Console.ReadLine();
            Console.WriteLine("Do you want to load a file of already generated plates, so the program can avoid duplicating those?");
            var answer = "";
            while (answer != "y" && answer != "n") {
                Console.WriteLine("Please answer \"y\" or \"n\".");
                answer = Console.ReadLine();
            }
            var bng = new BingoNumberGenerator();
            if (answer == "y") {
                var success = false;
                while (!success) {
                    Console.WriteLine("What's the name of the file you want to load?");
                    bng.Clear();
                    success = LoadFile(bng, Console.ReadLine());
                }
            }
            MakeFile(bng, numPlates.Value, title, filename);
        }
        static void Main (string[] args) {
            if (args.Length == 0) {
                Wizard();
            } else if (args.Length == 3 || args.Length == 4){
                var numPlates = getNumPlates(args[0]);
                if (numPlates == null) {
                    return;
                }
                var bng = new BingoNumberGenerator();
                if (args.Length == 4) {
                    var success = LoadFile(bng, args[3]);
                    if (!success) {
                        return;
                    }
                }
                MakeFile(bng, numPlates.Value, args[1], args[2]);
            } else {
                Console.WriteLine("Wrong number of arguments!");
                Console.WriteLine("You can run this program with 0 arguments, to start the wizard."); 
                Console.WriteLine("Or with 3 or 4 arguments: the number of bingo plates to generate, the title you want them to have, the filename to output them to, and optionally, the name of an already-generated file to load.");
                return;
            }
        }
    }
}