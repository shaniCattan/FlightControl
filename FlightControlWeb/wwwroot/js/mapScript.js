const flightPlanUri = '../api/FlightPlan';
let mymap = L.map('mapid').setView([51.505, -0.09], 13);
let airplanesGroupLayer = L.layerGroup([]);
//map by key to keep track of all the planes
let mapAllPlanes = new Map();


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
    return coords;
}

function displayFlightPath(flightPlan) {
    let coords = createCoords(flightPlan);
    // create a red polyline from an array of LatLng points
    let polyline = L.polyline(coords, { color: 'red' }).addTo(mymap);
    // zoom the map to the polyline
    mymap.fitBounds(polyline.getBounds());
}

function getFlightById(flightId) {
   fetch(`${flightPlanUri}/${flightId}`)
  .then(response => response.json())
  .then(flightPlan => showData(flightPlan))
  .catch(error => console.error('Unable to get items.', error));
}

function showData(flightPlan) {
    displayFlightPath(flightPlan);
    displayFlightDetails(flightPlan);
    // more functions...
}

function displayFlightDetails(flightPlan,id) {
    $("#flightDetailsTable > tbody").append('<tr><td>' + id + '</td>' + '<td>' + flightPlan.company_name + '</td>' + '<td>' + flightPlan.passengers + '</td>' + '<td>' + flightPlan.initial_location.date_time + '</td>' + '</td></tr>');
}

let blackIcon = new airplaneIcon({ iconUrl: 'airplane-icon.png' });
let redIcon = new airplaneIcon({ iconUrl: 'redairplane.png' });

//function to insert a new airplane to map
function addNewPlaneToMap(latitude, longitude, idFlight) {
    let newMarker = L.marker([latitude, longitude], { icon: blackIcon });
    airplanesGroupLayer.addLayer(newMarker);
    newMarker.layerID = idFlight;
    newMarker.on('click', changeIconOnClick).addTo(mymap);
   // mapAllPlanes.set(idFlight, newMarker);
  //  alert("map size after adding:" + idFlight + "is: " + mapAllPlanes.size);
}

function changeIconOnClick(e) {
    e.target.setIcon(redIcon);
}