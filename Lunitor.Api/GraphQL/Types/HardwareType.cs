using GraphQL.Types;
using Lunitor.Shared.Dto;

namespace Lunitor.Api.GraphQL.Types
{
    public class HardwareType : ObjectGraphType<HardwareDto>
    {
        public HardwareType()
        {
            Field(x => x.Name);
            Field(x => x.Type);
        }
    }
}