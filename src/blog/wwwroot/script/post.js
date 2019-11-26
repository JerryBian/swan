document.addEventListener("DOMContentLoaded",
    function() {
        hljs.initHighlightingOnLoad();

        try {
            var modal = document.querySelector("#imgModal");
            if (!modal) {
                return;
            }

            const images = document.querySelectorAll(".post img");
            var modalImg = document.querySelector("#modal-content");
            var caption = document.querySelector("#modal-caption");
            images.forEach(function(img) {
                img.onclick = function() {
                    modal.style.display = "block";
                    modalImg.src = this.src;
                    caption.innerHTML = this.alt;
                };
            });

            document.querySelector("#modal-close").onclick = function() {
                modal.style.display = "none";
            };
        } catch (e) {
            console.log(e);
        }
    },
    false);