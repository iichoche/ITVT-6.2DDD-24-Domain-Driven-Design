const API_URL = "http://localhost:5000/api/producten";
let alleProducten = [];

function laadProducten() {
    fetch(API_URL)
        .then(res => res.json())
        .then(data => {
            alleProducten = data;
            filterProducten();
        })
        .catch(() => {
            alert("Fout bij laden van producten.");
        });
}

function toonProducten(producten) {
    const tbody = document.getElementById('producten');
    if (!tbody) return;
    tbody.innerHTML = "";

    const isProductAanpassenPagina = document.body.id === 'product-aanpassen';

    producten.forEach(p => {
        const status = p.inGebruik 
            ? '<span class="in-gebruik">In gebruik</span>' 
            : '<span class="niet-in-gebruik">Uit gebruik</span>';

        const tr = document.createElement('tr');

        if (p.isEditing) {
            // Inline edit rij met invoervelden en opslaan/annuleer knoppen
            tr.innerHTML = `
                <td><input type="text" id="edit-naam-${p.id}" value="${p.naam}"></td>
                <td><input type="text" id="edit-omschrijving-${p.id}" value="${p.omschrijving}"></td>
                <td><input type="number" id="edit-kosten-${p.id}" min="0" step="0.01" value="${p.kosten}"></td>
                <td><input type="text" id="edit-type-${p.id}" value="${p.type}"></td>
                <td>${status}</td>
                <td>
                    <button onclick="opslaanAanpassing('${p.id}')">Opslaan</button>
                    <button onclick="annuleerAanpassing('${p.id}')">Annuleer</button>
                </td>
            `;
        } else {
            // Normale rij met knoppen bij aanpassen pagina
            tr.innerHTML = `
                <td>${p.naam}</td>
                <td>${p.omschrijving}</td>
                <td>€${p.kosten.toFixed(2)}</td>
                <td>${p.type}</td>
                <td>${status}</td>
                <td>
                    ${isProductAanpassenPagina ? `
                        <button onclick="startAanpassen('${p.id}')">Aanpassen</button>
                        <button onclick="toggleInGebruik('${p.id}', ${!p.inGebruik})">
                            Zet ${p.inGebruik ? 'uit gebruik' : 'in gebruik'}
                        </button>
                        <button onclick="verwijderProduct('${p.id}')" style="background-color: #c00; color: white;">
                            Verwijder
                        </button>
                    ` : ''}
                </td>
            `;
        }
        tbody.appendChild(tr);
    });
}

function filterProducten() {
    const filterEl = document.getElementById('filter');
    if (!filterEl) return;
    const filter = filterEl.value;
    let gefilterd = alleProducten;

    if (filter === "in") {
        gefilterd = alleProducten.filter(p => p.inGebruik === true);
    } else if (filter === "uit") {
        gefilterd = alleProducten.filter(p => p.inGebruik === false);
    }

    toonProducten(gefilterd);
}

// Start inline aanpassen
function startAanpassen(id) {
    alleProducten = alleProducten.map(p => ({
        ...p,
        isEditing: p.id === id
    }));
    toonProducten(alleProducten);
}

// Annuleer inline aanpassen
function annuleerAanpassing(id) {
    alleProducten = alleProducten.map(p => ({
        ...p,
        isEditing: false
    }));
    toonProducten(alleProducten);
}

// Opslaan van aanpassingen via PATCH
function opslaanAanpassing(id) {
    const naam = document.getElementById(`edit-naam-${id}`).value.trim();
    const omschrijving = document.getElementById(`edit-omschrijving-${id}`).value.trim();
    const kosten = parseFloat(document.getElementById(`edit-kosten-${id}`).value);
    const type = document.getElementById(`edit-type-${id}`).value.trim();

    if (!naam || !omschrijving || !type || isNaN(kosten) || kosten < 0) {
        toonMelding("Vul alle velden correct in.", "red");
        return;
    }

    const data = { naam, omschrijving, kosten, type };

    fetch(`${API_URL}/${id}`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
    .then(res => {
        if (res.ok) {
            alleProducten = alleProducten.map(p => 
                p.id === id ? {...p, ...data, isEditing: false} : {...p, isEditing: false}
            );
            toonMelding("Product aangepast.", "green");
            toonProducten(alleProducten);
        } else {
            toonMelding("Fout bij opslaan aanpassing.", "red");
        }
    })
    .catch(() => toonMelding("Verbindingsfout.", "red"));
}

function toggleInGebruik(id, inGebruik) {
    fetch(`${API_URL}/${id}/gebruik`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(inGebruik)
    })
    .then(res => {
        if (res.ok) {
            laadProducten();
            toonMelding(`Product ${inGebruik ? 'in gebruik gezet' : 'uit gebruik gezet'}`, 'green');
        } else {
            toonMelding('Fout bij aanpassen status', 'red');
        }
    })
    .catch(() => {
        toonMelding('Fout bij verbinden met server', 'red');
    });
}

function verwijderProduct(id) {
    if (!confirm('Weet je zeker dat je dit product wilt verwijderen?')) return;

    fetch(`${API_URL}/${id}`, { method: 'DELETE' })
    .then(res => {
        if (res.ok) {
            laadProducten();
            toonMelding('Product verwijderd', 'green');
        } else {
            toonMelding('Fout bij verwijderen product', 'red');
        }
    })
    .catch(() => {
        toonMelding('Fout bij verbinden met server', 'red');
    });
}

function toonMelding(tekst, kleur) {
    const meldingEl = document.getElementById('melding');
    if (!meldingEl) return;
    meldingEl.textContent = tekst;
    meldingEl.style.color = kleur;
    setTimeout(() => meldingEl.textContent = '', 3000);
}

document.addEventListener('DOMContentLoaded', () => {
    const filterEl = document.getElementById('filter');
    if (filterEl) {
        filterEl.addEventListener('change', filterProducten);
    }
    laadProducten();
});
