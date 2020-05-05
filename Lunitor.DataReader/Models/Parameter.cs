namespace Lunitor.DataReader.Models
{
    class Parameter
    {
        public string Name { get; set; }
        public string UnitofMeasure { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }

        public Parameter(string name)
        {
            Name = name;
        }
    }
}
