using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.ServiceModel.Description;
using System.Threading;
using System.Xml;
using Common.Interfaces;
using Common.Model_podataka;
using Service.Events_and_delegate;

namespace Service
{

    public class FileHandler : IFileHandling
    {

        public static Dictionary<string, List<Load>> load_obj = new Dictionary<string, List<Load>>();
        public static int cnt = 0;
        public static int cnt1 = 0;
        public static DateTime time;
        public static List<Load> temp = new List<Load>();
        public static List<Audit> audit = new List<Audit>();
        public delegate void DataSetEventHandler(object obj, EventArgsCsv args);
        public static event DataSetEventHandler DataSet;
        public static List<ImportedFile> importedFiles = new List<ImportedFile>();
        public static List<MemoryStream> file = new List<MemoryStream>();
        public static List<Load> checker = new List<Load>();
        public static bool promjena = true;
        public List<MemoryStream> AcceptData(string document, string name)
        {
            
            
            EventArgsCsv.SendTo(document, name);

            return file;
        }

        public static Dictionary<string, List<Load>> LoadAllFiles(List<Load> poml)
        {
            List<Load> pom = new List<Load>();

            if (cnt == 0)
            {
                checker = poml;
                pom = poml;
                cnt++;
            }
            else
            {
                
                    for(int j = 0; j < poml.Count; j++)
                    {
                        if (checker.Contains(poml[j]))
                    {
                        ;
                    }
                    else
                    {
                        pom.Add(poml[j]);
                        Console.WriteLine(pom[j]);  
                    }
                    }
                
            }
            if (pom.Count == 0)
            {
                promjena = false;
                ;
            }
            else
            {
                promjena = true;
                for (int i = 0; i < pom.Count; i++)
                {
                    string key = pom[i].TimeStamp.ToString("H:mm");

                    key = key.Split(':')[0];
                    key = key.Split('.')[0];

                    if (load_obj.ContainsKey(key))
                    {
                        if (load_obj[key].Count == 0)
                        {
                            load_obj[key].Add(pom[i]);
                        }
                        else
                        {

                            for (int j = 0; j < load_obj[key].Count; j++)
                            {

                                if (load_obj[key][j].TimeStamp.Equals(pom[i].TimeStamp))
                                {

                                    ;
                                }
                                else
                                {
                                    if (load_obj[key].Contains(pom[i]))
                                    {
                                        ;
                                    }
                                    else
                                    {
                                        load_obj[key].Add(pom[i]);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        List<Load> list = new List<Load>();
                        load_obj.Add(key, list);
                        load_obj[key].Add(pom[i]);

                    }
                }

            }
                DataBase.SendToDB(load_obj);


                return load_obj;
            
        }

        public static Load ExtractLoadFromXmlNode(XmlNode row)
        {
            
            string timeStampString = row.SelectSingleNode("TIME_STAMP").InnerText;

            
            try
            {
                DateTime timeStamp = Convert.ToDateTime(timeStampString);

                double forecastValue = double.Parse(row.SelectSingleNode("FORECAST_VALUE").InnerText, CultureInfo.InvariantCulture);

                double measuredValue = double.Parse(row.SelectSingleNode("MEASURED_VALUE").InnerText, CultureInfo.InvariantCulture);

                Load load = new Load
                {
                    Id = temp.Count,
                    TimeStamp = timeStamp,
                    ForecastValue = forecastValue,
                    MeasuredValue = measuredValue
                };
                return load;

            }
            catch(Exception e)
            {
               Console.WriteLine(e.Message);
                throw new Exception("Greska u Xml datoteci");
            }
            
        }

        public static  void Provjera(string document, string name)
        {
            XmlDocument xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.LoadXml(document);

                // Provera strukture XML datoteke
                if (!IsValidXmlStructure(xmlDocument))
                {
                    Console.WriteLine("Greška u strukturi XML datoteke.");
                    // Kreirajte i upišite Audit objekat u bazu podataka

                    // ...
                    return;
                }

                XmlElement rootElement = xmlDocument.DocumentElement;
                XmlNodeList rows = rootElement.GetElementsByTagName("row");
                XmlNodeReader reader = new XmlNodeReader(xmlDocument);
               
                foreach (XmlNode row in rows)
                {
                    reader.Read();
                    Load load = ExtractLoadFromXmlNode(row);
                    Type tip = load.Id.GetType();
                    Type tipFV = load.ForecastValue.GetType();
                    Type tipMV = load.MeasuredValue.GetType();

                    if (tip == typeof(double) || tip == typeof(float))
                    {
                        audit.Add(new Audit(load.Id, load.TimeStamp, Common.Enum.TypeOfMessage.Warning, "Upozorenje: Odstupanje od predvidjenog tipa/\n"));
                        DataBase.AddData("Audit", audit[load.Id]);
                        continue;
                    }

                    if (tipFV == typeof(int) || tipMV == typeof(int))
                    {
                        audit.Add(new Audit(load.Id, load.TimeStamp, Common.Enum.TypeOfMessage.Warning, "Upozorenje: Odstupanje od predvidjenog tipa/\n"));
                        DataBase.AddData("Audit", audit[load.Id]);
                        continue;
                    }

                    // Provera nedostajućih podataka
                    if (!HasRequiredData(load))
                    {
                  
                        audit.Add(new Audit(load.Id, load.TimeStamp, Common.Enum.TypeOfMessage.Error, "Nedostaju neki podaci"));
                        DataBase.AddData("Audit", audit[load.Id]);
                        continue;
                    }

                    // Validacija podataka
                    if (!ValidateLoadData(load))
                    {
                        audit.Add(new Audit(load.Id, load.TimeStamp, Common.Enum.TypeOfMessage.Error, "Nevalidni podaci"));
                        DataBase.AddData("Audit", audit[load.Id]);
                        continue;
                    }

                   
                    
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == rootElement.Name)
                    {
                        Console.WriteLine("End of Xml document reached");
                        temp.Add(load);
                        //Console.WriteLine(load.ToString());
                        audit.Add(new Audit(load.Id, load.TimeStamp, Common.Enum.TypeOfMessage.Warning, "Upozorenje: Ucitan poslednji objekat!\n"));
                        DataBase.AddData("Audit", audit[load.Id]);
                    }
                    else
                    {
                        temp.Add(load);
                        audit.Add(new Audit(load.Id, load.TimeStamp, Common.Enum.TypeOfMessage.Info, "Validan!\n"));
                        DataBase.AddData("Audit", audit[load.Id]);
                    }
                }
               

            }
            catch (XmlException ex)
            {
                Console.WriteLine("Greška pri učitavanju XML dokumenta: " + ex.Message);
                // Kreirajte i upišite Audit objekat u bazu podataka
                // ...
                

            }
           
            importedFiles.Add(new ImportedFile(importedFiles.Count, name));
            ImportedFile file = importedFiles[importedFiles.Count - 1];
            DataBase.AddData("ImportedFiles", file);

            LoadAllFiles(temp);
            DataBase.ShowData();
            OnDataSet();
        }


        private static bool IsValidXmlStructure(XmlDocument xmlDocument)
        {
            XmlNodeList rowElements = xmlDocument.DocumentElement.GetElementsByTagName("row");

            // Provera da li postoje elementi "row" u dokumentu
            if (rowElements.Count == 0 || xmlDocument.DocumentElement?.Name != "rows")
            {
                return false;
            }

            foreach (XmlNode rowElement in rowElements)
            {
                // Provera da li postoje sva neophodna polja unutar elementa "row"
                if (!rowElement.HasChildNodes || rowElement.ChildNodes.Count != 3)
                {
                    return false;
                }

                foreach (XmlNode childNode in rowElement.ChildNodes)
                {
                    if (childNode.Name != "TIME_STAMP" && childNode.Name != "FORECAST_VALUE" && childNode.Name != "MEASURED_VALUE")
                    {
                        return false;
                    }

                    // Provera da li je polje prazno
                    if (string.IsNullOrWhiteSpace(childNode.InnerText))
                    {
                        return false;

                        
                    }
                }
            }

            return true;
        }
        public static bool HasRequiredData(Load load)
        {

            if (string.IsNullOrEmpty(Convert.ToString(load.Id)) ||
                load.TimeStamp == DateTime.MinValue ||
                load.ForecastValue == 0 ||
                load.MeasuredValue == 0)
            {//dodatno
                return false;
            }

            return true;
        }

        private static bool ValidateLoadData(Load load)
        {
            DateTime timestamp = load.TimeStamp;
            int hour = timestamp.Hour;
            int minutes = timestamp.Minute;


            
            // Provera da li su sati validni
            if (hour >= 24)
            {
                return false; // Sat ne sme biti veći ili jednak 24
            }
            //Provjera da li su minute validne
            if(minutes<0 || minutes > 59)
            {
                return false;
            }
            // Provera da vrednosti ForecastValue i MeasuredValue nisu negativne
            if (load.ForecastValue < 0 || load.MeasuredValue < 0)
            {
                return false;
            }

            // Primer provere za tip polja "FORECAST_VALUE" (float ili decimal)
            if (!double.TryParse(load.ForecastValue.ToString(), out _))
            {
                return false; // Nevalidan tip podatka za "FORECAST_VALUE"
            }

            // Primer provere za tip polja "MEASURED_VALUE" (float ili decimal)
            if (!double.TryParse(load.MeasuredValue.ToString(), out _))
            {
                return false; // Nevalidan tip podatka za "MEASURED_VALUE"
            }

            return true;
        }

        public static  void OnDataSet()
        {
            if (DataSet != null)
            {
                DataSet(new FileHandler(), new EventArgsCsv{ Files = load_obj, load = temp });
            }
           
        }
        
        public static void Prihvati(List<MemoryStream> fajl)
        {
            file = fajl;
            load_obj.Clear();
        }
    }
}
