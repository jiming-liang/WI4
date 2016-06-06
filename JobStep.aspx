<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="JobStep.aspx.cs" Inherits="JobStep" %>

<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <style>
        .box {
            BORDER-LEFT:white 15px solid;
            BACKGROUND-COLOR: White;
        }
        body {
            font-family: Arial;
        }

        textarea {
            width: 100%;
            height: 100px;
            border-left: 1px solid #CCC;
            border-right: 1px solid #CCC;
            border-top: 1px solid #CCC;
            border-bottom: 1px solid #CCC;
        }

        input[type=text] {
            border-left: 1px solid #CCC;
            border-right: 1px solid #CCC;
            border-top: 1px solid #CCC;
            border-bottom: 1px solid #CCC;
        }
    </style>
    <script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="WI.js"></script>
    <script type="text/javascript" src="tinymce/jscripts/tiny_mce/tiny_mce.js"></script>
</head>

<body style1='display:none'>
    <form id="form1" runat="server">
        <br />
        <table id="t" width="100%">
            <colgroup>
                <col width="30%" style1="text-align: right" />
                <col />
            </colgroup>
            <tr>
                <td colspan="2"><b>Type of Step: </b>
                    <span class="box"> Global<input type="radio"  checked="checked" name="IsGlobal" value="0"/></span>
                    <span  class="box"> <span>Local Linked to Global</span><input type="radio"  name="IsGlobal" value="1"/> </span> 
                    <span  class="box"> Stand Alone<input type="radio"  name="IsGlobal" value="2"/> </span>
                </td>
            </tr>
            <tr>
                <td colspan="2"><b>Description</b><span style="color: rgb(232, 109, 31);font-size:larger">*</span>:
                    <textarea  id="Description"  ></textarea>
                </td>
            </tr>
            <tr>
                <td><b>Hazards</b> (select all that apply):     
                    <spans id="holder" runat="server"></spans>
                </td>
                <td style="vertical-align: top"><b>Specify any warning, caution and/or note:</b>
                    <table id="tt" width="100%" cellpadding="0">
                        <tr>
                            <td style="width: 5%; valign: top">
                                <img src="images/Warning.png" width="40px" /><br />
                                <center><b>Warning</b></center>
                            </td>
                            <td>
                                <textarea id="Warning"></textarea>
                            </td>
                        <tr>
                            <td style="width: 5%; valign: top">
                                <img src="images/Caution.png" width="40px" /><br />
                                   <center><b>Caution</b></center>
                            </td>
                            <td>
                                <textarea style="width: 100%" id="Caution"></textarea>
                            </td>
                        <tr>
                            <td style="width: 5%; valign: top">
                                <img src="images/Note.png" width="40px" /><br />
                                    <center><b>Note</b></center>
                            </td>
                            <td>
                                <textarea style="width: 100%" id="Note"></textarea>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2"><b>Check box if this job step is a human action barrier</b>
                    <input id="Barrier" type="checkbox" />
            </tr>
            <tr>
                <td colspan="2"><b>Photo Prompt</b><input id="Prompt" type="text" />
            </tr>
            <tr>
                <td colspan="2"><br /><b>Insert Photo</b><input type="file" id="file" runat="server" />
                <tr>
                    <td colspan="2" align="center">
                        <input type="button" onclick="on_Submit(this)" value="Save" />
                        <input type="button" onclick="on_Cancel(this)" value="Cancel" />
                    </td>
                </tr>
            </tr>
        </table>
        <input type="hidden" id="IsLocal" value="" runat="server" />
    </form>
</body>
</html>

