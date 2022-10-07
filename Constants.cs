using System;
using System.Collections.Generic;
using System.Linq;

namespace Bingo { 
    public class Constants {
        public static int PlatesInSheet = 6;
        public static int PlateWidth = 9;
        public static int PlateHeight = 3;
        public static int TwoColumnsPerSheet = 6; // this should really be calculated from the other numbers
        public static int BlanksPerRow = 4;
    }
    public class AvailableNumbers {

        static int[][] Columns = Generate();
        public static int[] Lengths = GetLengths();

        public static int[][] Generate() {
            var columnLists = new List<List<int>>();
            for (var i = 0; i < 9; i++) {
                columnLists.Add(Enumerable.Range(i * 10, 10).ToList());
            }
            columnLists[0].Remove(0);
            columnLists[8].Add(90);
            return columnLists.Select(x => x.ToArray()).ToArray();
        }
        static int[] GetLengths () {
            return Columns.Select(x => x.Length - 6).ToArray();
        }
    }
}