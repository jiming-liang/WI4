<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="WiByRig.aspx.cs" Inherits="WiByRig" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
h1{ font-size:20;}
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
<script type="text/javascript" src="wi.js"></script>
</HEAD>

<body style1='display:none'>
<form id="form1" runat="server">
<center ><h1  ></h1></center>
<center>
<b>Rig:</b><asp:TextBox runat="server" id="Rig" ></asp:TextBox> <asp:Button ID="Button1" runat="server" Text="Search" OnClick="Button1_Click" />
    <br />
 <asp:GridView ID="GridView1" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
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
    var width = 1100, height = 800
    var left = (screen.width / 2) - ((width / 2) + 10);
    var top = 50
    var size = 'dialogWidth:' + width + 'px; dialogHeight:' + height + 'px; dialogLeft:' + left + 'px;dialogTop:' + top + 'px;'
    var IsLocal
    $(document).ready(function () {
        var s = window.location.toString().toLowerCase()
        if (s.indexOf('islocal') != -1)
            IsLocal=true
        $('input', t).click(function () {
            var src = $(this)
            var td = src.closest('td')
            var status = td.prev().text()
            var rigId = td.next().text()
            var id = src.attr('id')
            var rig = src.attr('rig')
            var url = 'Create.aspx?'
            if (IsLocal) {
                if (status == 'New')
                    url += 'id=' + id + '&rigId=' + rigId
                else
                    url += 'localId=' + id

            } else
                url+='id='+id

            if (src.val() == 'Preview') {
                url += '&action=preview&rig=' + rig
            }
            alert(url)
            window.location = url
        })
        $('input a').each(function(){
            var src = $(this)
            src.attr("tabindex", -1);
        })
    })


</script>
