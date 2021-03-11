import * as React from 'react';
import { RouteComponentProps } from 'react-router';

import { Loading } from "./Loading";

interface TableState {
    rifaItems: RifaItem[];
    loading: boolean;
}

export class Home extends React.Component<RouteComponentProps<{}>, TableState> {
    constructor() {
        super();
        this.state = { rifaItems: [], loading: true };

        fetch('api/Item')
            .then(response => response.json() as Promise<RifaItem[]>)
            .then(data => {
                this.setState({ rifaItems: data, loading: false });
            });

        this.handleEdit = this.handleEdit.bind(this);
    }

    public render() {
        return this.state.loading ? <Loading /> : this.loadTable(this.state.rifaItems);
    }
    
    private loadTable(itens: RifaItem[]) {
        console.log(itens);
        return <div id="table">
                   {itens.map(item =>
                       <a key={`item-${item.id}`} className={`cell ${item.status != 0 ? "reserved" : (item.id % 2 == 0 ? "even" : "odd")}`}
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
