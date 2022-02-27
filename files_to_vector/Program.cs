using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace files_to_vector {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 1) {
                string[] filePaths;
                try {
                    filePaths = Directory.GetFiles(args[0]);
                } catch(Exception e) {
                    System.Console.WriteLine(e.Message);
                    return;
                }
                foreach (string filePath in filePaths) {
                    byte[] bytes;
                    try {
                        bytes = File.ReadAllBytes(filePath);
                    } catch (Exception e) {
                        System.Console.WriteLine(e.Message);
                        continue;
                    }
                    double[] vector = new double[256];
                    foreach(byte b in bytes) vector[b]++;
                    for (int i = 0; i < 256; i++) {
                        vector[i] /= bytes.Length;
                    }
                    try {
                        File.WriteAllText(filePath + ".vcr",
                            // F - fixed-point format specifier
                            // 17 - precision specifier
                            // en-CA - '.' as decimal separator
                            string.Join(" ", vector.Select(x => x.ToString("F17", CultureInfo.CreateSpecificCulture("en-CA"))).ToArray()));
                    } catch (Exception e) {
                        System.Console.WriteLine(e.Message);
                    }
                }
            } else {
                System.Console.WriteLine("Usage: files_to_vector.exe directory");
            }
        }
    }
}
