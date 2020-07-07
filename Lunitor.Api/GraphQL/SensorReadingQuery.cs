using GraphQL;
using GraphQL.Types;
using Lunitor.Api.GraphQL.Types;
using Lunitor.Api.Services;
using System;
using System.Collections.Generic;

namespace Lunitor.Api.GraphQL
{
    internal class SensorReadingQuery : ObjectGraphType
    {
        private readonly ISensorReadingService _sensorReadingService;

        public SensorReadingQuery(ISensorReadingService sensorReadingService)
        {
            _sensorReadingService = sensorReadingService;

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
                    var fromDate = context.GetArgument<DateTime?>("from");
                    var toDate = context.GetArgument<DateTime?>("to");

                    var criticalValuePercent = ClipValue(context);

                    return _sensorReadingService.Get(from: fromDate, to: toDate, criticalValuePercent: criticalValuePercent);

                });
        }

        private static float ClipValue(IResolveFieldContext<object> context)
        {
            var value = context.GetArgument<double?>("criticalValuePercent") ?? 1.0;

            if (value >= 1.0)
                return 1.0f;
            else if (value <= 0)
                return 0;
            else
                return (float)value;
        }
    }
}