let menuBtn = document.querySelector('.burger-btn');
let menu = document.querySelector('.mobile-menu');
let backdrop = document.querySelector('.mobile-menu-backdrop');
let menuItem = document.querySelectorAll('.close-menu');
let body = document.body;

if (menuBtn) {
  menuBtn.addEventListener('click', function () {
    menu.classList.toggle('active');
    backdrop.style.display = menu.classList.contains('active')
      ? 'block'
      : 'none';
    body.classList.toggle('no-scroll', menu.classList.contains('active'));
  });

  menuItem.forEach(function (item) {
    item.addEventListener('click', function () {
      menu.classList.toggle('active');
      backdrop.style.display = 'none';
      body.classList.remove('no-scroll');
    });
  });

  backdrop.addEventListener('click', function () {
    menu.classList.remove('active');
    backdrop.style.display = 'none';
    body.classList.remove('no-scroll');
  });
}

// scroll to top

const scrollToTopBtn = document.getElementById('scrollToTopBtn');
if(scrollToTopBtn != null)
  {
    scrollToTopBtn.addEventListener('click', function () {
  window.scrollTo({
    top: 0,
    behavior: 'smooth',
  });
});

window.addEventListener('scroll', function () {
  if (window.scrollY > 300) {
    scrollToTopBtn.classList.add('visible');
  } else {
    scrollToTopBtn.classList.remove('visible');
  }
});
}


// печатальщик текста

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

function onEntry(entry) {
  entry.forEach(change => {
    if (change.isIntersecting) {
      typeWriter(change.target, 30); // последнее знчение это скорость, чемь меньше - тем быстрее)))
    }
  });
}

let options = {
  threshold: [0.5], // Какая часть элемента должна быть видна, чтоб все сработало :) сейчас - 50% элемента
};
let observer = new IntersectionObserver(onEntry, options);

document.querySelectorAll('.typewriter').forEach(el => {
  observer.observe(el);
});

// Card move
document.addEventListener('DOMContentLoaded', function () {
  const observer = new IntersectionObserver(entries => {
    entries.forEach((entry, index) => {
      if (entry.isIntersecting) {
        setTimeout(() => {
          entry.target.classList.add('visible');
        }, index * 250); // Задержка для каждой следующей карточки
      }
    });
  });

  const cards = document.querySelectorAll('.move-card');
  cards.forEach(card => observer.observe(card));
});
