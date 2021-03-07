import * as React from 'react';


export class Loading extends React.Component<{}, {}> {

    public render() {
        return <div id="loading">
            <img src={require('../img/loading.gif')} />
        </div>;
    }
}
