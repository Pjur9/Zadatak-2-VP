using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Model_podataka;

namespace ClientReceiver
{
    public  class Program : IFileSend
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Program));
            host.Open();
            Console.WriteLine("Pokrenut server...");
            Console.ReadKey();
            host.Close();
            Console.WriteLine("Service is closed");
        }

        public void ReceiveFiles(List<MemoryStream> file)
        {
            string path = "C:\\Users\\User\\OneDrive\\Desktop\\CSV\\";
            for (int i = 0; i < file.Count; i++)
            {
                string paths = path + "data"+ Convert.ToString(i) + ".csv";
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    file[i].CopyTo(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }
                string fileContent = Encoding.UTF8.GetString(fileBytes);
                string[] lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                List<string[]> csvData = lines.Select(line => line.Split(',')).ToList();
                using (var writer = new StreamWriter(paths))
                {
                    writer.WriteLine(string.Join(",", "DATE", "TIME", "FORECAST_VALUE", "MEASURED_VALUE"));
                    foreach (string[] fields in csvData)
                    {
                        writer.WriteLine(string.Join(",", fields));
                    }
                }

                paths = "";

                
            }
        }

       
    }
}
