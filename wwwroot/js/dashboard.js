// Dashboard Charts

document.addEventListener('DOMContentLoaded', function() {
    loadCharts();
});

function loadCharts() {
    fetch('/Statistics/GetChartData')
        .then(r => r.json())
        .then(data => {
            if (data.success !== false) {
                createWeeklyChart(data.weeklyLabels, data.weeklyData);
                createSubjectChart(data.subjectLabels, data.subjectData);
            }
        })
        .catch(err => console.log('Chart data not available yet'));
}

function createWeeklyChart(labels, data) {
    const ctx = document.getElementById('weeklyChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Heures d'etude',
                data: data,
                backgroundColor: 'rgba(102, 126, 234, 0.8)',
                borderColor: 'rgba(102, 126, 234, 1)',
                borderWidth: 2,
                borderRadius: 8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1 } }
            }
        }
    });
}

function createSubjectChart(labels, data) {
    const ctx = document.getElementById('subjectChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    '#667eea', '#764ba2', '#f093fb', '#f5576c',
                    '#4facfe', '#00f2fe', '#43e97b', '#38f9d7'
                ],
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { position: 'bottom' }
            }
        }
    });
}