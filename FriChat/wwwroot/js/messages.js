<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>
<script>
    const conversationId = '@Model.ConversationId';
    const userName = '@User.Identity.Name';

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .build();

    connection.on("ReceiveMessage", function (user, message) {
        const msgDiv = document.createElement("div");
        msgDiv.innerHTML = `<strong>${user}:</strong> ${message}`;
        document.querySelector(".chat-messages").appendChild(msgDiv);
    });

    connection.start().then(function () {
        connection.invoke("JoinConversation", conversationId);
    });

    document.querySelector(".chat-input").addEventListener("submit", function (e) {
        e.preventDefault();
        const input = this.querySelector("input[type='text']");
        const message = input.value;
        connection.invoke("SendMessage", conversationId, userName, message);
        input.value = "";
    });
</script>