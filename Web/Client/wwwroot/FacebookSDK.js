window.FacebookLoginWrapper = function () {
    return new Promise(resolve => {
        FB.login(response => resolve(response))
    });
}

window.FacebookGetLoginStatusWrapper = function () {
    return new Promise(resolve => {
        FB.getLoginStatus(response => resolve(response))
    });
}

window.fbAsyncInit = function () {
    FB.init({
        appId: '1279884659174714',
        cookie: true,
        xfbml: true,
        version: 'v17.0'
    });

    FB.AppEvents.logPageView();

};

(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));
