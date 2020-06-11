const input = document.getElementById("fileInput");
const customButton = document.getElementById("customButton");
customButton.addEventListener('click', function () {
    input.click();
});

const allowedExtension = /(\.json)$/i;

function onChange(event) {
    let file = event.target.files[0];
    let filePath = file.name;
    if (!allowedExtension.exec(filePath)) {
        $("#fileInput").val('');
        return false;
    }
    const reader = new FileReader();
    reader.onload = () => {
        const obj = JSON.parse(reader.result);
        postflightplan(obj);
    }
    $("#fileInput").val('');
    reader.onerror = error => reject(error);
    reader.readAsText(file);
}

function postflightplan(flightplan) {
    (async () => {
        const rawResponse = await fetch("../api/flightplan", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(flightplan)
        });
        if (rawResponse.status != 200) {
            errorHandle(rawResponse.status, "Invalid Flight Plan Details");
        }
    })();
}

async function errorHandle(errStatus, errData) {
    $("#errMsg").text("Status error:" + errStatus + "," + errData);
    $("#error").show().delay(2000).fadeOut();
}