using GraphQL.Types;
using GraphQL.Utilities;
using System;

namespace Lunitor.Api.GraphQL
{
    public class SensorReadingSchema : Schema
    {
        public SensorReadingSchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<SensorReadingQuery>();
        }
    }
}
