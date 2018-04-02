using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using DotNetTests.Common;

namespace JSONSerializationDesrialization
{
    class Program
    {
        #region PRIVATE STATIC METHODS
        private static double std(List<long> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }

            return ret;
        }

        private static void Log(string method, int load, int count, List<long> timeElapsed)
        {
            string message = string.Format("Method: {0}, Load: {1}, Count: {2}\nResults => AVG: {3} ms, MAX: {4} ms, MIN: {5} ms, STD: {6} ms",
                method,
                load,
                count,
                timeElapsed.Average(),
                timeElapsed.Max(),
                timeElapsed.Min(),
                std(timeElapsed));

            Console.WriteLine(message);
            Debug.WriteLine(message);
            Trace.WriteLine(message);
        }
        #endregion

        #region tests
        private static void NormalSerializationTests()
        {
            //// Local variables
            List<long> timeElapsed = new List<long>();
            int load = 10000000, count = 1;
            DataModel model = DataModelExtension.GenerateDataModel(load);

            for (int i = 0; i < count; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string s = JSONSerializationTests.NewtonSoftSerialize(model);
                watch.Stop();
                timeElapsed.Add(watch.ElapsedMilliseconds);
            }

            Log("NewtonSoftSerialize", load, count, timeElapsed);

            for (int i = 0; i < count; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string s = JSONSerializationTests.fastJSONSerialize(model);
                watch.Stop();
                timeElapsed.Add(watch.ElapsedMilliseconds);
            }

            Log("fastJSONSerialize", load, count, timeElapsed);

            for (int i = 0; i < count; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string s = JSONSerializationTests.ServiceStackSerialize(model);
                watch.Stop();
                timeElapsed.Add(watch.ElapsedMilliseconds);
            }

            Log("ServiceStackSerialize", load, count, timeElapsed);


            for (int i = 0; i < count; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string s = JSONSerializationTests.JilSerialize(model);
                watch.Stop();
                timeElapsed.Add(watch.ElapsedMilliseconds);
            }

            Log("JilSerialize", load, count, timeElapsed);
        }
        
        private static void StreamSerializationTests()
        {
            //// Local variables
            List<long> timeElapsed = new List<long>();
            int load = 10000000, count = 1;
            DataModel model = DataModelExtension.GenerateDataModel(load);

            for (int i = 0; i < count; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string s = JSONSerializationUsingStreamsTests.NewtonSoftSerialize(model);
                watch.Stop();
                timeElapsed.Add(watch.ElapsedMilliseconds);
            }

            Log("NewtonSoftSerialize Streaming", load, count, timeElapsed);
        }
        
        private static void JsonSerializationAndPeristanceTest()
        {
            DataModel model = DataModelExtension.GenerateDataModel(3000000);

            string blobName = "testblob01";
            JSONSerializationZipAndBlobTest jszbTest = new JSONSerializationZipAndBlobTest();
            byte[] data = jszbTest.DataToMemoryStream(model);
            DataModel model3 = jszbTest.MemoryStreamToData(data);

            string etag = jszbTest.MemoryStreamToBlob(data, blobName);
            byte[] data2 = jszbTest.BlobToMemoryStream(blobName);
            DataModel model2 = jszbTest.MemoryStreamToData(data2);
        }
        #endregion

        static void Main(string[] args)
        {
            //NormalSerializationTests();
            //StreamSerializationTests();
            JsonSerializationAndPeristanceTest();
            Console.ReadKey();
        }
    }
}
