using System;
using System.Collections.Generic;
using System.Linq;

namespace Bingo {
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
                /*Console.WriteLine(String.Join("", numBlanks));
                for (var j = 0; j < 3; j++) {
                    Console.WriteLine(String.Join("", Enumerable.Range(0,9).Select(x => result[i][x,j] ? "X" : ".")));
                }*/
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
                    /*Console.WriteLine("plate {0}, col {1}, numbers are {2}, there are {3} blanks, ColumnsWithTwo is {4}",
                        plateNumber, col, String.Join(",", numbers[plateNumber][col]), 
                        Enumerable.Range(0,3).Select(x => blanks[plateNumber][col, x]).Where(x => x).Count(), 
                        ColumnsWithTwo[col, plateNumber]);*/
                    var i = 0;
                    for (var row = 0; row < 3; row++) {
                        if (blanks[plateNumber][col, row]) {
                            // no need to do anything, null is the default value of a nullable
                        } else {
                            //Console.WriteLine("{0} {1}",numbers[plateNumber][row].Count(), i);
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
            /*for (var plate = 0; plate < 6; plate++) {
                Console.WriteLine("Plate # {0} of the batch", plate);
                for (var row = 0; row < 3; row++) {
                    var output = new List<string>();
                    for (var col = 0; col < 9; col++) {
                        output.Add(Convert.ToString(plates[plate][row, col]));
                    }
                    Console.WriteLine(String.Join(",", output));
                }
            }*/
        }
    }
}