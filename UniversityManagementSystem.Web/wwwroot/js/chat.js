// ============================================================
//  Messenger client (Telegram-style UI) - SignalR + AJAX
// ============================================================
"use strict";

let connection = null;
let currentConversationId = null;
let currentRecipientId = null;
let loadingMessages = false;
let loadedCount = 0;
let pendingAttachment = null; // { base64, fileName, type }
let typingTimer = null;
let presenceTimer = null;
let emojiList = null;

const ROLE_COLORS = { Student: "#f06292", Instructor: "#4a90e2", Admin: "#26a69a" };

// ---------------- Helpers ----------------

function avatarColor(role) {
    return ROLE_COLORS[role] || "#7e57c2";
}

function initials(name) {
    if (!name) return "?";
    const parts = name.trim().split(/\s+/);
    if (parts.length === 0) return "?";
    if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
}

function escapeHtml(str) {
    return String(str ?? "").replace(/[&<>"']/g, c => ({
        "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
    }[c]));
}

function scrollToBottom(instant) {
    const box = document.getElementById("chatBody");
    if (!box) return;
    box.scrollTo({ top: box.scrollHeight, behavior: instant ? "auto" : "smooth" });
}

// ---------------- SignalR ----------------

function initializeChatHub() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveMessage", function (message) {
        if (message.conversationId && message.conversationId !== currentConversationId) return;
        appendMessage(message);
        markMessageAsRead(message.id);
        refreshConversationList();
    });

    connection.on("NewMessage", function (conversationId) {
        if (typeof window.handleNewMessage === "function") {
            window.handleNewMessage(conversationId);
        }
        refreshConversationList();
    });

    connection.on("UserTyping", function (userId, isTyping) {
        if (userId && currentRecipientId && userId === currentRecipientId) {
            showTyping(isTyping);
        }
    });

    connection.on("MessageRead", function (messageId) {
        $(`#msg-${messageId} .chat-ticks`).addClass("read").html('<i class="fa-solid fa-check-double"></i>');
    });

    connection.start()
        .then(function () {
            console.log("SignalR chat hub connected.");
            const convId = $("#conversationId").val();
            if (convId) {
                connection.invoke("JoinConversation", parseInt(convId));
            }
        })
        .catch(function (err) {
            console.error("Error connecting to chat hub: " + err.toString());
        });
}

// ---------------- Presence ----------------

function startPresencePolling() {
    stopPresencePolling();
    if (!currentRecipientId) return;
    checkPresence();
    presenceTimer = setInterval(checkPresence, 20000);
}

function stopPresencePolling() {
    if (presenceTimer) { clearInterval(presenceTimer); presenceTimer = null; }
}

function checkPresence() {
    if (!currentRecipientId) return;
    $.get("/Chat/IsOnline", { userId: currentRecipientId })
        .done(function (res) {
            const online = !!res.online;
            // Thread header dot + status text
            $("#threadAvatar .chat-status-dot").attr("data-dot", online ? "on" : "off");
            $(".chat-thread-status .dot").attr("data-on", online ? "true" : "false");
            if (online) {
                $("#threadStatusText").text("Active now");
            } else {
                $("#threadStatusText").text("Last seen recently");
            }
            // List dot for this conversation
            const item = $(`.chat-conv-item[data-conversation-id="${currentConversationId}"]`);
            item.find(".chat-status-dot").attr("data-dot", online ? "on" : "off");
        })
        .fail(function () { /* presence unavailable - leave as-is */ });
}

function showTyping(isTyping) {
    if (isTyping) {
        $("#threadStatusText").text("typing...");
        $("#threadAvatar .chat-status-dot").attr("data-dot", "on");
        $(".chat-thread-status .dot").attr("data-on", "true");
        clearTimeout(typingTimer);
        typingTimer = setTimeout(function () {
            checkPresence();
        }, 3000);
    } else {
        clearTimeout(typingTimer);
        checkPresence();
    }
}

// ---------------- Conversation list ----------------

