using Common.Model_podataka;
using Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DataBase
    {
        public static Dictionary<Int32, Load> load_DB = new Dictionary<Int32, Load>();
        public static Dictionary<Int32, Audit> audit_DB = new Dictionary<Int32, Audit>();
        public static Dictionary<Int32, ImportedFile> importedFIle_DB = new Dictionary<Int32, ImportedFile>();
        public static int brojac=0;


        public static Dictionary<string, Dictionary<Int32, DataBaseEntity>> database = new Dictionary<string, Dictionary<Int32, DataBaseEntity>>();
        

        public static void SendToDB(Dictionary<string, List<Load>> file)
        {
            foreach(string key in file.Keys)
            {
                foreach(Load l in file[key])
                {
                    AddData("Load", l);
                    //Load novi = GetData<Load>("Load", l.Id);
                   // Console.WriteLine(novi.ToString());
                }
            }
        }
        public static void AddData(string name, DataBaseEntity entity)
        {

            if (!database.ContainsKey(name))
            {
                database.Add(name, new Dictionary<int, DataBaseEntity>());
            }

            var table = database[name];
            if (table.ContainsKey(entity.Id))
            {
                Console.WriteLine("Entity with ID {0} already exists, updating entity", entity.Id);
                table[entity.Id] = entity;
            }
            else
            {
               // Console.WriteLine("Dodat je novi file");
                table.Add(entity.Id, entity);
            }


        }
        
        public static T GetData<T>(string name, int id) where T : DataBaseEntity
        {
            if (database.TryGetValue(name, out var table) && table.TryGetValue(id, out var entity) && entity is T typedEntity)
            {
                return typedEntity;
            }
            return null;
        }

        public static void ShowData()
        {
            foreach (string key in database.Keys)
            {
                if (key == "Audit")
                {
                    Console.WriteLine("TABELA: AUDIT");
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("ID  | TIMESTAMP          |TYPE OF MESSAGE | MESSAGE");
                    foreach(Audit a in database[key].Values)
                    {
                        Console.WriteLine($"{a.Id}   {a.TimeStamp}           {a.MessageType}        {a.Message}");
                    }
                }
                else if (key == "ImportedFiles")
                {
                    Console.WriteLine("TABELA: IMPORTED FILES");
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("ID | NAME");
                    foreach (ImportedFile f in database[key].Values)
                    {
                        Console.WriteLine($"{f.Id}   {f.FileName} ");
                    }

                }
                else if (key == "Load")
                {
                    Console.WriteLine("TABELA: LOAD");
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("ID  | TIMESTAMP        |FORECAST_VALUE | MEASURED_VALUE");
                    foreach (Load l in database[key].Values)
                    {
                        brojac++;
                        Console.WriteLine($"{brojac}   {l.TimeStamp}           {l.ForecastValue}    {l.MeasuredValue}");
                    }
                }
            }
            brojac = 0;
        }
    }
}



