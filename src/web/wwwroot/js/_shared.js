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

function showErrorToast(message) {
    showToast("Error", "text-bg-danger", message);
}
function showInfoToast(message) {
    showToast("Information", "text-bg-success", message);
}

function showToast(title, headerClass, message) {
    let id = makeId();
    let toast = `<div class="toast-container position-fixed bottom-0 end-0 p-3 ">
  <div id="${id}" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
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