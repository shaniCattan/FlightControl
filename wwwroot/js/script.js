const flightPlanUri = '../api/FlightPlan';
const uri = '../api/Flights?relative_to';
const delUri = '../api/Flights'
let mymap = L.map('mapid').setView([31.772790, 35.218874], 3);
let airplanesGroupLayer = L.layerGroup([]);
//map by key to keep track of all the planes
let mapAllPlanes = new Map();
let currFlightId = "";
let polyline;

L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
    maxZoom: 18,
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, ' +
        '<a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, ' +
        'Imagery ©️ <a href="https://www.mapbox.com/">Mapbox</a>',
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1
}).addTo(mymap);

let airplaneIcon = L.Icon.extend({
    options: {
        iconSize: [40, 95],
    }
});

// the following part is for displying the flight path on the map!!!
//and display the floght details


// This function iterates over data (flight object or an array of flights objects that the server sent) and parse it.
function createCoords(flightPlan) {
    let coords = [];
    coords.push([flightPlan.initial_location.latitude, flightPlan.initial_location.longitude]);
    flightPlan.segments.forEach(segment => {
        coords.push([segment.latitude, segment.longitude]);
    });
    return coords;
}

function displayFlightPath(flightPlan) {
    let coords = createCoords(flightPlan);
    // create a red polyline from an array of LatLng points
    polyline = L.polyline(coords, { color: 'red' });
    polyline.addTo(mymap);
    // zoom the map to the polyline
    mymap.fitBounds(polyline.getBounds());
}

function getFlightById(flightId) {
    // flight id after cutting the "tr_" at the beginning: 
    (async () => {
        let rowResponse = await fetch(`${flightPlanUri}/${flightId}`)
            .then(response => response.json())
            .then(flightPlan => {
                currFlightId = flightId;
                showData(flightPlan);
            }
            );
        if (rowResponse.status != 200) {
            errorHandle(rowResponse.status, "There is a problem in getting flight by id from the server");
        }
    })();

}


function showData(flightPlan) {
    displayFlightPath(flightPlan);
    displayFlightDetails(flightPlan);
    // more functions...
}

function displayFlightDetails(flightPlan) {
    //deleting what is in filghtsDetaild before new display:
    $("#flightDetailsTable > tbody").children().empty();
    //adding the new row:
    let strRow = '<tr><td>' + currFlightId + '</td>' + '<td>' + flightPlan.company_name + '</td>' +
        '<td>' + flightPlan.initial_location.date_time + '</td>' + '<td>' + flightPlan.initial_location.latitude + "," + flightPlan.initial_location.longitude+ '</td>' +
        '<td>' + flightPlan.passengers + '</td>' + '<td id="location">' + '</td>' + '<td id="time">' + '</td>' + '</tr>';
    $("#flightDetailsTable > tbody").append(strRow);
}

let blueIcon = new airplaneIcon({ iconUrl: 'blueplane.png' });
let redIcon = new airplaneIcon({ iconUrl: 'redplane.png' });

//function to insert a new airplane to map
//also adding to map of keys
function addNewPlaneToMap(latitude, longitude, idFlight) {
    let newMarker = L.marker([latitude, longitude], { icon: blueIcon });
    newMarker.addTo(mymap);
    airplanesGroupLayer.addLayer(newMarker);
    newMarker.layerID = "icon_"+idFlight;
    mapAllPlanes.set(idFlight, newMarker);
    newMarker.on('click', changeIconOnClick).addTo(mymap);
}
//changes the icon when it is being clicked
function changeIconOnClick(e) {
    e.target.setIcon(redIcon);
    getFlightById(this.layerID.split("icon_").join(""));
    //highlighting the cliccekd flight
    $(document.getElementById("tr_" + this.layerID)).addClass('text-info').siblings().removeClass('text-info');
    $(document.getElementById("tr_" + this.layerID)).addClass('selected').siblings().removeClass('selected');
}

$(document).on("click", 'button.del', function (e) {
    deleteFlightFromServer(this.id);
    if ((currFlightId==this.id)) {
        ResetDisplayWhenDeleteButtonClicked();
        currFlightId = "";
    }
    $(this).closest('tr').remove();
    e.stopPropagation();
});

