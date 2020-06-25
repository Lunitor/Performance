declare var require: any

var React = require('react');


export class ChartsMenu extends React.Component {

    constructor(props) {
        super(props)
    }

    render() {
        const hardwares = this.props.hardwares;
        const handleClick = this.props.handleClick;

        const hardwareSwitches = [];
        for (var i = 0; i < hardwares.length; i++) {
            if (hardwares[i][1])
                hardwareSwitches.push(<button value={hardwares[i][0]} class="btn btn-sm btn-primary m-1" onClick={(e) => handleClick(e.target.value)}> {hardwares[i][0]} </button>)
            else
                hardwareSwitches.push(<button value={hardwares[i][0]} class="btn btn-sm btn-secondary m-1" onClick={(e) => handleClick(e.target.value)}> {hardwares[i][0]} </button>)
        }

        return (
            <div class="row mb-10">
                <div class="col-12 justify-content-center">
                    {hardwareSwitches}
                </div>
            </div>
        );
    }
}