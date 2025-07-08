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
                        attachMessageTypeMenuHandlers();
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

    // Attach message type menu handlers for the conversation partial
    function attachMessageTypeMenuHandlers() {
        const moreBtn = document.getElementById('moreBtn');
        const menu = document.getElementById('messageTypeMenu');
        const messageTypeInput = document.getElementById('messageType');
        const fileInput = document.getElementById('fileInput');
        const fileLabel = document.getElementById('fileLabel');
        const messageInput = document.getElementById('messageInput');

        if (!moreBtn || !menu || !messageTypeInput || !fileInput || !fileLabel || !messageInput) return;

        moreBtn.addEventListener('click', function () {
            menu.style.display = (menu.style.display === 'flex') ? 'none' : 'flex';
        });

        document.querySelectorAll('.message-type-option').forEach(option => {
            option.addEventListener('click', function () {
                const selected = this.getAttribute('data-type');
                messageTypeInput.value = selected;
                menu.style.display = 'none';

                if (selected === 'Text' || selected === 'Link') {
                    messageInput.style.display = 'block';
                    fileLabel.style.display = 'none';
                } else {
                    messageInput.style.display = 'none';
                    fileLabel.style.display = 'block';
                    // Auto-open file picker for media types
                    setTimeout(function() { fileInput.click(); }, 100);
                }
            });
        });

        // Close popup when clicking outside
        document.addEventListener('click', function (event) {
            if (!menu.contains(event.target) && !moreBtn.contains(event.target)) {
                menu.style.display = 'none';
            }
        });
    }

    // Attach handlers on initial load (if partial is already present)
    attachMessageTypeMenuHandlers();
});

window.loadUserConversation = function (userId) {
    const container = document.getElementById('userConversationPartialContainer');
    if (!container) return;

    container.innerHTML = '<div>Loading...</div>';

    fetch(`/AppUser/GetConversationPartial?friendId=${encodeURIComponent(userId)}`, {
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    })
        .then(response => {
            if (!response.ok) throw new Error('Server error: ' + response.status);
            return response.text();
        })
        .then(html => {
            container.innerHTML = html;
            try {
                if (window.initMessages) window.initMessages();
            } catch (e) {
                container.innerHTML += '<div style="color:red;">initMessages failed.</div>';
            }
            // Attach handlers after loading new partial
            if (typeof attachMessageTypeMenuHandlers === 'function') attachMessageTypeMenuHandlers();
        })
        .catch(() => {
            container.innerHTML = '<div>Error loading conversationnnn.</div>';
        });
};