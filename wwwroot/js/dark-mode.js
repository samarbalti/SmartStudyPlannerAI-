// Dark Mode Toggle

document.addEventListener('DOMContentLoaded', function() {
    const darkModeToggle = document.getElementById('darkModeToggle');
    const body = document.body;

    // Check saved preference
    const isDarkMode = localStorage.getItem('darkMode') === 'true';
    if (isDarkMode) {
        body.classList.add('dark-mode');
        updateToggleButton(true);
    }

    if (darkModeToggle) {
        darkModeToggle.addEventListener('click', function() {
            body.classList.toggle('dark-mode');
            const isDark = body.classList.contains('dark-mode');
            localStorage.setItem('darkMode', isDark);
            updateToggleButton(isDark);
        });
    }

    function updateToggleButton(isDark) {
        if (darkModeToggle) {
            darkModeToggle.innerHTML = isDark 
                ? '<i class="fas fa-sun"></i> Mode Clair' 
                : '<i class="fas fa-moon"></i> Mode Sombre';
        }
    }
});