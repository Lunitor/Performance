import * as React from "react";
import * as ReactDOM from 'react-dom';

import { TimeSeries } from "pondjs";
import { ISensorReading } from "./models/ISensorReading";
import { ISensorReadingSeries } from "./models/ISensorReadingSeries";
import { ChartsMenu } from "./components/ChartsMenu";
import { HardwareCharts } from "./components/HardwareCharts";


export class Application extends React.Component {

    state: {
        sensorReadings: ISensorReadingSeries[],
        error: any,
        hardwares: [string, boolean][]
    }

    constructor(props) {
        super(props);

        this.state = {
            sensorReadings: [],
            error: null,
            hardwares: null
        };
    }

    render() {

        const sensorReadings = this.state.sensorReadings;
        const error = this.state.error;
        const hardwares = this.state.hardwares;

        if (error)
            return (<div className="row"><div className="col-12">{error}</div></div>);

        if (!sensorReadings || !hardwares)
            return (<div className="row"><div className="col-12 d-flex justify-content-center text-center">Loading...</div></div>);

        var page = [];

        const chartsMenu = <ChartsMenu hardwares={hardwares} handleClick={this.handleHardwareSwitch.bind(this)} />;
        page.push(chartsMenu);

        const charts = <HardwareCharts sensorReadings={sensorReadings} hardwares={hardwares} />
        page.push(charts);

        return ( page );
    }

    async componentDidMount() {
        try {
            const data = await getResponse<ISensorReading[]>('/sensorreadings');
            data.forEach(sensorReading => sensorReading.sensor.name += "-" + sensorReading.sensor.type);

            var hardwares = Array.from(new Set(data.map(sensorreading => sensorreading.hardware.name)));

            var sensorReadingsByHardware = [];

            for (var hardwareId = 0; hardwareId < hardwares.length; hardwareId++) {
                var sensorNames = this.getSensorNames(data, hardwares, hardwareId);

                this.fillSensorReadingsByHardware(sensorNames, sensorReadingsByHardware, hardwares, hardwareId, data);
            }

            this.setState({
                sensorReadings: sensorReadingsByHardware,
                hardwares: hardwares.map(hardware => [hardware, true])
            });
        }
        catch (error) {
            console.log(error);
            this.setState({ error: error });
        }
    }

    handleHardwareSwitch(hardwareName: string) {
        const hardwares = this.state.hardwares
        const hardwareState = hardwares.find(hardware => hardware[0] == hardwareName)[1];
        hardwares.find(hardware => hardware[0] == hardwareName)[1] = !hardwareState;

        this.setState({
            hardwares: hardwares
        });
    }

    private fillSensorReadingsByHardware(sensorNames: string[], sensorReadingsByHardware: any[], hardwares: string[], hardwareId: number, data: ISensorReading[]) {
        for (var sensorId = 0; sensorId < sensorNames.length; sensorId++) {
            sensorReadingsByHardware.push(
                {
                    hardwareName: hardwares[hardwareId],
                    sensor: this.getSensor(data, hardwares, hardwareId, sensorNames, sensorId),
                    readings: new TimeSeries({
                        name: hardwares[hardwareId] + " " + sensorNames[sensorId],
                        columns: ["time", "value"],
                        points: this.getSensorReadingAsTimeValuePairs(data, hardwares, hardwareId, sensorNames, sensorId)
                    })
                }
            );
        }
    }

    private getSensorReadingAsTimeValuePairs(data: ISensorReading[], hardwares: string[], hardwareId: number, sensorNames: string[], sensorId: number): any[] {
        return data.filter(reading => reading.hardware.name == hardwares[hardwareId] && reading.sensor.name == sensorNames[sensorId])
            .sort((a, b) => {
                if (a.timeStamp == b.timeStamp)
                    return 0;
                else if (a.timeStamp < b.timeStamp)
                    return -1;
                else if (a.timeStamp > b.timeStamp)
                    return 1;
            })
            .map(reading => {
                return [
                    (new Date(reading.timeStamp)).getTime(),
                    reading.value
                ];
            });
    }

    private getSensor(data: ISensorReading[], hardwares: string[], hardwareId: number, sensorNames: string[], sensorId: number) {
        return data.find(sensorreading =>
            sensorreading.hardware.name == hardwares[hardwareId] &&
            sensorreading.sensor.name == sensorNames[sensorId]).sensor;
    }

    private getSensorNames(data: ISensorReading[], hardwares: string[], hardwareId: number) {
        return Array.from(new Set(data
            .filter(sensorreading => sensorreading.hardware.name == hardwares[hardwareId])
            .map(sensorreading => sensorreading.sensor.name)));
    }
}

export async function getResponse<T>( request: RequestInfo ): Promise<T> {
    const response = await fetch(request);
    const data = await response.json();
    return data;
}

ReactDOM.render(<Application />, document.getElementById('root'));