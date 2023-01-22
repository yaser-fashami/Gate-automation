"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/GateOutHub")
    .build();

connection.start();

console.log(connection);

connection.on("getPlateNumber", function (carPlateNumber) {
    console.log('carPlateNumber: ', carPlateNumber);
    $('#carPlateSection1').val(carPlateNumber.carPlateSection1);
    $('#carPlateSection2').val(carPlateNumber.carPlateSection2);
    $('#carPlateSection3').val(carPlateNumber.carPlateSection3);
    $('#carPlateSection4').val(carPlateNumber.carPlateSection4);
    $('#carPlate').slideDown(1000);
    $('traffic-light').prop('color', 'green');
});

connection.on("containerNumber", function (containerNumber) {
    console.log('containerNumber: ', containerNumber);
    $('#containerNumber').val(containerNumber);
    $('#containerPlate').slideDown(1000);
});

