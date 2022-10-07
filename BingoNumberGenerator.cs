using System;
using System.Collections.Generic;
using System.Linq;

namespace Bingo {
    public class ColumnsWithTwo {
        public bool [,] Data;
        public ColumnsWithTwo (RandomUtils randomizer) {
            var twos = new int[Constants.PlatesInSheet];
            Array.Fill(twos, Constants.TwoColumnsPerSheet);
            Data = randomizer.Choose2D(twos, AvailableNumbers.Lengths);
        }
        public bool[] GetPlate (int plateNumber) {
            var result = new bool[Constants.PlateWidth];
            for (var col = 0; col < Constants.PlateWidth; col++) {
                result[col] = Data[plateNumber, col];
            }
            return result;
        }
    }
    public class ShuffledNumbers {

        int[][] Columns;
        int[] indexes;
        
        public ShuffledNumbers (RandomUtils randomizer) {
            // just setting this Columns equal to AvailableNumbers.Columns wouldn't break anything, 
            // and it'd be slightly more efficient, but it'd go against the siloing principle
            Columns = AvailableNumbers.Generate();
            foreach (var column in Columns) {
                randomizer.Shuffle(column);
            }
            indexes = new int[Columns.Length]; // ints have default value 0 so we don't need to initalize with a value
        }
        public int NextInColumn (int colNumber) {
            return Columns[colNumber][indexes[colNumber]++];
        }
    }
    // this currently has no content... it should enforce the strict length of Constants.PlatesInSheet
    public class Sheet<T> : List<T> {
    }

    public class BingoNumberGenerator {
        HashSet<string> PreviousPlates { get; set; }
        RandomUtils Randomizer;

        public BingoNumberGenerator () {
            Randomizer = new RandomUtils();
            PreviousPlates = new HashSet<string>();
        }
        public void AddPreviousPlate (PlateNumbers plate) {
            PreviousPlates.Add(plate.ToFlat());
        }
        public void Clear () {
            PreviousPlates.Clear();
        }
        Sheet<PlateNumbers> NextBatchNumbers (ColumnsWithTwo columnsWithTwo) {
            var numbers = new ShuffledNumbers(Randomizer);
            var result = new Sheet<PlateNumbers>();
            for (var i = 0; i < Constants.PlatesInSheet; i++) {
                result.Add(new PlateNumbers(numbers, columnsWithTwo.GetPlate(i)));
            }
            foreach (var plate in result) {
                // if just one of our generated plates is a duplicate, we have to throw out this result and start over
                if (PreviousPlates.Contains(plate.ToFlat())) {
                    return NextBatchNumbers(columnsWithTwo);
                }
            }
            // but if that's not the case, we can add these 6 numbersets to the list, and return them
            foreach (var plate in result) {
                PreviousPlates.Add(plate.ToFlat());
            }
            return result;
        }
        public Sheet<Plate> NextBatch () {
            var columnsWithTwo = new ColumnsWithTwo(Randomizer);

            var numbers = NextBatchNumbers(columnsWithTwo);

            var output = new Sheet<Plate>();
            for (var i = 0; i < Constants.PlatesInSheet; i++) {
                var blanks = new PlateBlanks(Randomizer, columnsWithTwo.GetPlate(i));
                output.Add(new Plate(blanks, numbers[i]));
            }
            return output;
        }
    }
}