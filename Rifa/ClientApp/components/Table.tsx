import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface TableState {
    rifaItems: RifaItem[];
    loading: boolean;
}

export class Table extends React.Component<{}, TableState> {
    constructor() {
        super();
        this.state = { rifaItems: [], loading: true };

        fetch('api/Table/LoadRifaItems')
            .then(response => response.json() as Promise<RifaItem[]>)
            .then(data => {
                this.setState({ rifaItems: data, loading: false });
            });
    }

    public render() {
        return this.state.loading
            ? this.loadLoading()
            : this.loadTable(this.state.rifaItems);
    }

    private loadLoading() {
        return <div id="loading">
            <img src={require('../img/loading.gif')}/>
        </div>;
    }

    private loadTable(itens: RifaItem[]) {
        return <div id="table">
            {itens.map(item =>
                <div className={`cell ${item.number % 2 == 0 ? "even" : "odd"}`}>
                    {item.number}
                </div>
            )}
        </div>;
    }
}

interface RifaItem {
    number: number;
}