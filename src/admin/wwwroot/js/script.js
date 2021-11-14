function toggleSpinner() {
    let main1 = document.querySelector("#main1");
    if (main1.classList.contains("visible")) {
        main1.classList.remove("visible");
        main1.classList.add("invisible");
    } else {
        main1.classList.remove("invisible");
        main1.classList.add("visible");
    }

    let main2 = document.querySelector("#main2");
    if (main2.classList.contains("visible")) {
        main2.classList.remove("visible");
        main2.classList.add("invisible");
    } else {
        main2.classList.remove("invisible");
        main2.classList.add("visible");
    }
}

function forceReloadBlogData() {
    Swal.fire({
        title: "确定要清除博客的缓存吗？",
        showCancelButton: true,
        confirmButtonText: "确定"
    }).then((result) => {
        if (result.isConfirmed) {
            fetch('/blog/reload',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'text/plain'
                    },
                    body: 'Request by Admin - Force Reload Blog Data button'
                })
                .then(response => {
                    if (response.ok) {
                        Swal.fire("清除成功", "", "success");
                    }
                });
        }
    });
}

function persistent() {
    Swal.fire({
        title: "确定要持久化数据库吗？",
        showCancelButton: true,
        confirmButtonText: "确定"
    }).then((result) => {
        if (result.isConfirmed) {
            fetch('/blog/persistent',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'text/plain'
                    },
                    body: ':meat_on_bone: forced by @Context.User.Identity?.Name'
                })
                .then(response => {
                    if (response.ok) {
                        Swal.fire("持久化数据库成功", "", "success");
                    }
                });
        }
    });
}

function validateForms() {
    const forms = document.querySelectorAll('.needs-validation');

    // Loop over them and prevent submission
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit',
                function (event) {
                    if (!form.checkValidity()) {
                        event.preventDefault();
                        event.stopPropagation();
                    }

                    form.classList.add('was-validated');
                },
                false);
        });
}

function prepareEditor(textArea, u) {
    const editor = new EasyMDE({
        element: textArea,
        autosave: {
            enabled: true,
            uniqueId: u,
            text: "自动保存："
        },
        lineNumbers: false,
        lineWrapping: true,
        maxHeight: "200px",
        previewClass: "editor-preview",
        promptURLs: true,
        uploadImage: true,
        imageMaxSize: 1024 * 1024 * 20,
        imageAccept: ["image/png", "image/jpeg", "application/pdf", "image/svg+xml", "image/bmp", "image/gif", "image/tiff", "image/webp"],
        imageUploadEndpoint: "/file/upload",
        imagePathAbsolute: true,
        imageTexts: {
            sbInit: "拖拽或者从剪切板复制图片",
            sbOnDragEnter: "拖拽图片",
            sbOnDrop: "正在上传图片 #images_names#",
            sbProgress: "正在上传 #file_name#: #progress#",
            sbOnUploaded: "成功上传 #image_name#"
        },
        errorCallback: function (err) {
            Swal.fire("文件上传错误！", err, "error");
        },
        renderingConfig: {
            codeSyntaxHighlighting: true,
            hljs: window.hljs
        },
        spellChecker: false
    });
    return editor;
}