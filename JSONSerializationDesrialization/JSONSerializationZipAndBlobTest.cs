using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;
using DotNetTests.Common;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace JSONSerializationDesrialization
{
    public class JSONSerializationZipAndBlobTest
    {
        private CloudBlobClient cloudBlobClient;
        private CloudBlobContainer cloudBlobContainer;

        public JSONSerializationZipAndBlobTest()
        {
            string connectionString = "";
            CloudStorageAccount storageAccount;
            CloudStorageAccount.TryParse(connectionString, out storageAccount);
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
            cloudBlobContainer = cloudBlobClient.GetContainerReference("blobstreamtest01");
            cloudBlobContainer.CreateIfNotExistsAsync();

        }

        public byte[] DataToMemoryStream(DataModel model)
        {
            MemoryStream stream = new MemoryStream();
            using (var sw = new StreamWriter(stream))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, model);
                sw.Flush();
                stream.Position = 0;

                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        CopyTo(stream, gs);
                    }

                    mso.Flush();
                    return mso.ToArray();
                }
            }
        }

        public string MemoryStreamToBlob(byte[] data, string blobName)
        {
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            blockBlob.UploadFromByteArray(data, 0, data.Length);
            return blockBlob.Properties.ETag;
        }

        public byte[] BlobToMemoryStream(string blobName)
        {
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            using (MemoryStream ms = new MemoryStream())
            {
                blockBlob.DownloadToStream(ms);
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public DataModel MemoryStreamToData(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                string jsonString = Encoding.UTF8.GetString(mso.ToArray());
                msi.Dispose();
                return JsonConvert.DeserializeObject<DataModel>(jsonString);
            }
        }
    
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
    }
}
