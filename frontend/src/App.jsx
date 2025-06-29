import { useEffect, useState } from 'react';

export default function App() {
    const [producten, setProducten] = useState([]);
    const [naam, setNaam] = useState("");

    useEffect(() => {
        fetch('http://localhost:5000/api/producten')
            .then(res => res.json())
            .then(setProducten);
    }, []);

    const toevoegen = () => {
        fetch('http://localhost:5000/api/producten', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ naam, omschrijving: "", kosten: 0, type: "" })
        }).then(() => window.location.reload());
    }

    return (
        <div>
            <h1>Zorgtechnologie Producten</h1>
            <ul>
                {producten.map(p => <li key={p.id}>{p.naam}</li>)}
            </ul>
            <input value={naam} onChange={e => setNaam(e.target.value)} placeholder="Naam" />
            <button onClick={toevoegen}>Toevoegen</button>
        </div>
    );
}
