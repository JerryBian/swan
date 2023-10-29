function setActive(ele) {
    if (ele) {
        if (typeof ele.forEach === 'function') {
            ele.forEach(x => {
                setActive(x);
            });
        } else {
            if (!ele.classList.contains('active')) {
                ele.classList.add('active');
            }
        }
    }
}