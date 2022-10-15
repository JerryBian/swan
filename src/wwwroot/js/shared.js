function activeNavItem(id) {
    try {
        document.querySelector(id).classList.add("active");
    } catch { }
}