using System;
using System.Collections.Generic;
using System.Linq;

namespace Bingo { 

    // really want ColumnsWithTwo to be a class, so it can have a method to return the ith column
    // so we can pass just the ith column to these guys, rather than the whole bool[,] and also i

    public class PlateNumbers {

        List<int>[] Data;
        int[] indexes;

        public PlateNumbers () {
            indexes = new int[Constants.PlateWidth];
            Data = new List<int>[Constants.PlateWidth];
            for (var col = 0; col < Constants.PlateWidth; col++) {
                Data[col] = new List<int>();
            }
        }
        public PlateNumbers (ShuffledNumbers numbers, bool[] ColumnsWithTwo) : this () {
            for (var col = 0; col < Constants.PlateWidth; col++) {
                var n = ColumnsWithTwo[col]? 2 : 1;
                for (var m = 0; m < n; m++) {
                    Data[col].Add(numbers.NextInColumn(col));
                }
                Data[col].Sort();
            }
            var counts = Data.ToList().Select(x => x.Count());
        }
        public int NextInColumn (int colNumber) {
            return Data[colNumber][indexes[colNumber]++];
        }
        public void AddInColumn (int number, int col) {
            Data[col].Add(number);
        }
        public string ToFlat() {
            var result = "";
            for (var col = 0; col < Constants.PlateWidth; col++) {
                result += String.Format("{0},", String.Join(",", Data[col]));
            }
            return result;
        }
    }
    public class PlateBlanks {
        bool[,] Data;
        public PlateBlanks (RandomUtils Randomizer, bool[] ColumnsWithTwo) {
            var initialRowsum = new int[Constants.PlateHeight];
            Array.Fill(initialRowsum, Constants.BlanksPerRow);
            var numBlanks = new int[Constants.PlateWidth];
            for (var col = 0; col < Constants.PlateWidth; col++) {
                numBlanks[col] = ColumnsWithTwo[col] ? 1 : 2;
            }
            Data = Randomizer.Choose2D(initialRowsum, numBlanks);
        }
        public bool IsBlank(int row, int col) {
            return Data[row, col];
        }
    }
    public class Plate {
        public int?[,] Data;
        public Plate (PlateBlanks blanks, PlateNumbers numbers) {
            Data = new int?[Constants.PlateHeight, Constants.PlateWidth];
            for (var col = 0; col < Constants.PlateWidth; col++) {
                for (var row = 0; row < Constants.PlateHeight; row++) {
                    if (!blanks.IsBlank(row, col)) {
                        Data[row,col] = numbers.NextInColumn(col);
                    } // no need to have an else, null is the default value of a nullable
                }
            }
        }
        public int?[] GetRow(int rowNumber) {
            var result = new int?[Constants.PlateWidth];
            for (var col = 0; col < Constants.PlateWidth; col++) {
                result[col] = Data[rowNumber, col];
            }
            return result;
        }
    }
}