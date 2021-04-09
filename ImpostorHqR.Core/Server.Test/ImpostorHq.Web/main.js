var handle = "%handle%";// replace
var connection = null;
var connectionState = 0;
var url  = "ws://" + document.location.hostname + ":%port%";// replace

%var%

%ctx%

function Authenticate() 
{
    document.getElementById("titleh1").innerHTML = "ImpostorMin - Hang on, connecting to " + titleInitial;
    connection = new WebSocket(url);
	connection.onopen = function(evt) 
	{
		// authenticate insecurely
		var request = 
		{
			Handle: handle,
			Secure: false
		};

		var baseMessage = 
		{
			Stage: 0,
			Data: JSON.stringify(request)
		};

		connection.send(JSON.stringify(baseMessage));
		connection.onmessage = function(evt) 
		{
			Received(evt.data);
		};
	}
}

function Received(str) 
{
	if (connectionState === 0) 
	{
		if (str === "Welcome") 
		{
			console.log("authenticated !");
            document.getElementById("titleh1").innerHTML = "ImpostorMin " + "<b style=\"color: #41BB3F; \"> | " + titleInitial + " |</b>";
            connectionState = 2;
			Authenticated();
			Plot();
		} 
		else if (str === "HNF") 
		{
			console.log("handle not found.");
			// fail connection
			// how?
			// user is trying to probe the server. Display a message to inform them about that.
		}
	}
	
	else if (connectionState === 1) 
	{
		// not possible
	}
	
	else if (connectionState === 2) 
	{
		Process(str);
	}
}

function process(str){
	var receivedJson = JSON.parse(message);
	%receivers%
}

function Authenticated() {
	// show message
}

window.onload = onload();

function onload() {
	Authenticate();
}

function Plot(){
	%plot%
}
