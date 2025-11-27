// let menuBtn = document.querySelector('.burger-btn');
// let menu = document.querySelector('.mobile-menu');
// let backdrop = document.querySelector('.mobile-menu-backdrop');
// let menuItem = document.querySelectorAll('.close-menu');
// let body = document.body;

// if (menuBtn) {
//   menuBtn.addEventListener('click', function () {
//     menu.classList.toggle('active');
//     backdrop.style.display = menu.classList.contains('active')
//       ? 'block'
//       : 'none';
//     body.classList.toggle('no-scroll', menu.classList.contains('active'));
//   });

//   menuItem.forEach(function (item) {
//     item.addEventListener('click', function () {
//       menu.classList.toggle('active');
//       backdrop.style.display = 'none';
//       body.classList.remove('no-scroll');
//     });
//   });

//   backdrop.addEventListener('click', function () {
//     menu.classList.remove('active');
//     backdrop.style.display = 'none';
//     body.classList.remove('no-scroll');
//   });
// }

// // scroll to top

// const scrollToTopBtn = document.getElementById('scrollToTopBtn');
// if(scrollToTopBtn != null)
//   {
//     scrollToTopBtn.addEventListener('click', function () {
//   window.scrollTo({
//     top: 0,
//     behavior: 'smooth',
//   });
// });

// window.addEventListener('scroll', function () {
//   if (window.scrollY > 300) {
//     scrollToTopBtn.classList.add('visible');
//   } else {
//     scrollToTopBtn.classList.remove('visible');
//   }
// });
// }


// // печатальщик текста

// function typeWriter(element, speed) {
//   if (element.dataset.typingStarted) return;
//   element.dataset.typingStarted = 'true';

//   const text = element.innerHTML;
//   let i = 0;
//   element.innerHTML = '';

//   function typing() {
//     if (i < text.length) {
//       element.innerHTML += text.charAt(i);
//       i++;
//       setTimeout(typing, speed);
//     }
//   }
//   typing();
// }

// function onEntry(entry) {
//   entry.forEach(change => {
//     if (change.isIntersecting) {
//       typeWriter(change.target, 30); // последнее знчение это скорость, чемь меньше - тем быстрее)))
//     }
//   });
// }

// let options = {
//   threshold: [0.5], // Какая часть элемента должна быть видна, чтоб все сработало :) сейчас - 50% элемента
// };
// let observer = new IntersectionObserver(onEntry, options);

// document.querySelectorAll('.typewriter').forEach(el => {
//   observer.observe(el);
// });

// // Card move
// document.addEventListener('DOMContentLoaded', function () {
//   const observer = new IntersectionObserver(entries => {
//     entries.forEach((entry, index) => {
//       if (entry.isIntersecting) {
//         setTimeout(() => {
//           entry.target.classList.add('visible');
//         }, index * 250); // Задержка для каждой следующей карточки
//       }
//     });
//   });

//   const cards = document.querySelectorAll('.move-card');
//   cards.forEach(card => observer.observe(card));
// });

// const notifBtn = document.getElementById("notifBtn");
// const notifCenter = document.getElementById("notifCenter");

// notifBtn.addEventListener("click", () => {
//   notifCenter.style.display =
//     notifCenter.style.display === "block" ? "none" : "block";
// });

// document.addEventListener("click", (e) => {
//   if (!notifCenter.contains(e.target) && !notifBtn.contains(e.target)) {
//     notifCenter.style.display = "none";
//   }
// });

// const progressBtn = document.getElementById("msgBtn");
// const progressCenter = document.getElementById("progressCenter");

// progressBtn.addEventListener("click", () => {
//   progressCenter.style.display =
//     progressCenter.style.display === "block" ? "none" : "block";
// });

// document.addEventListener("click", (e) => {
//   if (!progressCenter.contains(e.target) && !progressBtn.contains(e.target)) {
//     progressCenter.style.display = "none";
//   }
// });







// =========================
// Mobile menu
// =========================
let menuBtn = document.querySelector('.burger-btn');
let menu = document.querySelector('.mobile-menu');
let backdrop = document.querySelector('.mobile-menu-backdrop');
let menuItem = document.querySelectorAll('.close-menu');
let body = document.body;

if (menuBtn && menu && backdrop) {
  menuBtn.addEventListener('click', () => {
    menu.classList.toggle('active');
    backdrop.style.display = menu.classList.contains('active') ? 'block' : 'none';
    body.classList.toggle('no-scroll', menu.classList.contains('active'));
  });

  menuItem.forEach(item => {
    item.addEventListener('click', () => {
      menu.classList.remove('active');
      backdrop.style.display = 'none';
      body.classList.remove('no-scroll');
    });
  });

  backdrop.addEventListener('click', () => {
    menu.classList.remove('active');
    backdrop.style.display = 'none';
    body.classList.remove('no-scroll');
  });
}



// =========================
// Scroll to top
// =========================
const scrollToTopBtn = document.getElementById('scrollToTopBtn');

if (scrollToTopBtn) {
  scrollToTopBtn.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  });

  window.addEventListener('scroll', () => {
    scrollToTopBtn.classList.toggle('visible', window.scrollY > 300);
  });
}



