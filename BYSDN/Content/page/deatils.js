var pageCount = 0;
var currentPage = 0;

$(document).ready(function () {
    tinymce.init({
        selector: "textarea#elm1",

        theme: "modern",

        thieme_url: "/themes/modern/theme.min.js",

        plugins: 'link image code codesample table textcolor emoticons colorpicker autolink advlist lists charmap print preview  fullscreen hr anchor pagebreak spellchecker wordcount',

        add_unload_trigger: false,

        toolbar: "insertfile undo redo |styleselect fontsizeselect bold italic size | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | print preview media fullpage | forecolor backcolor emoticons table codesample",

        height: "300px",

        image_advtab: true,

        image_caption: true,

        relative_urls: false,

        style_formats: [
            { title: 'Bold text', format: 'h1' },
            { title: 'Red text', inline: 'span', styles: { color: '#ff0000' } },
            { title: 'Red header', block: 'h1', styles: { color: '#ff0000' } },
            { title: 'Example 1', inline: 'span', classes: 'example1' },
            { title: 'Example 2', inline: 'span', classes: 'example2' },
            { title: 'Table styles' },
            { title: 'Table row 1', selector: 'tr', classes: 'tablerow1' }
        ],
        menu: {

        },

        skin: "custom",

        templates: [
            { title: 'Some title 1', description: 'Some desc 1', content: '<strong class="red">My content: {$username}</strong>' },
            { title: 'Some title 2', description: 'Some desc 2', url: 'development.html' }
        ],

        fontsize: "20px",

        fontsize_formats: "8pt 9pt 10pt 11pt 12pt 26pt 36pt",

        setup: function (ed) {
            /*ed.on(
                'Init PreInit PostRender PreProcess PostProcess BeforeExecCommand ExecCommand Activate Deactivate ' +
                'NodeChange SetAttrib Load Save BeforeSetContent SetContent BeforeGetContent GetContent Remove Show Hide' +
                'Change Undo Redo AddUndo BeforeAddUndo', function(e) {
                console.log(e.type, e);
            });*/
        },

        spellchecker_callback: function (method, data, success) {
            if (method == "spellcheck") {
                var words = data.match(this.getWordCharPattern());
                var suggestions = {};

                for (var i = 0; i < words.length; i++) {
                    suggestions[words[i]] = ["First", "second"];
                }

                success({ words: suggestions, dictionary: true });
            }

            if (method == "addToDictionary") {
                success();
            }
        }
    });
    hightLight();
    getReplysInfo();
    getAttachments();
});

function hightLight() {
    $('pre code').each(function (i, block) {
        hljs.highlightBlock(block);
    });
}

function getAttachments() {
    var id = getId();
    if(id != '')
    {
        $.ajax({
            url: "/BBS/GetAttachments",
            type: 'Get',
            data: { Id : id },
            success: function (e) {
                if (e.success) {
                    if (e.retData.length > 0) {
                        var html = '';
                        $.each(e.retData, function (index) {
                            html += '<li style="text-align:center">\
                                        <span class="fa-file-zip-o fa-2x"></span>\
                                        <div class="attchamentsTitle">\
                                            <p>' + getFileName(e.retData[index].Name) + '<small>(' + e.retData[index].Size + ')</small></p>\
                                        </div>\
                                        <div>\
                                        </div>\
                                        <div class="links">\
                                            <a href=\'../DownloadFile/'+ e.retData[index].ID + '\' }) >Download</a>\
                                        </div>\
                                    </li>';
                        });
                        $('#attmentsList').html(html);
                    }
                    else {
                        $('.bbsAttachments').css("display", "none");
                    }
                }
                else {
                    layer.msg(e.retData);
                }
            },
            error: function (e) {
                layer.msg(e.retData);
            }
        });
    }
}

function getLanguage(inputs)
{
    if (inputs.length) {
        inputs = inputs.replace(/class="language-markup"/g, 'class="language-html"');
        return inputs;
    }
}

function getId() {
    var url = window.location.pathname;
    var result = '';
    for(var i = url.length -1 ;i >= 0;i--)
    {
        if (url[i] != '/') {
            result = url[i] + result;
        }
        else
        {
            break;
        }
    }
    return result;
}

