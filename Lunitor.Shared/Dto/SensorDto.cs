namespace Lunitor.Shared.Dto
{
    public class SensorDto
    {
        public string HardwareName { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public float? MinValue { get; set; }
        public float? MaxValue { get; set; }
    }
}
