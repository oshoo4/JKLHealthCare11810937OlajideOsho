document.getElementById("StartDate").addEventListener("change", function () {
    var endDateInput = document.getElementById("EndDateInput");
    endDateInput.disabled = false;
    endDateInput.min = this.value;
});