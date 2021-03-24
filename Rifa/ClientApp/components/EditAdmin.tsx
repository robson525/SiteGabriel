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

export class EditAdmin extends React.Component<RouteComponentProps<{}>, EditState> {
    constructor(props: any) {
        super(props);

        var params: Record<string, any> = this.props.match.params;

        this.state = (({ loading: true, id: params.id, error: false, saved: false }) as any);

        fetch(`api/Admin/${params.id}`)
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
                        default: msg = "Operação Inválida"; break;
                    }
                    this.setState({ loading: false, error: true, message: msg });
                }
            });

        this.handleSave = this.handleSave.bind(this);
    }

    public render() {
        return this.state.loading ? <Loading /> :
            (this.state.error || this.state.saved) ? this.loadMessage() : this.loadEdit(this.state.item);
    }

    private handleSave(e: any) {
        e.preventDefault();
        
        fetch(`api/Admin/${this.state.id}`,
            {
                method: 'PUT',
                headers: HEADER,
            })
            .then((response) => {
                if (response.ok) {
                    this.setState({ saved: true, message: "Numero salvo com sucess" });
                } else {
                    this.setState({ error: true, message: "Operação Inválida" });
                }
            });
    }
    
    private loadEdit(item: RifaItem) {
        return <div>
            <form className="form" onSubmit={this.handleSave}>
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

                <a className="btn btn-default" onClick={() => this.props.history.goBack()}>Voltar</a>
                <button className={`btn btn-primary ${item.status !== 2 ? 'hidden' : ''}`} type="submit">Pago</button>
            </form>
        </div>;
    }

    private loadMessage() {
        return <div className="edit-message">
            <h1 className="title">Nº: {this.state.id}</h1>
            <div className={'alert alert-' + (this.state.saved ? 'success' : 'danger')} role="alert">{this.state.message}</div>
            <a className="btn btn-default center" onClick={() => this.props.history.goBack()}>Voltar</a>
        </div>;
    }
}
