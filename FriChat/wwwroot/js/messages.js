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
        if (message.trim() === "") return;
        const formData = new FormData(this);
        const tokenInput = this.querySelector('input[name="__RequestVerificationToken"]');
        if (tokenInput) {
            formData.append('__RequestVerificationToken', tokenInput.value);
        }
        fetch(this.action, {
            method: 'POST',
            body: formData,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        })
        .then(result => {
            if (result.success && result.html) {
                document.getElementById('messagesContainer').innerHTML = result.html;
                connection.invoke("SendMessage", conversationId, userName, message);
                input.value = "";
            } else if (result.error) {
                // Optionally show error to user
                alert(result.error);
            }
        })
        .catch(error => {
            // Optionally show error to user
            alert('Error sending message: ' + error.message);
        });
    });
}

window.initMessages = initMessages;