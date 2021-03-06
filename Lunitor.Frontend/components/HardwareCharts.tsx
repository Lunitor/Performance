import * as React from "react";
import { ISensorReadingSeries } from "../models/ISensorReadingSeries";
import { SensorsMenu } from "./SensorsMenu";
import { HardwareChart } from "./hardwarechart/HardwareChart";
import randomColor = require("randomcolor");

type HardwareChartsProp = {
    sensorReadings: ISensorReadingSeries[],
    hardwares: [string, boolean][]
}

type HardwareChartsState = {
    sensors: [string, string, string, boolean][],
    colors: string[]
}

export class HardwareCharts extends React.Component<HardwareChartsProp, HardwareChartsState> {

    constructor(prop: Readonly<HardwareChartsProp>) {
        super(prop)

        this.state = {
            sensors: this.props.sensorReadings
                .map(sensorReading => [sensorReading.hardwareName, sensorReading.sensor.name, sensorReading.sensor.type, true]),
            colors: randomColor({
                luminosity: 'dark',
                hue: 'random',
                count: 40
            })
        };
    }

    render() {
        const hardwares = this.props.hardwares;

        const charts = [];

        for (var hardwareId = 0; hardwareId < hardwares.length; hardwareId++) {
            if (!hardwares[hardwareId][1])
                continue;

            const hardwareName = hardwares[hardwareId][0];

            charts.push(
                <div className="row">
                    <div className="row">
                        <SensorsMenu
                            hardwareName={hardwareName}
                            sensors={this.state.sensors}
                            sensorClickHandler={this.handleSensorClick.bind(this)}
                            fullSensorName={this.fullSensorName}
                            colors={this.state.colors} />
                    </div>
                    <div className="row">
                        <div className="col-12 d-flex justify-content-center ">
                            <HardwareChart
                                sensorReadings={this.props.sensorReadings
                                    .filter(sensorReadings => sensorReadings.hardwareName == hardwareName)}
                                hardwareName={hardwareName}
                                sensors={this.state.sensors}
                                fullSensorName={this.fullSensorName}
                                colors={this.state.colors} />
                        </div>
                    </div>
                </div>
            );
        }

        return (charts);
    }

    handleSensorClick(hardwareSensorName: string) {
        var sensors = this.state.sensors;

        var sensorState = sensors.find(sensor => this.fullSensorName(sensor) == hardwareSensorName)[3];

        if (sensors.filter(sensor => sensor[0] == hardwareSensorName.split('_')[0] && sensor[3]).length == 1
            && sensorState)
            return;

        sensors.find(sensor => this.fullSensorName(sensor) == hardwareSensorName)[3] = !sensorState;

        this.setState({
            sensors: sensors
        });
    }

    private fullSensorName(sensor: [string, string, string, boolean]) {
        return sensor[0] + "_" + sensor[1] + "_" + sensor[2];
    }
}