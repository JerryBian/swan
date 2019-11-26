document.addEventListener("DOMContentLoaded",
    function() {
        window.onscroll = function() {
            const winScroll = document.body.scrollTop || document.documentElement.scrollTop;
            const height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
            const scrolled = (winScroll / height) * 100;
            const ele = document.getElementById("pbar");
            ele.style.width = scrolled + "%";
            ele.setAttribute("aria-valuenow", scrolled);
        };

        galite("create", "UA-97849167-1", "auto");
        galite("send", "pageview");
    },
    false);