function refreshConversationList(callback) {
    $.get("/Chat/Index")
        .done(function (html) {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, "text/html");
            const newList = doc.getElementById("conversationsList");
            const oldList = document.getElementById("conversationsList");
            if (newList && oldList) {
                oldList.innerHTML = newList.innerHTML;
                bindConversationItems();
                // Re-apply the active highlight for the open conversation
                if (currentConversationId) {
                    const active = $(`.chat-conv-item[data-conversation-id="${currentConversationId}"]`);
                    active.addClass("active").find(".chat-unread").remove();
                }
            }
            if (typeof callback === "function") callback();
        })
        .fail(function () {
            if (typeof callback === "function") callback();
        });
}

function bindConversationItems() {
    $("#chatSearch").off("input").on("input", filterConversations);
    $("#chatFilters .chat-pill").off("click").on("click", function () {
        $("#chatFilters .chat-pill").removeClass("active");
        $(this).addClass("active");
        filterConversations();
    });

    $(".chat-conv-item").off("click").on("click", function () {
        const rawConvId = $(this).data("conversation-id");
        const convId = rawConvId ? parseInt(rawConvId, 10) : null;
        const otherUserId = $(this).data("other-user-id");
        selectConversationItem(this);
        loadConversation(convId, otherUserId);
        if (convId) {
            history.pushState(null, "", "?conversationId=" + convId);
        }
    });

    // Auto-open conversation from query parameter on first bind
    if (!window._chatInitDone) {
        window._chatInitDone = true;
        const urlParams = new URLSearchParams(window.location.search);
        const autoConvId = urlParams.get("conversationId");
        if (autoConvId) {
            const item = $(`.chat-conv-item[data-conversation-id="${autoConvId}"]`);
            if (item.length) {
                selectConversationItem(item[0]);
                loadConversation(parseInt(autoConvId), item.data("other-user-id"));
            }
        }
    }
}

function selectConversationItem(el) {
    $(".chat-conv-item").removeClass("active");
    $(el).addClass("active");
    // clear unread badge on the item
    $(el).find(".chat-unread").remove();
}

function filterConversations() {
    const q = ($("#chatSearch").val() || "").toLowerCase().trim();
    const role = $("#chatFilters .chat-pill.active").data("role") || "";
    const hasAny = $(".chat-conv-item").length > 0;
    $(".chat-conv-item").each(function () {
        const item = $(this);
        const matchesRole = !role || item.data("role") === role;
        const haystack = (item.data("search") || "").toLowerCase();
        const matchesQuery = !q || haystack.indexOf(q) !== -1;
        item.toggle(matchesRole && matchesQuery);
    });
    const anyVisible = $(".chat-conv-item:visible").length > 0;
    const filtering = hasAny && (q || role);
    $("#filterEmpty").toggle(filtering && !anyVisible);
    $("#emptyConvs").toggle(!hasAny && !anyVisible);
}

// ---------------- Conversation loading ----------------

function loadConversation(convId, otherUserId) {
    currentConversationId = convId || null;
    currentRecipientId = otherUserId;
    loadedCount = 0;
    $("#conversationId").val(convId || "");
    $("#currentRecipientId").val(otherUserId);
    $("#chatComposer").show();

    // Populate the thread header from the list item (match by conversation, else by user)
    const item = convId
        ? $(`.chat-conv-item[data-conversation-id="${convId}"]`)
        : $(`.chat-conv-item[data-other-user-id="${otherUserId}"]`).first();
    const name = item.data("name") || "Conversation";
    const role = item.data("role") || "";
    const pic = item.data("pic") || "";
    $("#threadName").text(name);
    $("#threadAvatar").css("background", avatarColor(role));
    if (pic) {
        $("#threadAvatar").html(`<img src="${pic}" alt="${escapeHtml(name)}" /><span class="chat-status-dot" data-dot="off"></span>`);
    } else {
        $("#threadAvatar").html(`<span>${initials(name)}</span><span class="chat-status-dot" data-dot="off"></span>`);
    }

    // Mobile: show thread panel
    $("#chatApp").addClass("thread-open");
    $("#chatBackBtn").off("click").on("click", closeThreadMobile);

    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        if (convId) {
            connection.invoke("JoinConversation", parseInt(convId, 10));
        }
    }

    if (convId) {
        loadMessages(0, true);
    } else {
        // Brand-new contact with no conversation yet: show an empty thread.
        $("#chatBody").html('<div class="chat-empty-state"><i class="fa-regular fa-hand-peace"></i><h6>Say hello!</h6><p>Send the first message to start the conversation.</p></div>');
        scrollToBottom(true);
    }
    startPresencePolling();
}

