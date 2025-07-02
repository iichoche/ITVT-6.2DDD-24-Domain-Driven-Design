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

    producten.forEach(p => {
        const status = p.inGebruik ? '<span class="in-gebruik">In gebruik</span>' : '<span class="niet-in-gebruik">Uit gebruik</span>';

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${p.naam}</td>
            <td>${p.omschrijving}</td>
            <td>€${p.kosten.toFixed(2)}</td>
            <td>${p.type}</td>
            <td>${status}</td>
        `;
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


function toevoegen() {
    const naam = document.getElementById('naam').value.trim();
    const omschrijving = document.getElementById('omschrijving').value.trim();
    const kosten = parseFloat(document.getElementById('kosten').value) || 0;
    const type = document.getElementById('type').value.trim();
    const meldingEl = document.getElementById('melding');

    if (!naam || !omschrijving || !type) {
        meldingEl.style.color = "red";
        meldingEl.textContent = "Vul alle velden correct in.";
        return;
    }

    fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ naam, omschrijving, kosten, type })
    })
    .then(res => {
        if (res.ok) {
            meldingEl.style.color = "green";
            meldingEl.textContent = "Product toegevoegd!";
            document.getElementById('naam').value = "";
            document.getElementById('omschrijving').value = "";
            document.getElementById('kosten').value = "";
            document.getElementById('type').value = "";
        } else {
            meldingEl.style.color = "red";
            meldingEl.textContent = "Fout bij toevoegen!";
        }
    })
    .catch(() => {
        meldingEl.style.color = "red";
        meldingEl.textContent = "Fout bij verbinden met server.";
    });
}

function initBeschikbaarheidPagina() {
    const filterEl = document.getElementById('filter');
    if (filterEl) {
        filterEl.addEventListener('change', filterProducten);
        laadProducten();
    }
}
