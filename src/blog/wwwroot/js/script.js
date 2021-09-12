function activeNavItem(id) {
    document.querySelector(id).classList.add("active");
}

function setScrollspy(target) {
    const body = document.querySelector("body");
    body.setAttribute("data-bs-spy", "scroll");
    body.setAttribute("data-bs-target", target);
    body.setAttribute("data-bs-offset", "10");
}

window.fallbackTest = function () {};