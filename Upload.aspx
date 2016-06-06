<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Upload.aspx.cs" Inherits="Upload" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="WI.js"></script>
</HEAD>

<body >
<form id="form1" runat="server">
<h1 runat="server"  id="MyTitle"></h1>

    <br /><br />
<table id=t  width="100%" >
<colgroup  ><col width="50%"   style="text-align1: right"  /><col /></colgroup>

<tr><td >    Select a file <td><input type="file"  id="file" />
<tr><td >  Create prompt<td><input id="Prompt"  type="text" />
<tr id="tr1"><td>    Title <td> <asp:TextBox ID="PhotoTitle" runat="server" ></asp:TextBox>  
<tr><td>        <input type="button" onclick="on_Submit(this)"  value="Upload" /> 
    <td><input type="button" onclick="on_Cancel(this)"  value="Cancel" /> 

</table>


<input id="Guid" type="hidden"  runat="server" />
</form>
</body>
</html>
<script>
    var theForm = document.forms['form1']
    var t = $('#t')
    var photo = ''
    var IsPdf = false
    var PDFs = getParameterByName('PDFs')
    var WiType
    var Img = ''
    $(document).ready(function () {
        $(document).prop('title', 'Attachment' + $(document).prop('title'))
        WiType = getParameterByName('WiType')

        var data = window.dialogArguments
        if (data != null && data != 'dummy') {
            $('#PhotoTitle').val(data.title)
            photo = data.photo
            if (photo.indexOf('.pdf') != -1)
                IsPdf = true
            $('#Prompt').val(data.prompt)
        }
        //$('#PhotoTitle').val(getParameterByName('title'))
        //Img = getParameterByName('img')
        //$('#Prompt').val(getParameterByName('prompt'))
        if (WiType=='WI')
            $('#Prompt').closest('tr').hide()
        $('a').hide()
        // InitFileUpload()
        $('td:nth-child(1)').css('text-align', 'right')
        //if (data == null)
        //    $('#tr1').hide()
        var name = $('#Guid').val()
        var title = $('#Title1').val()
        if (name != null && name != '') {
            window.returnValue = name
            window.open('', '_self', '');
            window.close()
        }
        $('input[type=file]').click(function () {
            //if ($.trim($('#Prompt').val()) != '' && WiType!='WI')
            //    return false
        })
        $('#Prompt').keydown(function () {
            //if ($('#file').val() != '' && WiType!='WI')
            //    return false
        })
        $('input[type=file]').change(function () {
            var file = this.files[0];
            var name = file.name;
            var size = file.size;
            var type = file.type;

            var limit = 200
            if ($('#PhotoTitle').is(':visible'))
                limit = 10000
            
            if (!Validate(name, size, limit) ) {
                $(this).wrap('<form>').parent('form').trigger('reset');
                $(this).unwrap();
                return false
            }
            var file = this.files[0];
            var src = $('#file')
            ext = src.val().split('.').pop();
            photo = guid() + '.' + ext
            src.attr('name', photo)
            var xhr = new XMLHttpRequest();
            xhr.file = file; // not necessary if you create scopes like this

            xhr.onreadystatechange = function (e) {
                if (4 == this.readyState) {
                }
            };
            var url = window.location.toString().toLowerCase()
            url = url.replace('jobstep', 'upload')
            xhr.open('post', url, true);
            var formData = new FormData();
            formData.append(photo, file);
            xhr.send(formData);//send(file)
        });
    })
    function Validate(name, size, limit) {
        var ext = name.split('.').pop().toLowerCase();
        if ($.inArray(ext, ['gif', 'png', 'jpg', 'jpeg', 'pdf']) == -1) {
            alert('invalid extension!');
            return  false
        }
        if (!IsPdf && name.indexOf('.pdf') != -1) {
            if (PDFs == 3) {
                alert('The max number of the PDF files allowed is 3.')
                return false
            }
        }
        if (name.indexOf('.pdf') != -1) {
            limit = 3000
            if (size / 1024 > limit) {
                var s = 'The max size for the upload is '
                if (limit >= 1000)
                    s += (limit / 1000) + ' megabyte'
                else
                    s += limit + ' kilobyte'
                alert(s)
                return false
            }
        }
        return true 
    }
    function on_Cancel() {
        window.close()
        return
    }
    function on_Submit() {
        var src = $('input:file').eq(0) // $(this)
        var title = $.trim($('#PhotoTitle').val())
        if ($('#PhotoTitle').is(':visible') && title == '') {
            alert('Please select a title')
            return
        }
        var prompt = $.trim($('#Prompt').val())
        //if (photo!='') prompt=''
        if (src.val() != '' || prompt != '' || photo !='') {
            var arr = []
            arr.push({ name: photo, title: title, prompt: prompt });
            //  __doPostBack('Submit', '')
            window.returnValue = arr;
            window.close();
            return
        } else {
            alert('Please select a file')
            return
        }
    }

</script>
