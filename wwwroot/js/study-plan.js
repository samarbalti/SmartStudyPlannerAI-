// Study Plan JavaScript

function generateStudyPlan() {
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

    if (!startDate || !endDate) {
        alert('Veuillez selectionner les dates de debut et de fin.');
        return;
    }

    const btn = document.getElementById('generateBtn');
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Generation en cours...';

    fetch('/StudyPlan/Generate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}`
    })
    .then(r => {
        if (r.redirected) {
            window.location.href = r.url;
        }
    })
    .catch(err => {
        btn.disabled = false;
        btn.innerHTML = '<i class="fas fa-magic me-2"></i>Generer le planning';
        alert('Erreur lors de la generation.');
    });
}