using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model_podataka
{
    [DataContract]
    public class ImportedFile : DataBaseEntity
    {
        
        private string fileName;


        [DataMember]
        public string FileName { get { return fileName; } set { fileName = value; } }

        public ImportedFile() {

            Id = 0;
            FileName = string.Empty;
        }

        public ImportedFile(int id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }

    }
}
