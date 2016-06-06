<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >

<head>
    <style>
        body {
            font-family: Arial;
            font-size: 10;
        }
        p{
            line-height:1.8;
        }
.box {
            border: 1px solid black;text-align:top;
        }

        table {
            border-collapse1: collapse; 
        }
        .header {
            font-size: 18pt;color: rgb(232, 109, 31);
            font-weight: bold;
        }
    </style>
    <script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="wi.js"></script>
</head>

<body style1='display:none'>
    <form id="form1" runat="server">
        <br />
        <center>
            <table width="80%" id="t" cellpadding="40" cellspacing="10">

                <tr>
                    <td class="box"><p>
                        <img src="images/admin.png" width="40" /><span class="header">Administration </span>
                        <br />   <a href="Mapping.aspx">Create Work Instruction Template</a>
                        <br />   <a href="Search.aspx">Work Instruction Template Library</a>
                        <br />    <a href="admin.aspx">System List Management</a>
                          <br />    <a href="users.aspx">User Management</a>
                        <br /><br />
                        </p>
                    <td class="box" valign="top" >
                        <p>
                        <img src="images/wi.png" width="40" /><span class="header">Work Instruction </span>
                        <br />   <a href="Search.aspx?IsLocal=1">Work Instruction Library</a>
                        </p>
                <tr>
                    <td class="box">
                        <p>
                        <img src="images/help.png" width="40" /><span class="header">Help             </span>
                        <br /> <a target="_blank" href='https://team.enscoplc.com/RigStandards/WIProject/Shared%20Documents/3.0%20Develop/2.0%20Writing%20Style%20Guide/WI%20-%20Writing%20Style%20Guide.pdf' >Online Help</a>

                        <br /><a href="http://esp.enscoplc.com" target="_blank"> Contact IT Support (Ensco Services Portal)</a>
                        </p>

                    <td class="box" valign="top">
                        <p>
                        <img src="images/reporting.png" width="40" /> <span class="header">Reporting </span>

                        <br /><a href="status.aspx">	Work Instruction Status Report  </a>
                        <br /><a href="Feedback.aspx">    Work Instruction Template Feedback Log</a>
                        <br /><a href="STG.aspx">	Strategic Team Goal Tracking </a>
                       </p>
</table>
        </center>
        <input id=h1 type=hidden runat=server />
        <div id=holder></div>

    </form>
</body>
</html>
<script>

    var t = $('#t')
    var parent
    $(document).ready(function () {
        $('td', t).css('text-align', 'left')
        if (!IsRole('Admin')) {
            RemoveLink('admin')
            RemoveLink('users')
        }
    })
    function RemoveLink(name) {
        var src = $('a[href*="'+name+'"]')
        src.prev().remove()
        src.remove()
    }

</script>
