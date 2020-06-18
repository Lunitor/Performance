declare var require: any

var React = require('react');
var ReactDOM = require('react-dom');

export class Application extends React.Component {

    state = {
        sensorReadings: []
    }

    render() {
        return (
            <div>
                {this.state.sensorReadings
                    .sort((a, b) => {
                        if (a.timeStamp == b.timeStamp)
                            return 0;
                        else if (a.timeStamp < b.timeStamp)
                            return -1;
                        else if (a.timeStamp > b.timeStamp)
                            return 1;
                    })
                    .filter((i, index) => (index < 50))
                    .map((sensorReading) => (
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <span>{sensorReading.timeStamp}</span>
                            </div>
                            <div class="row">
                                <div class="col-5">{sensorReading.hardware.name}</div>
                                <div class="col-5">{sensorReading.sensor.name}</div>
                                <div class="col-2">{sensorReading.value}</div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        );
    }

    componentDidMount() {
        fetch('/sensorreadings')
            .then(res => res.json())
            .then((data) => {
                this.setState({ sensorReadings: data })
            })
            .catch(console.log)
    }
}

ReactDOM.render(<Application />, document.getElementById('root'));