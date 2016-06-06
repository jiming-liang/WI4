<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
    body{font-family:Arial;
         font-size:9pt;
    }
    table{
        font-size:9pt;
    }
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
</HEAD>

<body >
<form id="form1" runat="server">
    
    <center>
        <img src="Images/ensco.png" />
    <table  width="35%" border="1" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
        <tr><td width="50%"  style="background-color:#F5F5F5" > 
            <table style="background-color:#F5F5F5" cellpadding="5" >
                <asp:Label ID="Msg" runat="server" Text="" ForeColor="Red"></asp:Label>
                <tr><td width="50%" align="right" >
               <b><asp:Label ID="Label1" runat="server" Text="User Id:"></asp:Label></b>
        <td>    <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
        <tr><td align="right">    <b><asp:Label  ID="Label2" runat="server" Text="Password:"></asp:Label></b>
        <td>    <asp:TextBox ID="Password" runat="server" ></asp:TextBox>

        <tr><td><td>       <asp:Button ID="Button1" BackColor="#666666"  ForeColor="White" runat="server" Text="Sign In" OnClick="Button1_Click" />
            </table>
        <td   style="padding:12px;"  ><font size="2"> <b>HR Service Center</b></font>


    
   
<br /><br /><br />
     <img src="images/stop.png" />
            
                Employees should log in using their Employee ID. If you experience problems logging in, or have not registered with Password Manager, please contact the Help Desk at <b>713-430-HELP (4357)  </b>


    </table>
    </center>
</form>
</body>
</html>
<script>
    $(document).ready(function () {
        //$('#Button1').css('border', '1px solid #D6D6D6')
        //$('td:nth-child(1)').css('text-align', 'right').width('50%')
        var l=100
        $('input:text').width(l)
        $('input:password').width(l)
        $('input:text').eq(0).focus()
    })

  </script>
