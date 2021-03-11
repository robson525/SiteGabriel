import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Loading } from "./Loading";

const HEADER = {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
};

interface EditState {
    loading: boolean;
    id: number;
    item: RifaItem;
    saved: boolean;
    error: boolean;
    message: string;
}

export class Edit extends React.Component<RouteComponentProps<{}>, EditState> {
    constructor(props: any) {
        super(props);

        var params: Record<string, any> = this.props.match.params;

        this.state = (({ loading: true, id: params.id, error: false, saved: false }) as any);

        fetch(`api/Item/${params.id}`)
            .then(response => {
                if (response.ok) {
                    (response.json() as Promise<RifaItem>)
                        .then(data => {
                            this.setState({ item: data, loading: false });
                        });
                } else {
                    let msg = "";
                    switch (response.status) {
                        case 400: msg = "O Numero selecionado não existe"; break;
                        case 401: msg = "O Numero selecionado já está reservado"; break;
                        default: msg = "Operação Inválida"; break;
                    }
                    this.setState({ loading: false, error: true, message: msg });
                }
            });

        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    public render() {
        return this.state.loading ? <Loading /> :
            (this.state.error || this.state.saved) ? this.loadMessage() : this.loadEdit(this.state.item);
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
                headers: HEADER,
            })
            .then((response) => {
                if (response.ok) {
                    this.setState({ saved: true, message: "Numero reservado com sucess"});
                } else {
                    let msg = "";
                    switch (response.status) {
                    case 401: msg = "O Numero selecionado já está em uso"; break;
                    default: msg = "Operação Inválida"; break;
                    }
                    this.setState({ error: true, message: msg });
                }
            });
    }

    private handleCancel() {
        fetch(`api/Item/${this.state.id}`,
            {
                method: 'DELETE',
                headers: HEADER,
            })
            .then(() => {
                this.props.history.goBack();
            });
    }

    private loadEdit(item: RifaItem) {
        return <form className="form" onSubmit={this.handleSave}>
            <h1 className="title">Nº: {item.id}</h1>
            <span className="help-block">Para registrar sua solicitação. Por favor, preecha as informações abaixo.</span>
            <div className="form-group">
                <label htmlFor="name">Nome</label>
                <input type="text" className="form-control" id="name" name="name" placeholder="Nome" required={true} defaultValue={item.name} maxLength={100} />
            </div>
            <div className="form-group">
                <label htmlFor="email">Email</label>
                <input type="email" className="form-control" id="email" name="email" placeholder="Email" required={true} defaultValue={item.email} maxLength={50} />
            </div>
            <div className="form-group">
                <label htmlFor="comment">Deixe uma mensagem para Papai, Mamãe e Gabriel</label>
                <textarea className="form-control" id="comment" name="comment" placeholder="Mensagem" defaultValue={item.comment} rows={5} maxLength={500} />
            </div>

            <a className="btn btn-default" onClick={this.handleCancel}>Cancelar</a>
            <button className="btn btn-primary" type="submit">Salvar</button>
        </form>;
    }

    private loadMessage() {
        return <div className="edit-message">
            <h1 className="title">Nº: {this.state.id}</h1>
            <div className={'alert alert-' + (this.state.saved ? 'success' : 'danger')} role="alert">{this.state.message}</div>
            <a className="btn btn-default center" onClick={() => this.props.history.goBack()}>Voltar</a>
        </div>;
    }
}
