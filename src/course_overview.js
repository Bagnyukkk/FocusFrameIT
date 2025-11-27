/* src/course_overview.js */
import { course } from "./lessons.js"; // Імпортуємо реальну структуру уроків

//
// 1. Визначаємо ID курсу з URL (наприклад course_overview.html?course=1)
//
const url = new URL(window.location.href);
const courseId = Number(url.searchParams.get("course")) || 1;

// Знаходимо модуль у нашій "базі" (lessons.js)
const selectedModule = course.find(m => m.moduleId === courseId);

//
// DOM ЕЛЕМЕНТИ
//
const heroEl = document.getElementById("courseHero");
const aboutEl = document.getElementById("courseAbout");
const learnEl = document.getElementById("courseLearn");
const lessonsEl = document.getElementById("courseLessons");
const mentorEl = document.getElementById("courseMentor");
const reviewsEl = document.getElementById("courseReviews");
const joinBtn = document.getElementById("joinCourseBtn");

//
// 2. ДАНІ ПРО КУРС (Опис, картинки)
// Це дані для відображення лендінгу.
//
const courseInfo = {
  1: {
    title: "Basics of Photography",
    subtitle: "Master the fundamentals and start creating beautiful images",
    image: "./img/hero/basic.png", // Використав існуючу картинку з твоїх файлів

    about: `
      This course will guide you through the essentials of photography — 
      from camera basics and exposure to composition and working with light. 
      Whether you're using a professional camera or a smartphone, you’ll learn 
      how to capture clean, sharp and emotionally engaging photographs.
    `,

    whatYouLearn: [
      "How cameras work and how they differ from human vision",
      "The Exposure Triangle: ISO, Shutter Speed, Aperture",
      "How to use natural and artificial light effectively",
      "Fundamental rules of composition and visual balance",
      "How to take sharp, clean and aesthetically pleasing photos",
      "How to shoot confidently even in difficult lighting conditions"
    ],

    mentor: {
      name: "Alex Lebedev", // Синхронізував ім'я з lessons.js
      photo: "img/mentors/alex_lebedev.webp",
      experience: "3.5 years of experience",
      description:
        "Alex is a professional photographer specializing in portrait and product photography. His teaching approach is simple, structured and beginner-friendly."
    },

    reviews: [
      {
        author: "Diana R.",
        text: "This course completely changed the way I take pictures. Super easy to understand!",
        rating: 5
      },
      {
        author: "Anton H.",
        text: "Finally learned how ISO works. The lessons are short, structured, and very practical.",
        rating: 5
      }
    ]
  },
  2: {
    title: "Landscape Photography",
    subtitle: "Capture the beauty of nature like a pro",
    image: "./img/hero/boy1_desk@1x.webp", // Використовуємо іншу картинку для різноманіття

    about: `
      Nature is the best artist, but capturing its beauty requires skill. 
      In this course, you will learn how to find the perfect light, use filters, 
      and compose breathtaking landscapes that tell a story.
    `,

    whatYouLearn: [
      "Planning shots: Golden Hour vs Blue Hour",
      "Using ND and Polarizing filters",
      "Composition rules for vast spaces",
      "Long exposure techniques for water and clouds",
      "Focus stacking for maximum sharpness"
    ],

    mentor: {
      name: "Mia Dubrovska",
      photo: "img/mentors/mia_dubrovska.webp",
      experience: "5 years of experience",
      description:
        "Mia is a travel photographer published in National Geographic. She loves hiking and capturing raw nature."
    },

    reviews: [
      {
        author: "Oleg K.",
        text: "The lesson on filters was a game changer for my sea photos!",
        rating: 5
      },
      {
        author: "Sarah L.",
        text: "Mia is so inspiring. I finally stopped shooting boring flat horizons.",
        rating: 5
      }
    ]
  },

  3: {
    title: "Portrait Photography",
    subtitle: "Master the art of capturing people and emotions",
    image: "./img/hero/girl2_desk@1x.webp", // Картинка для прикладу

    about: `
      Portrait photography is all about connection. In this course, 
      you will learn how to work with models, control studio and natural light, 
      and edit skin tones to perfection.
    `,

    whatYouLearn: [
      "Lighting patterns: Rembrandt, Butterfly, Split",
      "Posing guide for men and women",
      "Choosing the right lens (85mm vs 35mm)",
      "Psychology of working with a model",
      "High-end skin retouching basics"
    ],

    mentor: {
      name: "Eliot Kovalev",
      photo: "img/mentors/eliot_kovalev.webp",
      experience: "4.0 years of experience",
      description:
        "Eliot is a studio photographer known for his dramatic lighting and emotive portraits. He teaches you to see light differently."
    },

    reviews: [
      {
        author: "Kate M.",
        text: "I finally understood how to use flash properly!",
        rating: 5
      },
      {
        author: "John D.",
        text: "The posing tips were super helpful instantly.",
        rating: 5
      }
    ]
  }
};

