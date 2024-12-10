let currentIndex = 0;

const slides = document.querySelectorAll(".slide");
const totalSlides = slides.length;
const slider = document.querySelector(".slider");

document.getElementById("prev").addEventListener("click", () => {
    currentIndex = (currentIndex > 0) ? currentIndex - 1 : totalSlides - 1;
    updateSlider();
});

document.getElementById("next").addEventListener("click", () => {
    currentIndex = (currentIndex < totalSlides - 1) ? currentIndex + 1 : 0;
    updateSlider();
});

function updateSlider() {
    slider.style.transform = `translateX(-${currentIndex * 100}%)`;
}