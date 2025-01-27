document.addEventListener("DOMContentLoaded", function () {
    const universalModal = document.getElementById("universalModal");
    const modalTitle = document.getElementById("universalModalLabel");
    const modalBody = document.getElementById("universalModalBody");
    const confirmButton = document.getElementById("universalModalConfirmButton");

    if (universalModal) {
        universalModal.addEventListener("show.bs.modal", function (event) {
            const button = event.relatedTarget;

            const title = button.getAttribute("data-modal-title");
            const body = button.getAttribute("data-modal-body");
            const confirmAction = button.getAttribute("data-modal-action");

            modalTitle.textContent = title || "Default Title";
            modalBody.textContent = body || "Default Message";

            confirmButton.replaceWith(confirmButton.cloneNode(true));
            const newConfirmButton = document.getElementById("universalModalConfirmButton");

            newConfirmButton.addEventListener("click", function () {
                if (confirmAction) {
                    window[confirmAction]();
                }
                const bootstrapModal = bootstrap.Modal.getInstance(universalModal);
                bootstrapModal.hide();
            });
        });
    }
});

function deleteItem(itemId) {
    alert("Deleting item with ID: " + itemId);
}

function sellItem(itemId) {
    alert("Selling item with ID: " + itemId);
}