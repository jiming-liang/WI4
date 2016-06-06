<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Feedback.aspx.cs" Inherits="Feedback" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>

<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
<script type="text/javascript" src="wi.js"></script>
</HEAD>

<body style1='display:none'>
<form id="form1" runat="server">
<center ><h1  >	Work Instruction Template Feedback Log</h1></center>
<center>
 <asp:GridView ID="GridView1" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle HorizontalAlign="Left"  Font-Size=""/>       
</asp:GridView>  
    <br /><br />
<asp:GridView ID="GridView2" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle HorizontalAlign="Left"  Font-Size=""/>       
</asp:GridView>  
</center>   

<div id=holder></div>

</form>
</body>
</html>
<script>

var t = $('#GridView1')

$(document).ready(function () {
    Init()
})
function Init() {
    if ($('tr', t).length == 0) return
    var index = GetColumnIndex('Work Instruction Template No.')
    $('td:nth-child(' + index + ')', t).each(function (i) {
        var src = $(this)
        src.html(src.text())
    })
    index = GetColumnIndex('Status')
    $('td:nth-child(' + index + ')', t).each(function (i) {
        var src = $(this)
        var name =src.next().text()
        var status = src.text()
        var s = CreateRadio(name, 'Open', status)
        s += CreateRadio(name, 'Completed', status)
        s += CreateRadio(name, 'Rejected', status)
        src.html(s)
    })
    $('input:radio').click(function () {
        var src = $(this)
        var name = src.attr('name')
        var status = src.attr('value')
        var sql="update Feedback set status='"+status+"' where id="+name
        CallAjax('sql', sql, 'runSql', function (data, status, xhr) {
        }, function (e) { alert(e.responseText); alert(s) }
        )
    })
    RemoveColumn('id', t )
}
function RemoveColumn(name, t) {
    var th = $('th', t).filter(function () {
        return $(this).text() === name
    })
    if (th.length == 0) return
    var index = th.index()
    $('td:nth-child(' + (index + 1) + ')', t).remove()
    th.remove()
}
function CreateRadio(name, value, status){
    if ($.trim(status)=='')
        status='Open'
    var checked=''
    if (status==value) checked='checked'
    return value + '<input type="radio"  ' + checked + ' name="' + name + '" value="' + value + '"/>&nbsp&nbsp&nbsp'
}
function GetColumnIndex(name) {
    var index
    $('th', t).each(function (i) {
        var src = $(this)
        if (src.text() == name)
            index = i + 1
    })
    return index
}
</script>