function getFileName(inputs) {
    var index = inputs.indexOf("%%");
    var result = '';
    if(index != -1)
    {
        for (var i = index + 2; i < inputs.length; i++)
        {
            result += inputs[i];
        }
    }
    return result;
}

function getReply(page) {
    var id = getId();
    if (id != '') {
        $.ajax({
            url: "/BBS/GetReplyDetails",
            type: 'Get',
            data: { Id: id, Page: currentPage },
            cache: false,
            success: function (e) {
                if (e.success) {
                    var html = '';
                    $.each(e.retData, function (index) {
                        html += '<div class="row" id="reply' + e.retData[index].Order + '">\
                                    <div class="col-sm-12">\
                                        <div class="row">\
                                            <div class="col-sm-9">\
                                                <p class="replyOrder">\
                                                    #' + e.retData[index].Order + '\
                                                </p>\
                                            </div>\
                                        </div>\
                                        <hr style="margin-bottom:5px" />\
                                        <div class="row">\
                                            <div class="col-sm-1">\
                                                <img width="38" style="float:right" src="../' + e.retData[index].UserImg + '">\
                                            </div>\
                                            <div class="col-sm-5">\
                                                <em style="font-size:14px">' + e.retData[index].UserName + '</em>\
                                                <p style="color: gray">' + e.retData[index].UserEmail + '</p>\
                                            </div>\
                                        </div>\
                                        <hr style="margin-top:5px" />\
                                        <div id="content" style="padding:0px 50px;margin-bottom:30px">\
                                            ' + e.retData[index].ReplyContent + '\
                                        </div>\
                                        <hr />\
                                    </div>\
                                </div>';
                    });
                    $('#replyContents').html(html);
                    hightLight();
                }
                else {
                    layer.msg(e.retData);
                }                
            },
            error: function (e) {
                layer.msg(e.retData);
            }
        });
    }
}

function getReplysInfo() {
    var id = getId();
    if (id != '') {
        $.ajax({
            url: "/BBS/GetReplyInfos",
            type: 'Get',
            data: { Id: id },
            cache: false,
            success: function (e) {
                if (e.success) {
                    if(e.retData > 0)
                    {
                        pageCount = Math.floor(e.retData / 10);
                        pageCount = e.retData % 10 == 0 ? pageCount : pageCount + 1;

                        $('#pagination').twbsPagination({
                            totalPages: pageCount,
                            visiblePages: 5,
                            onPageClick: function (event, page) {
                                currentPage = page;
                                getReply(page);
                            }
                        });
                    }
                    else
                    {
                        $('#replys').css("display", "none");
                    }
                }
            },
            error: function (e) {
                layer.msg(e.retData);
            }
        });
    }
}

function validateRequest() {

    if (tinymce.activeEditor.getContent() == '') {
        layer.msg('The reply content is required');
        return false;
    }
    return true;
}

function submitReply() {
    var id = getId();
    if (id != '') {

        if (!validateRequest())
        {
            return;
        }

        var content = tinymce.activeEditor.getContent();
        content = getLanguage(content);

        var requestData = {
            Id : id,
            replyContent: common.htmlEncode(content),
        };

        $.ajax({
            url: "/BBS/SubmitReply",
            type: 'POST',
            data: requestData,
            success: function (e) {
                if (e.success) {
                    layer.msg('Submit reply successfully!');
                    location.reload();
                }
                else {
                    layer.msg(e.retData);
                }
            },
            error: function (e) {
                layer.msg(e.retData);
            }
        });
    }
}

var common = {};
common.htmlEncode = function (str) {
    var s = "";
    if (str.length == 0) return "";
    s = str.replace(/&/g, "&namp;");
    s = s.replace(/</g, "&nlt;");
    s = s.replace(/>/g, "&ngt;");
    //s = s.replace(/ /g, "&nbsp;");
    //s = s.replace(/\'/g, "&#39;");
    s = s.replace(/\"/g, "&nquot;");
    return s;
}
common.htmlDecode = function (str) {
    var s = "";
    if (str.length == 0) return "";
    s = str.replace(/&namp;/g, "&");
    s = s.replace(/&nlt;/g, "<");
    s = s.replace(/&ngt;/g, ">");
    //s = s.replace(/&nbsp;/g, " ");
    //s = s.replace(/'/g, "\'");
    s = s.replace(/&nquot;/g, "\"");
    //s = s.replace(/<br>/g, "\n");
    return s;
}