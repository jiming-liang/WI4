<%@ Page Language="C#" enableEventValidation="false" ValidateRequest="false"  EnableViewStateMac="false" AutoEventWireup="true" CodeFile="SendWorkflow.aspx.cs" Inherits="SendWorkflow" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
body {
 font-family:Arial;
}
.label {
 font-size:11pt;font-weight:bold;vertical-align:top
}
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
<script type="text/javascript" src="WI.js"></script>
</HEAD>

<body style1='display:none'>

    <form id="form1" name="form1"  data-ajax="false" method="post" runat="server" enctype="multipart/form-data" >
        <br />
<center ><h1 ></h1></center>

<table id=t  width="100%" cellpadding="5">
<tr><td width="1px" colspan="1" class="label">Describe requested change to global content:<span style="color:rgb(232, 109, 31)" >*</span> 
<tr>   <td><asp:TextBox ID="Comment" Width="90%" Height="100px" runat="server" TextMode="MultiLine"></asp:TextBox>
<%--     <td1> <textarea  id="Description"   runat="server"></textarea>--%>

 
<tr><td colspan="2" ><table  width="100%" border="0" id="t1" cellspacing="0"  cellpadding="1" style="border-collapse:collapse;"></table>
<tr><td colspan="2">
    <input type="button" onclick="on_Submit(this)"  value="Submit" /> 
    <input type="button" onclick="on_Cancel(this)"  value="Cancel" /> 
</table>
</form>
</body>
</html>

<script>
    var theForm = document.forms['form1']
    var t = $('#t')
    var t1 = $('#t1')
    var Photo
    $(document).ready(function () {
        var obj = window.dialogArguments
        if (obj == null) {
            window.open('', '_self', '');
            window.close()
        }
        //SetupTinyMCE()
        //InitTinyMCE()
        //InitFileUpload()
        AddAttachment()
        $(document).prop('title', 'Submit Feedback on Global Content' + $(document).prop('title'))
    })
    function AddAttachment() {
        var s = '<tr><td align=left nowrap width=1% >Add Attachment:<td><input type=file />'
         $(s).appendTo(t1)
        
        $('input[type=file]').change(function () {
            var src1 = $(this)
            src1.attr('name', guid())
            s = '<input type=button onclick=on_AttachmentDelete(this) value=Delete />'
            src1.parent().prev().html(s).css('text-align', 'right')
            AddAttachment()
        });
    }
    function on_AttachmentDelete(src0) {
        var src = $(src0)
        src.parent().parent().remove()
    }
    function on_Cancel() {
        window.close()
        return
    }
    function Validate() {
        if ($.trim($('#Description')) == '') {
            //if (GetContent('Description') == '') {
            alert('Please fill in description.')
            return false
        }
        return true
    }
    function GetContent(id) {
        var s = tinyMCE.get(id).getContent()
        return $.trim($(s).text())
    }
    function on_Submit() {
        var arr = []
        if (!Validate())
            return 
        __doPostBack('', "")
        window.returnValue = arr;
        window.close();
        return
    }
</script>
