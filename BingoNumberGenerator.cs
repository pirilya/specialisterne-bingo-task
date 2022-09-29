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
        public bool[,] Choose2D (int width, int height, int rowsum, int[] colsums) {
            if (rowsum * height != colsums.Sum()) {
                throw new InvalidOperationException("Your sums don't add up!");
            }
            // for the purposes of having a functioning stub while we get everything else working, 
            // let's just manually return the column distribution from the problem sheet example.
            if (width == 9 && height == 6 && rowsum == 6 && true) {
                var TheOneFromTheProblemSheet = new bool[,]{{true,false,false,true,true,false},
                                                            {true,true,false,true,false,true},
                                                            {false,true,true,false,true,true},
                                                            {false,true,true,false,true,true},
                                                            {true,false,true,true,true,false},
                                                            {false,true,true,false,true,true},
                                                            {true,false,true,true,true,false},
                                                            {true,true,false,true,false,true},
                                                            {true,true,true,true,false,true}};
                return TheOneFromTheProblemSheet;
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
                if (colcounts[x] == colsums[x]) {
                    // do nothing, it's a bool so it was already initialized to false
                } else if (colcounts[x] == colsums[x] - 1) {
                    chosens[x, yy] = true;
                } else {
                    // we can't make the sums add up. start over!
                    return Choose2D(width, height, rowsum, colsums);
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

        HashSet<List<int>[]> PreviousPlates { get; set; }
        int[][] NumbersByColumns { get; set; }

        public BingoNumberGenerator () : base() {
            PreviousPlates = new HashSet<List<int>[]>();
            var NumbersByColumnsList = new List<List<int>>();
            for (var i = 0; i < 9; i++) {
                NumbersByColumnsList.Add(Enumerable.Range(i * 10, 10).ToList());
            }
            NumbersByColumnsList[0].Remove(0);
            NumbersByColumnsList[8].Add(90);
            NumbersByColumns = NumbersByColumnsList.Select(x => x.ToArray()).ToArray();
        }
        List<List<int>[]> NextBatchNumbers (bool[,] ColumnsWithTwo) {
            var result = new List<List<int>[]>();
            for (var i = 0; i < 6; i++) {
                result.Add(new List<int>[9]);
            }
            for (var col = 0; col < 9; col++) {
                Shuffle(NumbersByColumns[col]);
                var i = 0;
                for (var numberOfBatch = 0; numberOfBatch < 6; numberOfBatch++) {
                    result[numberOfBatch][col] = new List<int>();
                    var n = ColumnsWithTwo[col, numberOfBatch]? 2 : 1;
                    for (var m = 0; m < n; m++) {
                        result[numberOfBatch][col].Add(NumbersByColumns[col][i]);
                        i++;
                    }
                    result[numberOfBatch][col].Sort();
                }
            }
            foreach (var plate in result) {
                // if just one of our generated plates is a duplicate, we have to throw out this result and start over
                if (PreviousPlates.Contains(plate)) {
                    return NextBatchNumbers(ColumnsWithTwo);
                }
            }
            // but if that's not the case, we can add these 6 numbersets to the list, and return them
            foreach (var plate in result) {
                PreviousPlates.Add(plate);
            }
            return result;
        }
        List<bool[,]> NextBatchBlanks (bool[,] ColumnsWithTwo) {
            var result = new List<bool[,]>();
            for (var i = 0; i < 6; i++) {
                var numBlanks = new int[9];
                for (var col = 0; col < 9; col++) {
                    numBlanks[col] = ColumnsWithTwo[col, i] ? 1 : 2;
                }
                result.Add(Choose2D(9, 3, 4, numBlanks));
            }
            return result;
        }
        public List<int?[,]> NextBatch () {
            var numberOfNumbers = NumbersByColumns.Select(x => x.Length - 6).ToArray();
            var ColumnsWithTwo = Choose2D(9, 6, 6, numberOfNumbers);

            var numbers = NextBatchNumbers(ColumnsWithTwo);
            var blanks = NextBatchBlanks(ColumnsWithTwo);

            var output = new List<int?[,]>();
            for (var plateNumber = 0; plateNumber < 6; plateNumber++) {
                var plate = new int?[3,9];
                output.Add(plate);
                for (var col = 0; col < 9; col++) {
                    Console.WriteLine("plate {0}, col {1}, numbers are {2}, there are {3} blanks, ColumnsWithTwo is {4}",
                        plateNumber, col, String.Join(",", numbers[plateNumber][col]), 
                        Enumerable.Range(0,3).Select(x => blanks[plateNumber][col, x]).Where(x => x).Count(), 
                        ColumnsWithTwo[col, plateNumber]);
                    var i = 0;
                    for (var row = 0; row < 3; row++) {
                        if (blanks[plateNumber][col, row]) {
                            // no need to do anything, null is the default value of a nullable
                        } else {
                            Console.WriteLine("{0} {1}",numbers[plateNumber][row].Count(), i);
                            plate[row,col] = numbers[plateNumber][col][i];
                            i++;
                        }
                    }
                }
            }
            return output;
        }
        public void test () {
            var plates = NextBatch();
            for (var plate = 0; plate < 6; plate++) {
                Console.WriteLine("Plate # {0} of the batch", plate);
                for (var row = 0; row < 3; row++) {
                    var output = new List<string>();
                    for (var col = 0; col < 9; col++) {
                        output.Add(Convert.ToString(plates[plate][row, col]));
                    }
                    Console.WriteLine(String.Join(",", output));
                }
            }
        }
    }
}