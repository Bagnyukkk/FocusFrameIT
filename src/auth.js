/* src/auth.js */
console.log("Auth script loaded"); // Перевірка в консолі

const loginForm = document.querySelector('.sign-up-form');

// Перевіряємо сторінку
const isRegisterPage = window.location.pathname.includes('auth.html');
const isLoginPage = window.location.pathname.includes('sign_in.html');

if (loginForm) {
  console.log("Form found, attaching listener"); // Перевірка, чи знайшов форму

  loginForm.addEventListener('submit', (e) => {
    e.preventDefault(); // Зупиняємо перезавантаження сторінки
    console.log("Form submitted!"); 

    const nameInput = document.getElementById('user-name');
    const emailInput = document.getElementById('user-email');

    // Формуємо об'єкт юзера
    const user = {
      name: nameInput ? nameInput.value : 'Student',
      email: emailInput.value,
      isLoggedIn: true,
    };

    // --- ЛОГІКА РЕЄСТРАЦІЇ ---
    if (isRegisterPage) {
      localStorage.setItem('profileData', JSON.stringify({
        name: user.name,
        email: user.email,
        country: 'Ukraine',
        nickname: user.name
      }));
      
      localStorage.setItem('currentUser', JSON.stringify(user));
      
      if (!localStorage.getItem('enrolledCourses')) {
          localStorage.setItem('enrolledCourses', JSON.stringify([]));
      }

      alert(`Welcome, ${user.name}!`);
      window.location.href = './my_account.html';
    }

    // --- ЛОГІКА ВХОДУ ---
    if (isLoginPage) {
      // Шукаємо, чи є збережений профіль з таким email
      const savedProfile = JSON.parse(localStorage.getItem('profileData'));
      
      if (savedProfile && savedProfile.email === user.email) {
          user.name = savedProfile.name;
      }

      localStorage.setItem('currentUser', JSON.stringify(user));
      // Перенаправлення
      window.location.href = './my_account.html';
    }
  });
} else {
  console.error("Form NOT found! Check class name .sign-up-form");
}