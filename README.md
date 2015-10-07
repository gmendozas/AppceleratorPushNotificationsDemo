# AppceleratorPushNotificationsDemo
Appcelerator PushNotifications C# Client Demo

Call using http://{you_host}/api/AppceleratorApiRest/{your_channel}

Body Header Content-Type: application/json.

Request Body to specific devices:

{
	"Payload" : {
		"alert" : "Soy una alerta.",
		"badge" : '1',
		"icon" : "appicon",
		"title" : "Información",
		"vibrate" : true
	},
	"Location" : {},
	Devices : [
		"APA91bEE58AoKQaQhUKQsGwlPg9_ai680FeE1VAeByqETXO0zwVxv3TYKnPOEJD66im84vs7G25vhXcc6kGllhNPZc9z-_z6RR8KfmjiXs8yAUaYpbjr84Q_zXcgWgU1f2Ucp4gssAOIsKe0sgo9qhgvl5WRcnlRng",
		"APA91bGCTi0KA5S4r8eld9tvnqGkNTiOeSYce4p7_ZAY2kTtfNDkxjX10ZZiuoWeYuQgw8lifU3MHJ9D8lDyBDfd1uajy7s5vBhew5XRfE3psSQIwc8BFXzXMljSiDGhb7cDAtJhSgpqRaEN9SyLeogPKOcv_a5vug"
	]
}

Request Body to all devices:

Request Body to specific devices:

{
	"Payload" : {
		"alert" : "Soy una alerta.",
		"badge" : '1',
		"icon" : "appicon",
		"title" : "Información",
		"vibrate" : true
	},
	"Location" : {},
	Devices : []
}

Location is not supported yet.



