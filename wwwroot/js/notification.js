const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

notificationConnection.on("ReceiveNotification", function (message) {
    // simple: append to #notificationsList if present, or alert
    const list = document.getElementById("notificationsList");
    if (list) {
        const li = document.createElement("li");
        li.textContent = message;
        list.prepend(li);
    } else {
        // fallback
        console.log("Notification:", message);
        // optionally show toast
    }
});

notificationConnection.start().catch(err => console.error(err.toString()));