//recieiving a json of all flights and iterating over them,
//adding or setting as needed
function parseFlightsDataForMap(flights) {
    //first we delete unnecessary flights from both maps
    checkIfFlightFinished(flights);
    if (flights != "") {
        //then adding/setting the flights needed
        if ($.isArray(flights)) {//if it is an array
            for (let i = 0; i < flights.length; i++) {
                addOrSet(flights[i].flight_id, flights[i].latitude, flights[i].longitude);
            }
        } else {//if it is a single flight
            addOrSet(flights.flight_id, flights.latitude, flights.longitude);
        }
    }

}
//checks a current flight if need to add or just set it
//then adds or sets regardingly
function addOrSet(id, lat, lng) {
    if (mapAllPlanes.has(id)) {
        mapAllPlanes.get(id).setLatLng([lat, lng]);
    } else {//create a new planeIcon
        addNewPlaneToMap(lat, lng, id);
    }
}

function checkIfFlightFinished(flights) {
    let exist = false;
    if ($.isArray(flights)) {//if it is an array
        for (let layerKey of mapAllPlanes.keys()) {//iterate over dictionary map
            flights.forEach(flight => {//iterate over flights
                if (layerKey == flight.flight_id) {
                    exist = true;
                }
            });

            //if flight does not exist, need to remove the layer, and remove from mapOfAllPlanes
            if (!exist) {
                airplanesGroupLayer.remove(mapAllPlanes.get(layerKey));
                mymap.removeLayer(mapAllPlanes.get(layerKey));
                mapAllPlanes.delete(layerKey);
            }
            //reset 
            exist = false;
        }
    } else {//if it is a single flight
        for (let layerKey of mapAllPlanes.keys()) {//iterate over dictionary map
            if (layerKey == flights.flilght_id) {
                exist = true;
            }
            //if flight does not exist, need to remove the layer, and remove from mapOfAllPlanes
            if (!exist) {
                airplanesGroupLayer.remove(mapAllPlanes.get(layerKey));
                mymap.removeLayer(mapAllPlanes.get(layerKey));
                mapAllPlanes.delete(layerKey);
            }
            //reset 
            exist = false;
        }
    }
}

function ResetDisplayWhenDeleteButtonClicked() {
    //reset Flight Details table
    $("#flightDetailsTable tbody tr").remove();
    //clear flight path from map
    mymap.removeLayer(polyline);
}

function resetDisplayWhenMapClicked() {
    //check if there is a flight in the flight details
    if (currFlightId != "") {
        //reset Icon
        mapAllPlanes.get(currFlightId).setIcon(blueIcon);
        //reset Flight Details table
        $("#flightDetailsTable tbody tr").remove();
        //clear flight path from map
        mymap.removeLayer(polyline);
        //un-highlight flight row
        $(document).ready(function () {
            $('.flightDetailsTable tbody tr').click(function () {
                $(document.getElementById("tr_" + currFlightId)).css('background', 'none');
            })
        })
    }
    currFlightId = "";
}


$(document).ready(function () {
    getAllActiveFlights();
    setInterval(function () {
        getAllActiveFlights();
    }, 3000);
});
// This function sends a get request to the server in order to
// get an update regarding the current active flights
function getMyActiveFlights() {
    let time = toUtc();
    (async () => {
        let rowResponse = fetch(`${uri}=${time}`, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data != "") {
                    displayData(data);
                    parseFlightsDataForMap(data);
                }
            });
        if (rowResponse.status != 200) {
            errorHandle(rowResponse.status, "There is a broblem to get internal active flights from server");
        }
    })();
}

function getAllActiveFlights() {
    let time = toUtc();
    (async () => {
        let rowResponse = fetch(`${uri}=${time}&sync_all`, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data != "") {
                    displayData(data);
                    parseFlightsDataForMap(data);
                } else {
                    deleteAllIconsFromMap();
                }
            });
        if ((rowResponse.status != 200) && (rowResponse.status != undefined)) {
            errorHandle(rowResponse.status, "There is a problem to get all active flights from server");
        }
    })();
}

//This function converts current time to utc time in format ssZ:mm:ddTHH-MM-yyyy 
function toUtc() {
    let time = (new Date().toISOString().split('.')[0] + "Z");
    return time;
}


