<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Status.aspx.cs" Inherits="Status" %>
<%@ Reference Page="WIBase.aspx" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" 
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %> 
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
body {
 font-family:Arial;font-size:10;color1:rgb(0, 70, 127)
}
.class1{
    color:rgb(0, 70, 127);font-size:11pt;
}
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <link  rel="stylesheet"  href="js/jquery-ui.css"></link>
    <script type="text/javascript" src="js/jquery-ui.js"></script>
<script type="text/javascript" src="wi.js"></script>
</HEAD>

<body style1='display:none'>
<form id="form1" runat="server">

<table width="80%" id="t">
<colgroup  ><col width1="10%"  /><col /><col /><col /></colgroup>
<tr><td class="class1"  ><b>Report Type: </b>  <td > <asp:DropDownList ID="ReportType"   runat="server"> </asp:DropDownList>
<tr><td  class="class1" colspan="4">&nbsp
<tr><td >Job Category:  <td > <asp:DropDownList ID="WICategory"   runat="server"  AutoPostBack="true" OnSelectedIndexChanged="WICategory_SelectedIndexChanged" > </asp:DropDownList>
<td >Facility Type:  <td > <asp:DropDownList ID="RigType"   runat="server" AutoPostBack="true" OnSelectedIndexChanged="RigType_SelectedIndexChanged" > </asp:DropDownList>
<td  >Work Instruction Type:  <td> <asp:DropDownList ID="WiType"   runat="server"   > </asp:DropDownList>

<tr><td >Job Title:  <td > <asp:DropDownList ID="JobDescription"   runat="server"   > </asp:DropDownList>

<td >Rig Design:  <td > <asp:DropDownList ID="RigDesign"   runat="server"   AutoPostBack="true" OnSelectedIndexChanged="RigDesign_SelectedIndexChanged"> </asp:DropDownList>
<tr><td ><td ><td >Rig:  <td > <asp:DropDownList runat="server" ID="Rig" ></asp:DropDownList> 
    <tr><td colspan="4">&nbsp
  <tr><td >
    From:
    <td colspan="3"><asp:TextBox ID="FromDate" runat="server" TabIndex="10" Enabled=true  ></asp:TextBox>
    
   <tr><td >To:   
    <td><asp:TextBox ID="ToDate" runat="server" TabIndex="10" Enabled=true></asp:TextBox>

<tr><td><td colspan="3" > <asp:Button ID="Submit" runat="server"  Text="Run Report" OnClick="Submit_Click" /> 
    <input type="button"  onclick=" window.location='status.aspx'"  value="Cancel" />
    <asp:Button ID="Export" runat="server"  Text="Export to Excel" OnClick="Export_Click" /> 
    
</table>
    <div id="DivPieChart" runat="server"></div>
 <asp:GridView ID="GridView1" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="11pt" ForeColor=White BackColor= darkblue />
<RowStyle HorizontalAlign="Left"  Font-Size=""/>       
</asp:GridView>  
<input id=h1 type=hidden  runat=server/>
<div id=holder></div>

</form>
</body>
</html>
<script>
    var t = $('#GridView1')
    var img = $('#ctl00')
    var t0 = $('#t')
    var IsLocal
    $(document).ready(function () {
     //   return 
        $("#FromDate").datepicker({ dateFormat: 'd-M-y' });
        $('#ToDate').datepicker({ dateFormat: 'd-M-y' });
        $('td', t0).css('text-align', 'right')
        $('select,input').parent().css('text-align', 'left')
        $('select').width($('#RigType').width())
        $('td:nth-child(8)', t).remove()
        $('th:nth-child(8)', t).remove()
        if (img.length!=0) {
            var offset = t.offset();
            img.offset({ top: offset.top, left: offset.left + t.width()+10 })
        }
        if ($('#ReportType').val() != 'WI Printed') {
            $('tr:last', t).find('td').eq(0).text('Total')
            $('tr:last', t).css("font-weight", "bold");
        }
    })
</script>