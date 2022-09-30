using System;
using System.Collections.Generic;
using System.Linq;

namespace Bingo {
    class UI {
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
                        FileMethods.LoadFile(bng, args[3]);
                    }
                    catch (System.IO.FileNotFoundException) {
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
                FileMethods.MakeFile(bng, numSheets, args[1], args[2]);
                s.Stop();
                Console.WriteLine("Done! (It took {0} ms)", s.ElapsedMilliseconds);
            }
        }
    }
}