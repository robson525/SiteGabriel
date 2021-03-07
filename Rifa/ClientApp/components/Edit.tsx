import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Loading } from "./Loading";

interface EditState {
    loading: boolean;
    id: number;
    item: RifaItem;
}

export class Edit extends React.Component<RouteComponentProps<{}>, EditState> {
    constructor(props: any) {
        super(props);

        var params: Record<string, any> = this.props.match.params;

        this.state = (({ loading: true, id: params.id }) as any);

        fetch(`api/Item/${params.id}`)
            .then(response => response.json() as Promise<RifaItem>)
            .then(data => {
                this.setState({ item: data, loading: false });
            });

        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    public render() {
        return this.state.loading ? <Loading /> : this.loadEdit(this.state.item);
    }

    private handleSave(e: any) {
        e.preventDefault();

        let item: RifaItem = {
            id: this.state.id,
            name: e.target.name.value,
            email: e.target.email.value,
            comment: e.target.comment.value,
            status: 0
        };

        fetch(`api/Item/${this.state.id}`,
                {
                    method: 'POST',
                    body: JSON.stringify(item),
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                })
            .then((response) => {
                 if (response.ok) {
                     this.props.history.goBack();
                 }
            });
    }

    private handleCancel() {
        this.props.history.goBack();
    }
    
    private loadEdit(item: RifaItem) {
        return <form className="form" onSubmit={this.handleSave}>
            <div className="form-group">
                <label htmlFor="name">Nome</label>
                <input type="text" className="form-control" id="name" name="name" placeholder="Nome" required={true} defaultValue={item.name} />
            </div>
            <div className="form-group">
                <label htmlFor="email">Email</label>
                <input type="email" className="form-control" id="email" name="email" placeholder="Email" required={true} defaultValue={item.email} />
            </div>
            <div className="form-group">
                <label htmlFor="comment">Comentário</label>
                <textarea className="form-control" id="comment" name="comment" placeholder="Comentário" defaultValue={item.comment} rows={5} />
            </div>

            <a className="btn btn-default" onClick={this.handleCancel}>Cancelar</a>
            <button className="btn btn-primary" type="submit">Salvar</button>
        </form>;
    }
}
