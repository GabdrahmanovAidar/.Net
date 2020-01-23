// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
src = "http://www.google.com/jsapi?key=YOURAPIKEY" type = "text/javascript";

google.load("jquery", "1.2.6");
google.load("jqueryui", "1.5.2");
var yourLocation = google.loader.ClientLocation.address.city;

