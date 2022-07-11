using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;

namespace ComtradeConsole
{
    public class CfgInfo
    {
        public float Calibration { get; set; }
        public float Offset { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int n = 0;
            int chanelAmount = 0;
            int analogChAmount = 0;
            List <string[]> Data= new List<string[]>();
            List<CfgInfo> Info = new List<CfgInfo>();
            Console.WriteLine("Введите путь к CFG файлу:");
            string cgfPath = Console.ReadLine();
            Console.WriteLine("Введите путь к DAT файлу:");
            string dataPath = Console.ReadLine();

            using (StreamReader reader = new StreamReader(cgfPath))
            {
                string line;
                reader.ReadLine();
                line = reader.ReadLine();
                chanelAmount = Convert.ToInt32(line.Split(',')[0]);
                analogChAmount = Convert.ToInt32(line.Split(',')[1].TrimEnd('A'));
                for (int i = 0; i < analogChAmount; i++)
                {
                    line = reader.ReadLine();
                    var record = new CfgInfo
                    {
                        Calibration = Convert.ToSingle(line.Split(',', StringSplitOptions.None)[5].Replace('.', ',')),
                        Offset = Convert.ToSingle(line.Split(',', StringSplitOptions.None)[6].Replace('.', ',')),
                    };
                    Info.Add(record);
                }

            }

            using (StreamReader reader = new StreamReader(dataPath))
            {
                while (reader.ReadLine() != null)
                    n++;
            }

            using (StreamReader reader = new StreamReader(dataPath))
            {
                string line;
                for (int i = 0; i < n - 1; i++)
                {
                    line = reader.ReadLine();
                    Data.Add(line.Split(','));
                }
            }

            Console.WriteLine("Общее количество сигналов: " + chanelAmount);
            Console.WriteLine("Количество аналоговых сигналов: " + analogChAmount);

            Console.WriteLine("Введите путь и название выходного CSV файла:");
            string inputPath = Console.ReadLine();

            List<string[]> InputData = new List<string[]>();
            for (int i = 0; i < n - 1; i++)           
            {
                var record = new string[analogChAmount + 2];
                record[0] = int.Parse(Data[i][0]).ToString();
                record[1] = int.Parse(Data[i][1]).ToString();
                for (int k = 2; k < analogChAmount + 2; k++)
                {
                    record[k] = (Convert.ToInt32(Data[i][k]) * Info[k - 2].Calibration + Info[k - 2].Offset).ToString().Replace(',','.');
                }
                InputData.Add(record);                
            }

            try
            {
                StreamWriter sw = new StreamWriter(inputPath);      
                foreach (var item in InputData)
                {
                    string csvLine = string.Empty;
                    for (int i = 0; i < item.Length - 1; i++)
                    {
                        csvLine += item[i] + ", ";
                    }
                    csvLine += item[item.Length - 1];
                    sw.WriteLine(csvLine);
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("CSV файл создан.");
            }
        }
    }
}
