using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;
using System.IO;

namespace BmpFuzzyMaker {


    public class Program {

        public const int TEST_X = 15;

        // The random seed
        public static RNGCryptoServiceProvider csp;
        public static Random ra;

        public static void Main(string[] args) {
            csp = new RNGCryptoServiceProvider();
            byte[] data = new byte[4];
            csp.GetBytes(data);
            int randomSeed = BitConverter.ToInt32(data, 0);
            ra = new Random(randomSeed);

            Bitmap img = new Bitmap(Directory.GetCurrentDirectory() + "\\test_32_32.bmp");
            byte[][] bmpData = new byte[img.Width][];
            for (int i = 0; i < bmpData.Length; i++) {
                bmpData[i] = new byte[img.Height];
            }
            for (int i = 0; i < img.Width; i++) {
                for (int j = 0; j < img.Height; j++) {
                    Color pixel = img.GetPixel(i, img.Height - j - 1);
                    bmpData[i][j] = pixel.R;
                }
            }

            // The test

            for (int i = 0; i < bmpData[TEST_X].Length; i++) {
                Console.WriteLine(bmpData[TEST_X][i]);
            }
            Console.WriteLine("****");
            int[] hitResult = new int[bmpData[0].Length];
            for (int i = 0; i < hitResult.Length; i++) {
                hitResult[i] = 0;
            }
            for (int i = 0; i < 10000; i++) {
                int fuzzyResult = BmpFuzzy(bmpData, TEST_X);
                hitResult[fuzzyResult] += 1;
            }
            for (int i = 0; i < hitResult.Length; i++) {
                Console.WriteLine(hitResult[i]);
            }
            Console.ReadKey();
        }

        /// <summary>
        /// This is an un scaled fuzzy. It needs an int x, 
        /// and returns an int y.
        /// </summary>
        public static int BmpFuzzy(byte[][] bmpData, int x) {
            if (x > bmpData.Length) {
                return -1;
            }
            return BmpColumFuzzy(bmpData[x]);
        }

        /// <summary>
        /// This will make the ColumData into an Probability function
        /// and random an Y value depend on that function
        /// </summary>
        /// <returns>The Y range is >0 so if returns -1 means no avaliable result </returns>
        public static int BmpColumFuzzy(byte[] bmpColumData) {
            int Sum = 0;
            int[] iColumData = new int[bmpColumData.Length];
            for (int i = 0; i < bmpColumData.Length; i++) {
                iColumData[i] = (int)bmpColumData[i];
                iColumData[i] += Sum;
                Sum = iColumData[i];
            }
            int randomNumber = ra.Next(0, Sum);
            for (int i = 0; i < iColumData.Length; i++) {
                if (randomNumber < iColumData[i]) {
                    return i;
                }
            }
            return -1;
        }
    }
}
