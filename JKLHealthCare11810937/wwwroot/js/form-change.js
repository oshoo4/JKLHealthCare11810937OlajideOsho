document.addEventListener('DOMContentLoaded', function() {
    var currentPath = window.location.pathname; 

    if (currentPath === "/Patients/Create") {
        document.querySelector('form[action="/Patients/Create"]').addEventListener('submit', setPatientDefaultValues);
    } 
    else if (currentPath.startsWith("/Patients/Edit/")) { 
        document.querySelector('form[action="/Patients/Edit"]').addEventListener('submit', setPatientDefaultValues);
    } 
    else if (currentPath === "/Caregivers/Create") {
        document.querySelector('form[action="/Caregivers/Create"]').addEventListener('submit', setCaregiverDefaultValues);
    } 
    else if (currentPath.startsWith("/Caregivers/Edit/")) {
        document.querySelector('form[action="/Caregivers/Edit"]').addEventListener('submit', setCaregiverDefaultValues);
    }
});

function setCaregiverDefaultValues(event) {
    var contact = document.getElementById("Contact").value;
    var qualifications = document.getElementById("Qualifications").value;
    var availability = document.getElementById("Availability").value;

    if (qualifications === null || qualifications.trim() === "") {
        document.getElementById("Qualifications").value = "N/A"; 
    }
    if (availability === null || availability.trim() === "") {
        document.getElementById("Availability").selectedIndex = 0;
    }
}

function setPatientDefaultValues(event) {
    var address = document.getElementById("Address").value;
    var medicalRecords = document.getElementById("MedicalRecords").value;

    if (address === null || address.trim() === "") {
        document.getElementById("Address").value = "N/A";
    }
    if (medicalRecords === null || medicalRecords.trim() === "") {
        document.getElementById("MedicalRecords").value = "N/A";
    }
}