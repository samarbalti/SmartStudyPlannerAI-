// Notifications temps reel avec SignalR

let connection = null;

document.addEventListener('DOMContentLoaded', function() {
    // Initialize SignalR
    if (typeof signalR !== 'undefined') {
        connection = new signalR.HubConnectionBuilder()
            .withUrl('/notificationHub')
            .withAutomaticReconnect()
            .build();

        connection.on('ReceiveNotification', function(data) {
            showNotification(data.title, data.message);
            updateNotificationBadge();
        });

        connection.start().catch(function(err) {
            console.error('SignalR Error:', err);
        });
    }

    // Load notifications
    loadNotifications();

    // Update badge periodically
    setInterval(updateNotificationBadge, 30000);
});

function showNotification(title, message) {
    const container = document.getElementById('notificationContainer') || document.body;
    const toast = document.createElement('div');
    toast.className = 'toast show';
    toast.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 1050; min-width: 300px;';
    toast.innerHTML = `
        <div class="toast-header bg-primary text-white">
            <i class="fas fa-bell me-2"></i>
            <strong class="me-auto">${escapeHtml(title)}</strong>
            <button type="button" class="btn-close btn-close-white" onclick="this.closest('.toast').remove()"></button>
        </div>
        <div class="toast-body">${escapeHtml(message)}</div>
    `;
    container.appendChild(toast);

    setTimeout(() => toast.remove(), 5000);
}

function loadNotifications() {
    fetch('/Notifications/GetNotifications')
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                updateNotificationList(data.notifications);
                updateBadge(data.notifications.filter(n => !n.isRead).length);
            }
        });
}

function updateNotificationList(notifications) {
    const list = document.getElementById('notificationList');
    if (!list) return;

    if (notifications.length === 0) {
        list.innerHTML = '<div class="dropdown-item text-center text-muted">Aucune notification</div>';
        return;
    }

    list.innerHTML = notifications.map(n => `
        <div class="dropdown-item notification-item ${n.isRead ? '' : 'unread'}" onclick="markAsRead(${n.id})">
            <div class="d-flex justify-content-between">
                <strong>${escapeHtml(n.title)}</strong>
                <small class="text-muted">${new Date(n.createdAt).toLocaleDateString()}</small>
            </div>
            <small>${escapeHtml(n.message)}</small>
        </div>
    `).join('');
}

function updateNotificationBadge() {
    fetch('/Notifications/GetNotifications')
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                const unread = data.notifications.filter(n => !n.isRead).length;
                updateBadge(unread);
            }
        });
}

function updateBadge(count) {
    const badge = document.getElementById('notificationBadge');
    if (badge) {
        badge.textContent = count;
        badge.style.display = count > 0 ? 'block' : 'none';
    }
}

function markAsRead(id) {
    fetch('/Notifications/MarkAsRead/' + id, { method: 'POST' })
        .then(() => loadNotifications());
}

function markAllAsRead() {
    fetch('/Notifications/MarkAllAsRead', { method: 'POST' })
        .then(() => loadNotifications());
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}