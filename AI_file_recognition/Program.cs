using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_file_recognition {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 3 || args.Length == 2) {
                string controlDir;
                if (args.Length == 3) {
                    int limit = 10000, i = 0;
                    double result = 0.0;
                    Neuron.initializeRandomWeights();
                    while (result < 0.9 && i < limit) {
                        result = Training(args[0], args[1]);
                        System.Console.WriteLine("Training pass " + i++ + " complete with success rate of " + result);
                    }
                    controlDir = args[2];
                    try {
                        File.WriteAllText("weights.txt",
                            // F - fixed-point format specifier
                            // 17 - precision specifier
                            // en-CA - '.' as decimal separator
                            string.Join(" ", Neuron.Weights.Select(x => x.ToString("F17", CultureInfo.CreateSpecificCulture("en-CA"))).ToArray()));
                        System.Console.WriteLine("Weights saved to weights.txt");
                    } catch (Exception e) {
                        System.Console.WriteLine(e.Message);
                    }
                } else {
                    controlDir = args[1];
                    try {
                        Neuron.Weights = File.ReadAllText(args[0]).Split(' ').Select(str => double.Parse(str, CultureInfo.CreateSpecificCulture("en-CA"))).ToArray();
                    } catch (Exception e) {
                        System.Console.WriteLine(args[0] + " failed with error: " + e.Message);
                        return;
                    }
                }
                string[] ctlFilePaths;
                try {
                    ctlFilePaths = Directory.GetFiles(controlDir);
                } catch (Exception e) {
                    System.Console.WriteLine(e.Message);
                    return;
                }
                foreach (string file in ctlFilePaths) {
                    if (!file.Contains(".vcr"))
                        System.Console.WriteLine(file + " is not .vcr");
                    double[] input;
                    try {
                        input = File.ReadAllText(file).Split(' ').Select(str => double.Parse(str, CultureInfo.CreateSpecificCulture("en-CA"))).ToArray();
                    } catch (Exception e) {
                        System.Console.WriteLine(file + " failed with error: " + e.Message);
                        continue;
                    }
                    string ans = "";
                    if (!Neuron.GetOutput(input)) {
                        ans = " not";
                    }
                    System.Console.WriteLine(file + " is{0} a pdf", ans);
                }
                System.Console.ReadKey();

            } else {
                System.Console.WriteLine("Usage:\n1. AI_file_recognition.exe directory_positive directory_negative directory_control");
                System.Console.WriteLine("2. AI_file_recognition.exe weights.txt directory_control");
                System.Console.WriteLine("directory_positive - directory with vectors of files with target type");
                System.Console.WriteLine("directory_negative - directory with vectors of files with any type other then target");
                System.Console.WriteLine("directory_control - directory with vectors of files of any type for control purpose");
                System.Console.WriteLine("weights.txt - text file with 256 weights separated by \' \'");
            }
        }

        static double Training(string dirPos, string dirNeg) {
            string[] posFilePaths;
            string[] negFilePaths;
            try {
                posFilePaths = Directory.GetFiles(dirPos);
                negFilePaths = Directory.GetFiles(dirNeg);
            } catch (Exception e) {
                System.Console.WriteLine(e.Message);
                return -1.0;
            }
            ulong currect = 0, processed = 0;
            foreach (string file in posFilePaths) {
                if (!file.Contains(".vcr")) {
                    System.Console.WriteLine(file + " is not .vcr. Skipping");
                    continue;
                }
                double[] input;
                try {
                    input = File.ReadAllText(file).Split(' ').Select(str => double.Parse(str, CultureInfo.CreateSpecificCulture("en-CA"))).ToArray();
                } catch (Exception e) {
                    System.Console.WriteLine(file + " failed with error: " + e.Message);
                    continue; 
                }
                if (Neuron.GetOutput(input)) {
                    currect++;
                } else {
                    Neuron.AdjustWeights(input, false);
                }
                processed++;
            }
            foreach (string file in negFilePaths) {
                if (!file.Contains(".vcr")) {
                    System.Console.WriteLine(file + " is not .vcr. Skipping");
                    continue;
                }
                double[] input;
                try {
                    input = File.ReadAllText(file).Split(' ').Select(str => double.Parse(str, CultureInfo.CreateSpecificCulture("en-CA"))).ToArray();
                } catch (Exception e) {
                    System.Console.WriteLine(file + " failed with error: " + e.Message);
                    continue;
                }
                if (!Neuron.GetOutput(input)) {
                    currect++;
                } else {
                    Neuron.AdjustWeights(input, true);
                }
                processed++;
            }
            if (processed == 0.0)
                return -1.0; 
            return (double)currect / processed;
        }
    }
}
