using GraphQL;
using GraphQL.Types;

namespace Lunitor.Api.GraphQL
{
    public class SensorReadingSchema : Schema
    {
        public SensorReadingSchema(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        {
            Query = dependencyResolver.Resolve<SensorReadingQuery>();
        }
    }
}
