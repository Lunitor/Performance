import {
    Charts,
    ChartContainer,
    ChartRow,
    YAxis,
    LineChart
} from "react-timeseries-charts";
import { TimeSeries } from "pondjs";
import { ISensorReading } from "./models/ISensorReading";

declare var require: any

var React = require('react');
var ReactDOM = require('react-dom');

export class Application extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            sensorReadings: [],
            error: null,
            hardwares: []
        };
    }

    render() {

        if (this.state.error)
            return (<p> {this.state.error} </p>);

        if (!this.state.sensorReadings)
            return (<div>Loading...</div>);

        var charts = [];

        for (var hardwareId = 0; hardwareId < this.state.sensorReadings.length; hardwareId++) {
            var hardwareSensors = this.state.sensorReadings.filter(sensorReading => sensorReading.hardwareName == this.state.hardwares[hardwareId]);

            for (var sensorId = 0; sensorId < hardwareSensors.length; sensorId++) {
                var sensorReadings = hardwareSensors[sensorId];
                charts.push(
                    <ChartContainer
                        timeRange={sensorReadings.readings.timerange()}
                        width={1000}
                        format="%Y-%m-%d %H:%M:%S"
                        timeAxisHeight={130}
                        timeAxisAngledLabels={true}
                        title={sensorReadings.hardwareName + " " + sensorReadings.sensor.name}>
                        <ChartRow height="500">
                            <YAxis id="axis1"
                                label={sensorReadings.sensor.type}
                                min={sensorReadings.sensor.minValue ?? sensorReadings.readings.min("value")}
                                max={sensorReadings.sensor.maxValue ?? sensorReadings.readings.max("value")}
                                width="100"
                                type="linear"
                                format=",.2f" />
                            <Charts>
                                <LineChart axis="axis1" series={sensorReadings.readings} column={[sensorReadings.sensor.type]} />
                            </Charts>
                        </ChartRow>
                    </ChartContainer>
                );

            }
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

        //fetch('/sensorreadings')
        //    .then(res => res.json())
        //    .then((data) => {
        //        this.setState({
        //            sensorReadings: data,
        //            series: new TimeSeries({
        //                name: "sensor readings",
        //                columns: ["time", "value"],
        //                points: data
        //                    .filter((sensorReading) => (sensorReading.hardware.name == "WiFi" && sensorReading.sensor.name == "Download Speed"))
        //                    .sort((a, b) => {
        //                        if (a.timeStamp == b.timeStamp)
        //                            return 0;
        //                        else if (a.timeStamp < b.timeStamp)
        //                            return -1;
        //                        else if (a.timeStamp > b.timeStamp)
        //                            return 1;
        //                        })
        //                    .map((sensorReading) => {
        //                    return [
        //                        (new Date(sensorReading.timeStamp)).getTime(),
        //                        sensorReading.value
        //                    ];
        //                })
        //            })
        //        });
        //    })
        //    .catch((error)=>{
        //        console.log(error);
        //        this.setState({ error: error });
        //    })
    }

    componentDidUpdate() { }
}

export async function getResponse<T>( request: RequestInfo ): Promise<T> {
    const response = await fetch(request);
    const data = await response.json();
    return data;
}

ReactDOM.render(<Application />, document.getElementById('root'));