import * as React from 'react';


export class Footer extends React.Component<{}, {}> {

    public render() {
        return <div className="baseboard">
            <p>Efetue o pagamento através da chave PIX</p>
            <p id="pix">995594299</p>
            <p>ou use o QR Code abaixo pelo celular</p>
            <img src={require('../img/qrcode.png')} />
        </div>;
    }
}
