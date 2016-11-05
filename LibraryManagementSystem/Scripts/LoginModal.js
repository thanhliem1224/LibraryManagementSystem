$(document).ready(function () {
    var loginLink = $("a[id='loginLink']");
    loginLink.attr({ "href": "#", "data-toggle": "modal", "data-target": "#ModalLogin" });

    $("#loginform").submit(function (event) {
        if ($("#loginform").valid()) {
            var username = $("#Email").val();
            var password = $("#Password").val();
            var rememberme = $("#RememberMe").val();
            var antiForgeryToken = LibraryManagementSystem.Views.Common.getAntiForgeryValue();

            LibraryManagementSystem.Identity.LoginIntoStd(username, password, rememberme, antiForgeryToken,
                LibraryManagementSystem.Views.LoginModal.loginSuccess, LibraryManagementSystem.Views.LoginModal.loginFailure);
        }
        return false;
    });

    $("#ModalLogin").on("hidden.bs.modal", function (e) {
        LibraryManagementSystem.Views.LoginModal.resetLoginForm();
    });

    $("#ModalLogin").on("shown.bs.modal", function (e) {
        $("#Email").focus();
    });

});

var 
LibraryManagementSystem = LibraryManagementSystem || {};
LibraryManagementSystem.Views = LibraryManagementSystem.Views || {}

LibraryManagementSystem.Views.Common = {
    getAntiForgeryValue: function () {
        return $('input[name="__RequestVerificationToken"]').val();
    }
}

LibraryManagementSystem.Views.LoginModal = {
    resetLoginForm: function () {
        $("#loginform").get(0).reset();
        $("#alertBox").css("display", "none");
    },

    loginFailure: function (message) {
        var alertBox = $("#alertBox");
        alertBox.html(message);
        alertBox.css("display", "block");
    },

    loginSuccess: function () {
        window.location.href = window.location.href;
    }
}


LibraryManagementSystem.Identity = {
    LoginIntoStd: function (username, password, rememberme, antiForgeryToken, successCallback, failureCallback) {
        var data = {
            "__RequestVerificationToken": antiForgeryToken, "username": username, "password": password,
            "rememberme": rememberme
        };

        $.ajax({
            url: "/Account/LoginJson",
            type: "POST",
            data: data
        })
        .done(function (loginSuccessful) {
            if (loginSuccessful) {
                successCallback();
            }
            else {
                failureCallback("Email hoặc Mật khẩu không hợp lệ.");
            }
        })
        .fail(function (jqxhr, textStatus, errorThrown) {
            failureCallback(errorThrown);
        });
    }
}

$('#loginForm').keypress(function (e) {
    if(e.keyCode=='13') 
        $('#btnLogin').click();
}
);

