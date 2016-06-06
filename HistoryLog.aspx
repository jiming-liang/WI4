<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="HistoryLog.aspx.cs" Inherits="HistoryLog" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
    .nowrap
{
white-space:nowrap;
}
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
<script type="text/javascript" src="wi.js"></script>
</HEAD>

<body style1='display:none'>
<form id="form1" runat="server">

<center ><h1  ></h1></center>
<center>
    <br />
 <asp:GridView   ID="GridView1" ShowHeaderWhenEmpty Width="90%" runat="server"   Caption="" Font-Size1 ="11pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="11pt" ForeColor=White BackColor= darkblue />
<RowStyle HorizontalAlign="Left"  Font-Size=""/>       
</asp:GridView>  
</center>   

<input id=h1 type=hidden  runat=server/>
<div id=holder></div>

</form>
</body>
</html>
<script>

    var t = $('#GridView1')
    $(document).ready(function () {
        $(document).prop('title', 'Version History Log' + $(document).prop('title'))
        var td = $('td:first', t)
        td.html('<a href="#">' + td.text() + '</a>')
        $('td:nth-child(1)', t).css('text-align', 'center')
        $('td:nth-child(2)', t).addClass('nowrap')

        $('a', t).click(function () {
            var url = window.location.toString().toLowerCase()
            var wiType = getParameterByName('wiType')
            url = url.replace('historylog', 'create')
            //if (wiType == 'wi')
            //    url=url.replace('id=', 'localId=')
            url+='&action=approved'
            window.returnValue = url
            window.close()
        })
    })

    
</script>
