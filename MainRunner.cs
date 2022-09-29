using System;
using System.Collections.Generic;
using System.Linq;

namespace Bingo {
    class MainRunner {
        static void Main(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("You need to call this with some arguments!");
            }
            else {
                int numPlates;
                bool success = int.TryParse(args[0], out numPlates);
                if (!success) {
                    Console.WriteLine("The first argument needs to be a number.");
                }
                var bng = new BingoNumberGenerator();
                for (var i = 0; i < numPlates; i += 6) {
                    var plates = bng.NextBatch();
                    for (var plateNumber = 0; plateNumber < 6; plateNumber++) {
                        Console.WriteLine("---");
                        Console.WriteLine("Plate {0}", i + plateNumber + 1);
                        for (var row = 0; row < 3; row++) {
                            for (var col = 0; col < 9; col++) {
                                Console.Write("{0,3}", plates[plateNumber][row,col]);
                            }
                            Console.Write("\n");
                        }
                    }
                }
            }
        }
    }
}