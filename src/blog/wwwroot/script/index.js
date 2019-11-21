document.addEventListener("DOMContentLoaded", function () {
    window.onscroll = function () {
        var winScroll = document.body.scrollTop || document.documentElement.scrollTop;
        var height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
        var scrolled = (winScroll / height) * 100;
        var ele = document.getElementById("pbar");
        ele.style.width = scrolled + "%";
        ele.setAttribute("aria-valuenow", scrolled);
    };

    galite('create', 'UA-97849167-1', 'auto');
    galite('send', 'pageview');
}, false);