import * as React from 'react';

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    public render() {
        return <div id="layout" className="container-fluid" style={{ backgroundImage: `url(${require('../img/background.png')})` }}>
            <div id="central" className="row">
                <div id="central-container" className="col-md-offset-3 col-md-6 col-sm-offset-2 col-sm-8 col-xs-offset-1 col-xs-10"
                    style={{ backgroundImage: `url(${require('../img/background.jpg')})` }}>
                    <div id="central-content">
                        <div>
                            <p className="title">Chá Rifa do Gabriel</p>
                            <p className="sub-title">Cada numero da rifa R$ 20,00</p>
                            <p className="sub-title">O sorteio será realizado dia 5 de Abril</p>
                            <p className="sub-title">Ecolha seu número e boa sorte!</p>
                        </div>
                        {this.props.children}
                        <div className="baseboard">
                            <p>Efetue o pagamento através da chave PIX</p>
                            <p id="pix">995594299</p>
                            <p>ou use o QR Code abaixo pelo celular</p>
                            <img src={require('../img/qrcode.png')} />
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }
}
