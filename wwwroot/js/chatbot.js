// Chatbot IA

function sendMessage() {
    const input = document.getElementById('messageInput');
    const message = input.value.trim();
    if (!message) return;

    addMessage(message, 'user');
    input.value = '';
    input.disabled = true;

    // Show loading
    const loadingId = addLoadingMessage();

    fetch('/AIChat/SendMessage', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message: message })
    })
    .then(r => r.json())
    .then(data => {
        removeLoadingMessage(loadingId);
        if (data.success) {
            addMessage(data.response, 'ai');
        } else {
            addMessage('Desole, une erreur est survenue.', 'ai');
        }
        input.disabled = false;
        input.focus();
    })
    .catch(err => {
        removeLoadingMessage(loadingId);
        addMessage('Erreur de connexion. Veuillez reessayer.', 'ai');
        input.disabled = false;
    });
}

function addMessage(text, type) {
    const container = document.getElementById('chatMessages');
    const div = document.createElement('div');
    div.className = `message ${type}-message`;
    div.innerHTML = `
        <div class="message-content">
            <i class="fas ${type === 'user' ? 'fa-user' : 'fa-robot'} me-2"></i>
            <span>${escapeHtml(text).replace(/\n/g, '<br/>')}</span>
        </div>
    `;
    container.appendChild(div);
    container.scrollTop = container.scrollHeight;
}

function addLoadingMessage() {
    const container = document.getElementById('chatMessages');
    const id = 'loading-' + Date.now();
    const div = document.createElement('div');
    div.id = id;
    div.className = 'message ai-message';
    div.innerHTML = `
        <div class="message-content">
            <i class="fas fa-robot me-2"></i>
            <span><i class="fas fa-spinner fa-spin"></i> L'IA reflechit...</span>
        </div>
    `;
    container.appendChild(div);
    container.scrollTop = container.scrollHeight;
    return id;
}

function removeLoadingMessage(id) {
    const el = document.getElementById(id);
    if (el) el.remove();
}

function quickAction(type) {
    const input = document.getElementById('messageInput');
    const prompts = {
        'resume': 'Peux-tu resumer ce cours pour moi ? [Collez votre texte ici]',
        'quiz': 'Genere un quiz de revision pour moi',
        'plan': 'Aide-moi a creer un planning de revision efficace'
    };
    input.value = prompts[type] || '';
    input.focus();
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}