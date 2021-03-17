import * as React from 'react';


export class Header extends React.Component<{}, {}> {

    public render() {
        return <div >
            <p className="sub-title">Cada numero da rifa R$ 20,00</p>
            <p className="sub-title">O sorteio será dia 5 de Abril</p>
            <p className="sub-title">Ecolha seu número e boa sorte!</p>
        </div>;
    }
}
