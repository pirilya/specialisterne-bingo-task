using System;
using System.Collections.Generic;

namespace Bingo {
    // We probably want to define a BingoPlate class actually. Have it inherit from whichever complicated structure of
    // lists of arrays I decide on, and put nothing inside the class definition, then it's basically a type alias.
    
    public class BingoNumberGenerator {
        HashSet<int[]> PreviousPlates { get; set; }
        Random Random { get; set; }
        public BingoNumberGenerator () {
            PreviousPlates = new HashSet<int[]>();
            Random = new Random();
        }
        public int[] NextBatch () {
            // This function is currently a stub.
            // The return type should be a list (or array?) of our chosen plate data format 
            // but i haven't decided on that yet so this initial stub just returns 6 random ints from the 0-9 range
            var output = new int[6];
            for (var i = 0; i < 6; i++) {
                output[i] = Random.Next(10);
            }
            return output;
        }
        public void test () {
            // we can put code in here if there's something we want to quickly test -- one of the private functions perhaps.
            // obviously delete this function before handing in the completed solution
        }
    }
}