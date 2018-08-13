using System;
using System.Collections.Generic;
using System.IO;

namespace ai_learning
{

    /**
    */
    public class NormalizeProgram : INormalization
    {
        /**
        Public constructor
        */
        public NormalizeProgram()
        {
            this.Init();
        }

        public string Dummyencoding(int index, int N)
        {
            int[] values = new int[N];
            values[index] = 1;

            string s = values[0].ToString();
            for (int i = 0; i < values.Length; ++i)
            {
                s += "," + values[i];
            }

            return s;
        }

        public string EffectsEncoding(int index, int N)
        {
            if (N == 2)
            {
                if (index == 0)
                {
                    return "-1";
                }
                else if (index == 1)
                {
                    return "1";
                }
            }

            int[] values = new int[N - 1];
            if (index == N - 1) // last imte is all
            {
                for (int i = 0; i < values.Length; ++i)
                {
                    values[i] = -1;
                }
            }
            else
            {
                values[index] = 1; // 0 values are already there. 
            }

            string s = values[0].ToString();
            for (int i = 1; i < values.Length; ++i)
            {
                s += "," + values[i];
            }

            return s;
        }

        public void EncodeFile(string originalFile, string encodedFile, int column, string encodingType)
        {
            // encoding Type
            FileStream ifs = new FileStream(originalFile, FileMode.Open);
            StreamReader sr = new StreamReader(ifs);
            string line = "";
            string[] tokens = null;

            Dictionary<string, int> d = new Dictionary<string, int>();
            int itemNum = 0;
            while ((line = sr.ReadLine()) != null)
            {
                tokens = line.Split(','); //Assumes items are comma-delimated
                if (d.ContainsKey(tokens[column]) == false)
                {
                    d.Add(tokens[column], itemNum++);
                }
            }

            sr.Close();  // close
            ifs.Close();  // close

            int N = d.Count;
            ifs = new FileStream(originalFile, FileMode.Open);
            sr = new StreamReader(ifs);
            FileStream ofs = new FileStream(encodedFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(ofs);
            string s = null;

            while ((line = sr.ReadLine()) != null)
            {
                s = "";
                tokens = line.Split(','); // break apart strings

                for (int i = 0; i < tokens.Length; i++)
                {
                    if (i == column)
                    {
                        int index = d[tokens[i]]; // 0, 1, 2
                        if (encodingType == "effects")
                        {
                            s += EffectsEncoding(index, N) + ",";
                        }
                        else if (encodingType == "dummy")
                        {
                            s += Dummyencoding(index, N);
                        }
                    }
                    else
                    {
                        s += tokens[i] + ",";
                    }
                }

                s.Remove(s.Length - 1); //remove trailing
                sw.WriteLine(s); // write the string to file
            }

            sw.Close();
            ofs.Close();
            sr.Close();
            ifs.Close();

        }

        public void GaussNormal(double[][] data, int column)
        {
            int j = column;
            double sum = 0.0;
            for (int i = 0; i < data.Length; ++i)
            {
                sum += data[i][j];
            }

            double mean = sum / data.Length;

            double sumSquares = 0.0;
            for (int i = 0; i < data.Length; ++i)
            {
                sumSquares += (data[i][j] - mean) * (data[i][j] - mean);
            }

            double stdDev = Math.Sqrt(sumSquares / data.Length);

            for (int i = 0; i < data.Length; ++i)
            {
                data[i][j] = (data[i][j] - mean) / stdDev;

                if (data[i][j] == 0)
                {
                    throw new NotFiniteNumberException("Standard Devation cannot be null");
                }
            }
        }

        public void MinMaxNormal(double[][] data, int column)
        {
            int j = column;
            double min = data[0][j];
            double max = data[0][j];

            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i][j] < min)
                {
                    min = data[i][j];
                }
                if (data[i][j] > max)
                {
                    max = data[i][j];
                }
            }


            double range = max - min;
            if (range == 0.0) // ugly
            {

                for (int i = 0; i < data.Length; ++i)
                {
                    data[i][j] = 0.5;
                }
                return;
            }

            if (Math.Abs(range) < 0.00000001)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    data[i][j] = (data[i][j] - min) / range;
                }
            }
        }

        public void Init()
        {
            Console.WriteLine("\nBegin data encoding and normliazaiton\n");
            string[] sourceData = new string[]{
                 "Sex Age Locale Income Politics",
                 "==============================================",
                 "Male 25 Rural 63,000.00 Conservative",
                 "Female 36 Suburban 55,000.00 Liberal",
                 "Male 40 Urban 74,000.00 Moderate",
                 "Female 23 Rural 28,000.00 Liberal"
             };

            // co
            Console.WriteLine("Dummy data in raw form:\n");
            ShowData(sourceData);

            // encode
            string[] encodeData = new string[]{
                 "-1 25 1 0 63,000.00 1 0 0",
                 " 1 36 0 1 55,000.00 0 1 0",
                 "-1 40 -1 -1 74,000.00 0 0 1",
                 " 1 23 1 0 28,000.00 0 1 0"
             };

            Console.WriteLine("\nData after categorical encoding:\n");
            ShowData(encodeData);


            Console.WriteLine("\nNumeric data stored in matrix:\n");
            double[][] numericData = new double[4][];
            numericData[0] = new double[] { -1, 25.0, 1, 0, 63000.00, 1, 0, 0 };
            numericData[1] = new double[] { 1, 36.0, 0, 1, 55000.00, 0, 1, 0 };
            numericData[2] = new double[] { -1, 40.0, -1, -1, 74000.00, 0, 0, 1 };
            numericData[3] = new double[] { 1, 23.0, 1, 0, 28000.00, 0, 1, 0 };

            // show marix
            ShowMatrix(numericData, 2);

            //normalization
            GaussNormal(numericData, 1);
            MinMaxNormal(numericData, 4);

            Console.WriteLine("\nMatrix after normalization (Gaussian col. 1 and MinMax col. 4):\n");
            Console.WriteLine("\nEnd data encoding and normalization demo\n");
            Console.ReadLine(); // main

        }

        public void ShowData(string[] rawData)
        {
            for (int i = 0; i < rawData.Length; ++i)
            {
                Console.WriteLine(rawData[i]);
            }
            Console.WriteLine("");
        }

        public void ShowMatrix(double[][] matrix, int decimals)
        {
            for (int i = 0; i < matrix.Length; ++i)
            {
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    double v = Math.Abs(matrix[i][j]);
                    if (matrix[i][j] >= 0.0)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write("-");
                    }
                    Console.Write(v.ToString("F" + decimals).PadRight(5) + " ");
                }
                Console.WriteLine("");
            }
        }
    }
}
