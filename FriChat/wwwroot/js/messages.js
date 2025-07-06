function convertMessageTimestampsToLocal() {
    document.querySelectorAll('.message-timestamp').forEach(function (el) {
        const utc = el.getAttribute('data-utc');
        if (utc) {
            const local = new Date(utc);
            // Use the user's locale, show date and time, but no seconds
            const options = {
                year: 'numeric',
                month: 'short',
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
                hour12: false // Remove this line if you want 12-hour format where appropriate
            };
            el.textContent = local.toLocaleString(undefined, options);
        }
    });
}

function initMessages() {
    const container = document.getElementById('userConversationPartialContainer');
    const chatWindow = container ? container.querySelector('.chat-window') : null;
    if (!chatWindow) return;

    const conversationId = chatWindow.getAttribute('data-conversation-id');
    const friendId = chatWindow.getAttribute('data-friend-id');
    const userName = chatWindow.getAttribute('data-user-name');

    function loadMessages() {
        fetch(`/AppUser/GetUserMessagesForConversationPartial?friendId=${friendId}&conversationId=${conversationId}`, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
        .then(response => response.text())
            .then(html => {
                const messagesContainer = document.getElementById('messagesContainer');
                if (messagesContainer) {
                    messagesContainer.innerHTML = html;
                    convertMessageTimestampsToLocal(); // Convert timestamps after loading
                }
            });
    }

    loadMessages();

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .build();

    connection.on("ReceiveMessage", function (user, message) {
        loadMessages();
    });

    connection.start().then(function () {
        connection.invoke("JoinConversation", conversationId);
    });

    document.getElementById('sendMessageForm').addEventListener('submit', function (e) {
        e.preventDefault();
        const input = document.getElementById('messageInput');
        const message = input.value;
        const friendId = this.querySelector('input[name="friendId"]').value;
        const conversationId = this.querySelector('input[name="conversationId"]').value;
        // No need to get tokenInput or append it manually
        if (!friendId || !conversationId || message.trim() === "") {
            alert('All fields are required.');
            return;
        }
        const formData = new FormData(this); // This will include the anti-forgery token automatically
        fetch(this.action, {
            method: 'POST',
            body: formData,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
        .then(async response => {
            let result;
            try {
                result = await response.json();
            } catch (err) {
                // If not JSON, show error
                throw new Error('Server returned an invalid response.');
            }
            if (!response.ok) {
                throw new Error(result && result.error ? result.error : 'Network response was not ok');
            }
            return result;
        })
        .then(result => {
            if (result.success && result.html) {
                document.getElementById('messagesContainer').innerHTML = result.html;
                convertMessageTimestampsToLocal(); // Convert timestamps after sending
                connection.invoke("SendMessage", conversationId, userName, message);
                input.value = "";
            } else if (result.error) {
                alert(result.error);
            }
        })
        .catch(error => {
            alert('Error sending message: ' + error.message);
        });
    });
}

window.initMessages = initMessages;