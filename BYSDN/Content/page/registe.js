var imgPath = '';
var x, y, width, height = 0;
var isFirstRun = true;

$(document).ready(function ($)
{
    setTimeout(function(){
        layer.tips('Click here to set user photo ', $('#userPhotos'));
    },1000);
    //init upload plug-in
    var example_dropzone = $("#advancedDropzone").dropzone({
        url: '../User/Upload',
        acceptedFiles: "image/jpeg,image/png,image/gif,image/bmp,image/jpg",
        maxFilesize: 2,
        previewsContainer: '#preview-template',
        createImageThumbnails: false,
        success: function (file, retData) {
            if (retData.success)
            {
                var filePath = "../Images/" + retData.serverFileName;
                imgPath = filePath;
                $("#userPhoto").attr("src", filePath);
                if (isFirstRun) {
                    isFirstRun = false;
                    initCorpPlugin();
                }
                else
                {
                    $('#userPhoto').cropper("destroy");
                    x = y = width = height = 0;
                    initCorpPlugin();
                }               
            }
        },
        accept: function (file, done) {
            var extension = getFileExtension(file.name);
            if (extension != 'jpeg' &&
                extension != 'png' &&
                extension != 'gif' &&
                extension != 'bmp' &&
                extension != 'jpg') {
                layer.msg('File type are not supported');
                done("File type not support");
            }
            else
            {
                done();
            }
        },
        error: function (file) {
            var c = file;
        }					
    });
});

function getFileExtension(input)
{
    if (input.length)
    {
        var extension = '';
        for (var i = input.length - 1; i > 0 ; i--)
        {
            if (input[i] != '.') {
                extension = input[i] + extension;
            }
            else {
                break;
            }
        }
        return extension;
    }
}

function commitChange()
{
    $.ajax({
        url: "../User/CropperImage",
        method: 'post',
        dataType: 'json',
        cache: false,
        async: true,
        data: {
            url: imgPath,
            x: x,
            y: y,
            width: width,
            height: height
        },
        success: function (e) {
            if (e.success)
            {
                $('#userPhotos').attr("src", "../Images/" + e.fileName);
                imgPath = "../Images/" + e.fileName;
            }
        }
    });
}

function initCorpPlugin()
{
    var $image = $('#userPhoto');
    $image.cropper({
        aspectRatio: 1 / 1,
        preview: '.corpimg-preview',
        minCanvasWidth: 500,
        minCanvasHeight: 500,
        minContainerWidth: 600,
        minContainerHeight: 500,
        crop: function (e) {
            x = e.x;
            y = e.y;
            width = e.width;
            height = e.height;
        },
    });
}

function showCropWindow()
{
    initCropWindowHtml();
    isFirstRun = true;
    imgPath = '';
    x, y, width, height = 0;
    jQuery('#setUeerPhoto').modal('show', { backdrop: 'fade' });
}

function initCropWindowHtml()
{
    $('#img-container').html('<img id="userPhoto" src="" alt="picture" />');
    $('#img-preview').html('<div class="corpimg-preview preview-lg"></div>\
                            <div class="corpimg-preview preview-md"></div>');
}

function requestRegist()
{
    $.ajax({
        url: "../User/RegisteUser",
        method: 'Get',
        dataType: 'json',
        cache : false,
        async: true,
        data: {
            UserName : $('#username').val(),
            Sex: $("#sexSelect").val(),
            Comments: $("#Comments").val(),
            imgPath: imgPath
        },
        success: function (e)
        {
            if (e.success) {
                window.location.pathname = "";
            }
        }
    });
}