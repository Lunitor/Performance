import * as React from "react";

type SensorsMenunProp = {
    hardwareName: string,
    sensors: [string, string, boolean][],
    sensorClickHandler: (sensorFullName: string) => void,
    fullSensorName: (sensor: [string, string, boolean]) => string
}

export class SensorsMenu extends React.Component<SensorsMenunProp> {
    render() {
        const sensorSwitches = [];
        const sensors = this.props.sensors.filter(sensor => sensor[0] == this.props.hardwareName);
        for (var i = 0; i < sensors.length; i++) {
            if (sensors[i][2])
                sensorSwitches.push(<button value={this.props.fullSensorName(sensors[i])} className="btn btn-sm btn-primary m-1" onClick={(e: React.MouseEvent<HTMLButtonElement>) => this.props.sensorClickHandler(e.currentTarget.value)}> {sensors[i][1]} </button>)
            else
                sensorSwitches.push(<button value={this.props.fullSensorName(sensors[i])} className="btn btn-sm btn-secondary m-1" onClick={(e: React.MouseEvent<HTMLButtonElement>) => this.props.sensorClickHandler(e.currentTarget.value)}> {sensors[i][1]} </button>)
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