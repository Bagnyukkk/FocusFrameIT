/* src/forum.js */

document.addEventListener("DOMContentLoaded", () => {
  console.log("Forum Script Loaded (Comments Version)");

  const currentUser = JSON.parse(localStorage.getItem("currentUser"));
  if (!currentUser) {
    window.location.href = "sign_in.html";
    return;
  }

  // --- DOM ELEMENTS ---
  const container = document.getElementById("topicsContainer");
  const createBtn = document.getElementById("newTopicBtn");
  const modal = document.getElementById("topicModal");
  const postBtn = document.getElementById("postTopicBtn");
  const closeButtons = document.querySelectorAll(".close-modal");
  
  const titleInput = document.getElementById("topicTitle");
  const tagInput = document.getElementById("topicTag");
  const contentInput = document.getElementById("topicContent");
  const searchInput = document.getElementById("searchForum");
  const feedTitle = document.getElementById("feedTitle");
  const filterLinks = document.querySelectorAll(".filter-list li");

  // --- 1. МОДАЛКА ---
  const openModal = () => { if (modal) modal.style.display = "flex"; };
  const closeModal = () => {
    if (modal) modal.style.display = "none";
    if(titleInput) titleInput.value = "";
    if(contentInput) contentInput.value = "";
    if(tagInput) tagInput.value = "General";
  };

  if (createBtn) createBtn.addEventListener("click", openModal);
  closeButtons.forEach(btn => btn.addEventListener("click", closeModal));
  window.addEventListener("click", (e) => { if (e.target === modal) closeModal(); });

  // --- 2. РОБОТА З ДАНИМИ ---
  const defaultTopics = [
    {
      id: 1,
      title: "Which lens is best for street photography?",
      author: "Alex Lebedev",
      avatar: "./img/mentors/alex_lebedev.webp",
      tag: "Gear",
      content: "I'm deciding between 35mm and 50mm. Any thoughts?",
      time: "2 hours ago",
      likes: 12,
      comments: 4,
      isLiked: false
    },
    {
      id: 2,
      title: "Critique needed: My first portrait session",
      author: "Mia Dubrovska",
      avatar: "./img/mentors/mia_dubrovska.webp",
      tag: "Critique",
      content: "Hi everyone! I tried the Rembrandt lighting setup. How did I do?",
      time: "5 hours ago",
      likes: 28,
      comments: 10,
      isLiked: true
    }
  ];

  function getTopics() {
    try {
      const stored = localStorage.getItem("forumTopics");
      if (!stored) return defaultTopics;
      const parsed = JSON.parse(stored);
      return Array.isArray(parsed) ? parsed : defaultTopics;
    } catch (e) {
      return defaultTopics;
    }
  }

  function saveTopics(topicsArray) {
    localStorage.setItem("forumTopics", JSON.stringify(topicsArray));
  }

  // Ініціалізація (якщо пусто)
  if (!localStorage.getItem("forumTopics")) {
    saveTopics(defaultTopics);
  }

  // --- 3. СТВОРЕННЯ ПОСТА ---
  if (postBtn) {
    postBtn.addEventListener("click", (e) => {
      e.preventDefault();
      const title = titleInput.value.trim();
      const content = contentInput.value.trim();
      const tag = tagInput.value;

      if (!title || !content || tag === "General") {
        alert("Please fill all fields correctly.");
        return;
      }

      const userAvatar = localStorage.getItem("profilePhoto") || "./img/hero/girl1_mob.webp";

      const newTopic = {
        id: Date.now(),
        title: title,
        content: content,
        tag: tag,
        author: currentUser.name,
        avatar: userAvatar,
        time: "Just now",
        likes: 0,
        comments: 0,
        isLiked: false
      };

      const currentTopics = getTopics();
      currentTopics.unshift(newTopic);
      saveTopics(currentTopics);
      renderTopics();
      closeModal();
    });
  }

  // --- 4. РЕНДЕРИНГ ---
  let activeFilter = "all"; 
  let activeTag = null;

  function renderTopics(searchTerm = "") {
    if (!container) return;
    container.innerHTML = "";

    let topics = getTopics();
    let filtered = [...topics].sort((a, b) => b.id - a.id);

    if (activeFilter === "my") filtered = filtered.filter(t => t.author === currentUser.name);
    else if (activeFilter === "popular") filtered.sort((a, b) => b.likes - a.likes);

    if (activeTag) filtered = filtered.filter(t => t.tag === activeTag);

    if (searchTerm) {
      filtered = filtered.filter(t => 
        t.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        t.content.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (filtered.length === 0) {
      container.innerHTML = `<div style="text-align:center; padding:40px; color:#666;">No topics found.</div>`;
      return;
    }

    filtered.forEach(t => {
      const card = document.createElement("div");
      card.className = "topic-card";
      const likeClass = t.isLiked ? "liked" : "";
      const isMyPost = t.author === currentUser.name;
      
      const deleteBtnHTML = isMyPost 
        ? `<button class="action-btn delete-btn" data-id="${t.id}" style="color:#ff6b6b; margin-left:auto;">🗑 Delete</button>` 
        : "";

      // Додаємо прихований блок коментарів (comment-box)
      card.innerHTML = `
        <div class="topic-header">
          <img src="${t.avatar}" class="topic-avatar" onerror="this.src='./img/hero/girl1_mob.webp'">
          <div>
            <span class="topic-author">${t.author}</span>
            <span class="topic-meta">• ${t.time}</span>
          </div>
          <span class="topic-tag tag-${t.tag}">${t.tag}</span>
        </div>
        
        <h3 class="topic-title">${t.title}</h3>
        <p class="topic-preview">${t.content}</p>
        
        <div class="topic-footer">
          <button class="action-btn like-btn ${likeClass}" data-id="${t.id}">
            <span>${t.isLiked ? '❤️' : '🤍'}</span> ${t.likes}
          </button>
          <button class="action-btn comment-btn" data-id="${t.id}">
            <span>💬</span> ${t.comments} Comments
          </button>
          ${deleteBtnHTML}
        </div>

        <div id="comments-${t.id}" class="comments-section" style="display:none; margin-top:15px; padding-top:15px; border-top:1px solid #333;">
            <div style="display:flex; gap:10px;">
                <input type="text" class="comment-input" placeholder="Write a comment..." style="flex:1; padding:10px; background:#111; border:1px solid #333; color:#fff; border-radius:8px;">
                <button class="send-comment-btn" data-id="${t.id}" style="background:#d1a476; color:#000; border:none; padding:0 20px; border-radius:8px; font-weight:bold; cursor:pointer;">Send</button>
            </div>
            ${t.comments > 0 ? `<p style="font-size:12px; color:#666; margin-top:10px;">(Previous comments hidden in demo)</p>` : ''}
        </div>
      `;
      container.appendChild(card);
    });
  }

  // --- 5. ОБРОБКА КЛІКІВ ---
  container.addEventListener("click", (e) => {
    const btn = e.target.closest("button");
    if (!btn) return;

    const id = Number(btn.dataset.id);
    let currentTopics = getTopics();

    // LIKE
    if (btn.classList.contains("like-btn")) {
      const topic = currentTopics.find(t => t.id === id);
      if (topic) {
        topic.isLiked = !topic.isLiked;
        topic.likes += topic.isLiked ? 1 : -1;
        saveTopics(currentTopics);
        renderTopics(searchInput ? searchInput.value : "");
      }
    }

    // DELETE
    if (btn.classList.contains("delete-btn")) {
      if (confirm("Delete this post?")) {
        currentTopics = currentTopics.filter(t => t.id !== id);
        saveTopics(currentTopics);
        renderTopics(searchInput ? searchInput.value : "");
      }
    }

    // TOGGLE COMMENTS (Відкрити/Закрити поле)
    if (btn.classList.contains("comment-btn")) {
      const commentBox = document.getElementById(`comments-${id}`);
      if (commentBox) {
        commentBox.style.display = commentBox.style.display === "none" ? "block" : "none";
      }
    }

    // SEND COMMENT
    if (btn.classList.contains("send-comment-btn")) {
      // Знаходимо інпут поруч з кнопкою
      const input = btn.previousElementSibling;
      if (input && input.value.trim() !== "") {
        const topic = currentTopics.find(t => t.id === id);
        if (topic) {
          topic.comments++; // Просто збільшуємо лічильник
          saveTopics(currentTopics);
          alert("Comment added!"); // Підтвердження
          renderTopics(searchInput ? searchInput.value : "");
        }
      } else {
        alert("Please write something!");
      }
    }
  });

  // --- 6. ФІЛЬТРИ ТА ПОШУК ---
  filterLinks.forEach(li => {
    li.addEventListener("click", () => {
      filterLinks.forEach(el => el.classList.remove("active"));
      li.classList.add("active");
      const filterType = li.getAttribute("data-filter");
      const tagType = li.getAttribute("data-tag");

      if (filterType) {
        activeFilter = filterType;
        activeTag = null; 
        feedTitle.textContent = li.textContent;
      } else if (tagType) {
        activeTag = tagType;
        activeFilter = "all"; 
        feedTitle.textContent = `${tagType} Topics`;
      }
      renderTopics(searchInput ? searchInput.value : "");
    });
  });

  if (searchInput) {
    searchInput.addEventListener("input", (e) => renderTopics(e.target.value));
  }

  // Initial Render
  renderTopics();
});