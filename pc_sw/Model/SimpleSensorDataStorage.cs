using GalaSoft.MvvmLight.Messaging;
using pc_sw.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Model
{
    public class SimpleSensorDataStorage : IDataStorage
    {
        private ISensorDataProvider _source;
        private List<DataSource> _sensors;
        
        public SimpleSensorDataStorage(ISensorDataProvider source)
        {
            _source = source;
            _sensors = new List<DataSource>(8);

            _source.DataReceived += _source_DataReceived;
            LoadData();
            System.Console.Write(UnixTime.FromDateTime(new DateTime(2014, 3, 15)).ToString());
        }

        private String GetFileName(DateTime time)
        {
            String fileName = time.ToString("") + ".mlg";

            return fileName;
        }

        private void _source_DataReceived(object sender, EventArgs e)
        {
            DataReceivedEventArgs dr_e = (DataReceivedEventArgs)e;
            SensorDataSample sample = dr_e.DataSample;
            Int32 timeOffset = (Int32)(3600 * TimeZoneInfo.Local.GetUtcOffset(
                UnixTime.ToDateTime(sample.Timestamp)).TotalHours);

            Tuple<DateTime, float> s = new Tuple<DateTime,float>(
                UnixTime.ToDateTime(sample.Timestamp + timeOffset), sample.ToFloat());

            var sensor_query = _sensors.Where(x => x.SensorId == sample.SensorId);

            if (sensor_query.Any())
            {
                var sensor = sensor_query.ElementAt(0);
                
                if (sensor.Samples.Contains(s) == false)
                {
                    StoreSample(UnixTime.ToDateTime(sample.Timestamp).ToString("yyyyMMdd") + ".mlg", sample);
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
            Int32 mostRecentTimestamp = MagicNumbers.FIRST_LOG_DATE;

            foreach (FileInfo file in dir.GetFiles("*.mlg"))
            {
                Console.WriteLine("File {0}", file.FullName);
                LoadDataFromFile(file.FullName, data);
            }

            for (int i = 0; i < 8; i++)
            {
                var query = data.Where(s => s.SensorId == i);
                
                if (query.Any())
                {
                    SensorDataSample s = query.ElementAt(0);
                    List<Tuple<DateTime, float>> sensorData = new List<Tuple<DateTime,float>>();
                    
                    foreach (SensorDataSample sample in query)
                    {
                        mostRecentTimestamp =
                            mostRecentTimestamp > sample.Timestamp ? mostRecentTimestamp : sample.Timestamp;

                        Int32 timeOffset = (Int32)(3200 * TimeZoneInfo.Local.GetUtcOffset(
                            UnixTime.ToDateTime(sample.Timestamp)).TotalHours);

                        sensorData.Add(new Tuple<DateTime,float>(
                            UnixTime.ToDateTime(sample.Timestamp + timeOffset), sample.ToFloat()));
                    }

                    _sensors.Add(new DataSource(s.SensorId, s.SensorType, sensorData));
                }
            }

            Messenger.Default.Send<InitialTimestampMessage>(
                new InitialTimestampMessage(mostRecentTimestamp));
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
        
        private void StoreSample(string fileName, SensorDataSample sample)
        {
            using (var bw = new BinaryWriter(File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.None)))
            {
                bw.Write(sample.Timestamp);
                bw.Write(sample.Data);
                bw.Write(sample.SensorId);
                bw.Write((byte)sample.SensorType);
            }
        }

        public List<DataSource> GetDataSources()
        {
            return _sensors;
        }

        public event EventHandler SourceAdded;
    }
}
