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

type HardwareChartStates = {
    tracker: Date,
    trackerInfos: any[]
}

export interface ITrackerInfo {
    label: string,
    value: string
}

export class HardwareChart extends React.Component<HardwareChartProps, HardwareChartStates> {

    constructor(props)
    {
        super(props);
        this.state = {
            tracker: new Date(Date.now()),
            trackerInfos: []
        };

        this.handleTrackerChange = this.handleTrackerChange.bind(this);
        this.calculateTrackerInfoWidth = this.calculateTrackerInfoWidth.bind(this);
    }

    render() {

        var sensorReadingSerieses = this.props.sensorReadings;

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
                title={this.props.hardwareName}
                onTrackerChanged={this.handleTrackerChange}
                trackerPosition={this.state.tracker}>
                <ChartRow height="500"
                    trackerInfoValues={this.state.trackerInfos}
                    trackerInfoHeight={this.state.trackerInfos.length * 14 + 4}
                    trackerInfoWidth={this.calculateTrackerInfoWidth()}>
                    {yAxises}
                    <Charts>
                        {lineCharts}
                    </Charts>
                </ChartRow>
            </ChartContainer>
        );
    }

    calculateTrackerInfoWidth() {
        var width = 0;
        for (let info of this.state.trackerInfos as Array<ITrackerInfo>) {
            const infoLength = (info.label + info.value).length;
            if (infoLength > width)
                width = infoLength;
        }

        return width*6 + 15;
    }

    private handleTrackerChange(tracker: Date) {
        const trackerInfos: any[] = [];

        if (tracker) {
            var sensorReadingSerieses = this.props.sensorReadings;

            for (var sensorId = 0; sensorId < sensorReadingSerieses.length; sensorId++) {
                var sensorReadingSeries = sensorReadingSerieses[sensorId];

                if (!this.sensorChartEnabled(this.props.hardwareName, sensorReadingSeries.sensor.name, sensorReadingSeries.sensor.type))
                    continue;

                const sensorFullName = sensorReadingSeries.sensor.name + "-" + sensorReadingSeries.sensor.type;
                const value = sensorReadingSeries.readings.atTime(tracker).get("value");

                trackerInfos.push({ label: sensorFullName, value: value })
            }
        }

        this.setState({
            tracker: tracker,
            trackerInfos: trackerInfos
        });
    }

    private sensorChartEnabled(hardwareName: string, sensorName: string, sensorType: string) {
        return this.props.sensors.find(sensorSwitch =>
            sensorSwitch[0] == hardwareName &&
            sensorSwitch[1] == sensorName &&
            sensorSwitch[2] == sensorType)[3];
    }
}