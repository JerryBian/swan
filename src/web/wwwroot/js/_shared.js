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
        var ele = document.querySelector(id);
        if (!ele) return;
        var children = Array.prototype.slice.call(ele.children, 0);
        var searchTerm = (txt || '').toLowerCase();

        children.forEach(function (x) {
            if (filterFunc && !filterFunc(x)) {
                hideEle(x);
            } else if (!searchTerm) {
                showEle(x);
            } else {
                var text = (x.textContent || '').toLowerCase();
                if (text.indexOf(searchTerm) >= 0) {
                    showEle(x);
                } else {
                    hideEle(x);
                }
            }
        });

        if (sortAttr) {
            children.sort(function (a, b) {
                var val1 = parseFloat(a.getAttribute(sortAttr)) || 0;
                var val2 = parseFloat(b.getAttribute(sortAttr)) || 0;
                return val2 - val1;
            });
            // Reorder DOM without destroying elements
            var ref = ele.firstChild;
            children.forEach(function (child) {
                if (child.parentNode === ele) {
                    ele.insertBefore(child, ref);
                }
            });
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
