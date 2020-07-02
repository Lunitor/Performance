using GraphQL.Types;
using Lunitor.HardwareMonitorAPI.Models;

namespace Lunitor.Api.GraphQL.Types
{
    public class HardwareType : ObjectGraphType<Hardware>
    {
        public HardwareType()
        {
            Field(x => x.Name);
            Field(x => x.Type);
        }
    }
}