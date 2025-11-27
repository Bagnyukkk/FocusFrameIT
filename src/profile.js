/* src/profile.js */
document.addEventListener("DOMContentLoaded", () => {
  console.log("profile.js loaded");

  // Перевірка, чи залогінений юзер
  const currentUser = JSON.parse(localStorage.getItem("currentUser"));
  if (!currentUser) {
    window.location.href = "sign_in.html";
    return;
  }

  const editBtn = document.getElementById("editProfileBtn");
  const modal = document.getElementById("editModal");
  const backdrop = document.getElementById("modalBackdrop");

  const saveBtn = document.getElementById("saveProfileBtn");
  const cancelBtn = document.getElementById("cancelProfileBtn");

  // Поля вводу
  const nameField = document.getElementById("editName");
  const emailField = document.getElementById("editEmail");
  const countryField = document.getElementById("editCountry");
  const nickField = document.getElementById("editNickname");
  const photoField = document.getElementById("editPhoto");

  // Текстові елементи на сторінці
  const profileName = document.getElementById("profileName");
  const infoName = document.getElementById("infoName");
  const infoEmail = document.getElementById("infoEmail");
  const infoCountry = document.getElementById("infoCountry");
  const infoNick = document.getElementById("infoNickname");
  const profilePhoto = document.getElementById("profilePhoto");

  // 1. ЗАВАНТАЖЕННЯ ДАНИХ ПРИ СТАРТІ
  // Пріоритет: збережений повний профіль -> дані поточної сесії -> заглушки
  const savedProfile = JSON.parse(localStorage.getItem("profileData")) || {};
  
  // Об'єднуємо дані (актуалізуємо email/name з currentUser, якщо профілю ще немає)
  const userData = {
    name: savedProfile.name || currentUser.name,
    email: savedProfile.email || currentUser.email,
    country: savedProfile.country || "Ukraine",
    nickname: savedProfile.nickname || currentUser.name,
    photo: localStorage.getItem("profilePhoto") || "./img/hero/girl1_mob.webp" // Дефолтне фото
  };

  // Відображаємо на сторінці
  if (infoName) infoName.textContent = userData.name;
  if (profileName) profileName.textContent = userData.name;
  if (infoEmail) infoEmail.textContent = userData.email;
  if (infoCountry) infoCountry.textContent = userData.country;
  if (infoNick) infoNick.textContent = userData.nickname;
  if (profilePhoto) profilePhoto.src = userData.photo;

  // --- ВІДКРИТИ МОДАЛКУ ---
  if (editBtn) {
    editBtn.addEventListener("click", () => {
      modal.style.display = "block";
      backdrop.style.display = "block";

      // Заповнюємо поля поточними значеннями
      nameField.value = infoName.textContent;
      emailField.value = infoEmail.textContent;
      countryField.value = infoCountry.textContent;
      nickField.value = infoNick.textContent;
    });
  }

  // --- ЗАКРИТИ МОДАЛКУ ---
  const closeModal = () => {
    modal.style.display = "none";
    backdrop.style.display = "none";
  };

  if (cancelBtn) cancelBtn.addEventListener("click", closeModal);
  if (backdrop) backdrop.addEventListener("click", closeModal);

  // --- ЗБЕРЕГТИ ЗМІНИ ---
  if (saveBtn) {
    saveBtn.addEventListener("click", () => {
      // 1. Оновлюємо текст на сторінці
      infoName.textContent = nameField.value;
      infoEmail.textContent = emailField.value;
      infoCountry.textContent = countryField.value;
      infoNick.textContent = nickField.value;
      profileName.textContent = nameField.value;

      // 2. Обробка фото
      if (photoField.files[0]) {
        const reader = new FileReader();
        reader.onload = () => {
          profilePhoto.src = reader.result;
          localStorage.setItem("profilePhoto", reader.result);
        };
        reader.readAsDataURL(photoField.files[0]);
      }

      // 3. Зберігаємо в LocalStorage (ProfileData)
      const newProfileData = {
        name: nameField.value,
        email: emailField.value,
        country: countryField.value,
        nickname: nickField.value
      };
      localStorage.setItem("profileData", JSON.stringify(newProfileData));

      // 4. ВАЖЛИВО: Оновлюємо CurrentUser (щоб в хедері ім'я теж змінилося)
      const updatedUser = {
        ...currentUser,
        name: nameField.value,
        email: emailField.value
      };
      localStorage.setItem("currentUser", JSON.stringify(updatedUser));

      closeModal();
      alert("Profile updated successfully!");
    });
  }
});