using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model_podataka
{
    [DataContract]
    public class Load : DataBaseEntity
    {
        //private int id;
        private DateTime timeStamp = new DateTime();
        private double forecastValue;
        private double measuredValue;
        /*[DataMember]
        public int Id { get { return id; } set {  id = value; } }
        */
        [DataMember]
        public DateTime TimeStamp { get { return timeStamp; } set {  timeStamp = value; } }
        [DataMember]
        public double ForecastValue { get {  return forecastValue; } set {  forecastValue = value; } }
        [DataMember]
        public double MeasuredValue { get { return measuredValue; } set {  measuredValue = value; } }

        public Load()
        {
            Id = 0;
            TimeStamp = DateTime.Now;
            ForecastValue = 0;
            MeasuredValue = 0;
        }

        public Load(int id, DateTime timeStamp, double forecastValue, double measuredValue)
        {
            Id = id;
            TimeStamp = timeStamp;
            ForecastValue = forecastValue;
            MeasuredValue = measuredValue;
            
        }

        public override string ToString()
        {
            return $"Id: {Id}, timestamp: {TimeStamp}, forecast value: {ForecastValue}, measuredValue: {MeasuredValue}";
        }
    }
}
