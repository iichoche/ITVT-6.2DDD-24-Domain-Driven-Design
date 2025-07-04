const API_URL = "http://localhost:5000/api/Producten"; // Voor lokaal testen
let alleProducten = [];

// Haalt producten op uit de API
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

// Toont de producten in de tabel
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

        // Als het product wordt aangepast, toon invoervelden
        if (p.isEditing) {
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
            // Normale weergave van product
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

// Filtert producten op beschikbaarheid
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

// Zet een product in bewerkmodus
function startAanpassen(id) {
    alleProducten = alleProducten.map(p => ({
        ...p,
        isEditing: p.id === id
    }));
    toonProducten(alleProducten);
}

// Annuleert het aanpassen van een product
function annuleerAanpassing(id) {
    alleProducten = alleProducten.map(p => ({
        ...p,
        isEditing: false
    }));
    toonProducten(alleProducten);
}

// Slaat de aanpassing van een product op via de API
function opslaanAanpassing(id) {
    const naam = document.getElementById(`edit-naam-${id}`).value.trim();
    const omschrijving = document.getElementById(`edit-omschrijving-${id}`).value.trim();
    const kosten = parseFloat(document.getElementById(`edit-kosten-${id}`).value);
    const type = document.getElementById(`edit-type-${id}`).value.trim();

    if (!naam || !omschrijving || !type || isNaN(kosten) || kosten < 0) {
        toonMelding("Vul alle velden correct in.", "red");
        return;
    }

    const data = { id, naam, omschrijving, kosten, type };

    fetch(`${API_URL}/${id}`, {
        method: 'PUT',
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

// Zet een product in of uit gebruik via de API
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

// Verwijdert een product via de API
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

// Toont een melding aan de gebruiker
function toonMelding(tekst, kleur) {
    const meldingEl = document.getElementById('melding');
    if (!meldingEl) return;
    meldingEl.textContent = tekst;
    meldingEl.style.color = kleur;
    setTimeout(() => meldingEl.textContent = '', 3000);
}

// Voegt een nieuw product toe via de API
function toevoegen(event) {
    event.preventDefault();

    const naam = document.getElementById('naam').value.trim();
    const omschrijving = document.getElementById('omschrijving').value.trim();
    const kosten = parseFloat(document.getElementById('kosten').value);
    const type = document.getElementById('type').value.trim();

    if (!naam || !omschrijving || !type || isNaN(kosten) || kosten < 0) {
        toonMelding("Vul alle velden correct in.", "red");
        return;
    }

    const nieuwProduct = { naam, omschrijving, kosten, type };

    fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(nieuwProduct)
    })
    .then(res => {
        if (res.ok) return res.json();
        else throw new Error('Fout bij toevoegen product');
    })
    .then(data => {
        toonMelding(`Product "${data.naam}" toegevoegd.`, 'green');
        document.getElementById('productForm').reset();
        laadProducten();
    })
    .catch(error => {
        toonMelding(error.message, 'red');
    });
}

// Initialiseert de pagina: voegt event listeners toe en laadt producten
function initBeschikbaarheidPagina() {
    const filterEl = document.getElementById('filter');
    if (filterEl) {
        filterEl.addEventListener('change', filterProducten);
    }
    laadProducten();

    const form = document.getElementById('productForm');
    if (form) {
        form.addEventListener('submit', toevoegen);
    }
}

// Start de app als de pagina geladen is
document.addEventListener('DOMContentLoaded', initBeschikbaarheidPagina);