import * as React from "react";

type SensorsMenunProps = {
    hardwareName: string,
    sensors: [string, string, string, boolean][],
    sensorClickHandler: (sensorFullName: string) => void,
    fullSensorName: (sensor: [string, string, string, boolean]) => string,
    colors: string[]
}

export class SensorsMenu extends React.Component<SensorsMenunProps> {
    render() {
        const sensorSwitches = [];
        const sensors = this.props.sensors.filter(sensor => sensor[0] == this.props.hardwareName);

        for (var i = 0; i < sensors.length; i++) {
            if (sensors[i][3])
                sensorSwitches.push(<button value={this.props.fullSensorName(sensors[i])}
                    className="btn btn-sm btn-primary m-1"
                    style={{ backgroundColor: this.props.colors[i] }}
                    onClick={(e: React.MouseEvent<HTMLButtonElement>) => this.props.sensorClickHandler(e.currentTarget.value)}> {sensors[i][1] + " - " + sensors[i][2]}
                </button>)
            else
                sensorSwitches.push(<button value={this.props.fullSensorName(sensors[i])}
                    className="btn btn-sm btn-secondary m-1"
                    onClick={(e: React.MouseEvent<HTMLButtonElement>) => this.props.sensorClickHandler(e.currentTarget.value)}> {sensors[i][1] + " - " + sensors[i][2]}
                </button>)
        }

        return (
            <div className="row mb-10">
                <div className=" col-12 justify-content-center">
                    {sensorSwitches}
                </div>
            </div>
        );
    }
}