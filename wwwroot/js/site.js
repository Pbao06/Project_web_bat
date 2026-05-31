/**
 * Global Slider Logic
 */

function scrollSlider(id, direction) {
    const slider = document.getElementById(id);
    if (!slider) return;

    // For hero slider, scroll 100% of width. For others, 80%.
    const isHero = id === 'heroSlider';
    const scrollAmount = isHero ? slider.clientWidth : slider.clientWidth * 0.8;

    slider.scrollBy({
        left: direction * scrollAmount,
        behavior: 'smooth'
    });
}

/**
 * Handle Slider Button Visibility
 */
function updateSliderButtons(sliderId) {
    const slider = document.getElementById(sliderId);
    if (!slider) return;

    const container = slider.closest('.slider-container');
    if (!container) return;

    const prevBtn = container.querySelector('.slider-btn.prev');
    const nextBtn = container.querySelector('.slider-btn.next');

    if (prevBtn) {
        if (slider.scrollLeft <= 5) {
            prevBtn.classList.add('hidden');
        } else {
            prevBtn.classList.remove('hidden');
        }
    }

    if (nextBtn) {
        // Check if we can scroll more
        if (slider.scrollLeft + slider.clientWidth >= slider.scrollWidth - 5) {
            nextBtn.classList.add('hidden');
        } else {
            nextBtn.classList.remove('hidden');
        }
    }
}

// Attach scroll listeners to all sliders
document.addEventListener('DOMContentLoaded', () => {
    const sliders = document.querySelectorAll('.products-strip, .standard-slider, .reviews-slider, .related-slider');
    sliders.forEach(slider => {
        if (slider.id) {
            // Initial update
            updateSliderButtons(slider.id);
            // Update on scroll
            slider.addEventListener('scroll', () => updateSliderButtons(slider.id));
        }
    });

    // Also update on window resize
    window.addEventListener('resize', () => {
        sliders.forEach(slider => {
            if (slider.id) updateSliderButtons(slider.id);
        });
    });
});
