var attchmentsList = [];
$(document).ready(function ()
{
    initFileUploader();
    tinymce.init({
        selector: "textarea#elm1",

        theme: "modern",

        thieme_url: "/themes/modern/theme.min.js",

        plugins: 'link image code codesample table textcolor emoticons colorpicker autolink advlist lists charmap print preview  fullscreen hr anchor pagebreak spellchecker wordcount',
        //plugins: [
        //    "link image code",
        //    "advlist autolink lists charmap print preview hr anchor pagebreak spellchecker",
        //    "searchreplace wordcount visualblocks visualchars fullscreen insertdatetime media nonbreaking",
        //    "save table contextmenu directionality emoticons template paste textcolor importcss colorpicker textpattern codesample"
        //],
        external_plugins: {
            //"moxiemanager": "/moxiemanager-php/plugin.js"
        },
        //content_css: "css/development.css",

        add_unload_trigger: false,

        toolbar: "insertfile undo redo |styleselect fontsizeselect bold italic size | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | print preview media fullpage | forecolor backcolor emoticons table codesample",

        height: "500px",

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
        
        //template_replace_values: {
        //    username: "Jack Black"
        //},

        //template_preview_replace_values: {
        //    username: "Preview user name"
        //},

        //link_class_list: [
        //    { title: 'Example 1', value: 'example1' },
        //    { title: 'Example 2', value: 'example2' }
        //],

        //image_class_list: [
        //    { title: 'Example 1', value: 'example1' },
        //    { title: 'Example 2', value: 'example2' }
        //],

        templates: [
            { title: 'Some title 1', description: 'Some desc 1', content: '<strong class="red">My content: {$username}</strong>' },
            { title: 'Some title 2', description: 'Some desc 2', url: 'development.html' }
        ],

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
});

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

function validateRequest() {
    if($('#title').val() == ''){
        layer.msg('The question title is required');
        return false;
    }
    if ($('#title').val().length > 250) {
        layer.msg('The question title accepte most 250 characters)');
        return false;
    }
    if (tinymce.activeEditor.getContent() == '') {
        layer.msg('The question description is required');
        return false;
    }
    return true;
}

function initFileUploader() {
    var i = 1,
	$example_dropzone_filetable = $("#attachmentsList"),
    example_dropzone = $("#advancedDropzone").dropzone({
        url: '../BBS/UploadAttachments',
        maxFilesize: 20,
        // Events
        addedfile: function (file) {
            if (i == 1) {
                $example_dropzone_filetable.find('tbody').html('');
            }
            var size = parseInt(file.size / 1024, 10);
            size = size < 1024 ? (size + " KB") : (parseInt(size / 1024, 10) + " MB");

            var $el = $('<tr>\
							<td class="text-center">'+ (i++) + '</td>\
							<td>'+ file.name + '</td>\
							<td><div class="progress progress-striped"><div class="progress-bar progress-bar-warning"></div></div></td>\
							<td>'+ size + '</td>\
							<td>Uploading...</td>\
						</tr>');

            $example_dropzone_filetable.find('tbody').append($el);
            file.fileEntryTd = $el;
            file.progressBar = $el.find('.progress-bar');
        },
        accept: function (file, done) {
            if (file.size > 20978018) {
                layer.msg('Error: File size exceed 20MB.');
                done('File size exceed');
            }
            else {
                done();
            }
        },
        success: function (file, retData) {
            var size = parseInt(file.size / 1024, 10);
            size = size < 1024 ? (size + " KB") : (parseInt(size / 1024, 10) + " MB");
            attchmentsList.push({ FileName: retData.serverFileName, FileSize: size });
            file.fileEntryTd.find('td:last').html('<span class="text-success">Uploaded</span>');
            file.progressBar.removeClass('progress-bar-warning').addClass('progress-bar-success');
        },
        uploadprogress: function (file, progress, bytesSent) {
            file.progressBar.width(progress + '%');
        },
        error: function (file, error) {
            if (file.accept) {
                file.fileEntryTd.find('td:last').html('<span class="text-danger">Failed</span>');
                file.progressBar.removeClass('progress-bar-warning').addClass('progress-bar-red');
            }
            else {
                file.fileEntryTd.find('td:last').html('<span class="text-danger">' + error + '</span>');
                file.progressBar.removeClass('progress-bar-warning').addClass('progress-bar-red');
            }
        }
    });
}

function publish() {

    if (!validateRequest()) {
        return;
    }

    var content = tinymce.activeEditor.getContent();
    content = getLanguage(content);
    getTags();
    var requestData = {
        title: $('#title').val(),
        tags: getTags(),
        bbsContent: common.htmlEncode(content),
        attachments: JSON.stringify(attchmentsList)
    };
    $.ajax({
        url: "../BBS/RequestPublish",
        type: 'POST',
        data: requestData,
        success: function (e) {
            if (e.success) {
                window.location = window.location.protocol + "//" + window.location.host + "/BBS/Details/"+ e.retData;
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

function getLanguage(inputs) {
    if (inputs.length) {
        inputs = inputs.replace(/class="language-markup"/g, 'class="language-html"');
        return inputs;
    }
}

function getTags() {
    var tags = $('.bootstrap-tagsinput span');
    var result = '';
    $.each(tags, function (index) {
        var single = tags[index];
        if (single.className != '') {
            result += (single.innerText+",");
        }
    });
    if(result.length > 0)
    {
        result = result.substr(0,result.length - 2);
    }
    return result;
}
