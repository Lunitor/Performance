import * as React from "react";
import {
    Charts,
    ChartContainer,
    ChartRow,
    YAxis,
    LineChart
} from "react-timeseries-charts";
import { ISensorReadingSeries } from "../models/ISensorReadingSeries";

type HardwareChartProps = {
    sensorReadings: ISensorReadingSeries[],
    hardwareName: string,
    sensors: [string, string, boolean][]
}

export class HardwareChart extends React.Component<HardwareChartProps> {
    render() {

        var sensorReadingSerieses = this.props.sensorReadings.filter(sensorReading =>
            sensorReading.hardwareName == this.props.hardwareName &&
            this.sensorChartEnabled(this.props.hardwareName, sensorReading.sensor.name));

        const yAxises = [];
        const lineCharts = [];

        for (var sensorId = 0; sensorId < sensorReadingSerieses.length; sensorId++) {
            var sensorReadingSeries = sensorReadingSerieses[sensorId];

            const min = isNaN(Number(sensorReadingSeries.sensor.minValue)) ? sensorReadingSeries.readings.min("value") : sensorReadingSeries.sensor.minValue;
            const max = isNaN(Number(sensorReadingSeries.sensor.maxValue)) ? sensorReadingSeries.readings.max("value") : sensorReadingSeries.sensor.maxValue;

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

        return (
            <ChartContainer
                timeRange={sensorReadingSerieses[0].readings.timerange()}
                width={1500}
                format="%Y-%m-%d %H:%M:%S"
                timeAxisHeight={130}
                timeAxisAngledLabels={true}
                title={this.props.hardwareName}>
                <ChartRow height="500">
                    {yAxises}
                    <Charts>
                        {lineCharts}
                    </Charts>
                </ChartRow>
            </ChartContainer>
        );
    }

    private sensorChartEnabled(hardwareName: string, sensorName: string) {
        return this.props.sensors.find(sensorSwitch => sensorSwitch[0] == hardwareName && sensorSwitch[1] == sensorName)[2];
    }
}