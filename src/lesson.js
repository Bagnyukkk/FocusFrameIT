/* src/lesson.js */
import { course } from "./lessons.js";

//
// INITIALIZATION
//
const allLessons = course.flatMap(m => m.lessons);
const url = new URL(window.location.href);
let currentLessonId = Number(url.searchParams.get("id"));

if (!currentLessonId) {
  // Якщо ID немає, спробуємо перший урок першого курсу
  currentLessonId = 1;
  window.history.replaceState(null, null, "?id=1");
}

let lesson = allLessons.find(l => l.id === currentLessonId);

// Якщо урок з таким ID не знайдено (наприклад, неправильне посилання)
if (!lesson) {
  lesson = allLessons[0];
  currentLessonId = lesson.id;
}

// !!! ВАЖЛИВО: Знаходимо, до якого модуля (курсу) належить цей урок
const currentModule = course.find(m => m.lessons.some(l => l.id === currentLessonId));

//
// DOM ELEMENTS
//
const titleEl = document.getElementById("lessonTitle");
const durationEl = document.getElementById("lessonDuration");
const videoEl = document.getElementById("lessonVideo");
const videoSrc = document.getElementById("videoSrc");
const contentEl = document.getElementById("lessonContent");
const materialsBox = document.getElementById("materialsBox");
const commentInput = document.getElementById("commentInput");

//
// QUIZ DATA
//
const quizData = {
  // Lesson 1
  1: [
    { type: "single", question: "What is a camera diaphragm?", answers: ["Element that determines the focal length", "An opening that regulates the amount of light", "Lens type"], correct: [1] },
    { type: "truefalse", question: "ISO affects the noise level in photos.", answers: ["True", "False"], correct: [0] },
    { type: "multiple", question: "What affects exposure?", answers: ["ISO", "Diaphragm", "White balance", "Exposure"], correct: [0, 1, 3] }
  ],
  // Lesson 2
  2: [
    { type: "single", question: "Що буде, якщо збільшити витримку?", answers: ["Кадр стане темнішим", "Кадр стане світлішим і можливий змаз", "Нічого не зміниться"], correct: [1] },
    { type: "multiple", question: "Які параметри відносяться до трикутника експозиції?", answers: ["ISO", "Фокусна відстань", "Діафрагма", "Витримка"], correct: [0, 2, 3] },
    { type: "truefalse", question: "Високе значення ISO завжди краще, ніж низьке.", answers: ["True", "False"], correct: [1] }
  ],
  // Lesson 5
  5: [
    { type: "single", question: "Що таке правило третин?", answers: ["Розміщення об'єкта по центру", "Поділ кадру сіткою 3×3", "Техніка розмиття"], correct: [1] },
    { type: "multiple", question: "Що покращує композицію?", answers: ["Лідируючі лінії", "Хаос", "Рамкування"], correct: [0, 2] }
  ],
  // Lesson 202 (Landscape)
  202: [
    { type: "single", question: "Що таке 'Золота година'?", answers: ["Полудень", "Година після сходу/перед заходом", "Ніч"], correct: [1] },
    { type: "multiple", question: "Глибина в пейзажі:", answers: ["Передній план", "Лінії", "Кришка об'єктива"], correct: [0, 1] },
    { type: "truefalse", question: "Для пейзажу краще f/1.8?", answers: ["True", "False"], correct: [1] }
  ],
  // Lesson 302 (Portrait)
  302: [
    { type: "single", question: "Трикутник на щоці - це?", answers: ["Butterfly", "Rembrandt", "Split"], correct: [1] },
    { type: "truefalse", question: "Широкий кут (16mm) для портрета?", answers: ["True", "False"], correct: [1] }
  ]
};

//
// MARK COMPLETED
//
function markCompleted(id) {
  let completed = JSON.parse(localStorage.getItem("completedLessons") || "[]");
  if (!completed.includes(id)) {
    completed.push(id);
    localStorage.setItem("completedLessons", JSON.stringify(completed));
  }
}

