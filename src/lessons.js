/* src/lessons.js */
export const course = [

  // ============================
  // MODULE 1 — BASICS OF PHOTO
  // ============================
  {
    moduleId: 1,
    moduleTitle: "Basics of Photography",
    lessons: [

      // -------- LESSON 1 ----------
      {
        id: 1,
        title: "Introduction to Photography",
        duration: "7 min",
        video: "/src/video/Course_Introduction_Basic_Photography.mp4",
        poster: "./img/hero/basic.png",
        content: `
          <div class="lesson-block">
            <h2>Welcome to your photography journey 📸</h2>
            <p class="lesson-text">
              In this lesson, you will learn what photography is, how a camera works, and why a good photo depends not on the equipment, but on the person behind it.
            </p>
            <h3>What you will understand after this lesson:</h3>
            <ul class="lesson-list">
              <li>Why the photographer's vision is more important than the camera</li>
              <li>How a camera works: sensor, light, focus</li>
              <li>Types of cameras and which one suits you best</li>
            </ul>
            <h3>Practice:</h3>
            <p class="lesson-text">Take 3 photos with different lighting.</p>
          </div>
        `,
        materials: [
          { name: "📄 PDF: Introduction Guide", file: "files/intro.pdf" },
          { name: "☑ Checklist: Beginner Photographer", file: "files/checklist_intro.pdf" }
        ]
      },

      // -------- LESSON 2 ----------
      {
        id: 2,
        title: "Camera Settings",
        duration: "9 min",
        video: "video/lesson2.mp4",
        poster: "img/poster2.jpg",
        content: `
          <div class="lesson-block">
            <h2>Understanding Camera Settings</h2>
            <p class="lesson-text">The exposure triangle is the foundation.</p>
            <h3>ISO, Shutter Speed, Aperture</h3>
            <p class="lesson-text">Learn how these three parameters affect your shot.</p>
          </div>
        `,
        materials: [
          { name: "📄 Exposure Triangle Cheatsheet", file: "files/exposure_triangle.pdf" }
        ]
      },

      // -------- LESSON 3 ----------
      {
        id: 3,
        title: "Understanding Light",
        duration: "11 min",
        video: "video/light_placeholder.mp4",
        poster: "img/poster-light.jpg",
        content: `
          <div class="lesson-block">
            <h2>Light is EVERYTHING 💡</h2>
            <p class="lesson-text">Light shapes mood, depth, and volume.</p>
          </div>
        `,
        materials: [
          { name: "📄 Light Types Guideline", file: "files/light_guide.pdf" }
        ]
      },

      // -------- LESSON 4 ----------
      {
        id: 4,
        title: "Color & White Balance",
        duration: "9 min",
        video: "video/color_placeholder.mp4",
        poster: "img/poster-color.jpg",
        content: `
          <div class="lesson-block">
            <h2>Color & Mood 🎨</h2>
            <p class="lesson-text">Color defines the atmosphere of the shot.</p>
          </div>
        `,
        materials: [
          { name: "📄 White Balance Cheatsheet", file: "files/wb.pdf" }
        ]
      }
    ]
  },

  // ============================
  // MODULE 2 — LANDSCAPE PHOTOGRAPHY (NEW)
  // ============================
  {
    moduleId: 2, // THIS ID NOW MATCHES THE LINK ?course=2
    moduleTitle: "Landscape Photography",
    lessons: [
      {
        id: 201, // Unique IDs for the new course
        title: "Planning Your Shot",
        duration: "15 min",
        video: "video/lesson_landscape_1.mp4",
        poster: "img/hero/boy1_desk@1x.webp",
        content: `
          <div class="lesson-block">
            <h2>Scouting Locations 🗺️</h2>
            <p class="lesson-text">
              A beautiful landscape doesn't start with the camera, but with planning.
              Learn about the "Golden Hour" and how to choose a location.
            </p>
          </div>
        `,
        materials: [
          { name: "📱 App List for Photographers", file: "#" }
        ]
      },
      {
        id: 202,
        title: "Composition in Nature",
        duration: "12 min",
        video: "video/lesson_landscape_2.mp4",
        poster: "img/hero/girl2_desk@1x.webp",
        content: `
          <div class="lesson-block">
            <h2>Foreground, Middle ground, Background</h2>
            <p class="lesson-text">
              How to add depth to landscape photographs.
            </p>
          </div>
        `,
        materials: []
      }
    ]
  },

  // ============================
  // MODULE 3 — PORTRAIT PHOTOGRAPHY
  // ============================
  {
    moduleId: 3,
    moduleTitle: "Portrait Photography",
    lessons: [
      {
        id: 301,
        title: "Lighting Patterns",
        duration: "18 min",
        video: "video/lesson_portrait_1.mp4",
        poster: "img/hero/girl2_desk@1x.webp",
        content: `
          <div class="lesson-block">
            <h2>Classic Lighting Schemes 💡</h2>
            <p class="lesson-text">
              Light shapes the face. We will analyze 3 main schemes.
            </p>
            <h3>Rembrandt Light</h3>
            <p class="lesson-text">A characteristic triangle of light on the shadow side of the cheek. Dramatic and cinematic.</p>
            <h3>Butterfly Light</h3>
            <p class="lesson-text">Light from above, shadow under the nose in the shape of a butterfly. Ideal for beauty shots.</p>
          </div>
        `,
        materials: [
          { name: "📄 Posing Guide PDF", file: "#" }
        ]
      },
      {
        id: 302, // Last lesson of the course (final test will be here)
        title: "Working with Models",
        duration: "14 min",
        video: "video/lesson_portrait_2.mp4",
        poster: "img/hero/girl2_desk@1x.webp",
        content: `
          <div class="lesson-block">
            <h2>Communication is Key 🗣️</h2>
            <p class="lesson-text">
              Your task is to relax the person. Turn on music, give compliments, show successful shots during the process.
            </p>
          </div>
        `,
        materials: []
      }
    ]
  }

];
