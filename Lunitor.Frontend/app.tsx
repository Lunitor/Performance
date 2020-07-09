import * as React from "react";
import * as ReactDOM from 'react-dom';

import { Content } from "./components/Content";


export class Application extends React.Component {
    render() {
        var page = [];

        const content = <Content />
        page.push(content);

        return ( page );
    }
}

ReactDOM.render(<Application />, document.getElementById('root'));