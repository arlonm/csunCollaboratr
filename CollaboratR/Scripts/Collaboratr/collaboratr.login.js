/*
    Make sure to minify all javascript prior to production

    http://javascript-minifier.com/

    Naming convention:
        { filename }.min.js
*/
//Run ajax login command on Login click.
$(function () {
    //Handle the pagewide login feature
    $('#txt-password').keypress(function (e) {
        if (e.which == 13) {
            $('#btn-login').click();
        }
    });
    $('#btn-login').click(function () {
        $('#btn-login').hide();
        $('#login-loader').show();
        $('#msg-loginname').hide(200);
        $('#msg-password').hide(200);
        $('#msg-general').hide(200);
        var login = $('#txt-loginname').val();
        var pass = $('#txt-password').val();
        //Post an ajax query to the server for logging in
        var valid = true;
        if (login == '') {
            valid = false;
            $('#msg-loginname').text('Please enter your username or email');
            $('#msg-loginname').show(200);
            $('#btn-login').show();
            $('#login-loader').hide();
        }
        if (pass == '') {
            valid = false;
            $('#msg-password').text('Please enter your password');
            $('#msg-password').show(200);
            $('#btn-login').show();
            $('#login-loader').hide();
        }
        if (valid == true) {
            $.ajax({
                url: "/Account/Login",
                type: "POST",
                data: JSON.stringify({
                    //User JSON to send our data
                    LoginName: $('#txt-loginname').val(),
                    Password: $('#txt-password').val()
                }),
                contentType: "application/json",
                success: function (data) {
                    $('#btn-login').show();
                    $('#login-loader').hide();
                    if (data.Success == false) {
                        $('#msg-general').show(200);
                        $('#msg-general').text('' + data.Message)
                    }
                    else {
                        $('#msg-general').hide(200);
                        $('#login-modal').hide(200);
                        location.reload();
                    }
                },
                error: function (data) {
                    //TODO: redirect to error msg
                }
            });
        }
    });
});