<script>
    var theForm = document.forms['form1']
    var t = $('#t')
    var WiType='WIT'
    var Type = ['GlobalStep', 'LocalStep', 'StandaloneStep']
    var Photo, Obj
    $(document).ready(function () {
        $(document).prop('title', 'Job Step' + $(document).prop('title'))
        SetupTinyMCE()
        InitTinyMCE()
        InitFileUpload()
        WiType = getParameterByName('WiType')
        if (WiType=='wi') {
            $('input:radio[value=0]').parent().hide()
            $('input:radio[value=1]').prop('checked', true)
        } else if (WiType=='wit') {
            $('input:radio[value=2]').parent().hide()
            $('#Prompt').closest('tr').hide()
        } else {
            $('input:radio[value=0]').parent().hide()
            $('input:radio[value=1]').parent().hide()
            $('input:radio[value=2]').prop('checked', true)
        }
        $('table table td:nth-child(1)').css('text-align', 'left')

        var obj = window.dialogArguments
        if (!obj && obj!='dummy') {
            InitPrompt()
            return
        }
        Obj = obj
        Lockdown()
        if (obj.Type != null) {
            var Description = obj.Description
            Description = MyReplace(Description, 'font-family:', '')
            var Note = obj.Note
            Note = MyReplace(Note, 'font-family:', '')
            $('#Description').val((Description))
            $('#Warning').val((obj.Warning))
            $('#Caution').val(obj.Caution)
            $('#Note').val((Note))
            $('#Prompt').val(obj.Prompt)
            switch (obj.Type) {
                case 'GlobalStep':
                    $('input:radio[value=0]').prop('checked', true)
                    break
                case 'LocalStep':
                    $('input:radio[value=1]').prop('checked', true)
                    break
                default:
                    $('input:radio[value=2]').prop('checked', true)
            }
            if (obj.Barrier == 'Yes')
                $('#Barrier').prop('checked', true)

            if ($.trim(obj.Hazard) != '') {
                var arr = obj.Hazard.split(',')
                for (var j = 0; j < arr.length; j++) {
                    $('input:checkbox[value="' + arr[j] + '"]').prop("checked", true);
                }
            }
        }
        InitPrompt()
    })
    function Lockdown() {
        var stepId = Obj.StepId
        if (stepId == null)
            return 
        var sql = ' select * from jobStep where id=' + stepId + ' and counterPart is not null '
        CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                if ($($.parseXML(xml.d)).find('Table').length>0)
                   $('input:radio').attr('disabled', true)
            },
            function (e) {
                alert(e.responseText)
            }
        )
    }
    function InitPrompt() {
        $('#Prompt').closest('tr').hide()
        if ($('input:radio[value=1]').prop('checked') && WiType == 'wit')
            $('#Prompt').closest('tr').show()
        $('input:radio').change(function () {
            if ($('input:radio[value=1]').prop('checked') && WiType=='wit')
                $('#Prompt').closest('tr').show()
            else
                $('#Prompt').closest('tr').hide()
        })
        $('#Prompt').keydown(function () {
            if ($('input:file').eq(0).val() != '')
                return false
        })
        $('input[type=file]').click(function () {
            if ($.trim($('#Prompt').val()) != '' && ! IsLocal)
                return false
        })
    }
    function HidePrompt() {
        t.children('tbody').children('tr').eq(4).show()
        if ($('input:radio[value=0]').prop('checked'))
            $('#Prompt').closest('tr').hide()
    }
    function InitTinyMCE1() {
        var done = true
        $('textarea').each(function () {
            var src = $(this)
            var id = src.attr('id')
            if (id == 'Warning' || id == 'Caution')
                return
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

            $('span1').click(function () {
                // event.stopPropagation();
                $('iframe').contents().find('p').css('margin', 0)
                $('span', $('#menu_LocalPrecautions_LocalPrecautions_fontselect_menu_tbl')).click(function () {
                    $('iframe').contents().find('p').css('margin', 0)
                })
                $('span').click(function () {
                    $('iframe').contents().find('p').css('margin', 0)
                })

            })
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

    function on_Cancel() {
        window.close()
        return
    }
    function Validate() {
        if (GetContent('Description') == '') {
            alert('Please fill in description.')
            return false
        }
        if ($('input[type=checkbox]:checked', $('#holder')).length != 0 && $.trim($('#Warning').text()) == '' && $.trim($('#Caution').text()) == '') {
            alert('Please select a Warning or Caution for Hazard')
            return false
        }
        return true
    }
    function GetContent(id) {
        var s = tinyMCE.get(id).getContent()
        return $.trim($(s).text())
    }
    function InitFileUpload() {
        document.getElementById('file').addEventListener('change', function (e) {
            var file = this.files[0];
            var src = $('#file')
            if (!ValidateFileUpload(file) ) {
                this.files.length = 0
                return
            }
            if ($.trim(src.val()) == '') {
                Photo = ''
                return 
            }
            ext = src.val().split('.').pop();
            Photo = guid() + '.' + ext
            src.attr('name', Photo)
            var xhr = new XMLHttpRequest();
            xhr.file = file; // not necessary if you create scopes like this
            xhr.addEventListener('progress', function (e) {
                var done = e.position || e.loaded, total = e.totalSize || e.total;
                // alert('xhr progress: ' + (Math.floor(done/total*1000)/10) + '%');
            }, false);
            if (xhr.upload) {
                xhr.upload.onprogress = function (e) {
                    var done = e.position || e.loaded, total = e.totalSize || e.total;
                    //  alert('xhr.upload progress: ' + done + ' / ' + total + ' = ' + (Math.floor(done / total * 1000) / 10) + '%');
                };
            }
            xhr.onreadystatechange = function (e) {
                if (4 == this.readyState) {
                    //alert(['xhr upload complete', e]);
                }
            };
            var url = window.location.toString().toLowerCase()
            url = url.replace('jobstep', 'upload')
            url += '&thumbnail=0'
            xhr.open('post', url, true);
            var formData = new FormData();
            formData.append(Photo, file);
            xhr.send(formData);//send(file)
        }, false);

    }
    function ValidateFileUpload(file) {
        name = file.name;
        size = file.size;
        type = file.type;
        var ext = name.split('.').pop().toLowerCase();
        if ($.inArray(ext, ['gif', 'png', 'jpg', 'jpeg']) == -1) {
            alert('invalid extension!');
            $('input:file').val('')
            $(this).wrap('<form>').parent('form').trigger('reset');
            $(this).unwrap();
            return false
        }
        //if ( size / 1024 > 200) {
        //    alert('The maximum size allowed for upload is 200KB.');
        //    $('input:file').val('')
        //    $(this).wrap('<form>').parent('form').trigger('reset');
        //    $(this).unwrap();
        //    return false
        //}
        return true
    }
    function on_Submit() {
        var arr = []
        if (!Validate())
            return
        var index = $('input:radio:checked').val()
        // arr.Description = $('#Description').text()
        arr.Description = tinyMCE.get('Description').getContent()
        arr.Type = Type[index]
        if (Obj != null && arr.Type == Obj.Type)
            arr.StepId = Obj.StepId
        //arr.Note = $('#Note').text()
        arr.Note = tinyMCE.get('Note').getContent()
        arr.Warning = $('#Warning').text()
        arr.Caution = $('#Caution').text()
        arr.Barrier = $('#Barrier').prop('checked') ? 'Yes' : ''
        arr.Prompt = $('#Prompt').val()
        if ($('input:radio[value=0]').prop('checked'))
            arr.Prompt = ''
        arr.Photo = Photo
       if (Photo != null)
           arr.Prompt=''
        var haz = []
        $('input[type=checkbox]:checked', $('#holder')).each(function (index) {
            haz.push($(this).val())
        })
        if (haz.length != 0) {
            arr.Hazard = haz.toString()
        }
        window.returnValue = arr;
        window.close();
        return
    }
    function on_AddWarning(src) {
        var s = $(src).parent().next().find('input:nth-child(1)').val()
        s = '<tr><td>Warning:<td>' + s + '</td></tr>'
        $(s).appendTo(t).find('td:nth-child(1)').css('text-align', 'right')

    }
    function __doPostBack(eventTarget, eventArgument) {
        if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
            theForm.__EVENTTARGET.value = eventTarget;
            theForm.__EVENTARGUMENT.value = eventArgument;
            theForm.submit();
        }
    }
</script>
