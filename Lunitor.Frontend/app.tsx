import {
    Charts,
    ChartContainer,
    ChartRow,
    YAxis,
    LineChart
} from "react-timeseries-charts";
import { TimeSeries } from "pondjs"

declare var require: any

var React = require('react');
var ReactDOM = require('react-dom');

export class Application extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            sensorReadings: [],
            series: null,
            error: null
        };
    }

    render() {

        if (this.state.error)
            return (<p> {this.state.error} </p>);

        if (!this.state.series)
            return (<div>Loading...</div>);

        return (
            // format: https://d3-wiki.readthedocs.io/zh_CN/master/Time-Formatting/
            <ChartContainer
                timeRange={this.state.series.timerange()}
                width={1000}
                format="%Y-%m-%d %H:%M:%S"
                timeAxisHeight={ 130 }
                timeAxisAngledLabels={true}>
                <ChartRow height="500">
                    <YAxis id="axis1"
                        label="reading"
                        min={this.state.series.min("value")}
                        max={this.state.series.max("value")}
                        width="100"
                        type="linear"
                        format=",.2f" />
                    <Charts>
                        <LineChart axis="axis1" series={this.state.series} column={["readings"]} />
                    </Charts>
                </ChartRow>
            </ChartContainer>
        );
    }

    componentDidMount() {
        fetch('/sensorreadings')
            .then(res => res.json())
            .then((data) => {
                this.setState({
                    sensorReadings: data,
                    series: new TimeSeries({
                        name: "sensor readings",
                        columns: ["time", "value"],
                        points: data
                            .filter((sensorReading) => (sensorReading.hardware.name == "WiFi" && sensorReading.sensor.name == "Download Speed"))
                            .sort((a, b) => {
                                if (a.timeStamp == b.timeStamp)
                                    return 0;
                                else if (a.timeStamp < b.timeStamp)
                                    return -1;
                                else if (a.timeStamp > b.timeStamp)
                                    return 1;
                                })
                            .map((sensorReading) => {
                            return [
                                (new Date(sensorReading.timeStamp)).getTime(),
                                sensorReading.value
                            ];
                        })
                    })
                });
            })
            .catch((error)=>{
                console.log(error);
                this.setState({ error: error });
            })
    }

    componentDidUpdate() { }
}

ReactDOM.render(<Application />, document.getElementById('root'));