//
// RENDER SIDEBAR (ВИПРАВЛЕНО: Показує тільки поточний курс)
//
function renderSidebar() {
  const sidebar = document.getElementById("modulesSidebar");
  if (!sidebar || !currentModule) return;

  const completed = JSON.parse(localStorage.getItem("completedLessons") || "[]");

  sidebar.innerHTML = `
    <div class="module active-module">
      <div class="module-header" style="cursor:default; font-weight:700; color:#fff; margin-bottom:10px;">
        ${currentModule.moduleTitle}
      </div>
      
      <ul class="module-lessons">
        ${currentModule.lessons.map(l => {
            const isActive = l.id === lesson.id ? "active" : "";
            const isCompleted = completed.includes(l.id) ? "completed" : "";
            
            return `
              <li class="lesson-item ${isActive} ${isCompleted}" onclick="window.location.href='lesson.html?id=${l.id}'">
                ${l.title}
                ${isCompleted ? ' <span style="float:right; color:#d1a476;"></span>' : ''}
              </li>
            `;
        }).join("")}
      </ul>
    </div>

    <div style="margin-top: 30px; padding-top: 20px; border-top: 1px solid #333;">
       <a href="my_account.html" style="color: #888; font-size: 14px; text-decoration: none; display: flex; align-items: center; gap: 8px; transition: 0.2s;">
         <span>←</span> Back to Dashboard
       </a>
    </div>
  `;
}

//
// SIDEBAR MATERIALS
//
function renderSidebarMaterials() {
  const box = document.getElementById("sidebarMaterials");
  if (!box) return;

  box.innerHTML = lesson.materials.length
    ? lesson.materials.map(m => `<a href="${m.file}" download>${m.name}</a>`).join("")
    : `<p style="color:#777; font-size:13px;">No materials</p>`;
}

//
// LESSON CONTENT
//
function renderLesson() {
  titleEl.innerText = lesson.title;
  durationEl.innerText = `Estimated time: ${lesson.duration}`;

  if (videoSrc && videoEl) {
    videoSrc.src = lesson.video;
    if (lesson.poster) {
      videoEl.poster = lesson.poster;
    }
    videoEl.load();
  }

  contentEl.innerHTML = lesson.content;

  if(materialsBox) {
    materialsBox.innerHTML = lesson.materials
        .map(m => `<a class="file" href="${m.file}" download>${m.name}</a>`)
        .join("");
  }

  if (commentInput) {
    commentInput.value = localStorage.getItem(`comment_${lesson.id}`) || "";
    commentInput.oninput = () =>
      localStorage.setItem(`comment_${lesson.id}`, commentInput.value);
  }

  markCompleted(lesson.id);
  renderNavigation();
  updateProgress();
  renderSidebarMaterials();
}

//
// NAVIGATION: PREV / NEXT (В межах курсу або глобально)
//
function renderNavigation() {
  const index = allLessons.findIndex(l => l.id === lesson.id);
  const prev = allLessons[index - 1];
  const next = allLessons[index + 1];

  const prevBtn = document.getElementById("prevLessonBtn");
  const nextBtn = document.getElementById("nextLessonBtn");

  if (prevBtn) {
    if (prev) {
      prevBtn.onclick = () => (window.location.href = `lesson.html?id=${prev.id}`);
      prevBtn.style.opacity = "1";
      prevBtn.style.pointerEvents = "auto";
    } else {
      prevBtn.style.opacity = "0.35";
      prevBtn.style.pointerEvents = "none";
    }
  }

  if (nextBtn) {
    if (next) {
        nextBtn.onclick = () => (window.location.href = `lesson.html?id=${next.id}`);
        nextBtn.style.display = "inline-block";
    } else {
        nextBtn.style.display = "none";
    }
  }
}

//
// PROGRESS
//
function updateProgress() {
  const completed = JSON.parse(localStorage.getItem("completedLessons") || "[]");
  // Рахуємо прогрес тільки в межах поточного курсу для точності
  const moduleLessons = currentModule.lessons;
  const moduleCompleted = moduleLessons.filter(l => completed.includes(l.id)).length;
  
  const percent = Math.round((moduleCompleted / moduleLessons.length) * 100);

  const bar = document.querySelector(".progress-fill");
  const value = document.querySelector(".progress-value");

  if (bar) bar.style.width = percent + "%";
  if (value) value.innerText = percent + "%";
}

