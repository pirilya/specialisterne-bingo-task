using System;

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
                    Console.WriteLine(String.Join(", ", plates));
                }
            }
        }
    }
}