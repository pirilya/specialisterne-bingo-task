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
            var weights = new int[width];
            Array.Copy(colsums, weights, width);
            var rowsums = new int[height];
            Array.Fill(rowsums, rowsum);
            var chosens = new bool[height, width];
            for (var row = 0; row < height; row++) {
                for (var col = 0; col < width; col++) {
                    if (weights[col] == height - row) {
                        for (var i = row; i < height; i++) {
                            chosens[i, col] = true;
                            rowsums[i]--;
                        }
                        weights[col] = 0;
                    } // we don't have to equally check for the weight 0 case, that works just fine without special handling 
                }
                var rowChoices = WeightedChoose(weights, rowsums[row]);
                for (var col = 0; col < width; col++) {
                    if (rowChoices[col]) {
                        chosens[row, col] = true;
                        weights[col]--;
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
                var choice = Random.Next(weightsum);
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
                var j = Random.Next(i+1);
                T value = deck[j];
                deck[j] = deck[i];
                deck[i] = value;
            }
        }

    }
}