/* src/dashboard.js */
import { course } from "./lessons.js";

document.addEventListener("DOMContentLoaded", () => {
  // 1. ПЕРЕВІРКА АВТОРИЗАЦІЇ
  const currentUser = JSON.parse(localStorage.getItem("currentUser"));
  if (!currentUser) {
    window.location.href = "sign_in.html";
    return;
  }

  // 2. ВІДОБРАЖЕННЯ ІМЕНІ
  const nameElement = document.querySelector(".account-hero-title .highlight");
  if (nameElement) {
    nameElement.textContent = currentUser.name || "Student";
  }

  // 3. ОТРИМАННЯ ДАНИХ
  const enrolledIds = JSON.parse(localStorage.getItem("enrolledCourses") || "[]");
  const completedLessonIds = JSON.parse(localStorage.getItem("completedLessons") || "[]");
  const myCourses = course.filter(c => enrolledIds.includes(c.moduleId));

  // 4. СОРТУВАННЯ КУРСІВ
  const progressContainer = document.getElementById("progress");
  const completedContainer = document.getElementById("completed");
  const wishlistContainer = document.getElementById("wishlist");

  if (progressContainer) progressContainer.innerHTML = "";
  if (completedContainer) completedContainer.innerHTML = "";
  if (wishlistContainer) {
    wishlistContainer.innerHTML = `
      <div class="qa-card visible" style="opacity:0.6; justify-content:center;">
        <p>No items in wishlist yet.</p>
      </div>`;
  }

  let hasProgress = false;
  let hasCompleted = false;

  myCourses.forEach(mod => {
    // 4.1. Рахуємо прогрес уроків
    const totalLessons = mod.lessons.length;
    const finishedLessons = mod.lessons.filter(l => completedLessonIds.includes(l.id)).length;
    const lessonsPercent = totalLessons === 0 ? 0 : Math.round((finishedLessons / totalLessons) * 100);

    // 4.2. ПЕРЕВІРКА КВІЗУ (Логіка блокування сертифіката)
    // Для демо перевіряємо, чи складено тест хоча б до одного уроку цього курсу (наприклад, ID=1)
    // У реальному проекті тут була б перевірка фінального тесту.
    let quizPassed = false;
    
    // Перевіряємо, чи є збережений результат квізу для будь-якого уроку цього курсу
    const hasQuizScore = mod.lessons.some(l => {
      const score = localStorage.getItem(`quiz_${l.id}`);
      return score && Number(score) >= 70; // Прохідний бал 70%
    });

    if (lessonsPercent === 100 && hasQuizScore) {
      quizPassed = true;
    }

    // --- Визначаємо стан кнопки ---
    const level = "Beginner";
    const image = mod.lessons[0]?.poster || "./img/hero/girl1_mob.webp";
    
    let btnText = "Start";
    let btnClass = "qa-btn"; // звичайний стиль
    let clickAction = "";

    // Знаходимо куди йти (перший непройдений урок)
    const firstUnfinished = mod.lessons.find(l => !completedLessonIds.includes(l.id));
    const targetId = firstUnfinished ? firstUnfinished.id : mod.lessons[0].id;

    if (lessonsPercent === 100) {
      if (quizPassed) {
        // Усе пройдено + тест складено
        btnText = "Get Certificate";
        btnClass = "qa-btn cert-btn"; // додамо золотий стиль
        // Клік обробляється окремим слухачем нижче
      } else {
        // Уроки пройдено, але тест НЕ складено
        btnText = "Take Final Quiz";
        
        // ВИПРАВЛЕННЯ: Знаходимо ID останнього уроку саме цього курсу
        const lastLessonId = mod.lessons[mod.lessons.length - 1].id;
        
        clickAction = `onclick="window.location.href='lesson.html?id=${lastLessonId}'"`;
      }
    } else if (lessonsPercent > 0) {
      btnText = "Continue";
      clickAction = `onclick="window.location.href='lesson.html?id=${targetId}'"`;
    } else {
      clickAction = `onclick="window.location.href='lesson.html?id=${targetId}'"`;
    }

    // HTML картки
    const cardHTML = `
      <div class="qa-card visible">
        <div class="qa-card-left">
          <div class="qa-card-img">
            <img src="${image}" alt="${mod.moduleTitle}">
          </div>
          <div class="qa-info">
            <h3 class="qa-course-title">${mod.moduleTitle}</h3>
            <div class="qa-progress-line">
              <div style="width: ${lessonsPercent}%"></div>
            </div>
            <p class="qa-meta">
              Lessons: ${finishedLessons}/${totalLessons} 
              ${lessonsPercent === 100 ? '✔' : ''} 
              ${lessonsPercent === 100 && !quizPassed ? '(Quiz needed)' : ''}
            </p>
          </div>
        </div>
        <button class="${btnClass}" ${clickAction}>${btnText}</button>
      </div>
    `;

    // Розподіл по табах (тільки повністю завершений курс йде в Completed)
    if (quizPassed) {
      if (completedContainer) completedContainer.innerHTML += cardHTML;
      hasCompleted = true;
    } else {
      if (progressContainer) progressContainer.innerHTML += cardHTML;
      hasProgress = true;
    }

    // --- FORUM MOCK DATA ---
  const forumContainer = document.getElementById("forumList");
  if (forumContainer) {
    // Генеруємо теми, додаючи тему від поточного юзера
    const topics = [
      {
        title: "How to verify certificate?",
        author: currentUser.name || "Student",
        avatar: localStorage.getItem("profilePhoto") || "./img/hero/girl1_mob.webp",
        time: "Just now"
      },
      {
        title: "Help with composition rule of thirds",
        author: "Alex Lebedev",
        avatar: "./img/mentors/alex_lebedev.webp",
        time: "14 min ago"
      },
      {
        title: "Best lens for street photography?",
        author: "Mia Dubrovska",
        avatar: "./img/mentors/mia_dubrovska.webp",
        time: "2 hours ago"
      },
      {
        title: "Sharing my first landscape results!",
        author: "Eliot Kovalev",
        avatar: "./img/mentors/eliot_kovalev.webp",
        time: "5 hours ago"
      }
    ];

    forumContainer.innerHTML = topics.map(t => `
      <div class="forum-item">
        <img src="${t.avatar}" class="forum-avatar" alt="${t.author}">
        <div class="forum-info">
          <h4>${t.title}</h4>
          <p>By ${t.author}</p>
        </div>
        <span class="forum-time">${t.time}</span>
      </div>
    `).join("");
  }
  });
// --- DYNAMIC PORTFOLIO (REAL USER PHOTOS) ---
  const portfolioGrid = document.querySelector(".portfolio-grid");
  
  if (portfolioGrid) {
    // 1. Отримуємо дані з LocalStorage
    const portfolios = JSON.parse(localStorage.getItem("userPortfolios") || "[]");
    
    // 2. Збираємо всі фото в одну купу
    let allPhotos = [];
    portfolios.forEach(album => {
      // Додаємо фото з кожного альбому в загальний масив
      allPhotos = [...allPhotos, ...album.photos];
    });

    // 3. Сортуємо: найновіші (за ID/часом) перші
    allPhotos.sort((a, b) => b.id - a.id);

    // 4. Якщо фотографії є — відображаємо їх
    if (allPhotos.length > 0) {
      portfolioGrid.innerHTML = ""; // Очищаємо статичні картинки

      // Беремо максимум 6 фото (бо у нас така сітка: 1 велике + 5 маленьких)
      const photosToShow = allPhotos.slice(0, 6);

      photosToShow.forEach((photo, index) => {
        const div = document.createElement("div");
        
        // Першому елементу даємо клас 'large', щоб він був великим зліва
        if (index === 0) {
          div.className = "portfolio-item large";
        } else {
          div.className = "portfolio-item";
        }

        div.innerHTML = `<img src="${photo.src}" alt="My Work" loading="lazy">`;
        
        // Можна додати клік для переходу в портфоліо
        div.onclick = () => window.location.href = "portfolio.html";
        
        portfolioGrid.appendChild(div);
      });
    } 
    // Якщо фото немає — залишаються статичні заглушки з HTML, 
    // або можна вивести повідомлення "Upload your first photo"
  }
  // Заглушки
  if (!hasProgress && progressContainer) progressContainer.innerHTML = `<div class="qa-card visible" style="justify-content:center; opacity:0.7;"><p>No active courses.</p></div>`;
  if (!hasCompleted && completedContainer) completedContainer.innerHTML = `<div class="qa-card visible" style="justify-content:center; opacity:0.7;"><p>Complete lessons & pass quiz to get certified.</p></div>`;

  // 5. ЗАГАЛЬНИЙ ПРОГРЕС (Hero Section)
  updateHeroProgress(myCourses, completedLessonIds);

  // 6. ЛОГІКА ТАБІВ
  initTabs();

  // 7. ЛОГІКА СЕРТИФІКАТА (МОДАЛКА + ЗАВАНТАЖЕННЯ)
  initCertificateLogic(currentUser);

});