// =========================
// Typewriter effect
// =========================
function typeWriter(element, speed) {
  if (element.dataset.typingStarted) return;
  element.dataset.typingStarted = 'true';

  const text = element.innerHTML;
  let i = 0;
  element.innerHTML = '';

  function typing() {
    if (i < text.length) {
      element.innerHTML += text.charAt(i);
      i++;
      setTimeout(typing, speed);
    }
  }

  typing();
}

let observer1 = new IntersectionObserver(entries => {
  entries.forEach(entry => {
    if (entry.isIntersecting) typeWriter(entry.target, 30);
  });
}, { threshold: 0.5 });

document.querySelectorAll('.typewriter').forEach(el => observer1.observe(el));



// =========================
// Card animation
// =========================
document.addEventListener('DOMContentLoaded', () => {
  let observer2 = new IntersectionObserver(entries => {
    entries.forEach((entry, index) => {
      if (entry.isIntersecting) {
        setTimeout(() => {
          entry.target.classList.add('visible');
        }, index * 250);
      }
    });
  });

  document.querySelectorAll('.move-card').forEach(card => observer2.observe(card));
});


// =========================
// Notifications + Progress centers FIXED
// =========================
const notifBtn = document.getElementById("notifBtn");
const notifCenter = document.getElementById("notifCenter");

const progressBtn = document.getElementById("msgBtn");
const progressCenter = document.getElementById("progressCenter");

// Відкрити/закрити сповіщення
if (notifBtn && notifCenter) {
  notifBtn.addEventListener("click", (e) => {
    e.stopPropagation();

    // Закрити progress перед відкриттям notif
    progressCenter.style.display = "none";

    notifCenter.style.display =
      notifCenter.style.display === "block" ? "none" : "block";
  });
}

// Відкрити/закрити progress
if (progressBtn && progressCenter) {
  progressBtn.addEventListener("click", (e) => {
    e.stopPropagation();

    // Закрити notif перед відкриттям progress
    notifCenter.style.display = "none";

    progressCenter.style.display =
      progressCenter.style.display === "block" ? "none" : "block";
  });
}

// Клік поза блоками закриває обидва
document.addEventListener("click", (e) => {
  if (!notifCenter.contains(e.target) && e.target !== notifBtn) {
    notifCenter.style.display = "none";
  }

  if (!progressCenter.contains(e.target) && e.target !== progressBtn) {
    progressCenter.style.display = "none";
  }
});

// =========================
// Quick Actions Tabs
// =========================
const tabs = document.querySelectorAll(".qa-tab");
const contents = document.querySelectorAll(".tab-content");

tabs.forEach(tab => {
  tab.addEventListener("click", () => {
    // Активна кнопка
    tabs.forEach(t => t.classList.remove("active"));
    tab.classList.add("active");

    // Показ контенту
    const target = tab.dataset.tab;
    contents.forEach(c => {
      c.classList.remove("active");
      if (c.id === target) c.classList.add("active");
    });

    // Плавна анімація карток
    document.querySelectorAll(`#${target} .qa-card`).forEach((card, i) => {
      setTimeout(() => card.classList.add("visible"), i * 120);
    });
  });
});

// Первинна анімація
document.querySelectorAll("#progress .qa-card").forEach((card, i) => {
  setTimeout(() => card.classList.add("visible"), i * 120);
});


document.addEventListener("DOMContentLoaded", function () {
  const btn = document.getElementById("continueLessonBtn");

  if (btn) {
    btn.addEventListener("click", () => {
      console.log("BTN CLICKED");
      window.location.href = "./lesson.html"; // Зміни на свій шлях
    });
  } else {
    console.log("BTN NOT FOUND");
  }
});

/* src/main.js - додаємо в кінець файлу */

document.addEventListener("DOMContentLoaded", () => {
  // --- ЛОГІКА LOGOUT ---
  const logoutBtn = document.getElementById("logoutBtn");

  if (logoutBtn) {
    logoutBtn.addEventListener("click", (e) => {
      e.preventDefault();
      
      // Підтвердження виходу
      if (confirm("Are you sure you want to log out?")) {
        // Видаляємо поточного юзера з пам'яті
        // (Але залишаємо enrolledCourses та profileData, щоб дані не стерлися повністю, 
        // просто юзер "вийшов" із сесії)
        localStorage.removeItem("currentUser");
        
        // Перекидаємо на головну або на вхід
        window.location.href = "index.html";
      }
    });
  }
/* src/main.js — Додаємо логіку динамічного хедера */

  // Перевіряємо кнопку в хедері (та, що зазвичай "Sign Up")
  // Ми шукаємо її по класу, але ігноруємо, якщо це кнопка виходу (logoutBtn)
  const headerAuthBtn = document.querySelector(".header .sign-up-button");
  const logoutBtnRef = document.getElementById("logoutBtn"); // Перевірка, чи ми не в кабінеті

  // Отримуємо поточного юзера
  const currentUser = JSON.parse(localStorage.getItem("currentUser"));

  // Якщо юзер є, і це НЕ сторінка кабінету (де вже є кнопка Log Out)
  if (currentUser && headerAuthBtn && !logoutBtnRef) {
    headerAuthBtn.textContent = "My Cabinet";
    headerAuthBtn.href = "./my_account.html";
    
    // Можна додати клас, щоб стилізувати інакше, якщо треба
    // headerAuthBtn.classList.add("logged-in-state");
  }
  
});


