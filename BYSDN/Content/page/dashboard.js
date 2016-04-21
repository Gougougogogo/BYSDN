var person;
//load user info
window.onload = function () {
    $.ajax({
        url: '/Home/GetUserInfo',
        cache: false,
        success: function (e) {
            person = e;
            if (e) {
                $('#userImg').attr("src", person.Img);
                $('#userImg1').attr("src", person.Img);
            }
        }
    });
};