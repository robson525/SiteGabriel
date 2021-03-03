import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface TableState {
    rifaItems: RifaItem[];
    loading: boolean;
}

export class Home extends React.Component<RouteComponentProps<{}>, TableState> {
    constructor() {
        super();
        this.state = { rifaItems: [], loading: true };

        fetch('api/Table/LoadRifaItems')
            .then(response => response.json() as Promise<RifaItem[]>)
            .then(data => {
                this.setState({ rifaItems: data, loading: false });
            });

        this.handleEdit = this.handleEdit.bind(this);
    }

    public render() {
        return this.state.loading ? this.loadLoading() : this.loadTable(this.state.rifaItems);
    }

    private loadLoading() {
        return <div id="loading">
                   <img src={require('../img/loading.gif')} />
               </div>;
    }

    private loadTable(itens: RifaItem[]) {
        return <div id="table">
                   {itens.map(item =>
                       <a className={`cell ${item.id % 2 == 0 ? "even" : "odd"}`}
                          onClick={() => this.handleEdit(item)}>
                           {item.id}
                       </a>
                   )}
               </div>;
    }

    private handleEdit(item: RifaItem) {
        this.props.history.push("/edit/" + item.id);
    }
}

interface RifaItem {
    id: number;
}