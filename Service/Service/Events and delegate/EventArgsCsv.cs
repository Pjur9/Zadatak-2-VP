using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Common.Model_podataka;
namespace Service.Events_and_delegate
{
    public class EventArgsCsv 
    {
        // public string EventName { get; set; }

        public Dictionary<string, List<Load>> Files = new Dictionary<string, List<Load>>();
        public List<Load> load;
        public static void SendTo(string xml, string name)
        {
            FileHandler.DataSet += OnDataSet;
            FileHandler.Provjera(xml, name);
        }
       
        public static void OnDataSet(object obj, EventArgsCsv args)
        {

            CsvDataHandler.DataToCSV(args.load, args.Files);
        }
    }
}
