const fileInput = document.getElementById("fileUpload");
const thumbnail = document.getElementById("thumbnail");
const removeButton = document.getElementById("removeButton");
const fileLabel = document.querySelector("label[for='fileUpload']");

const defaultLabelText = "Select File";

fileInput.addEventListener("change", function (event) {
    const file = event.target.files[0];

    if (file && file.type.startsWith("image/")) {
        fileLabel.textContent = file.name;

        const reader = new FileReader();
        reader.onload = function (e) {
            thumbnail.src = e.target.result;
            thumbnail.style.display = "block";
            removeButton.style.display = "block";
        };
        reader.readAsDataURL(file);
    } else {
        clearThumbnail();
    }
});

removeButton.addEventListener("click", function () {
    clearThumbnail();
    fileInput.value = "";
    fileLabel.textContent = defaultLabelText;
});

function clearThumbnail() {
    thumbnail.style.display = "none";
    thumbnail.src = "#";
    removeButton.style.display = "none";
    fileLabel.textContent = defaultLabelText;
}

const showExpirationDateCheckbox = document.getElementById("ShowExpirationDateCheckbox");
const expirationDateContainer = document.getElementById("expirationDateContainer");

showExpirationDateCheckbox.addEventListener("change", function () {
    expirationDateContainer.style.display = showExpirationDateCheckbox.checked ? "block" : "none";
});