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

type HardwareChartsState = {
    sensors: [string, string, boolean][]
}

export class HardwareCharts extends React.Component<HardwareChartsProp, HardwareChartsState> {

    constructor(prop: Readonly<HardwareChartsProp>) {
        super(prop)

        this.state = {
            sensors: this.props.sensorReadings
                .map(sensorReading => [sensorReading.hardwareName, sensorReading.sensor.name, true])
        };
    }

    render() {

        const sensorReadings = this.props.sensorReadings;
        const hardwares = this.props.hardwares;

        const charts = [];

        for (var hardwareId = 0; hardwareId < hardwares.length; hardwareId++) {
            if (!hardwares[hardwareId][1])
                continue;

            const hardwareName = hardwares[hardwareId][0];

            const sensorSwitches = [];
            const sensors = this.state.sensors.filter(sensor => sensor[0] == hardwareName);
            for(var i = 0; i < sensors.length; i++) {
                if (sensors[i][2])
                    sensorSwitches.push(<button value={sensors[i][0] + sensors[i][1]} className="btn btn-sm btn-primary m-1" onClick={(e: React.MouseEvent<HTMLButtonElement>) => this.handleSensorClick(e.currentTarget.value)}> {sensors[i][1]} </button>)
                else
                    sensorSwitches.push(<button value={sensors[i][0] + sensors[i][1]} className="btn btn-sm btn-secondary m-1" onClick={(e: React.MouseEvent<HTMLButtonElement>) => this.handleSensorClick(e.currentTarget.value)}> {sensors[i][1]} </button>)
            }

            var sensorReadingSerieses = sensorReadings.filter(sensorReading =>
                sensorReading.hardwareName == hardwareName && 
                this.sensorChartEnabled(hardwareName, sensorReading.sensor.name));

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

            charts.push(
                <div className="row">
                    <div className="row">
                        {sensorSwitches}
                    </div>
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
                </div>
            );
        }

        return (charts);
    }

    private sensorChartEnabled(hardwareName: string, sensorName: string) {
        return this.state.sensors.find(sensorSwitch => sensorSwitch[0] == hardwareName && sensorSwitch[1] == sensorName)[2];
    }

    handleSensorClick(hardwareSensorName: string) {
        var sensors = this.state.sensors;
        var sensorState = sensors.find(sensor => sensor[0] + sensor[1] == hardwareSensorName)[2];
        sensors.find(sensor => sensor[0] + sensor[1] == hardwareSensorName)[2] = !sensorState;

        this.setState({
            sensors: sensors
        });
    }
}