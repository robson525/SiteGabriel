import * as React from 'react';
import { RouteComponentProps } from 'react-router';

const HEADER = {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
};

interface TableState {
    rifaItems: RifaItem[];
    logged: boolean;
    error: boolean;
    message: string;
}

export class Admin extends React.Component<RouteComponentProps<{}>, TableState> {
    constructor() {
        super();
        this.state = { rifaItems: [], logged: false, error: false, message: "" };

        fetch('api/Admin')
            .then(response => {
                if (response.ok) {
                    (response.json() as Promise<RifaItem[]>)
                        .then(data => {
                            this.setState({ logged: true, rifaItems: data });
                        });
                } else {
                    this.setState({ logged: false });
                }
            });

        this.handleEdit = this.handleEdit.bind(this);
        this.handleLogin = this.handleLogin.bind(this);
    }

    public render() {
        return this.state.logged ? this.loadTable(this.state.rifaItems) : this.loadForm();
    }

    private loadForm() {

        const message = !this.state.error
            ? (<div></div>) :
            <div className="edit-message">
                <button type="button" className="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <div className="alert alert-danger alert-dismissible" role="alert">{this.state.message}</div>
            </div>;

        return <div>
            {message}
            <form className="form" onSubmit={this.handleLogin}>
                <div className="form-group">
                    <label htmlFor="exampleInputEmail1">Usuário</label>
                    <input type="text" className="form-control" id="user" name="user" placeholder="Usuário" />
                </div>
                <div className="form-group">
                    <label htmlFor="exampleInputPassword1">Password</label>
                    <input type="password" className="form-control" id="password" name="password" placeholder="Password" />
                </div>
                <button type="submit" className="btn btn-default">Entrar</button>
            </form>
        </div >;
    }

    private handleLogin(e: any) {
        e.preventDefault();

        fetch(`api/Admin`,
            {
                method: 'POST',
                body: JSON.stringify({ user: e.target.user.value, password: e.target.password.value }),
                headers: HEADER,
            })
            .then((response) => {
                if (response.ok) {
                    (response.json() as Promise<RifaItem[]>)
                        .then(data => {
                            this.setState({ error: false, logged: true, rifaItems: data });
                        });
                } else {
                    this.setState({ error: true, logged: false, message: "Usuário Inválido" });
                }
            });
    }

    private loadTable(itens: RifaItem[]) {
        return <div>
            <p className="sub-title">Administração</p>
            <p className="info-title status-0">Disponiveis: {itens.filter(item => item.status == 0).length}</p>
            <p className="info-title status-1">Reservando: {itens.filter(item => item.status == 1).length}</p>
            <p className="info-title status-2">Reservados: {itens.filter(item => item.status == 2).length}</p>
            <p className="info-title status-3">Pagos: {itens.filter(item => item.status == 3).length}</p>

            <div id="table">
                {itens.map(item =>
                    <a key={`item-${item.id}`}
                        className={`cell status-${item.status}`}
                        onClick={() => item.status > 1 ? this.handleEdit(item) : null}
                        data-toggle="tooltip" data-placement="left" title="Tooltip on left"
                    >
                        {item.id}
                    </a>
                )}
            </div>
        </div >;
    }

    private handleEdit(item: RifaItem) {
        this.props.history.push("/editadmin/" + item.id);
    }

}
