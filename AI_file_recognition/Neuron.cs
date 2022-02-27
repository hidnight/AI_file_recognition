using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_file_recognition {
    static public class Neuron {
        private static double[] weights = new double[256];
        static public void initializeRandomWeights() {
            Random rng = new Random();
            for (int i = 0; i < 256; i++) {
                weights[i] = (rng.NextDouble()) * 20 - 10;
            }
        }
        public static double[] Weights {
            get {
                return weights;
            }
            set {
                weights = value;
            }
        }
        static public bool GetOutput(double[] input) {
            if (input.Length != 256)
                throw new ArgumentException("Array must have 256 elements");
            double sum = 0.0;
            for(int i = 0; i < 256; i++) {
                sum += weights[i] * input[i];
            }
            if (sum > 0) {
                return true;
            } else {
                return false;
            }
        }

        static public void AdjustWeights(double[] input, bool minus) {
            double c = minus ? -0.2 : 0.2;
            for (int i = 0; i < 256; i++) {
                weights[i] += 2 * c * input[i];
            }
        }
    }
}
