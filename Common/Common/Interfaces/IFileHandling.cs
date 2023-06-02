using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface IFileHandling
    {
        

        [OperationContract]
        List<MemoryStream> AcceptData(string document, string name);

        
    }
}
