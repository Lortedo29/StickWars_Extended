const BUTTONS_COUNT = 5;

var airconsole;
var touchedElement = new Map();
var activeFunctionMap = new Map();
var disableFunctionMap = new Map();

// #region AIR CONSOLE STARTUP
function init() {
    airconsole = new AirConsole({
        "orientation": "landscape"
    });

    ViewManager.init();
    ViewManager.show("Load");

    airconsole.onMessage = function (from, data) {
        if (from == AirConsole.SCREEN) {
            if (data.view) {
                ViewManager.show(data.view);
            }

            if (data.bgColor) {
                document.body.style.backgroundColor = data.bgColor;
            }
        }
    };

    addButton("button_left", true,
        function () {
            horizontal(-1)
        },
        function () {
            horizontal(0)
        });
    addButton("button_right", true,
        function () {
            horizontal(1)
        },
        function () {
            horizontal(0)
        });
    addButton("button_b", false,
        function () {
            bPressed(true)
        },
        function () {
            bPressed(false)
        });
    addButton("button_x", false,
        function () {
            xPressed(true)
        },
        function () {
            xPressed(false)
        });
    addButton("button_a", false,
        function () {
            aPressed(true)
        },
        function () {
            aPressed(false)
        });
}

// #endregion AIR CONSOLE STARTUP

// #region TOUCH LISTENER
document.addEventListener('touchstart', function (event) {
    event.preventDefault();

    var touches = event.touches;

    for (var i = 0; i < touches.length; i++) {
        var touch = touches[i];
        var identifier = touch.identifier;

        touchedElement.set(identifier, document.elementFromPoint(touch.pageX, touch.pageY));

        var touchedElementId = touchedElement.get(identifier).id;

        if (touchedElementId != undefined && activeFunctionMap.get(touchedElementId) != undefined) {
            activeFunctionMap.get(touchedElementId)();
        }
    }
}, false);

document.addEventListener('touchmove', function (event) {
    event.preventDefault();

    var touches = event.touches;

    for (var i = 0; i < touches.length; i++) {

        var touch = touches[i];
        var identifier = touch.identifier;
        var newTouchedElement = document.elementFromPoint(touch.pageX, touch.pageY);

        if (touchedElement.get(identifier) !== newTouchedElement) {
            var touchedElementId = touchedElement.get(identifier).id;

            if (touchedElementId != undefined && disableFunctionMap.get(touchedElementId) != undefined) {
                disableFunctionMap.get(touchedElementId)();
            }

            touchedElement.set(identifier, newTouchedElement);
            touchedElementId = touchedElement.get(identifier).id;

            if (touchedElementId != undefined && activeFunctionMap.get(touchedElementId) != undefined) {
                activeFunctionMap.get(touchedElementId)();
            }
        }
    }
}, false);

document.addEventListener("touchend", function (event) {

    var touches = event.changedTouches;

    for (var i = 0; i < touches.length; i++) {
        var identifier = touches[i].identifier;

        var touchedElementId = touchedElement.get(identifier).id;

        if (touchedElementId != undefined && disableFunctionMap.get(touchedElementId) != undefined) {
            disableFunctionMap.get(touchedElementId)();
        }

        touchedElement.set(identifier, null);
    }
});


function addButton(id, isDirectional, activeFunction, disableFunction) {

    var obj = document.getElementById(id);

    if (isDirectional == true) {
        activeFunctionMap.set(id, activeFunction);
        disableFunctionMap.set(id, disableFunction);
    } else {
        obj.addEventListener("touchstart", activeFunction);
        obj.addEventListener("touchend", disableFunction);
    }

    obj.addEventListener("mousedown", activeFunction);
    obj.addEventListener("mouseup", disableFunction);
}
// #endregion TOUCH LISTENER

// #region MESSAGE METHODS
function horizontal(amount) {
    console.log("horizontal(" + amount + ")");

    airconsole.message(AirConsole.SCREEN, {
        horizontal: amount
    })
}

function bPressed(pressed) {
    console.log("bPressed(" + pressed + ")");

    airconsole.message(AirConsole.SCREEN, {
        bPressed: pressed
    })
}

function xPressed(pressed) {
    console.log("xPressed(" + pressed + ")");

    airconsole.message(AirConsole.SCREEN, {
        xPressed: pressed
    })
}

function aPressed(pressed) {
    console.log("aPressed(" + pressed + ")");

    airconsole.message(AirConsole.SCREEN, {
        aPressed: pressed
    })
}
//#endregion