function setActive(ele) {
    if (ele) {
        if (!ele.classList.contains('active')) {
            ele.classList.add('active');
        }
    }
}