<%@ Page Language="C#" enableEventValidation="false" ValidateRequest="false"  EnableViewStateMac="false" AutoEventWireup="true" CodeFile="CopyWI.aspx.cs" Inherits="CopyWI" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
    body {
    padding:50px;
    font-size: 10pt;
    font-family: arial;
}
        p {
        margin: 0;
    }
    table, select, input {
    font-size: 10pt;
    font-family: arial;
}
textarea{
    background-color:lightgray;
}
td{
    padding-left:1px;
    border-left: 1px solid #CCC;
    border-right: 1px solid #CCC;
    border-top: 1px solid #CCC;
    border-bottom: 1px solid #CCC;
}
.label{
   font-weight:bold;
}

</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="WI1.js"></script>



<body style1='display:none'>
<form id="form1" runat="server">
<br />
   <center> <h1>Work Instruction Template</h1></center>
<table border="1"  width="100%" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
<tr><td> <b>Job Title: </b><asp:Label ID="JobDescription" runat="server"></asp:Label>
<td> <b>Facility:</b><asp:Label ID="Facility" runat="server"></asp:Label><input type="button" onclick ="on_Facility(this)" value='Edit'/>
<td>
    <table width="100%" ><tr>
    <td width="50%"><b>Equipment Type: </b><br /><asp:DropDownList runat="server"  id="EquipmentType" ></asp:DropDownList>
    <td> <b>Equipment Make: </b><br />    <asp:DropDownList Width="100%"  runat="server" id="EquipmentMake" ></asp:DropDownList>
    </table>
<tr><td colspan="2" style="width:50%"> <b>Status:  </b>
    <asp:RadioButtonList RepeatDirection="Horizontal" runat="server" ID="Status">
        <asp:ListItem  Selected="True" Text="In Progress" > </asp:ListItem>
        <asp:ListItem  Text="In Testing" ></asp:ListItem>
        <asp:ListItem  Text="Under Review" ></asp:ListItem>
            <asp:ListItem  Text="Approved" ></asp:ListItem>
    </asp:RadioButtonList>
<td> <b>Work Instruction Template No.:  </b><asp:Label  ID="WiNo" runat="server" />
</table>
    <br />
    <b>Resouces</b>
<table id="tblResources"  border="1"  width="100%" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
<tr><td style="width:25%;background-color:gray" colspan="2"> <b>Type: </b><td  style="background-color:gray" > <span class="label">Description: </span>   
<tr><td nowrap style="border-right: 0;"> Minimum Personnel Required <td  style="border-left: 0;"><input type="button" onclick ="on_Popup(this)" value='Edit'/> <td  id="resources" runat="server" /> 
<tr><td nowrap style="border-right: 0;"> Personal Protective Equipment <td style="border-left: 0;"><input type="button" onclick ="on_Popup(this)" value='Edit'/>&nbsp <td  id="PersonalProtectiveEquipment" runat="server" />  
<tr><td nowrap  style="border-right: 0;"> Equipment/Tools <td style="border-left: 0;"><input type="button" onclick ="on_Popup(this)" value='Edit'/>  <td  id="tool" runat="server" /> 
<tr><td style="border-left: 0;" colspan="3">&nbsp

    </table>
<table id="tblGeneralPrecautions" width="100%" border="1" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
        <tr><td colspan="1"   ><b>General Precautions: </b><td><asp:TextBox Width="100%" rows="5" runat="server" ID="GeneralPrecautions" TextMode="MultiLine"></asp:TextBox>
        <tr><td colspan="1"><b>Local Precautions: </b><td colspan="1"><asp:TextBox Width="100%" rows="5" runat="server" ID="LocalPrecautions" TextMode="MultiLine"></asp:TextBox>
</table>
      <table id="tblCriticality" width="100%" border="1" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
          <tr>
              <td style="width:5%"  nowrap ><b>Job Criticality:  <b style="color:rgb(232, 109, 31)">*</b></span> <asp:DropDownList runat="server"  id="Criticality" ></asp:DropDownList>
              <td style="width:5%"  nowrap><b>Permit Required: </b><asp:CheckBox runat="server"  text=""  id="Permit" />
              <td style="width:5%"  nowrap><b>Associated Work Instructions: </b> <input type="button" onclick ="on_Popup(this)" value='Edit'/>&nbsp  

              <td style="width:30%" id="WI" runat="server" />
              <td style="width:5%"  nowrap><b>Associated Links: </b> <input type="button" onclick ="on_PopupLink(this)" value='Edit'/>&nbsp    
              <td style1="width:30%" id="WILink" runat="server" />
           
    </table>
    <br />
<center><input type="button"  onclick="on_Add()" value="Add Job Step" />
    &nbsp&nbsp&nbsp<input type="button" id=SaveButton onclick="on_Save()" value="Save" />
    &nbsp&nbsp&nbsp<input type="button" id="Preview" onclick="on_Preview()" value="Preview In PDF" />
    &nbsp&nbsp&nbsp<input type="button" id="PreviewWord" onclick="on_PreviewWord()" value="Preview In Word" />
    &nbsp&nbsp&nbsp<input type="button" id=Duplicate onclick ="on_Duplicate(this)" value='Duplicate'/>
</center>

    <div id=holder></div>
<table id="t" runat="server" border="1"  width="100%" cellspacing="0" cellpadding="0">
<colgroup ><col style="width:10%;text-align:left" /><col /> </colgroup>
</table>

 <asp:GridView ID="GridView1" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle  Font-Size=""/>     
     <AlternatingRowStyle  />  
</asp:GridView>  

<br /><input type="button"  onclick="on_UploadAttachment()" value="Add Attachment" />
<asp:GridView ID="GridViewAttachment"  ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle HorizontalAlign="Left"  Font-Size=""/>       
</asp:GridView> 
<table id="id1" runat="server" ></table>


<input id=h1 type=hidden  runat=server/>
<input id=MappingXmlHidden type=hidden  runat=server/>

    <img id=help  />
<input type="hidden" id="WiRole"  runat="server"/>
<input type="hidden" id="Duplicate"  runat="server"/>
<input type="hidden" id="JobDescriptionId"  runat="server"/>
<input type="hidden" id="wiid"  runat="server"/>
</form>
</body>
</html>
    
    