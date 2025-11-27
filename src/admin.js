/* src/admin.js */
import { course } from "./lessons.js";

document.addEventListener("DOMContentLoaded", () => {
  console.log("Teacher Panel Initialized");

  const viewContainer = document.getElementById("adminView");
  const pageTitle = document.getElementById("pageTitle");
  const navLinks = document.querySelectorAll(".admin-nav a");

  // --- ROUTING ---
  navLinks.forEach(link => {
    link.addEventListener("click", (e) => {
      e.preventDefault();
      
      navLinks.forEach(l => l.classList.remove("active"));
      link.classList.add("active");

      const tab = link.getAttribute("data-tab");
      loadTab(tab);
    });
  });

  // Load default
  loadTab("dashboard");
  
  // Запуск перевірки нових повідомлень
  checkMail();
  
  // Слухаємо зміни в реальному часі (якщо студент написав, поки ми тут)
  window.addEventListener("storage", () => {
    checkMail();
    // Якщо відкритий список чатів - оновити його
    if (document.getElementById("chatUserList")) {
        loadChatUsers();
    }
  });

  function loadTab(tabName) {
    viewContainer.innerHTML = ""; 

    switch(tabName) {
      case "dashboard": pageTitle.textContent = "Dashboard Overview"; renderDashboard(); break;
      case "students": pageTitle.textContent = "Students Management"; renderStudents(); break;
      case "courses": pageTitle.textContent = "Courses Management"; renderCoursesPlaceholder(); break;
      case "homework": pageTitle.textContent = "Homework Review"; renderHomework(); break;
      case "certificates": pageTitle.textContent = "Issued Certificates"; renderCertificates(); break; 
      case "messages": pageTitle.textContent = "Direct Messages"; renderChatInterface(viewContainer); break;
      case "forum": pageTitle.textContent = "Forum Moderation"; renderForumModeration(); break; 
      case "profile": pageTitle.textContent = "Teacher Profile"; renderProfilePlaceholder(); break;
      default: renderDashboard();
    }
  }

  // =========================================
  //  BADGE LOGIC (Перевірка пошти)
  // =========================================
  function checkMail() {
    const badge = document.getElementById("msgBadge");
    if (!badge) return;

    let hasNew = false;

    // Перевіряємо всі чати в localStorage
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      if (key.startsWith("chat_history_")) {
        const msgs = JSON.parse(localStorage.getItem(key) || "[]");
        if (msgs.length > 0) {
          const lastMsg = msgs[msgs.length - 1];
          // Якщо останнє повідомлення від студента - значить адмін ще не відповів -> НОВЕ
          if (lastMsg.sender === "student") {
            hasNew = true;
            break; 
          }
        }
      }
    }

    // Показуємо або ховаємо бейдж
    badge.style.display = hasNew ? "inline-block" : "none";
  }

  // ... (Далі йдуть всі твої функції рендеру: renderDashboard, renderStudents і т.д.)
  // Щоб код не був гігантським, я залишаю попередні функції без змін.
  // Тобі потрібно лише вставити функцію checkMail() і її виклик на початку, 
  // або скопіювати старі функції нижче.
  
  // 👇 ВСТАВ СЮДИ РЕШТУ ФУНКЦІЙ З ПОПЕРЕДНЬОГО ФАЙЛУ (renderDashboard, renderStudents, renderHomework, renderChatInterface...)
  // Якщо тобі зручніше, я можу надіслати повний файл одним шматком знову.
  
  function renderDashboard() {
    const realUser = localStorage.getItem("profileData");
    const studentCount = realUser ? "1,501" : "1,500";
    const forumTopics = JSON.parse(localStorage.getItem("forumTopics") || "[]");
    
    // Підрахунок домашок
    let pendingCount = 0;
    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key.startsWith("hw_") && !key.startsWith("hw_comment_") && !key.startsWith("hw_status_")) {
            const lessonId = key.replace("hw_", "");
            const status = localStorage.getItem(`hw_status_${lessonId}`) || "Pending";
            if (status === "Pending") pendingCount++;
        }
    }

    viewContainer.innerHTML = `
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-label">Total Students</div>
          <div class="stat-number">${studentCount}</div>
          <div class="stat-trend">↑ 12% this month</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">Active Courses</div>
          <div class="stat-number">${course.length}</div>
          <div class="stat-trend">3 Published</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">Forum Activity</div>
          <div class="stat-number">${forumTopics.length}</div>
          <div class="stat-trend">New posts</div>
        </div>
        <div class="stat-card highlight">
          <div class="stat-label">Pending Homework</div>
          <div class="stat-number" style="color:#ff4d4d">${pendingCount}</div>
          <div class="stat-trend">Needs review</div>
        </div>
      </div>
      <div class="table-container">
        <div class="table-header"><h3>Recent Activity</h3></div>
        <table class="admin-table">
          <thead><tr><th>User</th><th>Action</th><th>Time</th></tr></thead>
          <tbody>
            <tr><td>Yana</td><td>Joined "Portrait Photography"</td><td>2 hours ago</td></tr>
            <tr><td>Alex Smith</td><td>Submitted Homework</td><td>5 hours ago</td></tr>
          </tbody>
        </table>
      </div>`;
  }

  function renderStudents() {
    const profile = JSON.parse(localStorage.getItem("profileData"));
    const completed = JSON.parse(localStorage.getItem("completedLessons") || "[]");
    const enrolled = JSON.parse(localStorage.getItem("enrolledCourses") || "[]");
    const progressPercent = Math.min(100, Math.round((completed.length / 12) * 100));

    viewContainer.innerHTML = `
      <div class="table-container">
        <div class="table-header">
          <h3>All Students</h3>
          <button class="action-btn" style="background:#d1a476; color:#000;">+ Add</button>
        </div>
        <table class="admin-table">
          <thead><tr><th>Name</th><th>Email</th><th>Progress</th><th>Status</th><th>Actions</th></tr></thead>
          <tbody id="studentsList"></tbody>
        </table>
      </div>`;
    
    const tbody = document.getElementById("studentsList");
    if (profile) {
      tbody.innerHTML += `
        <tr>
          <td style="font-weight:bold; color:#fff;">${profile.name}</td>
          <td>${profile.email}</td>
          <td>
            <div class="progress-track"><div class="progress-fill" style="width:${progressPercent}%"></div></div> ${progressPercent}%
          </td>
          <td><span class="status-badge status-active">Active</span></td>
          <td><button class="action-btn btn-delete"><i class='bx bxs-trash'></i></button></td>
        </tr>`;
        tbody.querySelector(".btn-delete").onclick = () => {
            if(confirm("Reset student data?")) { localStorage.clear(); location.reload(); }
        };
    }
  }

  function renderHomework() {
    viewContainer.innerHTML = `
      <div class="table-container">
        <div class="table-header"><h3>Homework Submissions</h3></div>
        <table class="admin-table">
          <thead><tr><th>Student</th><th>Lesson ID</th><th>Files</th><th>Status</th><th>Actions</th></tr></thead>
          <tbody id="hwListBody"></tbody>
        </table>
      </div>`;

    const tbody = document.getElementById("hwListBody");
    const realUser = JSON.parse(localStorage.getItem("profileData"));
    let hasHomework = false;

    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      if (key.startsWith("hw_") && !key.startsWith("hw_comment_") && !key.startsWith("hw_status_")) {
        const lessonId = key.replace("hw_", "");
        const files = JSON.parse(localStorage.getItem(key) || "[]");
        const statusKey = `hw_status_${lessonId}`;
        const currentStatus = localStorage.getItem(statusKey) || "Pending"; 

        if (files.length > 0) {
          hasHomework = true;
          const fileLinks = files.map(f => `<div style="font-size:12px; color:#d1a476;">📎 ${f.name}</div>`).join("");
          let statusClass = currentStatus === "Approved" ? "status-active" : (currentStatus === "Rejected" ? "status-rejected" : "status-pending");

          const tr = document.createElement("tr");
          tr.innerHTML = `
            <td style="font-weight:bold; color:#fff;">${realUser ? realUser.name : "Unknown"}</td>
            <td>#${lessonId}</td>
            <td>${fileLinks}</td>
            <td><span class="status-badge ${statusClass}">${currentStatus}</span></td>
            <td>
              <button class="action-btn btn-approve" title="Approve">✔</button>
              <button class="action-btn btn-reject" title="Reject">✖</button>
            </td>`;
          
          tr.querySelector(".btn-approve").onclick = () => { localStorage.setItem(statusKey, "Approved"); renderHomework(); };
          tr.querySelector(".btn-reject").onclick = () => { localStorage.setItem(statusKey, "Rejected"); renderHomework(); };
          tbody.appendChild(tr);
        }
      }
    }
    if (!hasHomework) tbody.innerHTML = `<tr><td colspan="5" style="text-align:center; padding:20px; color:#666;">No submissions</td></tr>`;
  }

  function renderCertificates() {
    viewContainer.innerHTML = `
      <div class="table-container">
        <div class="table-header"><h3>Issued Certificates</h3></div>
        <table class="admin-table">
          <thead><tr><th>Student</th><th>Course</th><th>Date Issued</th><th>ID</th><th>Status</th></tr></thead>
          <tbody id="certListBody"></tbody>
        </table>
      </div>
    `;
    const tbody = document.getElementById("certListBody");
    const realUser = JSON.parse(localStorage.getItem("profileData"));
    const completedLessons = JSON.parse(localStorage.getItem("completedLessons") || "[]");
    let hasCerts = false;
    
    course.forEach(c => {
        const total = c.lessons.length;
        const done = c.lessons.filter(l => completedLessons.includes(l.id)).length;
        if (done === total && total > 0) {
            hasCerts = true;
            tbody.innerHTML += `
                <tr>
                    <td style="font-weight:bold; color:#fff;">${realUser ? realUser.name : "User"}</td>
                    <td>${c.moduleTitle}</td>
                    <td>${new Date().toLocaleDateString()}</td>
                    <td style="font-family:monospace; color:#777;">CRT-${Date.now().toString().slice(-6)}</td>
                    <td><span class="status-badge status-active">Issued</span></td>
                </tr>
            `;
        }
    });
    if (!hasCerts) tbody.innerHTML = `<tr><td colspan="5" style="text-align:center; padding:20px; color:#666;">No certificates issued yet.</td></tr>`;
  }

  function renderForumModeration() {
    viewContainer.innerHTML = `
      <div class="table-container">
        <div class="table-header"><h3>Forum Posts</h3></div>
        <table class="admin-table">
          <thead><tr><th>Author</th><th>Topic</th><th>Category</th><th>Date</th><th>Action</th></tr></thead>
          <tbody id="forumModBody"></tbody>
        </table>
      </div>
    `;
    const tbody = document.getElementById("forumModBody");
    const topics = JSON.parse(localStorage.getItem("forumTopics") || "[]");
    if (topics.length === 0) {
        tbody.innerHTML = `<tr><td colspan="5" style="text-align:center; padding:20px; color:#666;">No posts found.</td></tr>`;
        return;
    }
    topics.forEach(t => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td style="font-weight:bold; color:#fff;">${t.author}</td>
            <td>${t.title}</td>
            <td><span class="status-badge" style="background:#333; color:#ccc;">${t.tag}</span></td>
            <td>${t.time}</td>
            <td><button class="action-btn btn-delete-post" style="border-color:#ff4d4d; color:#ff4d4d;">Delete</button></td>
        `;
        tr.querySelector(".btn-delete-post").onclick = () => {
            if(confirm("Delete this post permanently?")) {
                const newTopics = topics.filter(item => item.id !== t.id);
                localStorage.setItem("forumTopics", JSON.stringify(newTopics));
                renderForumModeration(); 
            }
        };
        tbody.appendChild(tr);
    });
  }

  function renderCoursesPlaceholder() {
    viewContainer.innerHTML = `<div style="padding:40px; text-align:center; border:1px dashed #444; border-radius:12px;"><h3>🚧 Course Editor</h3><p>This feature requires a backend server.</p></div>`;
  }
  function renderProfilePlaceholder() {
    viewContainer.innerHTML = `<div style="padding:40px; text-align:center; border:1px dashed #444; border-radius:12px;"><h3>Teacher Profile</h3><p>Edit name, bio and photo here.</p></div>`;
  }

  function renderChatInterface(container) {
    container.innerHTML = `
        <div class="admin-chat-layout">
        <div class="chat-users-list" id="chatUserList"></div>
        <div class="chat-window">
            <div class="chat-messages-area" id="adminMsgArea">
            <p style="color:#666; text-align:center; margin-top:200px;">Select a student to chat</p>
            </div>
            <div class="chat-input-area" id="adminInputArea" style="display:none;">
            <input type="text" id="adminMsgInput" placeholder="Type a reply...">
            <button id="adminSendBtn">Send</button>
            </div>
        </div>
        </div>
    `;
    loadChatUsers();
  }

  function loadChatUsers() {
    const list = document.getElementById("chatUserList");
    const chats = [];
    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key.startsWith("chat_history_")) {
        chats.push({ key, email: key.replace("chat_history_", "") });
        }
    }
    if(chats.length === 0) { list.innerHTML = "<p style='padding:15px; color:#666;'>No messages</p>"; return; }
    chats.forEach(chat => {
        const div = document.createElement("div");
        div.className = "chat-user-item";
        div.innerHTML = `<div class="user-avatar-small">${chat.email[0].toUpperCase()}</div><div class="user-name-preview"><div style="font-weight:bold; color:#fff;">${chat.email}</div><div style="font-size:11px; color:#777;">Student</div></div>`;
        div.onclick = () => {
            document.querySelectorAll(".chat-user-item").forEach(el => el.style.background = "transparent");
            div.style.background = "#222";
            openChat(chat.key);
        };
        list.appendChild(div);
    });
  }

  function openChat(key) {
    const msgArea = document.getElementById("adminMsgArea");
    const inputArea = document.getElementById("adminInputArea");
    const input = document.getElementById("adminMsgInput");
    const sendBtn = document.getElementById("adminSendBtn");
    inputArea.style.display = "flex";
    
    const render = () => {
        const messages = JSON.parse(localStorage.getItem(key) || "[]");
        msgArea.innerHTML = messages.map(m => `
        <div class="admin-msg ${m.sender === 'admin' ? 'msg-outgoing' : 'msg-incoming'}">
            ${m.text}
            <div style="font-size:10px; opacity:0.6; text-align:right; margin-top:3px;">${m.time}</div>
        </div>`).join("");
        msgArea.scrollTop = msgArea.scrollHeight;
    };
    render();

    const newBtn = sendBtn.cloneNode(true);
    sendBtn.parentNode.replaceChild(newBtn, sendBtn);
    newBtn.onclick = () => {
        if(!input.value.trim()) return;
        const messages = JSON.parse(localStorage.getItem(key) || "[]");
        messages.push({ sender: "admin", text: input.value.trim(), time: new Date().toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'}) });
        localStorage.setItem(key, JSON.stringify(messages));
        input.value = "";
        render();
        checkMail(); // Оновлюємо бейдж
    };
  }

});