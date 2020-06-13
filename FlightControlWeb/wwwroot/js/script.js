const flightPlanUri = '../api/FlightPlan';
const uri = '../api/Flights?relative_to';
const delUri = '../api/Flights'
let mymap = L.map('mapid').setView([51.505, -0.09], 13);
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
    coords.push([flightPlan.initial_location.longitude, flightPlan.initial_location.latitude]);
    flightPlan.segments.forEach(segment => {
        coords.push([segment.longitude, segment.latitude]);
    });
    alert(coords);
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
    fetch(`${flightPlanUri}/${flightId}`)
        .then(response => response.json())
        .then(flightPlan => {
            currFlightId = flightId;
            showData(flightPlan);
        }
        )
        .catch(error => console.error('Unable to get items.', error));
}

function showData(flightPlan) {
    alert("in show data");
    displayFlightPath(flightPlan);
    displayFlightDetails(flightPlan);
    // more functions...
}

function displayFlightDetails(flightPlan) {
    //deleting what is in filghtsDetaild before new display:
    $("#flightDetailsTable > tbody").children().empty();
    //adding the new row:
    let strRow = '<tr><td>' + currFlightId + '</td>' + '<td>' + flightPlan.company_name + '</td>' +
        '<td>' + flightPlan.initial_location.longitude + '</td>' + '<td>' + flightPlan.initial_location.latitude + '</td>' +
        '<td>' + flightPlan.passengers + '</td>' + '<td>' + flightPlan.initial_location.date_time + '</td>' + '</tr>';
    $("#flightDetailsTable > tbody").append(strRow);
}

let blackIcon = new airplaneIcon({ iconUrl: 'airplane-icon.png' });
let redIcon = new airplaneIcon({ iconUrl: 'redairplane.png' });

//function to insert a new airplane to map
//also adding to map of keys
function addNewPlaneToMap(longitude, latitude, idFlight) {
    let newMarker = L.marker([longitude, latitude], { icon: blackIcon });
    newMarker.addTo(mymap);
    airplanesGroupLayer.addLayer(newMarker);
    newMarker.layerID = "icon_"+idFlight;
    mapAllPlanes.set(idFlight, newMarker);
    //  alert(mapAllPlanes.has(idFlight) + " when addidng");
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

//when clicking on a row: in table EXflights or MYflights
//(rowFlights is the id of tose rows)
$(document).on("click", 'tr.rowFlight', function () {
    alert("in click row");
    //highlighting the cliccekd flight
    $(this).addClass('text-info').siblings().removeClass('text-info');
    $(this).addClass('selected').siblings().removeClass('selected');
    //displaying all is needed wahen clicking the row:
    let idCut = this.id.slice(3);
    getFlightById(idCut);
    //changing icon
    alert("mapAllPlanes has this.id " + mapAllPlanes.has(idCut));
    mapAllPlanes.get(idCut).setIcon(redIcon);
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
function addOrSet(id, lng, lat) {
    if (mapAllPlanes.has(id)) {
        //   alert("filght: " + id + "already exist");
        mapAllPlanes.get(id).setLatLng([lat, lng]);
    } else {//create a new planeIcon
        //  alert("filght: " + id + "DOES NOT exist");
        addNewPlaneToMap(lng, lat, id);
    }
}

function checkIfFlightFinished(flights) {
    let exist = false;
    //if there are no flights from server at all:
    if ((flights == "") && (mapAllPlanes.size != 0)) {
        //we want to delete all the plains we hold in our mapKEYS:
        for (let layerKey of mapAllPlanes.keys()) {
            airplanesGroupLayer.remove(mapAllPlanes.get(layerKey));
            mymap.removeLayer(mapAllPlanes.get(layerKey));
            mapAllPlanes.delete(layerKey);
        }
    }
    if ($.isArray(flights)) {//if it is an array
        for (let layerKey of mapAllPlanes.keys()) {//iterate over dictionary map
            flights.forEach(flight => {//iterate over flights
                if (layerKey == flight.flight_id) {
                    //alert("inside brackets " + flight.flight_id);
                    exist = true;
                }
            });

            //if flight does not exist, need to remove the layer, and remove from mapOfAllPlanes
            if (!exist) {
                //  alert(layerKey + "NOT EXIST when recieved info from server");
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
                mapAllPlanes.delete(layerKey);
            }
            //reset 
            exist = false;
        }
    }
}
//for checking:
//printing what map contains:
function printingMapAllPlanes() {
    //alert("map size before printing:" + mapAllPlanes.size);
    for (let layerKey of mapAllPlanes.keys()) {
        //    alert("flight: " + layerKey + "value: " + mapAllPlanes.get(layerKey) + "in mapAllPlanes");
    }
    for (let val of mapAllPlanes.values()) {
        // alert("value is: " + val);
    }
}

function resetDisplay() {
    //check if there is a flight in the flight details
    if (currFlightId != "") {
        //reset Icon
        mapAllPlanes.get(currFlightId).setIcon(blackIcon);
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
    currFlightId = 0;
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
    fetch(`${uri}=${time}`, {
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => {
            displayData(data);
            parseFlightsDataForMap(data);
        })
        .catch((error) => {
            console.error('Error:', error)
        });
}

// This function sends a get request to the server in order to
// get an update regarding the current active flights
function getAllActiveFlights() {
    let time = toUtc();
    fetch(`${uri}=${time}&sync_all`, {
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
        })
        .catch((error) => {
            console.error('Error:', error)
        });
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
            }
        } else {
            if (data.is_external) {
                strRow = '<tr class="rowFlight" id= tr_' + data.flight_id + ' ><td>' + data.flight_id + '</td>' + '<td>' + data.company_name + '</td>' + '<td>' + data.data_time + '</td><td><button class="del" id=' + data.flight_id + '>X</button></td></tr>';
                rowJq = $(strRow);
                $("#exFlightsTable > tbody").append(strRow);

            } else {
                strRow = '<tr class="rowFlight"  id= tr_' + data.flight_id + '><td>' + data.flight_id + '</td>' + '<td>' + data.company_name + '</td>' + '<td>' + data.data_time + '</td><td><button class="del" id=' + data.flight_id + '>X</button></td></tr>'
                rowJq = $(strRow);
                $("#myFlightsTable > tbody").append(strRow);
            }
            if (data.flight_id == currFlightId) {
                selectedRow = document.getElementById("tr_" + currFlightId);
                //highlighting the cliccekd flight
                $(selectedRow).addClass('text-info').siblings().removeClass('text-info');
                $(selectedRow).addClass('selected').siblings().removeClass('selected');
            }
        }
}

//working:MYFLIGHTS
//FUNC:DELETE
$(document).on("click", 'button.del', function () {
    alert("in click button");
    deleteFlightFromServer(this.id);
    // alert(this.id);
    $(this).closest('tr').remove();
});

function deleteFlightFromServer(id) {
    fetch(`${delUri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    }).then(response => response.json())
        .catch((error) => {
            console.error('Error:', error)
        });
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


