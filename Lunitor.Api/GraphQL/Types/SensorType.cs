using GraphQL.Types;
using Lunitor.Shared.Dto;

namespace Lunitor.Api.GraphQL.Types
{
    public class SensorType : ObjectGraphType<SensorDto>
    {
        public SensorType()
        {
            Field(x => x.HardwareName);
            Field(x => x.Name);
            Field(x => x.Type);
            Field(x => x.MinValue, nullable: true);
            Field(x => x.MaxValue, nullable: true);
        }
    }
}
