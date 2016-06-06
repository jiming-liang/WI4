<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Log.aspx.cs" Inherits="Log" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>

<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
<script type="text/javascript" src="wi.js"></script>
</HEAD>

<body style1='display:none'>
<form id="form1" runat="server">
<br /><br /><br /><br /><br />
<center ><h1  ></h1></center>
<br /><br />
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

<input id=h1 type=hidden  runat=server/>
<div id=holder></div>

</form>
</body>
</html>
<script>

    var t = $('#GridView1')
    var parent
    $(document).ready(function () {
        $('td:nth-child(3)', t).each(function(){

        })

    })

    
</script>
