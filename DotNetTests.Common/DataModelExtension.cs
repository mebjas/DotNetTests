namespace DotNetTests.Common
{
    using System;
    using System.Collections.Generic;

    public class DataModelExtension
    {
        public static DataModel GenerateDataModel(int load = 0)
        {
            Random random = new Random();
            string tmp;

            DataModel dm = new DataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Count = random.Next(0, 100),
                Objects = new List<string>(),
                Map = new Dictionary<string, string>()
            };

            //// TODO: implement custom logics for supporting load
            load = (load == 0) ? 10 : load;
            for (int i = 0; i < load; i++)
            {
                tmp = Guid.NewGuid().ToString();
                dm.Objects.Add(tmp);
                dm.Map.Add(tmp, tmp);
            }

            return dm;
        }
    }
}
