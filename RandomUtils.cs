using System;
using System.Collections.Generic;
using System.Linq;

namespace Bingo {
    public class RandomUtils {
        Random RNG { get; set; }
        public RandomUtils() {
            RNG = new Random();
        }
        public bool[,] Choose2D (int[] rowsumsIn, int[] colsumsIn) {
            if (rowsumsIn.Sum() != colsumsIn.Sum()) {
                throw new InvalidOperationException("Your sums don't add up!");
            }
            var width = colsumsIn.Length;
            var colsums = new int[width];
            Array.Copy(colsumsIn, colsums, width);

            var height = rowsumsIn.Length;
            var rowsums = new int[height];
            Array.Copy(rowsumsIn, rowsums, height);
            
            var chosens = new bool[height, width];
            for (var row = 0; row < height; row++) {
                for (var col = 0; col < width; col++) {
                    if (colsums[col] == height - row) {
                        for (var i = row; i < height; i++) {
                            chosens[i, col] = true;
                            rowsums[i]--;
                        }
                        colsums[col] = 0;
                    } // we don't have to equally check for the weight 0 case, that works just fine without special handling 
                }
                var rowChoices = WeightedChoose(colsums, rowsums[row]);
                for (var col = 0; col < width; col++) {
                    if (rowChoices[col]) {
                        chosens[row, col] = true;
                        colsums[col]--;
                    }
                }
            }
            return chosens;            
        }
        public bool[] WeightedChoose (int[] weightsIn, int numTrues) {
            var weights = new int[weightsIn.Length];
            Array.Copy(weightsIn, weights, weights.Length);
            var weightsum = weights.Sum();
            var result = new bool[weights.Length];
            for (var i = 0; i < numTrues; i++) {
                weightsum = weights.Sum();
                var choice = RNG.Next(weightsum);
                var runningSum = 0;
                var j = 0;
                while (runningSum <= choice) {
                    runningSum += weights[j];
                    j++;
                }
                j--;
                result[j] = true;
                weightsum -= weights[j];
                weights[j] = 0;
            }
            return result;
        }
        // apparently the C# Random doesn't have an array shuffling function?
        // so i'll write one myself. fortunately fisher-yates is pretty simple
        public void Shuffle<T> (T[] deck) {
            for (var i = 0; i < deck.Count(); i++) {
                var j = RNG.Next(i+1);
                T value = deck[j];
                deck[j] = deck[i];
                deck[i] = value;
            }
        }

    }
}