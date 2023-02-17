function activeNavItem(id) {
    try {
        document.querySelector(id).classList.add("active");
    } catch { }
}

function showMessageModal(title, bodyHtml, footerHtml) {
    let messageModalEl = document.querySelector("#modalDialog");
    if (!messageModalEl) {
        return;
    }

    let messageModal = bootstrap.Modal.getOrCreateInstance(messageModalEl);
    messageModal.show();

    let messageModalTitle = document.querySelector("#modalDialogHeader");
    if (messageModalTitle) {
        messageModalTitle.innerHTML = title;
    }

    let messageModalBody = document.querySelector("#modalDialogBody");
    if (messageModalBody) {
        messageModalBody.innerHTML = bodyHtml;
    }

    let messageModalFooter = document.querySelector("#modalDialogFooter");
    if (messageModalFooter) {
        footerHtml = footerHtml ?? "";
        footerHtml += "<button type=\"button\" class=\"btn btn-secondary\" data-bs-dismiss=\"modal\">关闭</button>";
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

function base64ArrayBuffer(arrayBuffer) {
    var base64 = ''
    var encodings = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'

    var bytes = new Uint8Array(arrayBuffer)
    var byteLength = bytes.byteLength
    var byteRemainder = byteLength % 3
    var mainLength = byteLength - byteRemainder

    var a, b, c, d
    var chunk

    // Main loop deals with bytes in chunks of 3
    for (var i = 0; i < mainLength; i = i + 3) {
        // Combine the three bytes into a single integer
        chunk = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2]

        // Use bitmasks to extract 6-bit segments from the triplet
        a = (chunk & 16515072) >> 18 // 16515072 = (2^6 - 1) << 18
        b = (chunk & 258048) >> 12 // 258048   = (2^6 - 1) << 12
        c = (chunk & 4032) >> 6 // 4032     = (2^6 - 1) << 6
        d = chunk & 63               // 63       = 2^6 - 1

        // Convert the raw binary segments to the appropriate ASCII encoding
        base64 += encodings[a] + encodings[b] + encodings[c] + encodings[d]
    }

    // Deal with the remaining bytes and padding
    if (byteRemainder == 1) {
        chunk = bytes[mainLength]

        a = (chunk & 252) >> 2 // 252 = (2^6 - 1) << 2

        // Set the 4 least significant bits to zero
        b = (chunk & 3) << 4 // 3   = 2^2 - 1

        base64 += encodings[a] + encodings[b] + '=='
    } else if (byteRemainder == 2) {
        chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1]

        a = (chunk & 64512) >> 10 // 64512 = (2^6 - 1) << 10
        b = (chunk & 1008) >> 4 // 1008  = (2^6 - 1) << 4

        // Set the 2 least significant bits to zero
        c = (chunk & 15) << 2 // 15    = 2^4 - 1

        base64 += encodings[a] + encodings[b] + encodings[c] + '='
    }

    return base64
}