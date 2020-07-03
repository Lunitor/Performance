using GraphQL.Types;
using Lunitor.Api.Cache;
using Lunitor.Api.GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunitor.Api.GraphQL
{
    internal class SensorReadingQuery : ObjectGraphType
    {
        private readonly ISensorCacheReader _sensorCache;

        public SensorReadingQuery(ISensorCacheReader sensorCache)
        {
            _sensorCache = sensorCache;

            Field<ListGraphType<SensorReadingType>>("sensorreadings",
                arguments: new QueryArguments(new List<QueryArgument>
                {
                    new QueryArgument<DateTimeGraphType>
                    {
                        Name = "from"
                    },
                    new QueryArgument<DateTimeGraphType>
                    {
                        Name = "to"
                    },
                    new QueryArgument<FloatGraphType>
                    {
                        Name = "criticalValuePercent"
                    },
                }),
                resolve: context =>
                {
                    var fromDate = context.GetArgument<DateTimeOffset?>("from");
                    var toDate = context.GetArgument<DateTimeOffset?>("to");
                    var criticalValuePercent = context.GetArgument<double?>("criticalValuePercent") ?? 1.0;

                    if (fromDate.HasValue && !toDate.HasValue)
                        return _sensorCache.GetAll().Where(sensorreading => sensorreading.TimeStamp >= fromDate.Value
                            && sensorreading.Value >= criticalValuePercent * (sensorreading.Sensor.MaxValue ?? double.MaxValue));

                    if(!fromDate.HasValue && toDate.HasValue)
                        return _sensorCache.GetAll().Where(sensorreading => sensorreading.TimeStamp <= toDate.Value
                            && sensorreading.Value >= criticalValuePercent * (sensorreading.Sensor.MaxValue ?? double.MaxValue));

                    if(fromDate.HasValue && toDate.HasValue)
                        return _sensorCache.GetAll().Where(sensorreading => sensorreading.TimeStamp <= toDate.Value
                            && sensorreading.TimeStamp >= fromDate.Value
                            && sensorreading.Value >= criticalValuePercent * (sensorreading.Sensor.MaxValue ?? double.MaxValue));

                    if (!fromDate.HasValue && !toDate.HasValue)
                        return _sensorCache.GetAll().Where(sensorreading => 
                            sensorreading.Value >= criticalValuePercent * (sensorreading.Sensor.MaxValue ?? double.MaxValue));

                    return _sensorCache.GetAll();
                });
        }
    }
}