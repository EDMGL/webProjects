let currentIndex = 0;

function showSlide(index) {
    const slides = document.querySelector('.slides');
    const totalSlides = slides.children.length;

    if (index >= totalSlides) {
        currentIndex = 0;
    } else if (index < 0) {
        currentIndex = totalSlides - 1;
    } else {
        currentIndex = index;
    }

    const offset = -currentIndex * 100;
    slides.style.transform = `translateX(${offset}%)`;
}

function nextSlide() {
    showSlide(currentIndex + 1);
}

function prevSlide() {
    showSlide(currentIndex - 1);
}

// Otomatik geçiş (isteğe bağlı)
const interval = setInterval(nextSlide, 3000);

document.addEventListener('DOMContentLoaded', () => {
    const prevButton = document.querySelector('.prev');
    const nextButton = document.querySelector('.next');

    if (prevButton && nextButton) {
        prevButton.addEventListener('click', () => {
            clearInterval(interval); // Otomatik geçişi durdurur
            prevSlide();
        });

        nextButton.addEventListener('click', () => {
            clearInterval(interval); // Otomatik geçişi durdurur
            nextSlide();
        });
    } else {
        console.error('Slider kontrol butonları bulunamadı.');
    }
});