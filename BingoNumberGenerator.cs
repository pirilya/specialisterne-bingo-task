using System;
using System.Collections.Generic;
using System.Linq;

namespace Bingo {
    // We probably want to define a BingoPlate class actually. Have it inherit from whichever complicated structure of
    // lists of arrays I decide on, and put nothing inside the class definition, then it's basically a type alias.
    public class RandomUtils {
        Random Random { get; set; }
        public RandomUtils() {
            Random = new Random();
        }
        // i feel like this isn't a good name for this function but i can't think of a better one
        public int[] RandomSubset (int min, int max, int numChosen) {
            var result = new int[numChosen];
            for (var i = 0; i < numChosen; i++) {
                result[i] = Random.Next(min, max - numChosen + 1);
            }
            for (var i = 0; i < numChosen; i++) {
                for (var j = i + 1; j < numChosen; j++) {
                    if (result[i] <= result[j]) {
                        result[j]++;
                    }
                }
            }
            return result;
        }
        public bool[,] Choose2D (int width, int height, int rowsum, int colsum) {
            if (rowsum * height != colsum * width) {
                throw new InvalidOperationException("Your sums don't add up!");
            }
            var chosens = new bool[width, height];
            var colcounts = new int[width];
            for (var y = 0; y < height-1; y++) {
                var rowChosens = RandomSubset(0, width, rowsum);
                foreach (var x in rowChosens) {
                    chosens[x,y] = true;
                    colcounts[x]++;
                }
            }
            var yy = height - 1;
            for (var x = 0; x < width; x++) {
                if (colcounts[x] == colsum) {
                    // do nothing, it's a bool so it was already initialized to false
                } else if (colcounts[x] == colsum - 1) {
                    chosens[x, yy] = true;
                } else {
                    // we can't make the sums add up. start over!
                    return Choose2D(width, height, rowsum, colsum);
                }
            }
            return chosens;
        }
        // apparently the C# Random doesn't have an array shuffling function?
        // so i'll write one myself. fortunately fisher-yates is pretty simple
        public void Shuffle<T> (T[] deck) {
            for (var i = 0; i < deck.Count(); i++) {
                var j = Random.Next(i+1);
                T value = deck[j];
                deck[j] = deck[i];
                deck[i] = value;
            }
        }

    }
    public class BingoNumberGenerator : RandomUtils {

        HashSet<int[]> PreviousPlates { get; set; }
        int[][] NumbersByColumns { get; set; }

        public BingoNumberGenerator () : base() {
            PreviousPlates = new HashSet<int[]>();
            var NumbersByColumnsList = new List<List<int>>();
            for (var i = 0; i < 9; i++) {
                NumbersByColumnsList.Add(Enumerable.Range(i * 10, 10).ToList());
            }
            //NumbersByColumnsList[0].Remove(0); // these lines commented out so i can get the simple version working first
            //NumbersByColumnsList[8].Add(90);
            NumbersByColumns = NumbersByColumnsList.Select(x => x.ToArray()).ToArray();
        }
        List<int>[,] NextBatchNumbers () {
            // might be more logical to swap the dimensions here, and call with 6,9,4,6? obviously it's equivalent it's just a matter of what makes more sense
            var ColumnsWithTwo = Choose2D(9, 6, 6, 4);
            var result = new List<int>[6,9];
            for (var col = 0; col < 9; col++) {
                Shuffle(NumbersByColumns[col]);
                var i = 0;
                for (var numberOfBatch = 0; numberOfBatch < 6; numberOfBatch++) {
                    result[numberOfBatch, col] = new List<int>();
                    var n = ColumnsWithTwo[col, numberOfBatch]? 2 : 1;
                    for (var m = 0; m < n; m++) {
                        result[numberOfBatch, col].Add(NumbersByColumns[col][i]);
                        i++;
                    }
                    result[numberOfBatch, col].Sort();
                }
            }
            return result;
        }
        bool[,] NextBlanksMap () {
            var result = new bool[3,9];
            for (var rowNumber = 0; rowNumber < 3; rowNumber++) {
                var blankPositions = RandomSubset(0, 9, 4);
                foreach (var position in blankPositions) {
                    result[rowNumber, position] = true;
                }
            }
            return result;
        }
        public List<int?[,]> NextBatch () {
            var output = new List<int?[,]>();
            for (var i = 0; i < 6; i++) {
                var blanks = NextBlanksMap();
                var plate = new int?[3,9];
                output.Add(plate);
                for (var row = 0; row < 3; row++){
                    for (var col = 0; col < 9; col++) {
                        if (blanks[row, col]) {
                            plate[row, col] = null;
                        } else {
                            plate[row, col] = 12;
                        }
                    }
                }
            }
            return output;
        }
        public void test () {
            var batchNumbers = NextBatchNumbers();
            for (var col = 0; col < 9; col++) {
                Console.WriteLine(String.Join(",", batchNumbers[0, col]));
            }
        }
    }
}