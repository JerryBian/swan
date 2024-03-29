﻿function createEditor(textArea, u) {
    const editor = new EasyMDE({
        element: textArea,
        autosave: {
            enabled: true,
            uniqueId: u,
            text: "自动保存：",
            delay: 10000,
            submit_delay: 10000
        },
        lineNumbers: false,
        lineWrapping: true,
        maxHeight: "265px",
        previewClass: ["editor-preview", "post"],
        promptURLs: true,
        uploadImage: true,
        imageMaxSize: 1024 * 1024 * 10,
        imageAccept: ["image/png", "image/jpeg", "application/pdf", "image/svg+xml", "image/bmp", "image/gif", "image/tiff", "image/webp"],
        imageUploadEndpoint: "/admin/image-upload",
        imagePathAbsolute: true,
        imageTexts: {
            sbInit: "拖拽或者从剪切板复制图片",
            sbOnDragEnter: "拖拽图片",
            sbOnDrop: "正在上传图片 #images_names#",
            sbProgress: "正在上传 #file_name#: #progress#",
            sbOnUploaded: "成功上传 #image_name#"
        },
        errorCallback: function (err) {
            showErrorToast(err);
        },
        renderingConfig: {
            codeSyntaxHighlighting: true,
            hljs: window.hljs
        },
        spellChecker: false,
        toolbarButtonClassPrefix: "mde",
        toolbar: ['bold', 'italic', 'strikethrough', 'heading', '|', 'quote', 'unordered-list', 'ordered-list', 'code', 'table', '|', 'image', 'horizontal-rule', 'link', {
            name: "upload-image",
            action: EasyMDE.drawUploadedImage,
            className: "bi bi-images",
            title: "Upload Image",
        }, '|', 'preview', 'side-by-side', 'fullscreen', '|', 'guide', '|']
    });
    return editor;
}

function submitRequest(url, option) {
    if (!option) {
        option = {}
    }

    if (option.form) {
        if (option.form.classList.contains("needs-validation")) {
            if (!option.form.checkValidity()) {
                option.form.classList.add("was-validated");
                return;
            }
        }

        const fieldset = option.form.closest("fieldset");
        if (fieldset) {
            fieldset.disabled = true;
        }
    }

    if (option.preAction) {
        option.preAction();
    }

    const method = option.method ?? "POST";
    const body = option.body ?? "";
    const formPostAction = function () {
        if (option.form) {
            if (option.form.classList.contains("was-validated")) {
                option.form.classList.remove("was-validated");
            }

            const fieldset = option.form.closest("fieldset");
            if (fieldset) {
                fieldset.disabled = false;
            }
        }
    };

    var headers;
    if (option.contentType && option.contentType.length > 0) {
        headers = {
            "Content-Type": option.contentType
        }
    } else {
        headers = {}
    }

    window.fetch(url,
        {
            method: method,
            headers: headers,
            body: body
        }).then(response => response.json()).then(result => {
            if (!result.ok) {
                showErrorToast(result.message);
                formPostAction();
            } else {
                if (option.form) {
                    showInfoToast("Submit successfully.");
                }
                
                if (result.redirectTo) {
                    window.location.href = result.redirectTo;
                } else {
                    if (option.okAction) {
                        option.okAction(result.content);
                    }
                }
            }

            if (option.postAction) {
                formPostAction();
                option.postAction();
            }
        }).catch(error => {
            showErrorToast(error);
            formPostAction();
        });
}