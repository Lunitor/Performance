import * as React from "react";
import {
    Charts,
    ChartContainer,
    ChartRow,
    YAxis,
    LineChart,
    styler
} from "react-timeseries-charts";
import { ISensorReadingSeries } from "../models/ISensorReadingSeries";

type HardwareChartProps = {
    sensorReadings: ISensorReadingSeries[],
    hardwareName: string,
    sensors: [string, string, string, boolean][],
    fullSensorName: (sensor: [string, string, string, boolean]) => string,
    colors: string[]
}

export class HardwareChart extends React.Component<HardwareChartProps> {
    render() {

        var sensorReadingSerieses = this.props.sensorReadings.filter(sensorReading =>
            sensorReading.hardwareName == this.props.hardwareName);

        const yAxises = [];
        const lineCharts = [];

        for (var sensorId = 0; sensorId < sensorReadingSerieses.length; sensorId++) {
            var sensorReadingSeries = sensorReadingSerieses[sensorId];

            if (!this.sensorChartEnabled(this.props.hardwareName, sensorReadingSeries.sensor.name, sensorReadingSeries.sensor.type))
                continue;

            const min = isNaN(Number(sensorReadingSeries.sensor.minValue)) ? sensorReadingSeries.readings.min("value") : sensorReadingSeries.sensor.minValue;
            const max = isNaN(Number(sensorReadingSeries.sensor.maxValue)) ? sensorReadingSeries.readings.max("value") : sensorReadingSeries.sensor.maxValue;

            const style = styler([{ key: "value", color: this.props.colors[sensorId] }]);

            yAxises.push(
                <YAxis id={sensorReadingSeries.sensor.name}
                    label={sensorReadingSeries.sensor.type}
                    min={min}
                    max={max}
                    width="50"
                    type="linear"
                    format=",.2f"
                    style={style.axisStyle("value")} />
            );


            lineCharts.push(
                <LineChart key={this.props.fullSensorName(this.props.sensors[sensorId])}
                    axis={sensorReadingSeries.sensor.name}
                    series={sensorReadingSeries.readings}
                    column={[sensorReadingSeries.sensor.type]}
                    style={style}
                />
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

    private sensorChartEnabled(hardwareName: string, sensorName: string, sensorType: string) {
        return this.props.sensors.find(sensorSwitch =>
            sensorSwitch[0] == hardwareName &&
            sensorSwitch[1] == sensorName &&
            sensorSwitch[2] == sensorType)[3];
    }
}