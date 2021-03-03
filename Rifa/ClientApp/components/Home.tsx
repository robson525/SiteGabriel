import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Table } from "./Table";

export class Home extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div id="home" className="row">
            <div id="home-central" className="col-md-offset-3 col-md-6 col-sm-offset-2 col-sm-8 col-xs-offset-1 col-xs-10"
                 style={{ backgroundImage: `url(${require('../img/background.jpg')})` }}>
                <div id="home-content">
                    <p className="title">Chá Rifa do Gabriel</p>
                    <Table />
                </div>
            </div>
        </div>;
    }
}
