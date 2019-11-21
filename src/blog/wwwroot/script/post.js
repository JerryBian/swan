document.addEventListener("DOMContentLoaded", function () {
    hljs.initHighlightingOnLoad();
}, false);

try {
    document.addEventListener("DOMContentLoaded", function () {
        var modal = document.querySelector("#imgModal");
        if (!modal) {
            return;
        }

        var images = document.querySelectorAll(".post img");
        var modalImg = document.querySelector("#modal-content");
        var caption = document.querySelector("#modal-caption");
        images.forEach(function(img) {
            img.onclick = function () {
                modal.style.display = "block";
                modalImg.src = this.src;
                caption.innerHTML = this.alt;
            };
        });

        function closeModal() {
            modal.style.display = "none";
        }

        document.querySelector("#modal-close").onclick = closeModal;
    }, false);

} catch (e) {
    console.log(e);
}