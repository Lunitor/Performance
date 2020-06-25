import {
    Charts,
    ChartContainer,
    ChartRow,
    YAxis,
    LineChart
} from "react-timeseries-charts";
import { TimeSeries } from "pondjs";
import { ISensorReading } from "./models/ISensorReading";
import { ISensorReadingSeries } from "./models/ISensorReadingSeries";

declare var require: any

var React = require('react');
var ReactDOM = require('react-dom');

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

        if (this.state.error)
            return (<div class="row"><div class="col-12">{this.state.error}</div></div>);

        const hardwares = this.state.hardwares;

        if (!this.state.sensorReadings || !hardwares)
            return (<div class="row"><div class="col-12 d-flex justify-content-center text-center">Loading...</div></div>);

        const hardwareSwitches = [];
        for (var i = 0; i < hardwares.length; i++) {
            if (hardwares[i][1])
                hardwareSwitches.push(<button value={hardwares[i][0]} class="btn btn-sm btn-primary m-1" onClick={this.handleHardwareSwitch.bind(this, hardwares[i][0])}> {hardwares[i][0]} </button>)
            else
                hardwareSwitches.push(<button value={hardwares[i][0]} class="btn btn-sm btn-secondary m-1" onClick={this.handleHardwareSwitch.bind(this, hardwares[i][0])}> {hardwares[i][0]} </button>)
        }

        var page = [];

        page.push(<div class="row mb-10"><div class="col-12  justify-content-center">{hardwareSwitches}</div></div>);

        for (var hardwareId = 0; hardwareId < hardwares.length; hardwareId++) {
            if (!hardwares[hardwareId][1])
                continue;

            const hardwareName = hardwares[hardwareId][0];
            var sensorReadingSerieses = this.state.sensorReadings.filter(sensorReading => sensorReading.hardwareName == hardwareName);

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

            page.push(
                <div class="row">
                    <div class="col-12 d-flex justify-content-center ">
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

        return ( page );
    }

    async componentDidMount() {
        try {
            const data = await getResponse<ISensorReading[]>('/sensorreadings');

            var hardwares = Array.from(new Set(data.map(sensorreading => sensorreading.hardware.name)));

            var sensorReadingsByHardware = [];

            for (var hardwareId = 0; hardwareId < hardwares.length; hardwareId++) {
                var sensorNames = Array.from(new Set(data
                    .filter(sensorreading => sensorreading.hardware.name == hardwares[hardwareId])
                    .map(sensorreading => sensorreading.sensor.name)));

                for (var sensorId = 0; sensorId < sensorNames.length; sensorId++) {
                    sensorReadingsByHardware.push(
                        {
                            hardwareName: hardwares[hardwareId],
                            sensor: data.find(sensorreading => sensorreading.hardware.name == hardwares[hardwareId] && sensorreading.sensor.name == sensorNames[sensorId]).sensor,
                            readings: new TimeSeries({
                                name: hardwares[hardwareId] + " " + sensorNames[sensorId],
                                columns: ["time", "value"],
                                points: data.filter(reading => reading.hardware.name == hardwares[hardwareId] && reading.sensor.name == sensorNames[sensorId])
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
                                    })
                            })
                        }
                    );
                }
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

    componentDidUpdate() { }

    handleHardwareSwitch(hardwareName: string) {
        const hardwares = this.state.hardwares
        const hardwareState = hardwares.find(hardware => hardware[0] == hardwareName)[1];
        hardwares.find(hardware => hardware[0] == hardwareName)[1] = !hardwareState;

        this.setState({
            hardwares: hardwares
        });
    }
}

export async function getResponse<T>( request: RequestInfo ): Promise<T> {
    const response = await fetch(request);
    const data = await response.json();
    return data;
}

ReactDOM.render(<Application />, document.getElementById('root'));