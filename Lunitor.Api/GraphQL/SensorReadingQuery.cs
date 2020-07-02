using GraphQL.Types;
using Lunitor.Api.Cache;
using Lunitor.Api.GraphQL.Types;

namespace Lunitor.Api.GraphQL
{
    internal class SensorReadingQuery : ObjectGraphType
    {
        private readonly ISensorCacheReader _sensorCache;

        public SensorReadingQuery(ISensorCacheReader sensorCache)
        {
            _sensorCache = sensorCache;

            Field<ListGraphType<SensorReadingType>>("sensorreadings",
                resolve: context => _sensorCache.GetAll());
        }
    }
}