namespace Lunitor.HardwareMonitorAPI.Models
{ 
    public class Sensor
    {
        public Hardware Hardware { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public float? MinValue { get; set; }
        public float? MaxValue { get; set; }
    }
}
