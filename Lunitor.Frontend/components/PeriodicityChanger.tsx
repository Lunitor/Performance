import * as React from "react";
import * as NumericInput from "react-numeric-input";
import { ConfigurationApi } from "../api/ConfigurationApi";


type PeriodicityChangerProps = {}
type PeriodicityChangerState = {
    periodicity: number
}

export class PeriodicityChanger extends React.Component<PeriodicityChangerProps, PeriodicityChangerState> {
    configurationApi: ConfigurationApi;

    constructor(props) {
        super(props);

        this.configurationApi = new ConfigurationApi();

        this.state = {
            periodicity: 0
        };
    }

    render() {
        return <NumericInput
            min={0}
            max={300}
            value={this.state.periodicity}
            onChange={this.onChange.bind(this)}
        />
    }

    async componentDidMount() {
        this.setState({
            periodicity: await this.configurationApi.getPeriodicity()
        });
    }

    async onChange(value) {
        await this.configurationApi.setPeriodicity(value);
        this.setState({
            periodicity: value
        });
    }
}