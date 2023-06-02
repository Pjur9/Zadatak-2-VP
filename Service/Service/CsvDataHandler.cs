using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Model_podataka;

namespace Service
{
    public class CsvDataHandler : IDisposable
    {
        public static FileStream fileStream;
        public static List<MemoryStream> fajlovi = new List<MemoryStream>();
        public static int counter1 = 0;
        public static  void DataToCSV(List<Load> fajl, Dictionary<string, List<Load>> lista)
        {
            if (FileHandler.promjena)
            {
                string path = "data.csv";
                int counter = 0;

                var csv_number = ConfigurationManager.AppSettings["NumberCsvFIles"];
                int csv_num = Convert.ToInt32(csv_number);
                if (csv_num <= 1)

                {

                    using (StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)))
                    {
                        sw.WriteLine(string.Join(",", "DATE", "TIME", "FORECAST_VALUE", "MEASURED_VALUE"));
                        foreach (string key in lista.Keys)
                        {
                            foreach (Load l in lista[key])
                            {
                                string date = l.TimeStamp.ToString("yyyy-MM-dd");
                                string time = l.TimeStamp.ToString("H:mm");
                                sw.WriteLine(string.Join(",", date, time, l.ForecastValue, l.MeasuredValue));
                            }
                        }
                        byte[] fileBytes = File.ReadAllBytes("C:\\Users\\User\\OneDrive\\Desktop\\Projekat VP-30.05\\Projekat VP\\Service\\Service\\bin\\Debug\\" + path);
                        MemoryStream stream = new MemoryStream(fileBytes);
                        stream.Position = 0;
                        fajlovi.Add(stream);
                    }

                }
                else
                {
                    //Console.WriteLine(csv_num + "   " + counter);
                    int csv = fajl.Count / csv_num;
                    int csv_moduo = counter % csv_num;
                    //Console.WriteLine(csv + "   " + csv_moduo);
                    List<Load> pom = new List<Load>();
                    for (int i = 0; i < fajl.Count; i++)
                    {
                        if (i > 0)
                        {
                            if (i % csv == 0)
                            {
                                counter1++;
                                if (counter1 == csv_num)
                                {
                                    for (int j = i; j <= i + csv_moduo; j++)
                                    {
                                        pom.Add(fajl[j]);

                                    }
                                    i += csv_moduo;
                                }
                                else
                                {
                                    ;
                                }
                                path = "data" + Convert.ToString(counter1) + ".csv";
                                using (StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)))
                                {
                                    sw.WriteLine(string.Join(",", "DATE", "TIME", "FORECAST_VALUE", "MEASURED_VALUE"));
                                    foreach (Load l in pom)
                                    {
                                        string date = l.TimeStamp.ToString("yyyy-MM-dd");
                                        string time = l.TimeStamp.ToString("H:mm");
                                        sw.WriteLine(string.Join(",", date, time, l.ForecastValue, l.MeasuredValue));
                                    }


                                }
                                Console.WriteLine("C:\\Users\\User\\OneDrive\\Desktop\\Projekat VP - 30.05\\Projekat VP\\Service\\Service\\bin\\Debug\\" + path);
                                byte[] fileBytes = File.ReadAllBytes("C:\\Users\\User\\OneDrive\\Desktop\\Projekat VP-30.05\\Projekat VP\\Service\\Service\\bin\\Debug\\" + path);
                                MemoryStream stream = new MemoryStream(fileBytes);
                                stream.Position = 0;
                                fajlovi.Add(stream);
                                path = "";
                                pom.Clear();
                                pom.Add(fajl[i]);
                            }
                            else
                            {
                                pom.Add(fajl[i]);
                            }
                        }
                        else
                        {
                            pom.Add(fajl[i]);
                        }
                    }
                    pom.Clear();
                }
            }
            else
            {
                ;
            }

           GetCsvFiles();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public static void GetCsvFiles()
        {

            FileHandler.Prihvati(fajlovi);
        }

        
    }
}
