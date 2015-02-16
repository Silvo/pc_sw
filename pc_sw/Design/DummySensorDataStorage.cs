using pc_sw.Helpers;
using pc_sw.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Design
{
    public class DummySensorDataStorage : IDataStorage
    {
        private ISensorDataProvider _source;
        List<DataSource> _sensors;

        public event EventHandler SourceAdded;

        public DummySensorDataStorage(ISensorDataProvider source)
        {
            _source = source;
            _sensors = new List<DataSource>(8);

            //_source.DataReceived += _source_DataReceived;
            _source.DataReceived += _source_DataReceived;
            LoadData();
        }

        void _source_DataReceived(object sender, EventArgs e)
        {
            DataReceivedEventArgs dr_e = (DataReceivedEventArgs)e;
            SensorDataSample sample = dr_e.DataSample;
            SByte timeOffset = (SByte)TimeZone.CurrentTimeZone.GetUtcOffset(
                UnixTime.ToDateTime(sample.Timestamp)).TotalHours;

            Tuple<DateTime, float> s = new Tuple<DateTime, float>(
                UnixTime.ToDateTime(sample.Timestamp + timeOffset), sample.ToFloat());

            var sensor_query = _sensors.Where(x => x.SensorId == sample.SensorId);

            if (sensor_query.Any())
            {
                var sensor = sensor_query.ElementAt(0);

                if (sensor.Samples.Contains(s) == false)
                {
                    sensor.AddSample(s);
                }
            }
            else
            {
                List<Tuple<DateTime, float>> sampleList = new List<Tuple<DateTime, float>>();
                sampleList.Add(s);
                DataSource newSource = new DataSource(sample.SensorId, sample.SensorType, sampleList);

                _sensors.Add(newSource);

                if (SourceAdded != null)
                {
                    SourceAdded(this, new SourceAddedEventArgs(newSource));
                }
            }
        }

        private void LoadData()
        {
            List<SensorDataSample> data = new List<SensorDataSample>(1024);
            DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);//new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            foreach (FileInfo file in dir.GetFiles("*.mlg"))
            {
                Console.WriteLine("File {0}", file.FullName);
                LoadDataFromFile(file.FullName, data);
            }

            for (int i = 0; i < 8; i++)
            {
                var query = data.Where(s => s.SensorId == i);

                if (query.Count() != 0)
                {
                    SensorDataSample s = query.ElementAt(0);
                    List<Tuple<DateTime, float>> sensorData = new List<Tuple<DateTime, float>>();

                    foreach (SensorDataSample sample in query)
                    {
                        SByte timeOffset = (SByte)TimeZone.CurrentTimeZone.GetUtcOffset(
                            UnixTime.ToDateTime(sample.Timestamp)).TotalHours;

                        sensorData.Add(new Tuple<DateTime, float>(
                            UnixTime.ToDateTime(sample.Timestamp + timeOffset), sample.ToFloat()));
                    }

                    _sensors.Add(new DataSource(s.SensorId, s.SensorType, sensorData));
                }
            }
        }

        private void LoadDataFromFile(string fileName, List<SensorDataSample> data)
        {
            using (var br = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None)))
            {
                const int increment = sizeof(int) + sizeof(Int16) + sizeof(byte) + sizeof(byte);

                for (long i = 0; i < br.BaseStream.Length; i += increment)
                {
                    Int32 timeStamp = br.ReadInt32();
                    Int16 sampleData = br.ReadInt16();
                    byte sensorId = br.ReadByte();
                    SensorType sensorType = (SensorType)br.ReadByte();
                    data.Add(new SensorDataSample()
                    {
                        Timestamp = timeStamp,
                        Data = sampleData,
                        SensorId = sensorId,
                        SensorType = sensorType
                    });
                }
            }
        }
        public List<DataSource> GetDataSources()
        {
            return _sensors;
        }

        
    }
}
