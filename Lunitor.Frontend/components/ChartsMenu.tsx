import * as React from "react";

type ChartsMenuProps = {
    hardwares: [string, boolean][],
    handleClick: (hardwareName: string) => void
};

export class ChartsMenu extends React.Component<ChartsMenuProps> {
    render() {
        const hardwares = this.props.hardwares;
        const handleClick = this.props.handleClick;

        const hardwareSwitches = [];
        for (var i = 0; i < hardwares.length; i++) {
            if (hardwares[i][1])
                hardwareSwitches.push(<button value={hardwares[i][0]}
                    className="btn btn-sm btn-primary m-1"
                    onClick={(e: React.MouseEvent<HTMLButtonElement>) => handleClick(e.currentTarget.value)}>
                        {hardwares[i][0]}
                </button>)
            else
                hardwareSwitches.push(<button value={hardwares[i][0]}
                    className="btn btn-sm btn-secondary m-1"
                    onClick={(e: React.MouseEvent<HTMLButtonElement>) => handleClick(e.currentTarget.value)}>
                        {hardwares[i][0]}
                </button>)
        }

        return (
            <div className="row mb-10">
                <div className="col-12 justify-content-center">
                    {hardwareSwitches}
                </div>
            </div>
        );
    }
}