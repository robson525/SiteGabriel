import * as React from 'react';

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    public render() {
        return <div id="layout" className="container-fluid" style={{ backgroundImage: `url(${require('../img/background.png')})` }}>
                   { this.props.children }
               </div>;
    }
}
