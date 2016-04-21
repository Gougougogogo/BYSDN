var person;
//load user info
window.onload = function () {
    $.ajax({
        url: '/Home/GetUserInfo',
        cache: false,
        success: function (e) {
            person = e;
            if (e) {
                var imgPath = person.Img.replace("..", "http://" + window.location.host);
                $('#userImg').attr("src", imgPath);
                $('#userImg1').attr("src", imgPath);
            }
        }
    });
};