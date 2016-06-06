<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="translation.aspx.cs" Inherits="translation" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >

<HEAD>
<style>
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="wi.js"></script>

</HEAD>

<body >
    
<form id="form1" runat="server">
    <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged">
    </asp:DropDownList>
    <asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="true">
    </asp:DropDownList>
    <asp:Button ID="Export" runat="server"  Text="Export" OnClick="Export_Click" OnClientClick="on_Export(this)" /> 

&nbsp;<asp:GridView ID="GridView1" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle  Font-Size=""/>     

</asp:GridView>  
<input id=h1 type=hidden  runat=server/>

</form>
</body>
</html>
<script>
    var t = $('#GridView1')
    $(document).ready(function () {

    })
    function on_Export() {
        var s = t[0].outerHTML
        s = escapeXml(s)
        $('#h1').val(s)
        SetTabIndex()
    }
    function SetTabIndex() {
        $('input,select,a ').each(function () {
            var $input = $(this);
            $input.attr("tabindex", -1);
        });

    }
  </script>
