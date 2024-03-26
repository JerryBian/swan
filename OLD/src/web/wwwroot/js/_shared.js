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

function showModalDialog(title, body) {
    let id = makeId();
    let dialog = `<div class="modal" tabindex="-1" id="${id}">
  <div class="modal-dialog modal-dialog-scrollable modal-dialog-centered modal-fullscreen-md-down">
    <div class="modal-content">
      <div class="modal-header border-bottom">
        <div class="modal-title fs-5">${title}</div>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body post">
        ${body}
      </div>
    </div>
  </div>
</div>`;
    let temp = document.querySelector('#temp');
    temp.innerHTML = dialog;
    const modal = bootstrap.Modal.getOrCreateInstance(document.querySelector(`#${id}`));
    modal.show();
}

function showErrorToast(message) {
    showToast("Error", "text-bg-danger", message);
}
function showInfoToast(message) {
    showToast("Information", "text-bg-success", message);
}

function hideEle(ele) {
    if (!ele.classList.contains("d-none")) {
        ele.classList.add("d-none");
    }
}

function showEle(ele) {
    if (ele.classList.contains("d-none")) {
        ele.classList.remove("d-none");
    }
}

function toggleEle(ele) {
    if (ele.classList.contains("d-none")) {
        ele.classList.remove("d-none");
    } else {
        ele.classList.add("d-none");
    }
}

function toggleLoadingSpinner() {
    let ele = document.querySelector("#loadingSpinner");
    toggleEle(ele);
}

function searchChildren(id, txt, filterFunc, sortAttr) {
    try {
        toggleLoadingSpinner();
        let ele = document.querySelector(id);
        let children = Array.prototype.slice.call(ele.children, 0);
        
        children.forEach(x => {
            if (filterFunc && !filterFunc(x)) {
                hideEle(x);
            } else {
                if (!txt) {
                    showEle(x);
                    return;
                } else {
                    hideEle(x);
                    let text = x.innerText || x.textContent;
                    if (text.search(new RegExp(txt, "i")) >= 0) {
                        showEle(x);
                    }
                } 
            }
        });

        if (sortAttr) {
            children.sort((a, b) => {
                let val1 = a.getAttribute(sortAttr);
                let val2 = b.getAttribute(sortAttr);
                return val2 - val1;
            });

            let html = "";
            ele.innerHTML = "";
            children.forEach(x => html = html + x.outerHTML);
            ele.innerHTML = html;
        }
    } finally {
        toggleLoadingSpinner();
    }
}

function showToast(title, headerClass, message) {
    let id = makeId();
    let toast = `<div class="toast-container position-fixed bottom-0 end-0 p-3 ">
  <div id="${id}" class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="30000">
    <div class="toast-header ${headerClass}">
      <i class="bi bi-bell me-2"></i>
      <strong class="me-auto">${title}</strong>
      <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    <div class="toast-body ">
      ${message}
    </div>
  </div>
</div>`;
    let temp = document.querySelector('#temp');
    temp.innerHTML = toast;
    const toastBootstrap = bootstrap.Toast.getOrCreateInstance(document.querySelector(`#${id}`));
    toastBootstrap.show();
}

function makeId() {
    return "id" + Math.random().toString(16).slice(2);
}