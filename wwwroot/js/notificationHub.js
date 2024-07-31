$(function () {

    const initializeSignalRConnection = () => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .build();

        connection.on("ReceiveMessage", (message) => {
            console.log(message);
            $("#msgContainer").append(`<p>${message}</p>`);
        })

        connection.start().catch(err => console.error(err.toString()));
        return connection;
    }


    const connection = initializeSignalRConnection();

    const sendMessages = (event) => {
        const message = $("#inputMessage").val();

        connection.invoke("SendMessage", { userAgent: detectBrowser(), message: message })
            .catch(err => console.log(err.message));

        event.preventDefault();
    }



    $("#btnSend").on("click", sendMessages);
})

// Function to detect the browser name
function detectBrowser() {
    var userAgent = navigator.userAgent;
    if (userAgent.indexOf("Edg") > -1) {
        return "Microsoft Edge";
    } else if (userAgent.indexOf("Chrome") > -1) {
        return "Chrome";
    } else if (userAgent.indexOf("Firefox") > -1) {
        return "Firefox";
    } else if (userAgent.indexOf("Safari") > -1) {
        return "Safari";
    } else if (userAgent.indexOf("Opera") > -1) {
        return "Opera";
    } else if (userAgent.indexOf("Trident") > -1 || userAgent.indexOf("MSIE") > -1) {
        return "Internet Explorer";
    }

    return "Unknown";
}

