<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BreadCrumbControl.ascx.cs" Inherits="BreadCrumbControl" %>
<style>
    .ActionRequired {
        border-style: solid;
        border-width: 2pt;
        border-color: pink;
    }
</style>
<table width="100%" border="0" cellpadding="0" cellspacing="0">
    <tr>
<td align="left">    <img id="ensco" style=" width: 120px; position: absole;" src="images/ensco.png">
<td align="center"  width="50%">
<div id=ActionRequired class="ActionRequired" style="display:none;background-color:pink;"><b>Action Required:</b> the global content of this work instruction has been updated. Please review the changes and update the local content accordingly.</div>

<td align="right">    <img id="wims"  src="images/wims.png" style="width:100px;">

</table>
<table border="0"  width="100%" style="background-image:url(images/bar.png); background-size:100% auto">
<tr><td>
        <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
        <asp:SiteMapPath ID="SiteMap1" runat="server">
							<PathSeparatorStyle Font-Names="Arial" Font-Bold="False" ForeColor="White" />
							<CurrentNodeStyle ForeColor="White" Font-Names="Arial" Font-Bold="true"/>
							<NodeStyle Font-Bold="False" ForeColor="White" Font-Names="Arial" />
							<RootNodeStyle Font-Bold="false" ForeColor="White" Font-Names="Arial"/>
        </asp:SiteMapPath>
    <td align="right" style="color:white">    <%=this.UserName %>
</table>
<table width="100%" border="0" >
<tr>
<td colspan="2" align="right">   
<span id=headerLogout><a  href=logout.aspx onclick1=on_Logout() >Logout</a>&nbsp|&nbsp<a target="_blank" href='https://team.enscoplc.com/RigStandards/WIProject/Shared%20Documents/3.0%20Develop/2.0%20Writing%20Style%20Guide/WI%20-%20Writing%20Style%20Guide.pdf' >Help</a>
&nbsp|&nbsp<a href="https://team.enscoplc.com/RigStandards/WIProject/Lists/WIMS%20Development%20Issue%20Tracking/AllItems.aspx" target=_blank >Issue Log</a></span>


</table>
