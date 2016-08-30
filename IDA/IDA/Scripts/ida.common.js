var ida = {
    common: {},
    core: {
        demi: {}
    }
};

if (!ida) {
    ida = {};
}

ida.common =

    {
        displayMessage: function displayMessage(options) {

            // alert(options.messageType);

            if (typeof options !== 'object')
                options = {};

            if (typeof options.messageType === 'undefined')
                options.messageType === 'error';
            if (typeof options.messageText === 'undefined' && options.messageType == 'error')
                options.messageText === 'A system error has occured.  If this problem persists, please contact your system administrator.'

            var popupNotification = $("#popupNotification").data("kendoNotification");


            if (options.messageType == 'error')
                popupNotification.show(options.messageText, options.messageType);
            if (options.messageType == 'info')
                popupNotification.show(options.messageText, options.messageType);
            if (options.messageType == 'error-data')
                popupNotification.show("Oops! I'm having trouble retrieving your data.  If this poblem persists, please contact your system administrator.", "error");
            if (options.messageType == 'info-development')
                popupNotification.show("Please note - this feature is in development.", "info");
        },

        onPopupNotificationShow: function onPopupNotificationShow(e) {
            if (!$("." + e.sender._guid)[1]) {
                var element = e.element.parent(),
                    eWidth = element.width(),
                    eHeight = element.height(),
                    wWidth = $(window).width(),
                    wHeight = $(window).height(),
                    newTop, newLeft;

                newLeft = Math.floor(wWidth / 2 - eWidth / 2);
                newTop = Math.floor(wHeight / 2 - eHeight / 2);

                e.element.parent().css({ top: newTop, left: newLeft });
            }
        },

        setCookie: function setCookie(cname, cvalue, exdays) {
            var d = new Date();
            d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
            var expires = "expires=" + d.toGMTString();
            document.cookie = cname + "=" + cvalue + "; " + expires + "; path=/";
        },

        getCookie: function getCookie(cname) {
            var name = cname + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1);
                if (c.indexOf(name) == 0) {
                    return c.substring(name.length, c.length);
                }
            }
            return "";
        }

    }