// === ДОДАТКОВІ ФУНКЦІЇ ===

function updateHeroProgress(myCourses, completedLessonIds) {
  let allMyLessons = [];
  myCourses.forEach(mod => allMyLessons = [...allMyLessons, ...mod.lessons]);
  
  const totalCompletedAll = allMyLessons.filter(l => completedLessonIds.includes(l.id)).length;
  const overallPercent = allMyLessons.length === 0 ? 0 : Math.round((totalCompletedAll / allMyLessons.length) * 100);

  const heroCircle = document.querySelector(".account-circle span");
  const heroLine = document.querySelector(".account-progress-line div");
  if (heroCircle) heroCircle.textContent = `${overallPercent}%`;
  if (heroLine) heroLine.style.width = `${overallPercent}%`;
  
  // Кнопка Continue Last Lesson
  const continueBtn = document.getElementById("continueLessonBtn");
  if (continueBtn && allMyLessons.length > 0) {
     const next = allMyLessons.find(l => !completedLessonIds.includes(l.id));
     if (next) {
       continueBtn.onclick = () => window.location.href = `lesson.html?id=${next.id}`;
       continueBtn.textContent = "Continue learning";
     } else {
       continueBtn.textContent = "Review courses";
       continueBtn.onclick = () => window.location.href = `lesson.html?id=${allMyLessons[0].id}`;
     }
  }
}

