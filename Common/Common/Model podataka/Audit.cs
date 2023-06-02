using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Common.Enum;
namespace Common.Model_podataka
{
    [DataContract]
    public class Audit : DataBaseEntity
    {
       // private int id;
        private DateTime timeStamp;
        private TypeOfMessage messageType;
        private string message;

       // [DataMember]
        /*public int Id { get { return id; }
            set
            {
                id = value;
            } }
        */
        [DataMember]
        public string Message { get { return message; } set { message= value; } }
        [DataMember]
        public DateTime TimeStamp { get {  return timeStamp; } set {  timeStamp = value; } }
        [DataMember]
        public TypeOfMessage MessageType { get {  return messageType; } set { messageType = value; } }

        public Audit()
        {
            Id = 0;
            TimeStamp = DateTime.Now;
            MessageType = TypeOfMessage.Info;
            Message = "";
            
        }

        public  Audit(int id, DateTime timestamp, TypeOfMessage type, string mess)
        {
            Id = id;
            TimeStamp = timestamp;
            MessageType = type;
            Message = mess;
        }



    }
}
