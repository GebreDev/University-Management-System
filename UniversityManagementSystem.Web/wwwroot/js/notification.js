// SignalR Notification Client
"use strict";

let notificationConnection = null;

function initializeNotificationHub() {
    notificationConnection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    notificationConnection.on("ReceiveNotification", function (title, message, notificationId, type, linkUrl) {
        // Show toast or alert
        showNotification(title, message, linkUrl);
        // Update badge count
        updateNotificationBadge();
    });

    notificationConnection.start()
        .then(function () {
            console.log("SignalR notification hub connected.");
            // Join user group
            const userId = $("#currentUserId").val();
            if (userId) {
                notificationConnection.invoke("JoinUserGroup", userId);
            }
        })
        .catch(function (err) {
            console.error("Error connecting to notification hub: " + err.toString());
        });
}

function showNotification(title, message, linkUrl) {
    // Use Bootstrap toast or custom alert
    const toastHtml = `
        <div class="toast align-items-center text-white bg-primary border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <strong>${title}</strong><br />
                    ${message}
                    ${linkUrl ? `<a href="${linkUrl}" class="text-white">View</a>` : ''}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    const toastContainer = document.getElementById("toastContainer") || createToastContainer();
    const toastElement = document.createElement("div");
    toastElement.innerHTML = toastHtml;
    toastContainer.appendChild(toastElement.firstElementChild);
    const toast = new bootstrap.Toast(toastElement.firstElementChild);
    toast.show();
}

function createToastContainer() {
    const container = document.createElement("div");
    container.id = "toastContainer";
    container.style.position = "fixed";
    container.style.top = "20px";
    container.style.right = "20px";
    container.style.zIndex = "9999";
    document.body.appendChild(container);
    return container;
}

function updateNotificationBadge() {
    $.ajax({
        url: "/Notification/GetUnreadCount",
        method: "GET",
        success: function (data) {
            const badge = $("#notificationBadge");
            if (data.count > 0) {
                badge.text(data.count).show();
            } else {
                badge.hide();
            }
        }
    });
}

// Expose functions
window.initializeNotificationHub = initializeNotificationHub;
window.updateNotificationBadge = updateNotificationBadge;