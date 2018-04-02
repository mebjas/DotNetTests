namespace DotNetTests.Common
{
    using System.Collections.Generic;
    public class DataModel
    {
        public string Id { get; set; }
        public int Count { get; set; }
        public List<string> Objects { get; set; }
        public Dictionary<string, string> Map { get; set; }
    }
}