function closeThreadMobile() {
    $("#chatApp").removeClass("thread-open");
}

function loadMessages(skip, resetScroll) {
    loadingMessages = true;
    $.get("/Chat/Conversation", { conversationId: currentConversationId, skip: skip, take: 50 })
        .done(function (html) {
            if (skip === 0) {
                $("#chatBody").html(html);
                scrollToBottom(true);
            } else {
                const temp = document.createElement("div");
                temp.innerHTML = html;
                // remove date pills that duplicate the boundary
                $(temp).find(".chat-date-pill:first").remove();
                const existing = document.getElementById("chatBody");
                // prepend ALL nodes of the older batch (not just the first)
                while (temp.firstChild) {
                    existing.insertBefore(temp.firstChild, existing.firstChild);
                }
                // keep scroll position anchored
                const prevHeight = existing.scrollHeight;
                existing.scrollTop = existing.scrollHeight - prevHeight + 40;
            }
            const endMarker = $("#chatBody .chat-load-older-end").last();
            if (endMarker.length) {
                loadedCount = parseInt(endMarker.data("skip")) || loadedCount;
            }
            loadingMessages = false;
        })
        .fail(function () {
            if (skip === 0) {
                $("#chatBody").html('<div class="chat-empty-state"><i class="fa-regular fa-circle-xmark"></i><h6>Failed to load messages</h6><p>Please try again.</p></div>');
            }
            loadingMessages = false;
        });
}

// ---------------- Real-time append ----------------

function appendMessage(message) {
    if (loadingMessages) return;
    const chatBody = document.getElementById("chatBody");
    if (!chatBody) return;

    const isMine = message.senderId === $("#currentUserId").val();
    const senderName = message.senderName || "User";
    const time = message.sentAt ? new Date(message.sentAt).toLocaleTimeString([], { hour: "numeric", minute: "2-digit" }) : "";

    const div = document.createElement("div");
    div.className = `chat-msg ${isMine ? "sent" : "received"}`;
    div.id = `msg-${message.id || Date.now()}`;

    let bubbleInner = "";
    if (message.content) {
        bubbleInner += `<span class="chat-bubble-text">${escapeHtml(message.content)}</span>`;
    }
    if (message.attachmentPath) {
        const lower = String(message.attachmentPath).toLowerCase();
        const safePath = escapeHtml(message.attachmentPath);
        if (/\.(png|jpe?g|gif|webp)$/.test(lower)) {
            bubbleInner += `<a href="${safePath}" target="_blank" class="chat-attach-img-link"><img src="${safePath}" class="chat-attach-img" alt="attachment" /></a>`;
        } else {
            const fname = message.attachmentPath.split("/").pop() || "file";
            bubbleInner += `<a href="${safePath}" target="_blank" class="chat-attach-file"><i class="fa-solid fa-file"></i><span>${escapeHtml(fname)}</span><i class="fa-solid fa-download"></i></a>`;
        }
    }
    bubbleInner += `<span class="chat-msg-time">${time}` +
        (isMine ? `<span class="chat-ticks ${message.isRead ? "read" : ""}">` +
        (message.isRead ? '<i class="fa-solid fa-check-double"></i>' : '<i class="fa-solid fa-check"></i>') + "</span>" : "") +
        `</span>`;

    div.innerHTML = (isMine ? "" : `<div class="chat-msg-avatar chat-avatar"><span>${initials(senderName)}</span></div>`) +
        `<div class="chat-bubble">${bubbleInner}</div>`;

    chatBody.appendChild(div);
    scrollToBottom(false);
}

function markMessageAsRead(messageId) {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke("MarkAsRead", messageId).catch(function () { });
    }
}

// ---------------- Send ----------------

