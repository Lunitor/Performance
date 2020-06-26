import * as React from "react";
import { ISensorReadingSeries } from "../models/ISensorReadingSeries";
import {
    Charts,
    ChartContainer,
    ChartRow,
    YAxis,
    LineChart
} from "react-timeseries-charts";

type HardwareChartsProp = {
    sensorReadings: ISensorReadingSeries[],
    hardwares: [string, boolean][]
}

export class HardwareCharts extends React.Component<HardwareChartsProp> {
    render() {

        const sensorReadings = this.props.sensorReadings;
        const hardwares = this.props.hardwares;

        const charts = [];

        for (var hardwareId = 0; hardwareId < hardwares.length; hardwareId++) {
            if (!hardwares[hardwareId][1])
                continue;

            const hardwareName = hardwares[hardwareId][0];
            var sensorReadingSerieses = sensorReadings.filter(sensorReading => sensorReading.hardwareName == hardwareName);

            const yAxises = [];
            const lineCharts = [];

            for (var sensorId = 0; sensorId < sensorReadingSerieses.length; sensorId++) {
                var sensorReadingSeries = sensorReadingSerieses[sensorId];

                //const min = isNaN(Number(sensorReadingSeries.sensor.minValue)) ? sensorReadingSeries.readings.min("value", null) : sensorReadingSeries.sensor.minValue;
                //const max = isNaN(Number(sensorReadingSeries.sensor.maxValue)) ? sensorReadingSeries.readings.max("value") : sensorReadingSeries.sensor.maxValue;
                const min = sensorReadingSeries.readings.min("value", filter => 0);
                const max = sensorReadingSeries.readings.max("value");

                yAxises.push(
                    <YAxis id={sensorReadingSeries.sensor.name}
                        label={sensorReadingSeries.sensor.type}
                        min={min}
                        max={max}
                        width="50"
                        type="linear"
                        format=",.2f" />
                );

                lineCharts.push(
                    <LineChart axis={sensorReadingSeries.sensor.name} series={sensorReadingSeries.readings} column={[sensorReadingSeries.sensor.type]} />
                );
            }

            charts.push(
                <div className="row">
                    <div className="col-12 d-flex justify-content-center ">
                        <ChartContainer
                            timeRange={sensorReadingSerieses[0].readings.timerange()}
                            width={1500}
                            format="%Y-%m-%d %H:%M:%S"
                            timeAxisHeight={130}
                            timeAxisAngledLabels={true}
                            title={hardwareName}>
                            <ChartRow height="500">
                                {yAxises}
                                <Charts>
                                    {lineCharts}
                                </Charts>
                            </ChartRow>
                        </ChartContainer>
                    </div>
                </div>
            );
        }

        return (charts);
    }
}