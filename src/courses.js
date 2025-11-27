export const courses = {
  basics: {
    id: "basics",
    title: "Basics of Photography",
    tagline: "Start your photography journey with confidence",
    heroImage: "./img/hero/basic.png",

    shortDescription: `
      Learn how cameras work, how light shapes your photos,
      and start shooting confidently — even on your phone.
    `,

    whatYouLearn: [
      "How a camera sees the world",
      "Exposure triangle: ISO, Aperture, Shutter speed",
      "Working with natural and artificial light",
      "Basics of composition",
      "How to take sharp and beautiful photos",
      "Phone vs camera — how to maximize both"
    ],

    about: `
      This course is designed for complete beginners.
      You will learn how photography works, how light affects your results,
      and how to take clean, aesthetic, well-exposed images even without
      expensive equipment. Perfect for those starting their creative journey.
    `,

    lessons: [
      { id: 1, title: "Introduction to Photography", duration: "7 min" },
      { id: 2, title: "Camera Settings", duration: "9 min" },
      { id: 3, title: "Understanding Light", duration: "12 min" },
      { id: 4, title: "Color & White Balance", duration: "10 min" },
      { id: 5, title: "Composition & Framing", duration: "14 min" }
    ],

    mentor: {
      name: "Alex Lebedev",
      experience: "15 years of experience",
      photo: "img/mentor_alex.jpg",
      bio: `
        Alex is a professional photographer specializing in portrait
        and product photography. His teaching approach is simple,
        structured and beginner-friendly.
      `
    },

    reviews: [
      {
        name: "Anna M.",
        text: "Amazing course! Everything is clear and practical.",
        rating: 5
      },
      {
        name: "Maria B.",
        text: "Learned more here in one week than in months of YouTube.",
        rating: 5
      }
    ]
  }
};
