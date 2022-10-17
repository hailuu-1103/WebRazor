"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/hubs").build();
connection.on("ReloadProduct", (data) => {
    var tr = '';
    $.each(data, (k, v) => {
        tr +=
            `
            <tr>
                <td>${v.ProductId}</td>
                <td>${v.ProductName}</td>
                <td>${v.UnitPrice}</td>
                <td>${v.QuantityPerUnit}</td>
                <td>${v.UnitsInStock}</td>
                <td>12</td>
                <td>${v.Discontinued}</td>
            </tr>
            `
    })
    $("#tableBody").html(tr);
});

connection.start().then().catch(function (err) {
    return console.log(err.toString());
});
