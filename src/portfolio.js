/* src/portfolio.js */

document.addEventListener("DOMContentLoaded", () => {
  const currentUser = JSON.parse(localStorage.getItem("currentUser"));
  if (!currentUser) {
    window.location.href = "sign_in.html";
    return;
  }

  // DOM елементи
  const albumsContainer = document.getElementById("albumsContainer");
  const createBtn = document.getElementById("createAlbumBtn");
  const createModal = document.getElementById("albumModal");
  const saveAlbumBtn = document.getElementById("saveAlbumBtn");
  const closeModals = document.querySelectorAll(".close-modal");
  
  const nameInput = document.getElementById("newAlbumName");
  const descInput = document.getElementById("newAlbumDesc");

  // Елементи модалки перегляду
  const viewModal = document.getElementById("viewAlbumModal");
  const viewTitle = document.getElementById("currentAlbumTitle");
  const photosContainer = document.getElementById("photosContainer");
  const photoInput = document.getElementById("photoInput");

  let currentAlbumId = null; // Щоб знати, який альбом відкритий

  // 1. ЗАВАНТАЖЕННЯ ДАНИХ
  // Структура: [{id: 123, title: "Nature", desc: "...", cover: "url", photos: [{id, src, fav}] }]
  let portfolios = JSON.parse(localStorage.getItem("userPortfolios") || "[]");

  function saveToStorage() {
    localStorage.setItem("userPortfolios", JSON.stringify(portfolios));
  }

  // 2. РЕНДЕР АЛЬБОМІВ
  function renderAlbums() {
    albumsContainer.innerHTML = "";

    if (portfolios.length === 0) {
      albumsContainer.innerHTML = `<p style="color:#666; grid-column: 1/-1; text-align:center;">You haven't created any albums yet. Start now!</p>`;
      return;
    }

    portfolios.forEach(album => {
      const card = document.createElement("div");
      card.className = "album-card";
      
      // Якщо немає фото, ставимо заглушку
      const coverImg = album.photos.length > 0 
        ? album.photos[0].src 
        : "./img/hero/girl1_mob.webp"; 

      card.innerHTML = `
        <div class="album-cover" onclick="openAlbum(${album.id})">
          <img src="${coverImg}" alt="${album.title}">
        </div>
        <div class="album-info" onclick="openAlbum(${album.id})">
          <h3>${album.title}</h3>
          <p>${album.photos.length} photos • ${album.desc || "No description"}</p>
        </div>
        <div class="album-actions">
          <button class="action-icon-btn delete-album" data-id="${album.id}" title="Delete Album">🗑</button>
        </div>
      `;
      albumsContainer.appendChild(card);
    });

    // Додаємо слухачі видалення
    document.querySelectorAll(".delete-album").forEach(btn => {
      btn.addEventListener("click", (e) => {
        e.stopPropagation();
        const id = Number(btn.dataset.id);
        if(confirm("Delete this album and all photos inside?")) {
          portfolios = portfolios.filter(a => a.id !== id);
          saveToStorage();
          renderAlbums();
        }
      });
    });
  }

  // 3. СТВОРЕННЯ АЛЬБОМУ
  createBtn.onclick = () => createModal.style.display = "flex";
  
  saveAlbumBtn.onclick = () => {
    const title = nameInput.value.trim();
    const desc = descInput.value.trim();

    if (!title) {
      alert("Please enter a title");
      return;
    }

    const newAlbum = {
      id: Date.now(),
      title: title,
      desc: desc,
      photos: []
    };

    portfolios.push(newAlbum);
    saveToStorage();
    renderAlbums();
    
    // Очищення і закриття
    nameInput.value = "";
    descInput.value = "";
    createModal.style.display = "none";
  };

  // 4. ВІДКРИТТЯ АЛЬБОМУ
  window.openAlbum = (id) => {
    currentAlbumId = id;
    const album = portfolios.find(a => a.id === id);
    if (!album) return;

    viewTitle.textContent = album.title;
    renderPhotos(album);
    viewModal.style.display = "flex";
  };

  function renderPhotos(album) {
    photosContainer.innerHTML = "";
    
    if (album.photos.length === 0) {
      photosContainer.innerHTML = `<p style="color:#555; grid-column: 1/-1; text-align:center; padding:20px;">No photos yet. Upload some!</p>`;
      return;
    }

    album.photos.forEach(photo => {
      const div = document.createElement("div");
      div.className = "photo-card";
      div.innerHTML = `
        <img src="${photo.src}" loading="lazy">
        <div class="photo-actions">
          <button class="action-icon-btn fav-btn ${photo.isFav ? 'active' : ''}" onclick="toggleFav(${photo.id})">★</button>
          <button class="action-icon-btn" onclick="deletePhoto(${photo.id})">🗑</button>
        </div>
      `;
      photosContainer.appendChild(div);
    });
  }

  // 5. ЗАВАНТАЖЕННЯ ФОТО (BASE64)
  photoInput.onchange = () => {
    if (!photoInput.files[0]) return;
    const file = photoInput.files[0];

    // Ліміт 2MB (localStorage має ліміт ~5-10MB всього)
        if (file.size > 500 * 1024) {
          alert("Стоп! Це фото завелике для демо-версії. Будь ласка, вибери фото менше 500КБ.");
          photoInput.value = ""; // Очистити інпут
          return; // Зупинити завантаження
        }

    const reader = new FileReader();
    reader.onload = (e) => {
      const base64String = e.target.result;
      
      const album = portfolios.find(a => a.id === currentAlbumId);
      if (album) {
        album.photos.push({
          id: Date.now(),
          src: base64String,
          isFav: false
        });
        saveToStorage();
        renderPhotos(album);
        renderAlbums(); // Оновити обкладинку альбому зовні
      }
    };
    reader.readAsDataURL(file);
    photoInput.value = ""; // Скинути інпут
  };

  // 6. ДІЇ З ФОТО
  window.deletePhoto = (photoId) => {
    const album = portfolios.find(a => a.id === currentAlbumId);
    if (album && confirm("Delete photo?")) {
      album.photos = album.photos.filter(p => p.id !== photoId);
      saveToStorage();
      renderPhotos(album);
      renderAlbums();
    }
  };

  window.toggleFav = (photoId) => {
    const album = portfolios.find(a => a.id === currentAlbumId);
    if (album) {
      const photo = album.photos.find(p => p.id === photoId);
      if (photo) {
        photo.isFav = !photo.isFav;
        saveToStorage();
        renderPhotos(album);
      }
    }
  };

  // Закриття модалок
  closeModals.forEach(btn => {
    btn.onclick = () => {
      createModal.style.display = "none";
      viewModal.style.display = "none";
    };
  });

  // Ініціалізація
  renderAlbums();
});