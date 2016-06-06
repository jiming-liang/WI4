//var Role = $('#Role').val()
$(document).ready(function () {
    var s, src, w, h
    // return
    $('body').css({
        fontSize: '10pt'
    , 'font-family': 'arial'
    })
    if ($('#__EVENTARGUMENT').length == 0) {
        s = '<input type="hidden" name="__EVENTARGUMENT" id="__EVENTARGUMENT" value="" /> '
        s += '<input type="hidden" name="__EVENTTARGET" id="__EVENTTARGET" value="" /> '
        $(s).appendTo($('form'))
    }
   // on_Init()
    //on_FormatHeader()
    $(window).resize(function () {
        //on_FormatHeader()
        //on_FormatHeader()
    });
    SetupTitle()
});
function on_FormatHeader() {
    var s, src, w, h, marginRight = 3
    $('#ensco').css({
        position: 'absolute',
        top: 0,
        left: 0
    })
    $('#ActionRequired').css({
        position: 'absolute',
        backgroundColor:'pink', 
        top: 10,
        padding:'6px', 
        left: $(window).width()/2 - $('#ActionRequired').width()/2
    }).hide()
    $('#wims').css({
        position: 'absolute',top: 0,
        left: $(window).width() - $('#wims').width() - marginRight + $(window).scrollLeft()
    })
    var top = 60
    $('#bar').css({
        position: 'absolute',
        top: top,
        left: 0,
        height: 20,
        width: $(window).width() - marginRight + $(window).scrollLeft()
    })
    $('#SiteMap1').css({
        position: 'absolute',
        top: top,
        left: 0,
        height: 20,
        width: $(window).width() - marginRight + $(window).scrollLeft()
    })
    $('#home').css({
        position: 'absolute',
        top: top,
        color: 'white',
        left: 10
    })
    $('#back').css({
        position: 'absolute',
        top: top,
        color: 'white',
        left: 60
    })
    src = $('#headerUserName')
    w = src.width()
    $('#headerUserName').css({
        position: 'absolute',
        top: top,
        color: 'white',
        left: $(window).width() - w - 20 - marginRight + $(window).scrollLeft()
    })
    src = $('#headerLogout')
    w = src.width()
    src.css({
        position: 'absolute',
        top: top + 20,
        color: 'blue',
        left: $(window).width() - w - 20 - marginRight + $(window).scrollLeft()
    })
}
function on_Init() {
    var s, src, w, h
    var arr = ['jobstep', 'upload', 'popupawi', 'popup', 'precaution', 'sendworkflow', 'mapping']
    var url = window.location.toString().toLowerCase()
    for (var i = 0; i < arr.length; i++) {
        if (arr[i] != 'mapping') {
            if (url.indexOf(arr[i].toLowerCase()) != -1)
                return
        }
        else {
            if ((url.indexOf('mappingxml=') != -1) || (url.indexOf('action=') != -1))
               return
        }
    }
    s = '<div ID="pageHeader"><img id="ensco" src=images/ensco.png style="width:120px" />'
    s += '<img  id="wims" style1="width:100px;" src=images/wims.png  />'
    s += '<span id=headerUserName style1="width:10px;" >' + $('#UserName').val() + '</span>'
    s += '<span id=headerLogout><a  href=logout.aspx onclick1=on_Logout() >Logout</a>&nbsp|&nbsp<a href=# onclick=on_WiHelp() >Help</a>'
    s += '&nbsp|&nbsp<a href="https://team.enscoplc.com/RigStandards/WIProject/Lists/WIMS%20Development%20Issue%20Tracking/AllItems.aspx" target=_blank >Log a Case</a></span>'
    s += '<div id=ActionRequired style="width:500px"><b>Action Required:</b> the global content of this work instruction has been updated. Please review the changes and update the local content accordingly.</div>'
    s += '</div>'
    src = $(s)
    src.appendTo($('body'))

    $('h1').css('color', 'rgb(0, 70, 127)')
}
function on_WiHelp() {
}
$.createElement = function (name, value) {
    return $('<' + name + ' name=' + value + ' />');
};
$.createElement = function (name) {
    return $('<' + name + '/>');
};
$.fn.addAttr = function (name, value) {
    $(this).attr(name, value)
};
function handleSelectAttempt(e) {
    var sender = e && e.target || window.event.srcElement;
    if (isInForm(sender)) {
        if (window.event) {
            event.returnValue = false;
        }
        return false;
    }
    if (window.event) {
        event.returnValue = true;
    }
    return true;
};