// Отримуємо дані для поточного ID, або беремо дефолтні (ID=1)
const data = courseInfo[courseId] || courseInfo[1];

//
// 3. РЕНДЕРИНГ (HERO, ABOUT, ETC.)
//

function renderHero() {
  if(!heroEl) return;
  heroEl.innerHTML = `
    <div class="course-hero-inner">
      <div class="hero-text">
        <h1>${data.title}</h1>
        <p>${data.subtitle}</p>
        </div>

      <div class="hero-image">
        <img src="${data.image}" alt="${data.title}">
      </div>
    </div>
  `;
}

function renderAbout() {
  if(!aboutEl) return;
  aboutEl.innerHTML = `
    <h2>About this course</h2>
    <p>${data.about}</p>
  `;
}

function renderLearn() {
  if(!learnEl) return;
  learnEl.innerHTML = `
    <h2>What you will learn</h2>
    <ul class="learn-list">
      ${data.whatYouLearn.map(i => `<li>${i}</li>`).join("")}
    </ul>
  `;
}

function renderProgram() {
  if(!lessonsEl || !selectedModule) return;
  
  lessonsEl.innerHTML = `
    <div class="program-block">
      ${selectedModule.lessons
        .map(
          l => `
        <div class="lesson-item">
          <div class="lesson-title">${l.title}</div>
          <div class="lesson-duration">${l.duration}</div>
        </div>
        `
        )
        .join("")}
    </div>
  `;
}

function renderMentor() {
  if(!mentorEl) return;
  mentorEl.innerHTML = `
    <h2>Your Mentor</h2>
    <div class="mentor-box">
      <img src="${data.mentor.photo}" class="mentor-photo"/>
      <div class="mentor-info">
        <h3>${data.mentor.name}</h3>
        <p>${data.mentor.experience}</p>
        <p>${data.mentor.description}</p>
      </div>
    </div>
  `;
}

function renderReviews() {
  if(!reviewsEl) return;
  reviewsEl.innerHTML = `
    <h2>Student Reviews</h2>
    <div class="reviews-grid">
      ${data.reviews
        .map(
          r => `
        <div class="review-card">
          <p class="review-text">"${r.text}"</p>
          <p class="review-author">— ${r.author}</p>
        </div>
      `
        )
        .join("")}
    </div>
  `;
}

//
// 4. ЛОГІКА КНОПКИ JOIN (НАЙВАЖЛИВІШЕ)
//
function initJoinButton() {
  // Ця кнопка знаходиться в кінці сторінки у формі
  const formBtn = document.getElementById("joinCourseBtn");
  
  if (!formBtn) return;

  // Перевіряємо, чи ми вже записані
  const enrolled = JSON.parse(localStorage.getItem("enrolledCourses") || "[]");
  const isEnrolled = enrolled.includes(courseId);
  const currentUser = JSON.parse(localStorage.getItem("currentUser"));

  // Оновлюємо текст кнопки при завантаженні
  if (isEnrolled) {
    formBtn.textContent = "Go to Lessons";
    formBtn.classList.add("joined");
  } else {
    formBtn.textContent = "Join Course";
  }

  formBtn.onclick = (e) => {
    e.preventDefault();

    // 1. Якщо не авторизований -> на логін
    if (!currentUser) {
      alert("Please Sign In to join the course.");
      window.location.href = "sign_in.html";
      return;
    }

    // 2. Якщо вже записаний -> в урок
    if (isEnrolled) {
      // Знаходимо ID першого уроку цього курсу
      const firstLessonId = selectedModule?.lessons[0]?.id || 1;
      window.location.href = `lesson.html?id=${firstLessonId}`;
      return;
    }

    // 3. Якщо новий запис -> додаємо в масив
    enrolled.push(courseId);
    localStorage.setItem("enrolledCourses", JSON.stringify(enrolled));
    
    // Моментально оновлюємо вигляд
    formBtn.textContent = "Go to Lessons";
    
    // Питаємо користувача, куди йти
    const go = confirm("Congratulations! You have joined the course. Go to Dashboard?");
    if (go) {
      window.location.href = "my_account.html";
    } else {
      // Або залишаємось тут, але кнопка вже веде на урок
      window.location.reload(); 
    }
  };
}

//
// ЗАПУСК
//
renderHero();
renderAbout();
renderLearn();
renderProgram();
renderMentor();
renderReviews();
initJoinButton();
