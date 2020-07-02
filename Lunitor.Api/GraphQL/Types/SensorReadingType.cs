using GraphQL.Types;
using Lunitor.HardwareMonitorAPI.Models;

namespace Lunitor.Api.GraphQL.Types
{
    public class SensorReadingType : ObjectGraphType<SensorReading>
    {
        public SensorReadingType()
        {
            Field(x => x.TimeStamp);
            Field<HardwareType>(nameof(SensorReading.Hardware));
            Field<SensorType>(nameof(SensorReading.Sensor));
            Field(x => x.Value);
        }
    }
}