function isInForm(element) {
    return true
    if (element.nodeName.toUpperCase() == 'INPUT'
    || element.nodeName.toUpperCase() == 'TEXTAREA') {
        return true;
    } else
        return false

    while (element.parentNode) {
        if (element.nodeName.toUpperCase() == 'INPUT'
            || element.nodeName.toUpperCase() == 'TEXTAREA') {
            return true;
        }
        if (!searchFor.parentNode) {
            return false;
        }
        searchFor = searchFor.parentNode;
    }
    return false;
}
// JQ plugin appends a new element created from 'name' to each matched element.
$.fn.appendNewElement = function (name) {
    this.each(function (i) {
        $(this).append('<' + name + ' />');
    });
    return this;
}
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}
function escapeXml(unsafe) {
    return unsafe.replace(/[<>&'"]/g, function (c) {
        switch (c) {
            case '<': return '&lt;';
            case '>': return '&gt;';
            case '&': return '&amp;';
            case '\'': return '&apos;';
            case '"': return '&quot;';
        }
    });
}
function htmlEscape(str) {
    return String(str)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
}
function htmlUnescape(str) {
    return String(str)
        .replace('&amp;', /&/g)
        .replace('&quot;', /"/g)
        .replace('&#39;', /'/g)
        .replace(/&lt;/g, '<')
        .replace('&gt;', />/g);

    return String(str)
            .replace('&amp;', /&/g)
            .replace('&quot;', /"/g)
            .replace('&#39;', /'/g)
            .replace('&lt;', /</g)
            .replace('&gt;', />/g);
}
function __doPostBack(eventTarget, eventArgument) {
    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
        theForm.__EVENTTARGET.value = eventTarget;
        theForm.__EVENTARGUMENT.value = eventArgument;
        theForm.submit();
    }
}
function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}
function getCookie(c_name) {
    var c_value = " " + document.cookie;
    var c_start = c_value.indexOf(" " + c_name + "=");
    if (c_start == -1) {
        c_value = null;
    }
    else {
        c_start = c_value.indexOf("=", c_start) + 1;
        var c_end = c_value.indexOf(";", c_start);
        if (c_end == -1) {
            c_end = c_value.length;
        }
        c_value = unescape(c_value.substring(c_start, c_end));
    }
    return c_value;
}
function MyReplace(str, s1, s2) {
    if (str==null)  return ''
    var re = new RegExp(s1, 'g');
    return str.replace(re, s2)//.replace(s2, s1);
}
function GetSiteUrl() {
    var url = window.location.toString()
    return url.split('/').slice(0, 4).join('/')
}
var CallAjax = function (name, value, method, fn1, fn2, mode) {
    value = escapeXml(value)
    var parameters = "{'" + name + "':'" + value + "'}"
    var async = true
    if (mode != null)
        async = mode
    $.ajax({
        type: 'POST',
        async: async,
        url: GetSiteUrl() + '/webservice.asmx/' + method,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: fn1,
        error: fn2
    });
}
var CallAjax2 = function (parameters, method, fn1, fn2, mode) {
   // var parameters = "{'sp':'" + sp + "', 'where':'" + where + "'}"
    var async = true
    if (mode != null)
        async = mode
    $.ajax({
        type: 'POST',
        async: async,
        url: GetSiteUrl() + '/webservice.asmx/' + method,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: fn1,
        error: fn2
    });
}
function getParameterByName(name) {
    name=name.toLowerCase()
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search.toLowerCase());
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
function SetupTitle() {
    var arr = [] //72, 160, 160, 160, 160, 160, 160, 160, 160, 76, 79]
    for (var i = 0; i < 400; i++)
        arr.push(160)
    var s = String.fromCharCode.apply(String, arr)
    //alert(window.dialogArguments)
    if(typeof window.dialogArguments == 'undefined')
        s="WIMS"+s
    $(document).prop('title', s)
    $('head').append('<link rel="shortcut icon" href="images/wims_Vwz_icon.ico?test=1" type="image/x-icon">')
}
function SetupTinyMCE() {
    var url=window.location.toString()
    var id = 'Precaution'
    if (url.toLowerCase().indexOf('jobstep') != -1)
        id='Description'
    tinyMCE.init({
        auto_focus: id,
        mode: "textareas"
        , setup: function (ed) {
            ed.onPaste.add(function (ed, e) {
                setTimeout(function () {
                    $('iframe').contents().find('p').css('margin', 0)
                }, 100);
            });
        },      
        browser_spellcheck: true,
        force_p_newlines: false,
        //forced_root_block: '<div>',
        paste_postprocess: function (pl, o) {
            return
            var tNode = o.node.innerHTML;
            tNode = tNode.replace(new RegExp('<\/p>', 'g'), '<br>');
            tNode = tNode.replace(new RegExp('<p>', 'g'), "");
            tNode = tNode.replace(new RegExp('<h1>', 'g'), "<div>");
            tNode = tNode.replace(new RegExp('</h1>', 'g'), "</div>");
            tNode = tNode.replace(new RegExp('<h2>', 'g'), "<div>");
            tNode = tNode.replace(new RegExp('</h2>', 'g'), "</div>");
            tNode = tNode.replace(new RegExp('<h3>', 'g'), "<div>");
            tNode = tNode.replace(new RegExp('</h3>', 'g'), "</div>");
            tNode = tNode.replace(new RegExp('<h4>', 'g'), "<div>");
            tNode = tNode.replace(new RegExp('</h4>', 'g'), "</div>");
            tNode = tNode.replace(new RegExp('<div>', 'g'), "<div style=\"font-family:Arial;font-size:12px\">");
            tNode = "<div>" + tNode + "</div>";
            o.node.innerHTML = tNode;
        },
        noneditable_regexp: /\[\[[^\]]+\]\]/g, theme: "advanced", plugins: "safari,pagebreak,style,layer,table,save,advhr,advimage,advlink, emotions,iespell,inlinepopups,insertdatetime,preview,media, searchreplace,print,contextmenu, paste,directionality,fullscreen,noneditable,visualchars,nonbreaking, xhtmlxtras,template,wordcount", theme_advanced_buttons1: "bold,italic,underline,strikethrough,|, justifyleft,justifycenter,justifyright,justifyfull ,cut,copy,paste,undo,selectall,fontselect,fontsizeselect", theme_advanced_buttons2: "bullist,numlist,| ,outdent,indent,|,forecolor,backcolor"
    })
    document.addEventListener("DOMNodeInserted", function (event) {
        //$('iframe').contents().find('p').css('margin', 0)
    });
}
function InitTinyMCE() {
    var done = true
    $('textarea').each(function () {
        var src = $(this)
        var id = src.attr('id')
        if (id == 'Warning' || id == 'Caution')
            return
        src.width('95%')
        var t1 = $('#' + id + '_toolbar1')
        var t2 = $('#' + id + '_toolbar2')
        var t3 = $('#' + id + '_toolbar3')
        if (t1.length == 0) {
            done = false
            return
        }
        if (t1.attr('done') != null)
            return
        t1.attr('done', 1)
        t3.remove()
        t1.css({ 'border': '1px solid black', 'float': 'left', 'width': '240' })
        t2.css({ 'border': '1px solid black', 'float': 'left', 'width': '240' })
        t1.parent().css({ 'margin': '0 auto', 'width': '500px' })
        $('#' + id + '_tbl').children('tbody').children('tr').eq(2).remove()

        $('#Warning_parent').remove()
        $('#Warning').show()
        $('#Caution_parent').remove()
        $('#Caution').show()

        src = $('iframe').contents().find('body')
        src.keyup(function (e) {
            var code = e.keyCode || e.which;
            if (code == 13) {
                $('iframe').contents().find('p').css('margin', 0)
            }
        })
    })
    if (!done)
        setTimeout(InitTinyMCE, 500);
}

String.prototype.format = function (args) {
    var str = this;
    return str.replace(String.prototype.format.regex, function (item) {
        var intVal = parseInt(item.substring(1, item.length - 1));
        var replace;
        if (intVal >= 0) {
            replace = args[intVal];
        } else if (intVal === -1) {
            replace = "{";
        } else if (intVal === -2) {
            replace = "}";
        } else {
            replace = "";
        }
        return replace;
    });
};
String.prototype.format.regex = new RegExp("{-?[0-9]+}", "g");
function IsRole() {
    for (var i = 0; i < Roles.length; i++) {
        for (var j = 0; j < arguments.length; j++) {
            if (Roles[i][0] == arguments[j])
                return true
        }
    }
    return false
}
function IsRole_012916(name) {
    for (var i = 0; i < Roles.length; i++) {
        if (Roles[i][0] == name)
            return true
    }
    return false
}