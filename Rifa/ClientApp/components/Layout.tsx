import * as React from 'react';

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    public render() {
        return <div id="layout" className="container-fluid" style={{ backgroundImage: `url(${require('../img/background.png')})` }}>
            <div id="central" className="row">
                <div id="central-container" className="col-md-offset-3 col-md-6 col-sm-offset-2 col-sm-8 col-xs-offset-1 col-xs-10"
                    style={{ backgroundImage: `url(${require('../img/background.jpg')})` }}>
                    <div id="central-content">
                        <p className="title">Chá Rifa do Gabriel</p>
                        {this.props.children}
                    </div>
                </div>
            </div>
        </div>;
    }
}
