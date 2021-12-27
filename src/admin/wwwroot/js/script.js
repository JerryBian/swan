function activeNavItem(id) {
    document.querySelector(id).classList.add("active");
}

function toggleContainers(c1, c2) {
    if (c1.classList.contains("visible")) {
        c1.classList.remove("visible");
        c1.classList.add("invisible");
    } else if (c1.classList.contains("invisible")) {
        c1.classList.remove("invisible");
        c1.classList.add("visible");
    }

    if (c2.classList.contains("visible")) {
        c2.classList.remove("visible");
        c2.classList.add("invisible");
    } else if (c2.classList.contains("invisible")) {
        c2.classList.remove("invisible");
        c2.classList.add("visible");
    }
}

function makeid(length) {
    var result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() *
            charactersLength));
    }
    return result;
}

function getRandomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

Chart.defaults.font.size = 12;
Chart.defaults.font.family = "Noto Sans SC";
function createChart(canvas, res) {
    if (!res.labels || !res.data || !res.type) {
        window.Swal.fire({
            title: "错误",
            text: "初始化 chart 失败！",
            icon: "error",
            backdrop: false
        });

        return null;
    }

    if (res.labels.length !== res.data.length) {
        window.Swal.fire({
            title: "错误",
            text: `chart(${res.title}) 的 X Y 轴数量不一致`,
            icon: "error",
            backdrop: false
        });

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
                window.Swal.fire({
                    title: "错误",
                    text: result.message,
                    icon: "error",
                    backdrop: false
                });
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
            window.Swal.fire({
                title: "错误",
                text: error,
                icon: "error",
                backdrop: false
            });

            formPostAction();
    });
}

function persistent(user) {
    window.Swal.fire({
        title: "确认",
        text: "确定要持久化数据库吗？",
        icon: "info",
        showCancelButton: true,
        cancelButtonText: "取消",
        confirmButtonText: "确定"
    }).then((result) => {
        if (result.isConfirmed) {
            submitRequest("/persistent",
                {
                    contentType: "text/plain",
                    body: `:meat_on_bone: forced by ${user}`,
                    postAction: function () {
                        window.Swal.fire({
                            title: "通知",
                            text: "数据库持久化成功。",
                            icon: "success",
                            backdrop: false
                        });
                    }
                });
        };
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
            window.Swal.fire({
                title: "图片上传错误",
                text: err,
                icon: "error",
                backdrop: false
            });
        },
        renderingConfig: {
            codeSyntaxHighlighting: true,
            hljs: window.hljs
        },
        spellChecker: false
    });
    return editor;
}