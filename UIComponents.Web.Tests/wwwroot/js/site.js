// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function makeToast(type, message, title) {
    alert(title +"\r\n"+message);
}

// #region Setup

var EventManager = {
    one: function (event, fn) {
        $(this).one(event, fn);
    },
    subscribe: function (event, fn) {
        $(this).on(event, fn);
    },
    unsubscribe: function (event, fn) {
        $(this).off(event, fn);
    },
    publish: function (event) {
        let arr = Array.from(arguments);

        // Remove the 'event' name from the arguments
        arr.splice(0, 1);

        $(this).trigger(event, arr);
    }
}

window.connected = false;

$(document).ready(async function () {
    "use strict";


    // SignalR
    window.connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Warning)
        .withAutomaticReconnect()
        .withUrl('/mainHub')
        .build();

    connection.onclose(function () {
        window.connected = false;
        EventManager.publish('disconnected', connection);
    })

    connection.onreconnecting(function () {
        window.connected = false;
        EventManager.publish('connecting', connection);
    });

    connection.onreconnected(function () {
        window.connected = true;
        EventManager.publish('connected', connection);
    });

    await connection.start().then(function () {
        window.connected = true;
        EventManager.publish('connected', connection);
    }).catch(function (err) {
        window.connected = false;
        return console.error(err.toString());
    });

    /* Prevent browser tab from sleeping
     * https://docs.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-6.0#bsleep
     */
    var lockResolver;
    if (navigator && navigator.locks && navigator.locks.request) {
        const promise = new Promise((res) => {
            lockResolver = res;
        });

        navigator.locks.request('unique-lock-name', { mode: 'shared' }, () => {
            return promise;
        });
    }


});
