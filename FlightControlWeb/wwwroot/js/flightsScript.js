const uri = '../api/Flights?relative_to';
const delUri = '../api/servers'
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
        .then(data => displayData(data))
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
        .then(data => displayData(data)) 
        .catch((error) => {
            console.error('Error:', error)
        });
}

//This function converts current time to utc time in format ssZ:mm:ddTHH-MM-yyyy 
function toUtc() {
    let time = (new Date().toISOString().split('.')[0] + "Z");
    return time;
}

// This function iterates over data (flight object or an array of flights objects that the server sent) and parse it.
function displayData(data) {
    //reset the table before desplayin the new data
    $("#flightsTable tbody tr").remove();
    $("#exFlightsTable tbody tr").remove();

    if (data == "") {
        return;
    }
    if ($.isArray(data)) {
        for (let i = 0; i < data.length; i++) {
            if (data[i].is_external) {
                $("#exFlightsTable > tbody").append('<tr><td>' + data[i].flight_id + '</td>' + '<td>' + data[i].company_name + '</td>' + '<td>' + data[i].date_time + '</td>' + '<td><button class="del" id=' + data[i].flight_id + '>X</button></td></tr>');
            } else {
                $("#flightsTable > tbody").append('<tr><td>' + data[i].flight_id + '</td>' + '<td>' + data[i].company_name + '</td>' + '<td>' + data[i].date_time + '</td>' + '<td><button class="del" id=' + data[i].flight_id + '>X</button></td></tr>');
            }
        }
    } else {
        if (data.is_external) {
            $("#exFlightsTable > tbody").append('<tr><td>' + data.flight_id + '</td>' + '<td>' + data.company_name + '</td>' + '<td>' + data.date_time + '</td><td><button class="del" id=' + data.flight_id + '>X</button></td></tr>');
        } else {
            $("#flightsTable > tbody").append('<tr><td>' + data.flight_id + '</td>' + '<td>' + data.company_name + '</td>' + '<td>' + data.date_time + '</td><td><button class="del" id=' + data.flight_id + '>X</button></td></tr>');
        }
    }    
}

//working:MYFLIGHTS
//FUNC:DELETE
$(document).on("click", 'button.del', function () {
    deleteFlightFromServer(this);
    alert(this.id);
    $(this).closest('tr').remove();
});

function deleteFlightFromServer(id) {
    fetch(`${Deluri}/${id}`, {
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
