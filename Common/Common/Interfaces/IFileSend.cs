using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface IFileSend
    {
        [OperationContract]
        void ReceiveFiles(List<MemoryStream> file);
    }
}
