using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Model
{
    public class DataSource
    {
        public delegate void SampleAddedEventHandler(object sender, SampleAddedEventArgs e);
        public event SampleAddedEventHandler SampleAdded;

        public List<Tuple<DateTime, float>> Samples { get; private set; }
        public SensorType SensorType { get; set; }
        public byte SensorId { get; set; }

        public DataSource(byte id)
        {
            SensorId = id;
        }

        public DataSource(byte id, SensorType type, List<Tuple<DateTime, float>> samples)
        {
            SensorId = id;
            SensorType = type;
            Samples = samples;
        }

        public void AddSample(Tuple<DateTime, float> sample)
        {
            Samples.Add(sample);
            SampleAddedEventArgs e = new SampleAddedEventArgs(sample);

            if(SampleAdded != null)
            {
                SampleAdded(this, e);
            }
        }

        public override bool Equals(Object obj)
        {
            DataSource dataSource = obj as DataSource;
            
            if (dataSource == null)
                return false;
            else
                return SensorId.Equals(dataSource.SensorId);
        }
    }
}