// This function iterates over data (flight object or an array of flights objects that 
// the server sent) and parse it.
function displayData(data) {
    //reset the table before desplayin the new data
    $("#myFlightsTable tbody tr").remove();
    $("#exFlightsTable tbody tr").remove();

    //the string that creates the line in the HTML
    let strRow;
    let rowJq;
    if ($.isArray(data)) {
        for (let i = 0; i < data.length; i++) {
            if (data[i].is_external) {
                strRow = '<tr class="rowFlight" id= tr_' + data[i].flight_id + '><td>' + data[i].flight_id + '</td>' + '<td>' + data[i].company_name + '</td>' + '<td>' + data[i].date_time + '</td>' + '<td><button class="del" id=' + data[i].flight_id + '>X</button></td></tr>'
                rowJq = $(strRow);

                $("#exFlightsTable > tbody").append(strRow);

            } else {
                strRow = '<tr class="rowFlight"  id= tr_' + data[i].flight_id + '><td>' + data[i].flight_id + '</td>' + '<td>' + data[i].company_name + '</td>' + '<td>' + data[i].date_time + '</td>' + '<td><button class="del" id=' + data[i].flight_id + '>X</button></td></tr>';
                rowJq = $(strRow);

                $("#myFlightsTable > tbody").append(strRow);

            }
            if (data[i].flight_id == currFlightId) {
                selectedRow = document.getElementById("tr_" + currFlightId);
                //highlighting the cliccekd flight
                $(selectedRow).addClass('text-info').siblings().removeClass('text-info');
                $(selectedRow).addClass('selected').siblings().removeClass('selected');
            }
            if (currFlightId != "") {
                document.getElementById("time").innerHTML = data[i].date_time;
                document.getElementById("location").innerHTML = data[i].latitude +","+data[i].longitude;
            }
        }
    } else {
        if (data.is_external) {
            strRow = '<tr class="rowFlight" id= tr_' + data.flight_id + '><td>' + data.flight_id + '</td>' + '<td>' + data.company_name + '</td>' + '<td>' + data.date_time + '</td>' + '<td><button class="del" id=' + data.flight_id + '>X</button></td></tr>'
            rowJq = $(strRow);

            $("#exFlightsTable > tbody").append(strRow);

        } else {
            strRow = '<tr class="rowFlight"  id= tr_' + data.flight_id + '><td>' + data.flight_id + '</td>' + '<td>' + data.company_name + '</td>' + '<td>' + data.date_time + '</td>' + '<td><button class="del" id=' + data.flight_id + '>X</button></td></tr>';
            rowJq = $(strRow);

            $("#myFlightsTable > tbody").append(strRow);

        }
        if (data.flight_id == currFlightId) {
            selectedRow = document.getElementById("tr_" + currFlightId);
            //highlighting the cliccekd flight
            $(selectedRow).addClass('text-info').siblings().removeClass('text-info');
            $(selectedRow).addClass('selected').siblings().removeClass('selected');
        }
        if (currFlightId != "") {
            document.getElementById("time").innerHTML = data.date_time;
            document.getElementById("location").innerHTML = data.latitude + "," + data.longitude;
        }
    }
}
//when clicking on a row: in table EXflights or MYflights
//(rowFlights is the id of tose rows)
$(document).on("click", 'tr.rowFlight', function () {
    if (currFlightId != this.id) {
        resetDisplayWhenMapClicked();
    }
    //highlighting the cliccekd flight
    $(this).addClass('text-info').siblings().removeClass('text-info');
    $(this).addClass('selected').siblings().removeClass('selected');
    //displaying all is needed wahen clicking the row:
    let idCut = this.id.slice(3);
    getFlightById(idCut);
    //changing icon
    mapAllPlanes.get(idCut).setIcon(redIcon);
});

function deleteFlightFromServer(id) {
    (async () => {
        let rowResponse = fetch(`${delUri}/${id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
        }).then(response => response.json());
        if (rowResponse.status != 200) {
            errorHandle(rowResponse.status, "There is a problem to delete an active flight from server");
        }
    })();
    }

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
            const rowResponse = await fetch("../api/flightplan", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(flightplan)
            });
            if (rowResponse.status != 200) {
                errorHandle(rowResponse.status, "There is a problem to post a new flight plan");
            }
        })();
    }

    async function errorHandle(errStatus, errData) {
        $("#errMsg").text("Status error:" + errStatus + "," + errData);
        $("#error").show().delay(3000).fadeOut();
    }

    //if there are no flights from server at all:
    //we want to delete all the plains we hold in our mapKEYS:
    function deleteAllIconsFromMap() {
        for (let layerKey of mapAllPlanes.keys()) {
            airplanesGroupLayer.remove(mapAllPlanes.get(layerKey));
            mymap.removeLayer(mapAllPlanes.get(layerKey));
            mapAllPlanes.delete(layerKey);
        }
    }
