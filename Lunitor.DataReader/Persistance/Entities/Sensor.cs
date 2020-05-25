namespace Lunitor.DataReader.Persistance.Entities
{
    class Sensor
    {
        public int Id { get; set; }
        public Hardware Hardware { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
    }
}
