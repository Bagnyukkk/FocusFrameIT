/* src/chat.js */

document.addEventListener("DOMContentLoaded", () => {
  const currentUser = JSON.parse(localStorage.getItem("currentUser"));
  if (!currentUser) return;

  // Вставляємо HTML (з новим елементом chatBadge)
  const chatHTML = `
    <button id="chatToggle" class="chat-toggle-btn">
      <svg class="chat-toggle-icon" viewBox="0 0 24 24"><path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"></path></svg>
      <span id="chatBadge" class="chat-badge" style="display: none;">1</span>
    </button>

    <div id="chatWidget" class="chat-widget">
      <div class="chat-header">
        <div class="chat-title">
          <h4>Support & Mentors</h4>
          <span>● Online</span>
        </div>
        <button id="closeChat" class="close-chat">&times;</button>
      </div>
      <div id="chatBody" class="chat-body"></div>
      <form id="chatForm" class="chat-footer">
        <input type="text" id="chatInput" class="chat-input" placeholder="Type a message..." autocomplete="off">
        <button type="submit" class="chat-send-btn">➤</button>
      </form>
    </div>
  `;

  document.body.insertAdjacentHTML("beforeend", chatHTML);

  const toggleBtn = document.getElementById("chatToggle");
  const badge = document.getElementById("chatBadge"); // Наш бейдж
  const widget = document.getElementById("chatWidget");
  const closeBtn = document.getElementById("closeChat");
  const chatBody = document.getElementById("chatBody");
  const chatForm = document.getElementById("chatForm");
  const chatInput = document.getElementById("chatInput");

  const chatKey = `chat_history_${currentUser.email}`;

  // --- ФУНКЦІЇ ---

  function getMessages() {
    return JSON.parse(localStorage.getItem(chatKey) || "[]");
  }

  function renderMessages() {
    const messages = getMessages();
    chatBody.innerHTML = messages.map(msg => `
      <div class="message ${msg.sender === 'student' ? 'msg-student' : 'msg-admin'}">
        ${msg.text}
        <span class="msg-time">${msg.time}</span>
      </div>
    `).join("");
    chatBody.scrollTop = chatBody.scrollHeight;
  }

  // Перевірка нових повідомлень (Для бейджика)
  function checkUnread() {
    const messages = getMessages();
    if (messages.length === 0) return;

    const lastMsg = messages[messages.length - 1];
    const isChatOpen = window.getComputedStyle(widget).display !== "none";

    // Якщо останнє повідомлення від Адміна і чат закритий -> показуємо "1"
    if (lastMsg.sender === "admin" && !isChatOpen) {
      badge.style.display = "flex";
    } else {
      badge.style.display = "none";
    }
  }

  function sendMessage(text, sender = "student") {
    const messages = getMessages();
    messages.push({
      sender: sender,
      text: text,
      time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    });
    localStorage.setItem(chatKey, JSON.stringify(messages));
    renderMessages();
  }

  // --- ОБРОБНИКИ ---

  toggleBtn.addEventListener("click", () => {
    const isHidden = window.getComputedStyle(widget).display === "none";
    if (isHidden) {
      widget.style.display = "flex";
      badge.style.display = "none"; // Приховуємо бейдж при відкритті
      renderMessages();
    } else {
      widget.style.display = "none";
    }
  });

  closeBtn.addEventListener("click", () => {
    widget.style.display = "none";
  });

  chatForm.addEventListener("submit", (e) => {
    e.preventDefault();
    const text = chatInput.value.trim();
    if (!text) return;
    sendMessage(text, "student");
    chatInput.value = "";
  });

  // СЛУХАЧ ЗМІН У LOCALSTORAGE (Магія!)
  // Якщо Адмін відпише в іншій вкладці, тут одразу з'явиться бейдж
  window.addEventListener("storage", (e) => {
    if (e.key === chatKey) {
      renderMessages();
      checkUnread();
    }
  });

  // INIT
  checkUnread(); // Перевіряємо при завантаженні сторінки
});