function initTabs() {
  const tabs = document.querySelectorAll(".qa-tab");
  const contents = document.querySelectorAll(".tab-content");
  tabs.forEach(tab => {
    tab.addEventListener("click", () => {
      tabs.forEach(t => t.classList.remove("active"));
      tab.classList.add("active");
      const target = tab.dataset.tab;
      contents.forEach(c => {
        c.classList.remove("active");
        if (c.id === target) c.classList.add("active");
      });
    });
  });
}

function initCertificateLogic(currentUser) {
  document.body.addEventListener("click", (e) => {
    if (e.target.classList.contains("cert-btn")) {
      e.preventDefault();

      const card = e.target.closest(".qa-card");
      const courseTitle = card.querySelector(".qa-course-title").textContent;
      const userName = currentUser.name || "Student";
      const date = new Date().toLocaleDateString();

      // HTML СЕРТИФІКАТА (Стилізований під друк)
      const modalHTML = `
        <div id="certModal" style="position:fixed; inset:0; background:rgba(0,0,0,0.9); z-index:9999; display:flex; flex-direction:column; justify-content:center; align-items:center; backdrop-filter:blur(5px);">
          
          <div id="certificate-node" style="background:#fff; color:#111; width:800px; height:600px; padding:40px; text-align:center; border:10px double #d1a476; position:relative; display:flex; flex-direction:column; justify-content:center; align-items:center; box-shadow: 0 0 50px rgba(0,0,0,0.5);">
            
            <div style="position:absolute; top:20px; left:20px; width:40px; height:40px; border-top:4px solid #d1a476; border-left:4px solid #d1a476;"></div>
            <div style="position:absolute; bottom:20px; right:20px; width:40px; height:40px; border-bottom:4px solid #d1a476; border-right:4px solid #d1a476;"></div>

            <div style="margin-bottom:20px;">
               <svg width="60" height="60" style="fill:#d1a476;"><use href="/img/icons.svg#icon-logo"></use></svg>
            </div>
            
            <h2 style="font-size:48px; font-weight:bold; margin:0; letter-spacing:4px; font-family:'Times New Roman', serif;">CERTIFICATE</h2>
            <p style="font-size:18px; margin-top:10px; text-transform:uppercase; letter-spacing:3px; color:#555;">of completion</p>
            
            <div style="margin:40px 0; width:100%;">
              <p style="font-size:20px; color:#444; font-style:italic;">This certifies that</p>
              <h1 style="font-size:52px; color:#d1a476; margin:15px 0; font-family:'Cursive', serif; border-bottom:1px solid #ccc; display:inline-block; padding:0 40px;">${userName}</h1>
              <p style="font-size:20px; color:#444; margin-top:20px; font-style:italic;">has successfully completed the course</p>
              <h3 style="font-size:32px; font-weight:bold; margin-top:10px; color:#000;">${courseTitle}</h3>
            </div>

            <div style="display:flex; justify-content:space-between; width:70%; margin-top:40px;">
              <div style="text-align:center;">
                <p style="border-top:1px solid #333; width:150px; margin:0 auto; padding-top:5px; font-weight:bold;">${date}</p>
                <p style="font-size:12px; color:#777;">Date</p>
              </div>
              <div style="text-align:center;">
                <p style="border-top:1px solid #333; width:150px; margin:0 auto; padding-top:5px; font-weight:bold;">Focus.Frame</p>
                <p style="font-size:12px; color:#777;">Signature</p>
              </div>
            </div>
          </div>

          <div style="margin-top:20px; display:flex; gap:15px;">
            <button id="downloadCertBtn" style="background:#d1a476; color:#000; border:none; padding:12px 30px; border-radius:50px; cursor:pointer; font-weight:bold; font-size:16px;">Download Image ⬇</button>
            <button id="closeCert" style="background:transparent; color:#fff; border:1px solid #fff; padding:12px 30px; border-radius:50px; cursor:pointer; font-weight:bold; font-size:16px;">Close</button>
          </div>
        </div>
      `;

      document.body.insertAdjacentHTML("beforeend", modalHTML);

      // Закриття
      document.getElementById("closeCert").onclick = () => document.getElementById("certModal").remove();

      // Завантаження (HTML2Canvas)
      document.getElementById("downloadCertBtn").onclick = () => {
        const element = document.getElementById("certificate-node");
        const btn = document.getElementById("downloadCertBtn");
        
        btn.textContent = "Generating...";
        
        // Використовуємо бібліотеку html2canvas
        if (typeof html2canvas !== 'undefined') {
            html2canvas(element, { scale: 2 }).then(canvas => {
                const link = document.createElement('a');
                link.download = `Certificate-${userName}.png`;
                link.href = canvas.toDataURL("image/png");
                link.click();
                btn.textContent = "Download Image ⬇";
            });
        } else {
            alert("Error: Library not loaded. Please check internet connection.");
            btn.textContent = "Download Image ⬇";
        }
      };
    }
  });
}

