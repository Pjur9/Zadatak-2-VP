using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common.Interfaces;
namespace ServiceSender
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
        }

        public static void CallMethod(List<MemoryStream> file)
        {
            SendToClient(file);
        }
        public static void SendToClient(List<MemoryStream> file)
        {
            ChannelFactory<IFileSend> factory = new ChannelFactory<IFileSend>("CsvSending");
            IFileSend proxy = factory.CreateChannel();

            proxy.ReceiveFiles(file);
        }
    }
}
