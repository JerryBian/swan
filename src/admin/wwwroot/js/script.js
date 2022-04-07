function activeNavItem(id) {
    document.querySelector(id).classList.add("active");
}

function makeid(length) {
    var result = "";
    const characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() *
            charactersLength));
    }
    return result;
}

function getRandomColor() {
    const letters = "0123456789ABCDEF";
    let color = "#";
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

Chart.defaults.font.size = 12;
function createChart(canvas, res) {
    if (!res.labels || !res.data || !res.type) {
        showErrorMessageModal("初始化 chart 失败！");
        return null;
    }

    if (res.labels.length !== res.data.length) {
        showErrorMessageModal(`chart(${res.title}) 的 X Y 轴数量不一致`);
        return null;
    }

    const bgColors = [];
    const bdColors = [];
    res.labels.forEach(function () {
        bgColors.push(getRandomColor());
        bdColors.push(getRandomColor());
    });
    let data = {
        labels: res.labels,
        datasets: [{
            label: res.title,
            backgroundColor: bgColors,
            borderColor: bdColors,
            borderWidth: 1,
            data: res.data
        }]
};
    const config = {
        data: data,
        type: res.type,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    };

    return new window.Chart(canvas, config);
}

function submitRequest(url, option) {
    if (option.form) {
        if (option.form.classList.contains("needs-validation")) {
            if (option.form.checkValidity()) {
                option.form.classList.add("was-validated");
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
    const contentType = option.contentType ?? "application/json";
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

    window.fetch(url,
        {
            method: method,
            headers: {
                "Content-Type": contentType
            },
            body: body
        }).then(response => response.json()).then(result => {
            if (!result.ok) {
                showErrorMessageModal(result.message);
                formPostAction();
            } else {
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
        showErrorMessageModal(error);
            formPostAction();
    });
}

function persistent(user) {
    showConfirmMessageModal("确定要持久化数据库吗？",
        function () {
            submitRequest("/persistent",
                {
                    contentType: "text/plain",
                    body: `:meat_on_bone: forced by ${user}`,
                    postAction: function () {
                        showInfoMessageModal("数据库持久化成功。");
                    }
                });
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
            showErrorMessageModal(err);
        },
        renderingConfig: {
            codeSyntaxHighlighting: true,
            hljs: window.hljs
        },
        spellChecker: false
    });
    return editor;
}

function showMessageModal(title, bodyHtml, footerHtml) {
    let messageModalEl = document.querySelector("#messageModal");
    if (!messageModalEl) {
        return;
    }

    let messageModal = bootstrap.Modal.getOrCreateInstance(messageModalEl);
    messageModal.show();

    let messageModalTitle = document.querySelector("#messageModalTitle");
    if (messageModalTitle) {
        messageModalTitle.innerHTML = title;
    }

    let messageModalBody = document.querySelector("#messageModalBody");
    if (messageModalBody) {
        messageModalBody.innerHTML = bodyHtml;
    }

    let messageModalFooter = document.querySelector("#messageModalFooter");
    if (messageModalFooter) {
        footerHtml = footerHtml ??
            "<button type=\"button\" class=\"btn btn-info\" data-bs-dismiss=\"modal\">Ok</button>";
        messageModalFooter.innerHTML = footerHtml;
    }
}

function showInfoMessageModal(message) {
    showMessageModal(`<i class="fa-solid fa-circle-info text-info"></i> Info`, `<p>${message}</p>`);
}

function showErrorMessageModal(message) {
    showMessageModal(`<i class="fa-solid fa-circle-exclamation text-danger"></i> Error`, `<p>${message}</p>`);
}

function showConfirmMessageModal(message, yesHandler) {
    if (!yesHandler) {
        return;
    }

    let yesId = makeId(8);
    let yesButton = `<button type="button" class="btn btn-danger" data-bs-dismiss="modal" id="${yesId}"><span class="px-2">Yes</span></button>`;
    let noButton = "<button type=\"button\" class=\"btn btn-dark\" data-bs-dismiss=\"modal\"><span class=\"px-2\">No</span></button>";
    showMessageModal(`<i class="fa-solid fa-circle-question text-warning"></i> Question`, `<p>${message}</p>`, yesButton + noButton);
    document.querySelector(`#${yesId}`).addEventListener("click", function () { yesHandler(); });
}

function makeId(length) {
    var result = "";
    const characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() *
            charactersLength));
    }
    return result;
}