//
// HOMEWORK PRO SYSTEM
//
function initHomeworkPRO() {
  const input = document.getElementById("hwInput");
  const preview = document.getElementById("hwPreview");
  const modal = document.getElementById("hwModal");
  const modalClose = document.getElementById("hwModalClose");
  const modalOpen = document.getElementById("openHWModal");
  const modalFiles = document.getElementById("hwModalFiles");
  const hwCommentInput = document.getElementById("homeworkComment");
  const submitBtn = document.getElementById("submitHW");

  if (!input || !preview || !modal) return;

  let saved = JSON.parse(localStorage.getItem(`hw_${lesson.id}`) || "[]");

  function saveFiles() {
    localStorage.setItem(`hw_${lesson.id}`, JSON.stringify(saved));
    // Встановлюємо статус Pending
    localStorage.setItem(`hw_status_${lesson.id}`, "Pending");
  }

  function renderFiles() {
    preview.innerHTML = saved.length
      ? saved.map((f, i) => `
          <div class="hw-file">
            <span>${f.name}</span>
            <span class="hw-remove" style="cursor:pointer; color:red;" onclick="removeFile(${i})">&times;</span>
          </div>
        `).join("")
      : `<p style="color:#777; font-size:13px;">No files uploaded</p>`;

    if(modalFiles) {
        modalFiles.innerHTML = saved.length === 0
            ? `<p style="color:#aaa; font-size:13px;">No files yet</p>`
            : saved.map(f => `<div style="padding:6px 0">${f.name}</div>`).join("");
    }
    updateSidebarHomework();
  }

  window.removeFile = (i) => {
      saved.splice(i, 1);
      saveFiles();
      renderFiles();
  };

  input.onchange = () => {
    const files = [...input.files];
    files.forEach(f => {
      if (f.size > 10 * 1024 * 1024) return; 
      saved.push({ name: f.name, size: f.size });
    });
    saveFiles();
    renderFiles();
    input.value = "";
    alert("File uploaded! Status: Pending review.");
  };

  if(modalOpen) modalOpen.onclick = () => (modal.style.display = "flex");
  if(modalClose) modalClose.onclick = () => (modal.style.display = "none");
  
  window.onclick = e => {
    if (e.target === modal) modal.style.display = "none";
  };

  if (hwCommentInput) {
    hwCommentInput.value = localStorage.getItem(`hw_comment_${lesson.id}`) || "";
    hwCommentInput.addEventListener("input", () => {
      localStorage.setItem(`hw_comment_${lesson.id}`, hwCommentInput.value);
    });
  }

  if (submitBtn) {
    submitBtn.addEventListener("click", () => {
      alert("Homework submitted for review!");
    });
  }

  renderFiles();
}

function updateSidebarHomework() {
  const hw = JSON.parse(localStorage.getItem(`hw_${lesson.id}`) || "[]");
  const statusEl = document.getElementById("sidebarHwStatus");
  const statusText = localStorage.getItem(`hw_status_${lesson.id}`) || "None";

  if (!statusEl) return;

  let color = "#aaa";
  if(statusText === "Approved") color = "#9bd17c";
  if(statusText === "Pending") color = "#ffeaa7";

  if (hw.length === 0) statusEl.innerText = "No files";
  else statusEl.innerHTML = `${hw.length} files <span style="color:${color}; font-size:11px; margin-left:5px;">(${statusText})</span>`;
}

//
// TABS
//
function initTabs() {
  const tabButtons = document.querySelectorAll(".lesson-tab");
  const tabPanels = document.querySelectorAll(".lesson-section");

  tabButtons.forEach(btn => {
    btn.addEventListener("click", () => {
      const target = btn.dataset.tab;
      tabButtons.forEach(b => b.classList.remove("is-active"));
      btn.classList.add("is-active");
      tabPanels.forEach(panel => {
        if (panel.dataset.tabPanel === target) panel.classList.add("is-active");
        else panel.classList.remove("is-active");
      });
    });
  });

  document.querySelectorAll(".sidebar-scroll").forEach(btn => {
    btn.addEventListener("click", () => {
      const targetTab = btn.getAttribute("data-target");
      const tabBtn = document.querySelector(`.lesson-tab[data-tab="${targetTab}"]`);
      if (tabBtn) tabBtn.click();
    });
  });
}

