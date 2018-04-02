using System;
using System.Text;
using System.IO;
using DotNetTests.Common;
using Newtonsoft.Json;

namespace JSONSerializationDesrialization
{
    /// <summary>
    /// Normal serialization without streaming
    /// </summary>
    internal class JSONSerializationTests
    {
        public static string NewtonSoftSerialize(DataModel model)
        {
            return JsonConvert.SerializeObject(model);
        }

        public static string fastJSONSerialize(DataModel model)
        {
            return fastJSON.JSON.ToJSON(model);
        }

        public static string ServiceStackSerialize(DataModel model)
        {
            return ServiceStack.JSON.stringify(model);
        }

        public static string JilSerialize(DataModel model)
        {
            return Jil.JSON.Serialize<DataModel>(model);
        }
    }

    /// <summary>
    /// Serialization with streaming
    /// </summary>
    internal class JSONSerializationUsingStreamsTests
    {
        public static string NewtonSoftSerialize(DataModel model)
        {
            using (var msi = new MemoryStream())
            using (TextWriter sw = new StreamWriter(msi))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, model);
                return sw.ToString();
            }
        }
    }
}
