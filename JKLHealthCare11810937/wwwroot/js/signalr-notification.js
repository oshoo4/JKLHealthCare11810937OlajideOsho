
function getUserId() {
    fetch('/Account/GetUserId')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            var userId = data;
            connectToSignalRHub(userId);
        })
        .catch(error => {
            console.error("Error fetching userId:", error);
        });
}

document.addEventListener('DOMContentLoaded', function() {
  getUserId();
});


function connectToSignalRHub(userId) {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/assignmentHub", {
            accessTokenFactory: () => {
                let cookieValue = document.cookie
                    .split('; ')
                    .find(row => row.startsWith('JKLHealthCareCookie'))
                    ?.split('=')[1];
            return cookieValue;
            }
        })
        .build();

    connection.start().then(function () {
        console.log("SignalR connection established");
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("ReceiveAssignmentNotification", function (message) {
        const parsedNotification = JSON.parse(message);
        if (parsedNotification.caregiverId == userId) {
            toastr.info(parsedNotification.message, "Assignment Notification", {
                timeOut: 5000
            });
        }
    });
}
