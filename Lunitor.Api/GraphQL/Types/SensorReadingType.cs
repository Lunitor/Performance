using GraphQL.Types;
using Lunitor.Shared.Dto;

namespace Lunitor.Api.GraphQL.Types
{
    public class SensorReadingType : ObjectGraphType<SensorReadingDto>
    {
        public SensorReadingType()
        {
            Field<DateTimeGraphType>(nameof(SensorReadingDto.TimeStamp));
            Field<HardwareType>(nameof(SensorReadingDto.Hardware));
            Field<SensorType>(nameof(SensorReadingDto.Sensor));
            Field(x => x.Value, nullable: true);
        }
    }
}