function sendCurrentMessage() {
    const input = $("#messageInput");
    const content = input.val().trim();
    if (!currentConversationId && !currentRecipientId) return;
    if (!content && !pendingAttachment) return;

    const dto = {
        conversationId: currentConversationId,
        recipientId: currentRecipientId || "",
        content: content,
        attachmentBase64: pendingAttachment ? pendingAttachment.base64 : null,
        attachmentFileName: pendingAttachment ? pendingAttachment.fileName : null,
        attachmentType: pendingAttachment ? pendingAttachment.type : null
    };

    const btn = $("#sendButton");
    btn.prop("disabled", true);

    $.ajax({
        url: "/Chat/SendMessage",
        type: "POST",
        contentType: "application/json",
        headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() },
        data: JSON.stringify(dto),
        success: function () {
            input.val("");
            clearPendingAttachment();
            btn.prop("disabled", false);
            if (currentConversationId) {
                // Existing conversation: reload messages and update the live preview
                loadMessages(0, true);
                const item = $(`.chat-conv-item[data-conversation-id="${currentConversationId}"]`);
                item.find(".chat-conv-preview").text(content || "📎 Attachment");
                item.find(".chat-conv-time").text(new Date().toLocaleTimeString([], { hour: "numeric", minute: "2-digit" }));
                if (connection && connection.state === signalR.HubConnectionState.Connected) {
                    connection.invoke("NotifyNewMessage", currentConversationId).catch(function () { });
                }
            } else {
                // First message of a brand-new conversation: refresh the list, then open it
                const recipientId = currentRecipientId;
                refreshConversationList(function () {
                    const item = $(`.chat-conv-item[data-other-user-id="${recipientId}"]`).first();
                    const convId = item.length ? item.data("conversation-id") : null;
                    if (convId) {
                        selectConversationItem(item[0]);
                        loadConversation(parseInt(convId, 10), recipientId);
                        if (connection && connection.state === signalR.HubConnectionState.Connected) {
                            connection.invoke("NotifyNewMessage", parseInt(convId, 10)).catch(function () { });
                        }
                    } else {
                        location.reload();
                    }
                });
            }
        },
        error: function (xhr) {
            console.error("Failed to send message:", xhr.responseText);
            btn.prop("disabled", false);
            alert("Failed to send message. Please try again.");
        }
    });
}

// ---------------- Attachment ----------------

function setupAttachment() {
    $("#chatAttachBtn").on("click", function () { $("#chatAttachInput").trigger("click"); });
    $("#chatAttachInput").on("change", function () {
        const file = this.files && this.files[0];
        if (!file) return;
        if (file.size > 10 * 1024 * 1024) {
            alert("File size exceeds the 10MB limit.");
            this.value = "";
            return;
        }
        const reader = new FileReader();
        reader.onload = function (e) {
            pendingAttachment = {
                base64: e.target.result,
                fileName: file.name,
                type: file.type || "application/octet-stream"
            };
            $("#attachChipName").text(file.name);
            $("#attachChip").show();
        };
        reader.readAsDataURL(file);
        this.value = "";
    });
    $("#attachChipRemove").on("click", clearPendingAttachment);
}

function clearPendingAttachment() {
    pendingAttachment = null;
    $("#attachChip").hide();
    $("#attachChipName").text("");
}

// ---------------- Emoji ----------------

function setupEmojiPicker() {
    emojiList = ["😀", "😁", "😂", "🤣", "😊", "😍", "😘", "😎",
        "🤩", "🥳", "😇", "🙂", "🤔", "😅", "😉", "🙃",
        "😢", "😭", "😡", "😤", "😱", "🥺", "😴", "🤯",
        "👍", "👎", "👏", "🙏", "💪", "🤝", "✌️", "🤞",
        "❤️", "💙", "💚", "💜", "🧡", "💯", "🔥", "✨",
        "🎉", "🎊", "🎁", "🥇", "☕", "🍕", "🍔", "🎓"];

    const picker = $("#emojiPicker");
    picker.html(emojiList.map(e => `<button type="button" class="emoji-opt" data-emoji="${e}">${e}</button>`).join(""));

    $("#emojiBtn").on("click", function (e) {
        e.stopPropagation();
        picker.toggleClass("open");
    });

    picker.on("click", ".emoji-opt", function () {
        const emoji = $(this).data("emoji");
        const input = $("#messageInput")[0];
        const start = input.selectionStart || input.value.length;
        input.value = input.value.slice(0, start) + emoji + input.value.slice(input.selectionEnd || start);
        input.focus();
        const pos = start + emoji.length;
        input.setSelectionRange(pos, pos);
        autoGrow(input);
        picker.removeClass("open");
    });

    $(document).on("click", function (e) {
        if (!$(e.target).closest(".chat-input-wrap").length) {
            picker.removeClass("open");
        }
    });
}

