using GraphQL.Types;
using Lunitor.HardwareMonitorAPI.Models;

namespace Lunitor.Api.GraphQL.Types
{
    public class SensorType : ObjectGraphType<Sensor>
    {
        public SensorType()
        {
            Field<HardwareType>(nameof(Sensor.Hardware));
            Field(x => x.Name);
            Field(x => x.Type);
            Field(x => x.MinValue);
            Field(x => x.MaxValue);
        }
    }
}
