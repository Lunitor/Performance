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

    constructor(props) {
        super(props);
        this.state = {
            sensorReadings: [] as ISensorReadingSeries[],
            error: null,
            hardwares: [] as string[]
        };
    }

    render() {

        if (this.state.error)
            return (<p> {this.state.error} </p>);

        if (!this.state.sensorReadings)
            return (<div>Loading...</div>);

        var charts = [];
        const hardwares = this.state.hardwares as string[];

        for (var hardwareId = 0; hardwareId < hardwares.length; hardwareId++) {
            const hardwareName = hardwares[hardwareId];
            var sensorReadingSerieses = this.state.sensorReadings.filter(sensorReading => sensorReading.hardwareName == hardwareName);

            const yAxises = [];
            const lineCharts = [];

            for (var sensorId = 0; sensorId < sensorReadingSerieses.length; sensorId++) {
                var sensorReadingSeries = sensorReadingSerieses[sensorId] as ISensorReadingSeries;

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
                    <LineChart axis={sensorReadingSeries.sensor.name} series = { sensorReadingSeries.readings } column = { [sensorReadingSeries.sensor.type]} />
                );
            }

            charts.push(
                <ChartContainer
                    timeRange={sensorReadingSerieses[0].readings.timerange()}
                    width={1500}
                    format="%Y-%m-%d %H:%M:%S"
                    timeAxisHeight={130}
                    timeAxisAngledLabels={true}
                    title={hardwareName}>
                    <ChartRow height="500">
                        { yAxises }
                        <Charts>
                            {lineCharts}
                        </Charts>
                    </ChartRow>
                </ChartContainer>
            );
        }

        return ( charts );
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
                hardwares: hardwares
            });
        }
        catch (error) {
            console.log(error);
            this.setState({ error: error });
        }
    }

    componentDidUpdate() { }
}

export async function getResponse<T>( request: RequestInfo ): Promise<T> {
    const response = await fetch(request);
    const data = await response.json();
    return data;
}

ReactDOM.render(<Application />, document.getElementById('root'));