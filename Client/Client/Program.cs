using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common.Interfaces;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Client
{
    public class Program
    {
        private static bool IsExecuting {  get; set; }
        public static int cnt=0;
        public static int cnt1=0;
        public static DateTime time;
        static void Main(string[] args)
        {
            var XmlPath = ConfigurationManager.AppSettings["uploadPath"];

            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = XmlPath;

            watcher.NotifyFilter=NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.IncludeSubdirectories = true;

            watcher.Filter = "*.xml";

            watcher.EnableRaisingEvents = true;

            watcher.Created += OnFileCreated;
            watcher.Changed += new FileSystemEventHandler( OnChanged);

            Console.WriteLine("Press [Enter] to quit...");
            Console.ReadLine();

            watcher.EnableRaisingEvents=false;
        }

        private static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Dodata je nova datoteka: " + e.Name + "\n\n");

        }

        public static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (cnt1 == 0)
            {
                cnt1++;
                Console.WriteLine("Izmjenjena je datoteka: " + e.Name + " " + e.FullPath);
                ChannelFactory<IFileHandling> factory = new ChannelFactory<IFileHandling>("XmlSending");
                IFileHandling proxy = factory.CreateChannel();
                XmlDocument xml = new XmlDocument();
                xml.Load(e.FullPath);
                string xmlString = xml.OuterXml;

                
                List<MemoryStream> file= proxy.AcceptData(xmlString, e.Name);
                string path = ConfigurationManager.AppSettings["uploadCsv"];
                
                for (int i = 0; i < file.Count; i++)
                {
                    string paths = path + "data" + Convert.ToString(i) + ".csv";
                    Console.WriteLine(paths);
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
                        
                        foreach (string[] fields in csvData)
                        {
                            writer.WriteLine(string.Join(",", fields));
                        }
                    }

                    paths = "";


                }


                factory.Close();
            }
            else
            {
                cnt1 = 0;
            }

        }

    }
}
