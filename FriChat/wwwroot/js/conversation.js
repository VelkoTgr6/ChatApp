document.addEventListener('DOMContentLoaded', function () {
    const userConversationBtn = document.getElementById('userConversationBtn');
    const userConversationPartialContainer = document.getElementById('userConversationPartialContainer');

    if (userConversationBtn && userConversationPartialContainer) {
        userConversationBtn.addEventListener('click', function (e) {
            e.preventDefault();

            if (userConversationPartialContainer.style.display === 'none' || userConversationPartialContainer.style.display === '') {
                fetch('/AppUser/GetUserConversationsPartial', {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                })
                    .then(response => response.text())
                    .then(html => {
                        userConversationPartialContainer.innerHTML = html;
                        userConversationPartialContainer.style.display = 'block';
                        attachConversationHandlers();
                    });
            } else {
                userConversationPartialContainer.style.display = 'none';
            }
        });

        document.addEventListener('mousedown', function (event) {
            if (
                userConversationPartialContainer.style.display === 'block' &&
                !userConversationPartialContainer.contains(event.target) &&
                !userConversationBtn.contains(event.target)
            ) {
                userConversationPartialContainer.style.display = 'none';
            }
        });
    }

    function attachConversationHandlers() {
        // Add any event handlers for conversation items here
        // Example: open a specific conversation, mark as read, etc.
    }
});

window.loadUserConversation = function(userId) {
    const container = document.getElementById('userConversationPartialContainer');
    if (!container) return;

    container.innerHTML = '<div>Loading...</div>';

    fetch(`/AppUser/GetConversationPartial?friendId=${encodeURIComponent(userId)}`, {
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    })
    .then(response => response.text())
    .then(html => {
        container.innerHTML = html;
        // Optionally attach handlers here
    })
    .catch(() => {
        container.innerHTML = '<div>Error loading conversation.</div>';
    });
};