//
// NOTES PRO
//
function initNotesPRO() {
  const textarea = document.getElementById("commentInput");
  if (!textarea) return;
  const storageKey = `comment_${lesson.id}`;
  
  // Buttons
  const clearBtn = document.getElementById("notesClearBtn");
  const copyBtn = document.getElementById("notesCopyBtn");
  const exportBtn = document.getElementById("notesExportBtn");
  const fullscreenBtn = document.getElementById("notesFullscreenBtn");
  const modal = document.getElementById("notesModal");
  const modalClose = document.getElementById("notesModalClose");

  textarea.value = localStorage.getItem(storageKey) || "";

  textarea.addEventListener("input", () => {
    localStorage.setItem(storageKey, textarea.value);
  });

  if (clearBtn) clearBtn.onclick = () => {
      if(confirm("Clear notes?")) { textarea.value = ""; localStorage.removeItem(storageKey); }
  };
  
  if (copyBtn) copyBtn.onclick = () => {
      navigator.clipboard.writeText(textarea.value);
      alert("Copied!");
  };

  if (fullscreenBtn && modal) {
      fullscreenBtn.onclick = () => {
          modal.style.display = "flex";
          document.getElementById("notesModalBody").innerText = textarea.value;
      };
      if(modalClose) modalClose.onclick = () => modal.style.display = "none";
  }
}

//
// QUIZ SYSTEM
//
function initQuiz() {
  const quizContainer = document.getElementById("quizContainer");
  const startBtn = document.getElementById("startQuizBtn");
  const submitBtn = document.getElementById("submitQuizBtn");
  const retryBtn = document.getElementById("retryQuizBtn");
  const resultBox = document.getElementById("quizResult");

  if (!quizContainer) return;

  const questions = quizData[lesson.id] || [];
  let userAnswers = [];

  if (!questions.length) {
    quizContainer.innerHTML = '<p class="lesson-subtext">Quiz буде доступний пізніше.</p>';
    if(startBtn) startBtn.style.display = "none";
    return;
  }

  function renderQuiz() {
    quizContainer.innerHTML = "";
    userAnswers = [];

    questions.forEach((q, qi) => {
      const block = document.createElement("div");
      block.classList.add("quiz-block");
      block.innerHTML = `<div class="quiz-question">${qi + 1}. ${q.question}</div><div class="quiz-answers" id="quizAnswers_${qi}"></div>`;
      quizContainer.appendChild(block);

      const answersBox = block.querySelector(`#quizAnswers_${qi}`);
      q.answers.forEach((ans, ai) => {
        const btn = document.createElement("div");
        btn.classList.add("quiz-answer");
        btn.innerText = ans;
        btn.onclick = () => {
            if (q.type === "single" || q.type === "truefalse") {
                [...answersBox.children].forEach(el => el.classList.remove("selected"));
                btn.classList.add("selected");
                userAnswers[qi] = [ai];
            } else {
                btn.classList.toggle("selected");
                const selected = [...answersBox.children].map((el, idx) => el.classList.contains("selected") ? idx : null).filter(v => v!==null);
                userAnswers[qi] = selected;
            }
        };
        answersBox.appendChild(btn);
      });
    });
  }

  function checkResults() {
    let score = 0;
    questions.forEach((q, i) => {
      const correct = [...q.correct].sort().join(",");
      const user = (userAnswers[i] || []).slice().sort().join(",");
      if (correct === user) score++;
    });
    const percent = Math.round((score / questions.length) * 100);
    
    resultBox.innerHTML = `<p>Result: <strong>${score}/${questions.length}</strong> (${percent}%)</p>`;
    resultBox.style.display = "block";
    localStorage.setItem(`quiz_${lesson.id}`, percent);
  }

  if(startBtn) startBtn.onclick = () => {
      startBtn.style.display = "none";
      submitBtn.style.display = "inline-block";
      quizContainer.style.display = "block";
      renderQuiz();
  };

  if(submitBtn) submitBtn.onclick = () => {
      checkResults();
      submitBtn.style.display = "none";
      retryBtn.style.display = "inline-block";
  };

  if(retryBtn) retryBtn.onclick = () => {
      resultBox.style.display = "none";
      retryBtn.style.display = "none";
      submitBtn.style.display = "inline-block";
      renderQuiz();
  };
}

//
// RUN
//
renderSidebar();
renderLesson();
initHomeworkPRO();
updateSidebarHomework();
initNotesPRO();
initTabs();
initQuiz();