function autoGrow(el) {
    el.style.height = "auto";
    el.style.height = Math.min(el.scrollHeight, 120) + "px";
}

// ---------------- Init ----------------

$(document).ready(function () {
    setupAttachment();
    setupEmojiPicker();
    bindConversationItems();

    // Send on Enter (Shift+Enter = newline)
    $("#messageInput").on("keydown", function (e) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            sendCurrentMessage();
        }
    });
    $("#messageInput").on("input", function () {
        autoGrow(this);
        if (currentConversationId && connection && connection.state === signalR.HubConnectionState.Connected) {
            connection.invoke("SendTyping", currentConversationId, true).catch(function () { });
            clearTimeout(window.chatTypingDebounce);
            window.chatTypingDebounce = setTimeout(function () {
                connection.invoke("SendTyping", currentConversationId, false).catch(function () { });
            }, 1500);
        }
    });

    $("#sendButton").on("click", sendCurrentMessage);

    // Load older messages when scrolled to the top
    $("#chatBody").on("scroll", function () {
        if (this.scrollTop < 60 && !loadingMessages) {
            loadMessages(loadedCount, false);
        }
    });

    // New conversation modal - filter recipient list
    $("#newConversationModal").on("show.bs.modal", function () {
        $("#userSearch").val("");
        $("#newConversationRecipient option").show();
    });
    $("#userSearch").on("input", function () {
        const q = $(this).val().toLowerCase();
        let visible = 0;
        $("#newConversationRecipient option").each(function () {
            const show = !q || $(this).text().toLowerCase().indexOf(q) !== -1;
            $(this).toggle(show);
            if (show) visible++;
        });
        $("#noUsersMatch").toggle(visible === 0);
    });

    $("#startConversationBtn").on("click", startNewConversation);
    $("#newConversationMessage").on("keydown", function (e) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            startNewConversation();
        }
    });
});

function startNewConversation() {
    const recipientId = $("#newConversationRecipient").val();
    const message = $("#newConversationMessage").val().trim();
    if (!recipientId) { alert("Please select a recipient."); return; }
    if (!message) { alert("Please enter a message."); return; }

    const btn = $("#startConversationBtn");
    btn.prop("disabled", true).html('<span class="spinner-border spinner-border-sm"></span> Sending...');

    $.ajax({
        url: "/Chat/StartConversation",
        type: "POST",
        contentType: "application/json",
        headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() },
        data: JSON.stringify({ recipientId: recipientId, initialMessage: message }),
        success: function (result) {
            $("#newConversationModal").modal("hide");
            $("#newConversationMessage").val("");
            btn.prop("disabled", false).html('<i class="fa-solid fa-paper-plane me-1"></i> Send');
            if (result.conversationId) {
                window.location.href = "/Chat/Index?conversationId=" + result.conversationId;
            } else {
                location.reload();
            }
        },
        error: function (xhr) {
            alert("Failed to start conversation: " + (xhr.responseText || "Unknown error"));
            btn.prop("disabled", false).html('<i class="fa-solid fa-paper-plane me-1"></i> Send');
        }
    });
}

// Incoming realtime message handler (used by the hub)
window.handleNewMessage = function (conversationId) {
    if (conversationId === currentConversationId) {
        loadMessages(0, true);
    }
};

// Public API (kept for compatibility with site.js init)
window.sendMessage = function (conversationId, content) {
    currentConversationId = conversationId;
    sendCurrentMessage();
};
window.setTyping = function (conversationId, isTyping) {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke("SendTyping", conversationId, isTyping);
    }
};
window.markMessageAsRead = markMessageAsRead;
