<%@ Page Language="C#" enableEventValidation="false" ValidateRequest="false"  EnableViewStateMac="false" AutoEventWireup="true" CodeFile="Create.aspx.cs" Inherits="Create" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
    body {
}
        p {
        margin: 0;
    }
    table, select, input {
    font-size1: 10pt;
    font-family: arial;
}
textarea{
    background-color:lightgray;
}
td{
    padding-left:2px;
    padding-right:2px;
}
.td1{
    text-align:right;
    border-left: 0px solid #CCC;
    border-right: 0px solid #CCC;
    border-top: 0px solid #CCC;
    border-bottom: 0px solid #CCC;
    width:99%
}
.label{
   font-weight:bold;
}

</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="WI.js"></script>
<body style1='display:none'>
<form id="form1" runat="server">
<table id="tblHeader" border="1"  width="100%" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
<tr><td> <b>Job Title: </b><asp:Label ID="JobDescription" runat="server"></asp:Label>
<td> <b>Facility: </b><asp:Label ID="Facility" runat="server"></asp:Label><input type="button" onclick ="on_Facility(this)" value='Edit'/>
<td>
    <table width="100%" ><tr>
    <td width="50%"><b>Equipment Type: </b><br /><asp:DropDownList runat="server"  id="EquipmentType" ></asp:DropDownList>
    <td> <b>Equipment Make: </b><br />    <asp:DropDownList Width="100%"  runat="server" id="EquipmentMake" ></asp:DropDownList>
    </table>
<tr><td colspan="2" style="width:50%"> <b>Status:  </b><span id="StatusText" runat="server"></span>
    <asp:RadioButtonList RepeatDirection="Horizontal" runat="server" ID="Status">
        <asp:ListItem  Selected="True" Text="In Progress" > </asp:ListItem>
        <asp:ListItem  Text="In Testing" ></asp:ListItem>
        <asp:ListItem  Text="Under Review" ></asp:ListItem>
        <asp:ListItem  Text="Approved" ></asp:ListItem>
    </asp:RadioButtonList>
<td> <b>Work Instruction Template No.:  </b><asp:Label  ID="WiNo" runat="server" />
</table>
<table width="100%" cellpadding="6" cellspacing=0 border="0" class="td11"><tr><td class="td11">    <b>Resources</b>
     <td  class="td1">
<input type="button" id=EditButton onclick="window.location=$(this).attr('url')" value="Edit" />
<input type="button" id=SaveButton onclick="on_Save()" value="Save for Later" />
<input type="button" id=CancelButton onclick="on_Cancel()" value="Cancel" />
<input type="button" id=DuplicateButton onclick ="on_Duplicate(this)" value='Duplicate'/>    
<input type="button" id=SubmitForApprovalButton onclick ="on_SendWorkflow(this)" value='Submit for Approval'/>
<input type="button" id=SubmitForDisableButton onclick ="on_SendWorkflow(this)" value='Submit to Disable'/>
<input type="button" id=SubmitFeedbackButton onclick ="on_SubmitFeedback(this)" value='Submit Feedback on Global Content'/>
<input type="button" id=ApproveButton onclick ="on_SendWorkflow(this)" value='Approve'/>
<input type="button" id=RejectButton onclick ="on_SendWorkflow(this)" value='Reject'/>
<input type="button" id="ConvertButton" onclick="on_Convert()" value="Convert" />

    </table>

<table id="tblResources"  border="1"  width="100%" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
<tr><td style="width:25%;background-color:gray" colspan="2"> <b>Type </b><td  style="background-color:gray" > <span class="label">Description</span>   
<tr><td nowrap style="border-right: 0;"> Minimum Personnel Required <td  style="border-left: 0;"><input type="button" onclick ="on_Popup(this)" value='Edit'/> <td  id="resources" runat="server" /> 
<tr><td nowrap style="border-right: 0;"> Personal Protective Equipment <td style="border-left: 0;"><input type="button" onclick ="on_Popup(this)" value='Edit'/>&nbsp <td  id="PersonalProtectiveEquipment" runat="server" />  
<tr><td nowrap  style="border-right: 0;"> Equipment/Tools <td style="border-left: 0;"><input type="button" onclick ="on_Popup(this)" value='Edit'/>  <td  id="tool" runat="server" /> 
<tr><td style="border-left: 0;" colspan="3">&nbsp

    </table>
<table id="tblGeneralPrecautions" width="100%" border="1" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
        <tr><td style="width: 5%; white-space: nowrap;"  ><b>General Precautions: </b> <input type="button" onclick="on_Precaution(this)" value="Edit" />
            <td id="GeneralPrecautions" runat="server"/>
        <tr><td nowrap><b>Local Precautions: </b><input id="LocalPrecautionsButton" type="button" onclick="on_Precaution(this)" value="Edit" />
        <td id="LocalPrecautions" runat="server"/>  
</table>
      <table id="tblCriticality" width="100%" border="1" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
          <tr>
              <td style="width:5%"  nowrap ><b>Job Criticality:  <b style="color:rgb(232, 109, 31)">*</b></span> <asp:DropDownList runat="server"  id="Criticality" ></asp:DropDownList>
              <td style="width:5%"  nowrap><b>Permit Required: </b><asp:CheckBox runat="server"  text=""  id="Permit" />
              <td style="width:5%"  nowrap><b>Associated Work Instruction Template: </b> <img width="20px" src="images/AssociatedWI.png" /><input type="button" onclick ="on_Popup(this)" value='Edit'/>&nbsp  

              <td style="width:30%" id="WI" runat="server" />
              <td style="width:5%"  nowrap><b>Associated Link: </b> <img width="20px" src="images/AssociatedWI.png" /><input type="button" onclick ="on_PopupLink(this)" value='Edit'/>&nbsp    
              <td style1="width:30%" id="WILink" runat="server" />
           
    </table>
    <br />
<center><input type="button"  onclick="on_Add()" value="Add Job Step" />
    &nbsp&nbsp&nbsp<input type="button" id="Preview" onclick="on_Preview()" value="Preview In PDF" />
    &nbsp&nbsp&nbsp<input type="button" id="PreviewWord" onclick="on_PreviewWord()" value="Preview In Word" />
</center>

    <div id=holder></div>
<table id="t" runat="server" border="1"  width="100%" cellspacing="0" cellpadding="0">
<colgroup ><col style="width:10%;text-align:left" /><col /> </colgroup>
</table>

 <asp:GridView ID="GridView1" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle  Font-Size=""/>     

</asp:GridView>  
<br /><input type="button"  onclick="on_UploadAttachment()" value="Add Attachment" />
<asp:GridView ID="GridViewAttachment"  ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle HorizontalAlign="Left"  Font-Size=""/>       
</asp:GridView> 
    <table id="Attach" runat="server" />
    <div id="DivAttachment" runat="server"></div>
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
<script>
    var theForm = document.forms['form1']
    var t, t0, t1
    var width = 1200, height = 900
    var left = (screen.width / 2) - ((width / 2) + 10);
    var top = 50
    var size = 'dialogWidth:' + width + 'px; dialogHeight:' + height + 'px; dialogLeft:' + left + 'px;dialogTop:' + top + 'px;'
    var url = 'jobStep.aspx'
    var root = $('<wi/>')
    var Drag, HazardDrag
    var WiType ='WIT'
    var Lockdown = false;
    var Dict = [] 
    var LessThanEncode = '<% =LessThanEncode%>'
    var AmpEncode = '<% =AmpEncode%>'
    var RigId = <% =this.RigId%>;
    var IsDirty=false

    var UserAccessRigIdList = '<% =this.UserAccessRigIdList%>';
    $(document).ready(function () {
        var s = window.location.toString().toLowerCase()        
        $('#help').hide()
        t = $('#GridView1');
        t0 = $('#t0')
        t1 = $('#GridViewAttachment')
        $('td:nth-child(1)', t0).css('text-align', 'right')
        WiType=getParameterByName('WiType').toUpperCase()
        if (WiType=='WI'){//  s.indexOf('rigid') != -1 || s.indexOf('localid') != -1) {
            InitLocal()
            //if ( s.indexOf('localid') == -1)
            //    $('#ConvertButton').hide()
        }
        if (WiType=='WIS'){//s.indexOf('wisid') != -1 ) {
            RenameWILocalLabel()
        }
        url += '?WiType=' + WiType
        InitSteps()
        FormatSteps()
        InitAttachments()
        $('input:file').width(150)
        $('#EquipmentType').change(function () {
            var id=$(this).val()
            RefreshEquipmentMake(id)
            IsDirty = true
        })
        $('#EquipmentMake').change(function () {
            IsDirty = true
        })
        LockdownSecurity()
        DragDrop()
        SetTabIndex(true)
        <% if (this.ActionRequired=="1")
            this.Response.Write(" $('#ActionRequired').show()");
        %>
        $('#Preview').focus()
        ConfigSave()
        RestoreScroll()
    })
    function ConfigSave(){
        document.onkeydown = function(){
            if ((event.keyCode == 83) && (event.ctrlKey)){
                event.cancelBubble = true;
                event.returnValue = false;
                event.keyCode = false;                
//                if ( !$('#SaveButton').prop('disabled') && $('#SaveButton').is(":visible"))
                    on_Save()
                return false;
            }
        }
    }
    function RestoreScroll(){
        var scroll=getCookie('scroll')
        if ( scroll!= null ) 
            $(document).scrollTop(scroll );
    }
    function SaveScroll(){
        setCookie('scroll', $(window).scrollTop() );
    }
    function InitLocal() {
        HideControls('tblHeader')
        HideControls('tblResources')
        HideControls('tblGeneralPrecautions')
        HideControls('tblCriticality')
        $('#LocalPrecautionsButton').show()
        $('#DuplicateButton').hide()
        $('#Permit').prop('disabled', true)
        $('table').eq(0).find('input, select').prop('disabled', true)
        var src=$('a:contains("Work Instruction Library")')
        src.text('Work Instruction Library').attr('href', src.attr('href')+'?isLocal=1')
        RenameWILocalLabel()
    }
    function RenameWILocalLabel(){
        $('#WiNo').prev().text('Work Instruction No.: ')
        $('#tblCriticality').find('td').eq(2).find('b').text('Associated Work Instruction: ')
    }
    function InitBreadCrumb() {
        if (WiType=='WIT')return 
        var span = $('table').eq(1).find('span').eq(0)
        var a=span.find('span').eq(2).find('a')
        var url=a.attr('href')
        a.text('Work Instruction Library').attr('href', url+'?IsLocal=1')
        span.find('span').eq(4).text('Work Instruction')
    }
    function HideControls(id){
        $('input:button', $('#'+id)).hide()
        $('select', $('#'+id)).prop('disabled', true )
    }
    function IsLocalRig() {
        if (WiType=='WIS')
            return true
        if (RigId==0)
            return false 
        for(var i = 0; i < Roles.length; i++) {
            if ( (Roles[i][0]=='Local' || Roles[i][0]=='User') &&  Roles[i][1]==RigId)
                return true 
        }
        return false 
    }
    function LockdownSecurity() {
        InitBreadCrumb()
        if (LockdownSecurityApproved()) return 
        $('#ApproveButton').hide();
        $('#RejectButton').hide();        
        if (WiType=='WIT' ){//Global
            var status = $('input[name=Status]:checked').val()
            if (status == 'Approved'||(!IsRole('Admin') && !IsRole('Global'))) {
                $('input').hide()
                $('input:radio').attr('disabled', true).show()
                $('select').attr('disabled', true)
                Lockdown =true
            } 
            $('#SubmitFeedbackButton').hide()
            $('#SubmitForApprovalButton').hide()
            if (IsRole('Admin')|| (IsRole('Global') && !$('input:radio[value=Approved]').prop('checked') ) ){ 
                $('input:radio').show()
                $('input:radio').attr('disabled', false)
                $('#SaveButton').show()
                if (!IsRole('Admin'))
                    $('input:radio[value=Approved]').attr('disabled', true)
            }
            if (IsRole('Admin', 'Global'))
                $('#DuplicateButton').show()
            $('#ConvertButton').hide()
            $('#SubmitForDisableButton').hide()
            $('#CancelButton').show()
        }else{//Local
            var StatusText=$('#StatusText').text()
            if ( StatusText=='Draft' && !IsLocalRig() && !IsRole('Admin') && !IsRole('Global'))
                $('input:button').hide()
            if (StatusText.indexOf('Pending')>-1 ) //|| StatusText=='Approved')
                $('input:button').hide()
            <% if (this.IsApprover  )
                 this.Response.Write(" $('#ApproveButton').show(); $('#RejectButton').show();");
            %>
            if (IsRole('Admin') && StatusText.indexOf('Pending')!=-1) { 
                $('#ApproveButton').show()
                $('#RejectButton').show()
            }
            $('#SubmitForApprovalButton').hide()
            if (IsRole('Admin') ||IsLocalRig())
                $('#SubmitForApprovalButton').show()
            if (StatusText.indexOf('Pending')!=-1)// !='Draft')
                $('#SubmitForApprovalButton').hide()
            $('#SubmitFeedbackButton').show()
            //InitBreadCrumb()
            if (WiType=='WIS'){
                $('#SubmitFeedbackButton').hide()
                $('#GeneralPrecautions').prev().find('input').hide() 
            }else{
                $('#SubmitForDisableButton').hide()
                $('#WI').prev().find('input').show()
                $('#WILink').prev().find('input').show()
            }
        }
        $('#Preview').prop('disabled', false).show()
        $('#PreviewWord').prop('disabled', false).show()
        $('input:hidden').attr('disabled', false)
        if (!IsRole('Admin', 'Global') && !IsRole('Local')) {
            $('#PreviewWord').hide()
        }
        $('input:radio').click(function(){
            var src=$(this)
            if (src.val()=='Approved' && !IsRole('Admin'))
                return false 
        })
    }
    function LockdownSecurity121915() {
        InitBreadCrumb()
        if (LockdownSecurityApproved()) return 
        $('#ApproveButton').hide();
        $('#RejectButton').hide();
        if (WiType=='WIT' ){//Global
            var status = $('input[name=Status]:checked').val()
            if (status == 'Approved'||(!IsRole('Admin') && !IsRole('Global'))) {
                $('input').attr('disabled', true)
                $('select').attr('disabled', true)
                Lockdown =true
            } 
            $('#SubmitFeedbackButton').hide()
            $('#SubmitForApprovalButton').hide()
            if (IsRole('Admin')|| (IsRole('Global') && !$('input:radio[value=Approved]').prop('checked') ) ){ 
                $('input:radio').attr('disabled', false)
                $('#SaveButton').attr('disabled', false)
                if (!IsRole('Admin'))
                    $('input:radio[value=Approved]').attr('disabled', true)
                $('#DuplicateButton').attr('disabled', false)
            }
            $('#ConvertButton').hide()
        }else{//Local
            var StatusText=$('#StatusText').text()

            if ( StatusText=='Draft' && !IsLocalRig() && !IsRole('Admin') && !IsRole('Global'))
                $('input:button').hide()
            if (StatusText.indexOf('Pending')>-1 ) //|| StatusText=='Approved')
                $('input:button').hide()
            <% if (this.IsApprover  )
                 this.Response.Write(" $('#ApproveButton').show(); $('#RejectButton').show();");
            %>
            if (IsRole('Admin') && StatusText.indexOf('Pending')!=-1) { 
                $('#ApproveButton').show()
                $('#RejectButton').show()
            }
            $('#SubmitForApprovalButton').hide()
            if (IsRole('Admin') ||IsLocalRig())
                $('#SubmitForApprovalButton').show()
            if (StatusText.indexOf('Pending')!=-1)// !='Draft')
                $('#SubmitForApprovalButton').hide()
            $('#SubmitFeedbackButton').show()
            //InitBreadCrumb()
            if (WiType=='WIS'){
                $('#SubmitFeedbackButton').hide()
                $('#GeneralPrecautions').prev().find('input').hide() 
            }
        }
        $('#Preview').prop('disabled', false).show()
        $('#PreviewWord').prop('disabled', false).show()
        $('input:hidden').attr('disabled', false)
        if (!IsRole('Admin') && !IsRole('Global') && !IsRole('Local')) {
            $('#PreviewWord').prop('disabled', true)
        }
        $('input:radio').click(function(){
            var src=$(this)
            if (src.val()=='Approved' && !IsRole('Admin'))
                return false 
        })
    }
    function LockdownSecurityApproved(){
        $('#EditButton').hide()
        var action=getParameterByName('action')
        if (action=='') return false
        $('input').hide()
        $('input:checkbox').show().prop('disabled', true)
        $('input:radio').show().prop('disabled', true)
        $('select').prop('disabled', true)
        $('#Preview').show()
        if ( WiType=='WI' &&  IsLocalRig())
            $('#PreviewWord').show()
        if ( IsRole('Admin') || IsRole('Global') )
            $('#PreviewWord').show()
        return true 
    }
    function LockdownSecurityApproved121915(){
        $('#EditButton').hide()
        var action=getParameterByName('action')
        if (action=='') return false
        $('input').attr('disabled', true)
        $('#Preview').prop('disabled', false).show()
        if ( WiType=='WI' &&  IsLocalRig())
            $('#PreviewWord').prop('disabled', false).show()
        if ( IsRole('Admin') || IsRole('Global') )
            $('#PreviewWord').prop('disabled', false).show()
        return true 
    }
    function DragDrop2() {
        if (Lockdown)
            return 
        t[0].onselectstart = handleSelectAttempt;
        t[0].onmousedown = handleSelectAttempt;
        $('td:nth-child(1)', t).each(function () {
            $(this)[0].onselectstart = handleSelectAttempt;
            $(this)[0].onmousedown = handleSelectAttempt;
        })
        t1[0].onselectstart = handleSelectAttempt;
        t1[0].onmousedown = handleSelectAttempt;
        var IsMove, IsMove2
        $('body').mouseover(function (e) {
            var src = $(e.target)
            if (!ValidateTarget(src) ) {
                $("body").css("cursor", "default");
                return 
            }
            $("body").css("cursor", "pointer");
            //$('body').css('cursor', 'url(downarrow.png),auto');
        })
        $('body').mousedown(function (e) {
            var src = $(e.target)
            if (!ValidateTarget(src) ) {
                return
            }
            $("body").css("cursor", "move");
            Drag = $(e.target)
        })
        $('body').mouseup(function (e) {
            var src = $(e.target)
            IsMove=IsStandaloneMove(src)
            IsMove2=IsAttachmentMove(src)
            $("body").css("cursor", "default");
            if (src.is(Drag) || !FromSameTable(Drag, src) || (!ValidateTarget(src) && !IsMove && !IsMove2) ) {
                Drag = null
                return
            }            
            var tr = src.parent()
            if (IsMove)
                MoveStandalone(Drag.parent(), tr)
            else if (IsMove2)
                MoveAttachment(Drag.parent(), tr)
            else 
                Swap(Drag.parent(), tr)
            Drag = null
        })
    }
    function ValidateDrag(td){
        if (td.prop('tagName') != 'TD' ) return false 
        var tr=td.parent()
        var index=td.index()
        if (index==0 || index>2 || tr.children().eq(0).find('input').length<2)
            return false
        if (WiType!='WIT' && index==1)
            return false
        return true 
    }
    function ValidateDragAttachment(td){
        if (td.prop('tagName') != 'TD' ) return false 
        var tr=td.parent()
        var index=td.index()
        if (index!=1 ||  tr.children().eq(0).find('input').length==0)
            return false
        return true 
    }
    function DragDrop() {
        if (Lockdown)
            return 
        t[0].onselectstart = handleSelectAttempt;
        t[0].onmousedown = handleSelectAttempt;
        $('td:nth-child(1)', t).each(function () {
            $(this)[0].onselectstart = handleSelectAttempt;
            $(this)[0].onmousedown = handleSelectAttempt;
        })
        t1[0].onselectstart = handleSelectAttempt;
        t1[0].onmousedown = handleSelectAttempt;
        var TD=t.children('tbody').children('tr').children('td')
        var TD1=t1.children('tbody').children('tr').children('td')
        t.on('mouseover', function (e) {
            if (!ValidateDrag($(e.target)) ) {
                $("body").css("cursor", "default");
                return 
            }
            $("body").css("cursor", "pointer");
        })
        t.on('mousedown', function (e) {
           // window.status='down'
            var td = $(e.target)
            if (!ValidateDrag(td))
                return
            window.status='down'
           // tr.css("cursor", "move");
            Drag =td.parent()
        })
        t.on('mouseup', function (e) {
            var td = $(e.target)
            var tr=td.parent()
            $("body").css("cursor", "default");
            var index=td.index()
            if (index==0 || index>2 || !StepMatch(tr)){
                Drag = null
                return
            }     
            window.status='up'
            StepMove(Drag, tr)
            Drag = null
        })
        t1.on('mouseover', function (e) {
            if (!ValidateDragAttachment($(e.target)) ) {
                $("body").css("cursor", "default");
                return 
            }
            $("body").css("cursor", "pointer");
        })
        t1.on('mousedown', function (e) {
            var td = $(e.target)
            if (!ValidateDragAttachment(td))
                return
           // tr.css("cursor", "move");
            Drag =td.parent()
        })
        t1.on('mouseup', function (e) {
            var td =  $(e.target)
            var tr=td.parent()
            $("body").css("cursor", "default");
            var index=td.index()
            if (index!=1 || !StepMatchAttachment(tr)){
                Drag = null
                return
            }            
            StepMoveAttachment(Drag, tr)
            Drag = null
        })
    }
    function StepMatch(tr){
        if (tr.prop('tagName') != 'TR' ) return false 
        if (tr.is(Drag))return false
        var s0=$('td:gt(0):lt(2)', Drag).text()
        var s=$('td:gt(0):lt(2)', tr).text()
        if (s=='')return false 
        if (s0.indexOf('.')==-1 && s.indexOf('.')==-1)
            return true
        if (s0.indexOf('.')!=-1 && s.indexOf('.')!=-1){
            if (Math.floor(s0)==Math.floor(s))
                return true
        }
        return false 
    }
    function StepMove(tr1, tr2){
        if (!CheckLocalDragDrop(tr1, tr2))
            return 
        var arr1=tr1
        var direction='up'  
        if (tr1.index()<tr2.index())
            direction='down'
        if (tr1.index()!=1 && $('td:gt(0):lt(2)', tr1.prev()).text()=='')
            arr1=tr1.prev()
        var lastStep=GetLastStep(tr1)
        arr1=arr1.prev()
        arr1=arr1.nextUntil(lastStep.next())
        if (direction=='up'){
            if (tr2.index()!=1 && $('td:gt(0):lt(2)', tr2.prev()).text()=='')
                tr2=tr2.prev()
            arr1.insertBefore(tr2)
        }else{
            var lastStep2=GetLastStep(tr2)
            if (lastStep2==null)
                arr1.insertAfter($('tr:last', t))
            else 
                arr1.insertAfter(lastStep2)
        }
        FormatSteps()
    }
    function StepMatchAttachment(tr){
        if (tr.prop('tagName') != 'TR' ) return false 
            return true
    }
    function StepMoveAttachment(tr1, tr2){
        if (tr1.index()>tr2.index()){
            tr1.insertBefore(tr2)
        }
        else{
            tr1.insertAfter(tr2)
        }
       FormatAttachments()
    }
    function GetLastStep(tr)    {
        var num=tr.children().eq(1).text()
        if (num=='')
            num=tr.children().eq(2).text()
        num+='.'
        var trs=$('tr', t).filter(function(){
            return $(this).children().eq(2).text().indexOf(num)==0
        })
        if (trs.length==0)
            return tr
        else 
            return trs.eq(trs.length-1)
    }
    function MoveAttachment(tr1, tr2){
        if (tr1.index()>tr2.index()){
            tr1.insertBefore(tr2)
        }
        else{
            tr1.insertAfter(tr2)
        }
        FormatAttachments()
    }
    function MoveStandalone(tr1, tr2){
        if (!CheckLocalDragDrop(tr1, tr2))
            return 
        var tr0
        var Key=tr1.children('td').eq(0).attr('Key')
        if (Dict[Key].Warning!='' || Dict[Key].Caution!='')
            tr0=tr1.prev()
        if (tr1.index()>tr2.index()){
            var td=tr2.children('td').eq(0)
            if ( $.trim(td.attr('warning'))!='' ||$.trim(td.attr('caution'))!='')
                tr2=tr2.prev()
            tr1.insertBefore(tr2)
        }
        else{
            var num=tr2.children('td').eq(1).text()+'.'
            tr2=tr2.next()
            while (tr2.length!=0 && tr2.children('td').eq(2).text().indexOf(num)!=-1 ){
                tr2=tr2.next()
            }
            if (tr2.length==0)
                tr1.appendTo(tr1.parent().parent())
            else 
                tr1.insertAfter(tr2)
        }
        if (tr0!=null)
            tr0.insertBefore(tr1)
    }
    function IsStandaloneMove(src){
        if (WiType=='WIT' ||Drag==null || t1.has(src).length>0 ) return false 
        var td=Drag.parent().children('td').eq(0)
        var Type=Dict[td.attr('key')].Type
        if (Type=='StandaloneStep' && !isNaN( src.text())){
            return true
        }
        return false
    }
    function IsAttachmentMove(src){
        if (WiType=='WIT' ||Drag==null || t.has(src).length>0 ) return false 
        var td=Drag.parent().children('td').eq(0)
        if (td.children('input').length>0)
            return true
        return false
    }
    function FromSameTable(td1, td2) {
        if (td1 == null || td2 == null)
            return false
        if (td1.closest('table').eq(0).is( td2.closest('table').eq(0)))
            return true
        else
            return false
    }
    function GetTrHtml(tr) {
        var s = '', tr1
        if (t1.has(tr).length != 0)
            return tr[0].outerHTML
        if (IsWarningCaution (tr.prev())) {
            s += tr.prev()[0].outerHTML
            tr.prev().remove()
        }
        s += tr[0].outerHTML
        //return s
        //var step = tr.find('td').eq(0).attr('name')
        //if (step != 'GlobalStep')
        //    return s 
        tr1 = tr.next()
        //while (tr1.length != 0 && (tr1.find('td').eq(0).attr('type') == null || tr1.find('td').eq(0).attr('type') != 'GlobalStep'))
        //    tr1 = tr1.next()
        var k = tr.find('td').eq(0).attr('Key')
        var o = Dict[k]
        while ( o.Type != 'LocalStep' && o.Type != 'StandaloneStep' && tr1.length != 0) {
            var Key = tr1.find('td').eq(0).attr('Key')
            if (Key == null) {
                tr1 = tr1.next()
            } else {
                var obj = Dict[Key]
                if ( obj.Type == 'GlobalStep')
                    break
                else
                    tr1 = tr1.next()
            }
        }
        if (tr1.length==0)
            arr = tr.nextAll()
        else {
            //  if (tr1.prev().find('td').eq(0).attr('type') == null)
            var Key = tr1.prev().find('td').eq(0).attr('Key')
            if (Key == null ||  Dict[Key].Type==null) 
                tr1 = tr1.prev()
            arr = tr.nextUntil(tr1)
        }
        for (var i = 0; i < arr.length;i++) {
            s += arr[i].outerHTML
            $(arr[i]).remove()
        }
        return s
    }
    function CheckLocalDragDrop(tr1, tr2){
        var key=tr1.children().eq(0).attr('key')
        //alert(key)
        var obj=Dict[key]
        if (!CheckLocalStep(obj, tr2))
            return  false
        return true 
    }
    function Swap(tr1, tr2) {
        if (!CheckLocalDragDrop(tr1, tr2))
            return 
        var s1 = GetTrHtml(tr1)  
        var s2 = GetTrHtml(tr2)
        $(s2).insertAfter(tr1)
        tr1.remove()
        $(s1).insertAfter(tr2)
        tr2.remove()
        FormatSteps()
        FormatAttachments()
    }
    function ValidateTarget(src) {
        //if (IsStandaloneMove(src))
        //    return true 
        if (t.has(src).length == 0 && t1.has(src).length == 0)
            return false 
        if (src.prop('tagName') != 'TD' ||(Drag!=null && Drag.parent().children('td').eq(0).children('input').length==0))
            return false 
        if (WiType!='WIT') {
            if (t.has(src).length > 0) {
                if (src.index() != 2 || src.text() == '') //|| src.prev().has('input').length == 0) {
                    return false
                if (Drag != null) {
                    var arr1 = Drag.text().split('.')
                    var arr2 = src.text().split('.')
                    if (arr1.length == 1 && arr2.length == 1)
                        return true
                    if (arr1[0] != arr2[0])
                        return false
                }
            }
            //if (t1.has(src).length > 0)
            //    if (src.index() != 1 || src.prev().has('input').length == 0)
            //        return false
        } else {
            //if ( src.index() != 1 || (t.has(src).length == 0 && t1.has(src).length == 0)|| src.prev().has('input').length == 0) 
            //    return false
            if (t.has(src).length == 0 && t1.has(src).length == 0)
                return false
            if (src.index() != 1 && src.index() != 2)
                return false
            if (t.has(src).length > 0) {
                if (src.index() == 2  && Drag != null) {
                    var arr1 = Drag.text().split('.')
                    var arr2 = src.text().split('.')
                    if (arr1[0] != arr2[0])
                        return false
                }
            }
        }
        return true 
    }
    function RefreshEquipmentMake(id) {
        var parameters = "{'query':'select * from EquipmentMake where  isnull(disabled, 0)=0 and  EquipmentTypeId=" + id + "' }"
        if (id == '') {
            $('#EquipmentMake').empty()
            return
        }
        $.ajax({
            type: 'POST',
            url: GetSiteUrl() + '/webservice.asmx/GetQuery',
            data: parameters,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (xml) {
                var src = $('#EquipmentMake')
                src.empty()
                src.append('<option value="" ></option>')
                $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                    var field = $(element)
                    var name = field.find('Name').text()
                    var id = field.find('id').text()
                    src.append('<option value="' + id + '" >' + name + '</option>')
                });
            },
            error: function (e) {
                // $('#holder').html('error');
            }
        });
    }
    function SetTabIndex(preview) {
        var tabindex = 100
        $('input,select,a ').each(function () {
            // if (this.type != "hidden") {
            var $input = $(this);
            $input.attr("tabindex", -1);
            tabindex++;
            //  }
        });
        if (preview)
            $('#Preview').attr('tabindex', 1)
    }
    function AddAttachment() {
        var s = '<tr><td ><td><td><input type=file />'
        src = $(s).appendTo(t1)
        src.change(function () {
            s = '<input type=button onclick=on_AttachmentDelete(this) value=Delete />'
            var tr = $(this)//.parent().parent()
            if (tr.children(':first').find('input').length == 0) {
                $(s).appendTo(tr.children(':first'))
                s = '<input style="width1:100%" type=text />'
                $(s).appendTo(tr.children('td').eq(1))
                AddAttachment()
            }
        })
        $('td:lt(2)', t1).width('10%')
    }
    function CreateAttachmentXml() {
        $('tr', t1).each(function (index) {
            if (index==0) return 
            var td = $(this).children('td').eq(0)
            if (td.find('input').length == 0)
                return
            var photo = td.attr('photo')
            var prompt = $.trim(td.next().next().next().find('i').text()) //td.attr('prompt')
            //if (photo.indexOf('.pdf') != -1)
            //    prompt=''
            var title = $.trim(td.next().next().text())
            prompt=escapeXml(prompt)
            title=escapeXml(title)
            var id=td.attr('id')
            var element = $.createElement('attachmnt')
            element.addAttr('id', id)
            element.addAttr('name', photo)
            element.addAttr('prompt', prompt)
            element.addAttr('title', title)
            element.addAttr('seq', index)
            var tr=td.parent()//.prev()
            //while (tr.length!=0 && isNaN( MyReplace( tr.children('td').eq(1).text(), 'Figure ', '')))
            //    tr=tr.prev()
            var counterPart
            //if (tr.length>0)
           
            counterPart=tr.children('td').eq(0).attr('counterPart')
            // alert(parentId)
            element.addAttr('counterPart', counterPart)
            root.append(element)
        })
    }
    function on_AttachmentDelete(src) {
        var tr = $(src).parent().parent()
        tr.remove()
        FormatAttachments()
    }
    function Validate() {
        if ($('#Criticality').val() == '') {
            alert('Please select a Job Criticality.')
            return false 
        }
        return true 
    }
    function FixDuplicate(){
        $('tr', t).each(function (index){
            if (index==0)   return 
            var td=$(this).children('td').eq(0)
            td.removeAttr('stepid')
        })
        $('tr', t1).each(function (index){
            if (index==0)   return 
            var td=$(this).children('td').eq(0)
            td.removeAttr('id')
        })
    }
    function on_Save() {
        if ( $('#SaveButton').prop('disabled') || !$('#SaveButton').is(":visible"))
            return 
        if (!Validate())
            return 
        if ($('#Duplicate').val()==1)
            FixDuplicate()
        CreateWIxml()
        // $('#holder').text($('#h1').val()) //return
        SaveScroll()
        __doPostBack('Save', $('#MappingXmlHidden').val());
        IsDirty = false

    }
    function CreateWIxml() {
        root = $('<wi/>')
        CreateAttachmentXml()
        CreateJobStepXml()
        CreatePrecautionXml()
        var xml = root.html() + GetXML('resources') + GetXML('tool') + GetXML('WI') + GetXML('PersonalProtectiveEquipment') + GetXML('WILink')
        xml = '<wi>' + xml + '</wi>'
        xml = escapeXml(xml)
        $('#h1').val(xml)
    }
    function GetXML(name) {
        return $('#' + name).attr('xml')
    }
    function CreatePrecautionXml() {
        var element = $.createElement('GeneralPrecautions')
        element.addAttr('Value', MyEscapeXml( $('#GeneralPrecautions').html() ))
        root.append(element)
        element = $.createElement('LocalPrecautions')
        element.addAttr('Value', MyEscapeXml($('#LocalPrecautions').html()))
        root.append(element)
    }
    function CreateJobStepXml() {
        var step = 'Step'
        var globalIndex = 0, localIndex = 0, seq
        $('td:nth-child(1)', t).filter(function (index) {
            //if ($(this).attr('name') == null) return false 
            return $(this).children('input').length>0
        }).each(function (index) {
            var tr = $(this).parent()
            var td=tr.children('td').eq(0)
            var Key=td.attr('Key')
            var obj=Dict[Key]
            // alert()
            var element = $.createElement('step')
            var Type =obj.Type
            element.addAttr('Type',Type)
            if (Type == 'GlobalStep') {
                globalIndex++
                localIndex = 0
                seq = globalIndex
            } else if (Type == 'LocalStep')  {
                localIndex++
                seq = globalIndex + .1 * localIndex
            } else {
                localIndex++
                seq = globalIndex + .1 * localIndex
            }
            //alert(value)
            element.addAttr('Description', MyEscapeXml(obj.Description))
            element.addAttr('StepId', obj.StepId)
            element.addAttr('Warning', MyEscapeXml(obj.Warning))
            element.addAttr('Caution', MyEscapeXml(obj.Caution))
            element.addAttr('Note', MyEscapeXml(obj.Note))
            element.addAttr('Hazard', obj.Hazard)
            element.addAttr('Barrier', obj.Barrier)
            element.addAttr('Photo', obj.Photo)            
            element.addAttr('Prompt', obj.Prompt)  
            element.addAttr('Seq', seq)
            //if (Find(tr, 4).find('img').length>0)
            //    element.addAttr('photo', Find(tr, 4).find('img').attr('photo'))
            //element.addAttr('prompt',  $.trim(Find(tr, 5).text()))
            //element.addAttr('barrier', $(this).attr('barrier'))
            root.append(element)
            // GetPhoto(tr, element)       
        })
        // alert(root.html())
        return escapeXml(root.html())
    }
    function CreateJobStepXml060515() {
        var step = 'Step'
        var globalIndex = 0, localIndex = 0, seq
        $('td:nth-child(1)', t).filter(function (index) {
            if ($(this).attr('name') == null)
                return false 
            return $(this).attr('name').indexOf('Step') > -1
        }).each(function (index) {
            var tr = $(this).parent()
            var element = $.createElement('step')
            step = tr.children(':first').attr('name')
            element.addAttr('type', step)
            if (step == 'GlobalStep') {
                globalIndex++
                localIndex = 0
                seq = globalIndex
                //element.addAttr('isGlobal', 1)
            } else if (step == 'LocalStep')  {
                localIndex++
                seq = globalIndex + .1 * localIndex
            } else {
                localIndex++
                seq = globalIndex + .1 * localIndex
            }
            var value = $(this).attr('value')
            //alert(value)
            element.addAttr('name', MyEscapeXml($(this).attr('value')))
            element.addAttr('stepId', $(this).attr('stepId'))
            element.addAttr('seq', seq)
            if (Find(tr, 4).find('img').length>0)
                element.addAttr('photo', Find(tr, 4).find('img').attr('photo'))
            element.addAttr('prompt',  $.trim(Find(tr, 5).text()))
            element.addAttr('barrier', $(this).attr('barrier'))
            root.append(element)
            GetPhoto(tr, element)
        
            CreateStepXml(tr.prev(), element)
            if (tr.prev().length != 0)
                CreateStepXml(tr.prev().prev(), element)
            CreateStepXml(tr.next(), element, 1)

            //while (IsNotStep(tr.prev())) {
            //    tr = tr.prev()
            //    var name = tr.children(':first').attr('name')
            //    var value = tr.children(':first').attr('value')
            //    value = MyEscapeXml(value)
            //    element.addAttr(name, value)
            //    var hazard = tr.children(':first').attr('hazard')
            //    if (hazard!=null)
            //        element.addAttr('hazard', hazard)
            //}

        })
        //    $('#holder').text(root.html())
        return escapeXml(root.html())
    }
    function CreateStepXml(tr, element, flag) {
        if (tr.length == 0) return
        var td = tr.children('td:first')
        if (td.length == 0)
            return
        var name = tr.children(':first').attr('name')
        var value = tr.children(':first').attr('value')
        if ((flag == null && name == 'Note') || (flag != null && name != 'Note'))
            return

        value = MyEscapeXml(value)
        element.addAttr(name, value)
        var hazard = tr.children(':first').attr('hazard')
        if (hazard != null)
            element.addAttr('hazard', hazard)
    }
    function MyEscapeXml(s) {       
        s = String(s).replace(/&lt;/g, LessThanEncode)
        s = String(s).replace(/&amp;/g, AmpEncode)
        s = s.replace(/[<]/g, function (c) {
            switch (c) {
                case '<': return '&lt;';
            }
        });
        return s
    }
    function GetPhoto(tr, element) {
        var photo 
        photo = Find(tr, 5).children('IMG')
        if (photo.prop('tagName') == 'IMG') {
            var s = photo.attr('src')
            s = s.split('/').pop()
            
            element.addAttr('photo', s)
        }
    }
    function Find(src, i) {
        return src.find('td:nth-child(' + i + ')')
    }
    function RemoveStep(tr0) {
        var i = 2
        var tr=tr0.prev()
        while (i-- > 0 && tr.length != 0 && IsNotStep(tr)) {
            tr.remove()
            tr=tr0.prev()
        }
        tr = tr0.next()
        if (tr.length != 0 && IsNotStep(tr, 1))
            tr.remove()
    }
    function on_Delete(src) {
        if (!confirm('Are you sure you want to delete this Job Step?'))
            return 
        var stepId=Dict[$(src).parent().attr('Key')].StepId
        if (stepId==null){
            var tr=$(src).parent().parent()
            var tr0=tr.prev()
            if (tr0.index()!=0 && $('td:gt(0):lt(2)', tr0).text()=='')
                tr0.remove()
            tr.remove()
            FormatSteps()
            return 
        }
        var sql = ' select * from jobStep where id=' + stepId + ' and counterPart is not null'
        var r
        CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                r=xml.d
            },
            function (e) {
                alert(e.responseText)
            }
            ,false
        )
        if ($($.parseXML(r)).find('Table').length>0){
            alert('This Job Step cannot be deleted.')
            return 
        }
        var tr = $(src).parent().parent()
        var arr1=tr
        if (tr.index()!=1 && $('td:gt(0):lt(2)', tr.prev()).text()=='')
            arr1=tr.prev()
        var lastStep=GetLastStep(tr)
        arr1=arr1.prev()
        arr1=arr1.nextUntil(lastStep.next())
        arr1.remove()

        FormatSteps()
    }
    function IsNotStep(tr, flag) {
        var name = tr.children(':first').attr('name')
        //  return tr.length != 0  && tr.children(':first').attr('name').indexOf('Step') == -1
        return tr.length!=0 && tr.prev().length && name.indexOf('Step') == -1 && ((flag==null && name!='Note') || (flag!=null && name=='Note')) 
    }
    function on_Edit(src) {
        var name, value
        var td = $(src).parent()
        var tr = $(src).parent().parent()
        var Key=td.attr('Key')
        var obj = Dict[Key]
        var Photo = obj.Photo
        var features = 'dialogWidth:1200px;dialogHeight:720px;center:on';
        obj = showModalDialog(url, obj, features);
        if (obj == null)
            return
        IsDirty = true
        if (obj.Photo ==null)
            obj.Photo = Photo
        Dict[Key] = obj
        if (IsWarningCaution(tr.prev()))
            tr.prev().remove()
        var locked=tr.find('input:disabled').length
        AddStep(tr, obj, 'Edit', locked)
        tr.remove()
        FormatSteps()
    }
    function CreateArr(tr, arr1, flag) {
        if (tr.length==0)return 
        var td=tr.children('td:first')
        if (td.length==0) 
            return 
        name = td.attr('name')
        if (name.indexOf('Step') != -1)
            return 
        if ((flag==null && name=='Note')||(flag!=null && name!='Note'))
            return 
        value = td.attr('value')
        if (value !=null)
            value = decodeXml(value)
        arr1.push({ name: name, value: value });
        var hazard = td.attr('hazard')
        if (hazard!=null)
            arr1.push({ name: 'Hazard', value: hazard });   
    }
    function decodeXml(string) {

        var escaped_one_to_xml_special_map = {
            '&amp;': '&',
            '&quot;': '"',
            '&#39;': "'",
            '&lt;': '<',
            '&gt;': '>'
        };
        return string.replace(/(&quot;|&#39;|&lt;|&gt;|&amp;)/g,
            function (str, item) {
                return escaped_one_to_xml_special_map[item];
            });
    }
    function on_Insert(src) {
        var features = 'dialogWidth:1200px;dialogHeight:720px;center:on';
        var obj = showModalDialog(url, 'dummy', features);
        if (obj == null)
            return
        IsDirty = true
        var tr = $(src).parent().parent()
        if (!CheckLocalStep(obj, tr))
            return 
        var step = tr.children(':first').attr('name')  // 'Step'
        if (IsWarningCaution(tr.prev()))
            tr = tr.prev()
        if (!AddStep(tr, obj, 'Insert'))
            return 
        var Key=guid()
        tr.prev().children('td').eq(0).attr('Key', Key)
        Dict[Key]=obj
        FormatSteps()
    }
    function CheckLocalStep(obj, tr0){
        if ( WiType=='WIT' ) return true 
        var s='Stand alone local step cannot be inserted between a global step and a linked local step, or between two linked local steps'
        var tr=tr0
        if (obj.Type=='LocalStep'){
            if (tr0==null)
                tr=t.children('tbody').children('tr').last()
            else{
                tr=tr.prev()
                if (tr.length>0 && $('td:gt(0):lt(2)', tr).text()==''){
                    tr=tr.prev()
                }
            }
            if (tr.length>0 && IsNotNumber(tr.children().eq(1).text()) && IsNotNumber(tr.children().eq(2).text())){
                alert(s)
                return false 
            }
        }else{//Standalone
            if (tr0!=null && !IsNotNumber(tr.children().eq(2).text())){
                alert(s)
                return false 
            }
        }
        return true 
    }
    function IsNotNumber(s){
        if (s=='' || isNaN(s))
            return true
        else 
            return false 
    }
    function IsWarningCaution(tr){
        if (tr.index()!=0 && tr.find('td').eq(0).find('input').length==0) 
            return true
        else
            return false
    }
    function on_DeletePhoto(src) {
        src=$(src)
        var td = src.parent()
        td.find('img').remove()
        src.remove()
        td = td.parent().find('td').eq(0)
        var Key = td.attr('Key')
        var obj = Dict[Key]
        obj.Photo=null
    }
    function AddStep(tr, obj, action, locked) {
        if (obj == null)
            return false
        var arr = []
        arr[0] = ''
        arr[1] = ''
        arr[0] += EncodeNewline('Warning', obj.Warning)
        arr[0] += EncodeNewline('Caution', obj.Caution)
        arr[1] += EncodeNewline('Description', obj.Description)
        arr[1] += EncodeNewline('Note', obj.Note)
        var Prompt='', Photo='' 
        if (obj.Photo!=null && obj.Photo!=''){
            Photo = '<img width=144px height=144px  src=upload/' + obj.Photo + ' /><input type=button onclick=on_DeletePhoto(this) value=Delete />'
        }
        if (obj.Prompt != null && obj.Prompt != '' && WiType=='WIT' ) {
            Prompt ='<font color=blue><i>'+ obj.Prompt + '</i><br></font>'
            Photo = ''
        }
        if (arr[0]!='')
            arr[0] = '<tr><td>'+GetHazardImage(obj) + '<td valign=top>' + arr[0] + '<td><td>'
        //arr[0] = '<tr><td><td>'+GetHazardImage(obj.Hazard)+' <td>' + '<td valign=top>' + arr[0] + '<td><td>'
        arr[1] = '<tr><td>' + AddLink('Edit') + AddLink('Delete', locked) + AddLink('Insert Above')
            + '<td style="text-align:center" ><td style="text-align:center"><td valign=top>' + arr[1] + '<td>' + Prompt + Photo + '</td><td>' + obj.Barrier + '</td>'
        for (var i = 0; i < arr.length; i++) {
            var src
            var s = arr[i]
            if (s=='') continue
            if (action == 'Add')
                src = $(s).appendTo(t)
            else
                src = $(s).insertBefore(tr)
            if (action == 'Edit')
                src.find('td').eq(0).attr('Key', tr.find('td').eq(0).attr('Key'))
        }
        return true 
    }
    function EncodeNewline(name, value) {
        if ($.trim(value) == '')
            return ''
        var s = '<table width=100% ><tr><td style="border:0" valign=top width=5%>'
        var iconSize = 40
        if (name == 'Note')
            iconSize=20
        if (name!='Description')
            s += '<img style="width:'+iconSize+'px" src=images/' + name + '.png /><td style="border:0" align=left>'
        switch (name) {
            case 'Warning': case 'Caution':
                var arr = value.split('\n')
                for (var i = 0; i < arr.length; i++) {
                    if ($.trim(arr[i]) !== '')
                        s += '<b>' + name + ': </b>' + arr[i]
                    s += '<br>'
                }
                break
            case 'Note':
                if ($.trim( $(value).text()) == '') 
                    return ''
                s+=value
                break
            default:
                s+=value 
        }
        s +='</table>'
        return s
    }
    function EncodeNewline2(value) {
        return value
        return value.split('\r\n').join('<br/>').split('\n').join('<br/>').split('\r').join('<br/>') //.split(' ').join('&nbsp;');
    }
    function AddLink(name, locked) {
        var disabled=''
        if (locked !=null && locked>0) return '' //disabled='disabled'
        var s = "&nbsp<input type=button value='{0}' "+disabled+" onclick=on_{0}(this)  />";
        s = s.replace('{0}', name).replace('{0}', name);
        if (name == "Insert Above")
            return s.replace("on_Insert Above", "on_Insert");
        return s
    }
    function on_Add() {
        var features = 'dialogWidth:1200px;dialogHeight:720px;center:on';
        var url='jobStep.aspx?WiType=' + WiType
        var obj = showModalDialog(url, 'dummy', features);
        if (!CheckLocalStep(obj))
            return 
        if (!AddStep(null, obj, 'Add'))
            return 
        IsDirty = true
        var Key=guid()
        t.children('tbody').children('tr').last().children('td').eq(0).attr('Key', Key)
        Dict[Key]=obj
        FormatSteps()
    }
    function   CreateDict(td){
        var Key=td.attr('Key')
        var obj=[]
        var arr=["StepId", "Type", "Description", "Warning", "Caution", "Note", "Hazard", "Photo", "Barrier","Prompt"]
        for (var i = 0; i < arr.length; i++) {
            var s = td.attr(arr[i])
            var re = new RegExp(LessThanEncode, 'g')
            s = s.replace(re, '&lt;')
            re = new RegExp(AmpEncode, 'g')
            s = s.replace(re, '&amp;')
            obj[arr[i]]=s
        }
        Dict[Key]=obj
    }
    function InitSteps() {              
        var globalIndex = 0, localIndex = 0, standalone = 0
        t.children('tbody').children('tr').each(function (index) {
            var tr = $(this)
            if (index==0){
                tr.children('th:gt(5)').remove()
                return 
            }
            var s = ''
            var td=tr.find('td').eq(0)
            var Key=td.attr('Key')
            CreateDict(td)
            var Prompt=Dict[Key].Prompt     
            s += GetWarningCaution(tr, 4, 'Warning')
            s += GetWarningCaution(tr, 5, 'Caution')
            var obj = Dict[tr.find('td').eq(0).attr('Key')]
            if (s != '') {
                s = '<tr><td>'+GetHazardImage(obj)+'<td valign=top>  <table width=100% >' + s + '</table><td><td></tr> '
                //s = '<tr><td><td align=center>'+GetHazardImage(obj.Hazard)+' <td><td valign=top>  <table width=100% >' + s + '</table><td><td></tr> '
                $(s).insertBefore(tr)
            }
            s=GetWarningCaution(tr, 6, 'Note')
            if (s != '') {
                s = '<table border=0>'+s+'</table>'
            }
            //s=tr.find('td').eq(3).html() +s
            s = GetWarningCaution(tr, 3, 'Description') + s
            tr.find('td').eq(3).html(s)
            if ($.trim(Prompt) != '') {
                s = '<font color=blue><i>' + Prompt + '<br></i><font>' 
                tr.children('td').eq(8).html(s)
            }
            tr.children('td:gt(3):lt(4)').remove()
            tr.children('td:gt(5)').remove()
            if (tr.children('td').eq(4).find('img').length!=0)
                $('<input type=button onclick=on_DeletePhoto(this) value=Delete />').appendTo(   tr.children('td').eq(4))
        })
    }
    function GetWarningCaution(tr, index, name){
        var td= tr.find('td').eq(0)
        var key=td.attr('Key')

        var s=Dict[key][name]
        s = $.trim(s)
        if (s != '' && s != '&nbsp;') {
            switch (name) {
                case 'Warning': case 'Caution':
                    var replace = '<b>' + name + ': </b>'
                    s = s.split('\n').join('<br>'+replace)
                    s=replace +s
                    break
                case 'Note':
                    if($(s).text()=='')
                        return ''
                    break
                default:
                    return s
            }
            var iconSize = 40
            if (name == 'Note') 
                iconSize = 20                           
            s = '<tr><td style="border:0" width=5% valign=top><img width=' + iconSize + 'px src=images/' + name + '.png /> <td style="border:0" valign2=top>' + s
        }
        return s 
    }
    function FormatSteps() {
        var globalIndex = 0, localIndex = 0, standalone=0
        t.children('tbody').children('tr').each(function (index) {
            if (index == 0) return
            var tr = $(this)
            if (tr.children('td').eq(0).find('input').length==0)
                return 
            var Key=tr.children('td').eq(0).attr('Key')
            var obj=Dict[Key]
            tr.children('td:gt(0):lt(2)').text('')
            switch (obj.Type){
                case 'GlobalStep':
                    globalIndex++
                    localIndex = 0
                    tr.children('td').eq(1).text(globalIndex)
                    if (WiType=='WI') {
                        $('input:lt(2)', tr.children('td').eq(0)).hide()
                        tr.children('td').eq(0).attr('align', 'right')
                    }
                    break
                case 'LocalStep':
                    localIndex++
                    tr.children('td').eq(2).text(globalIndex + '.' + localIndex)
                    break
                default:
                    standalone++
                    tr.children('td').eq(2).text(String.fromCharCode(standalone+64))
            }
        })
    }   
    function FormatAttachments() {
        var indexGlobal=0, indexLocal=0
        t1.children('tbody').children('tr').each(function (index) { 
            if (index == 0) return
            var tr = $(this)
            var td=tr.children('td').eq(0)
            var prompt=td.attr('prompt')
            var type=td.attr('type')
            var counterPart=td.attr('counterPart')
            if ( WiType=='WI' && counterPart!=null && counterPart!='' && counterPart!=0)
                    $('input[value*="Delete"]', td).remove()            
            if (td.find('input').length>0)
                if (WiType=='WIS' || WiType=='WI' || (WiType!='WI' && prompt!='' ))
                    td.next().text(GetFigureNo( String.fromCharCode((++indexLocal)+64))   )
                else
                    td.next().text(GetFigureNo(++indexGlobal))
            else
                    td.next().text(GetFigureNo(++indexGlobal))
        })
    } 

    function GetFigureNo(i){
        return 'Figure '+ i
    }
    function on_AttachmentClick(src){
        var y = event.clientY 
        var x = event.clientX 
        src = $(src)
        var name = src.attr('src').replace('thumb', '')
        var w, h, top, left 
        $('#help').hide().replaceWith('<img id=help />')
        $('#help').load(function () {
            w=$(this).width()
            h =$(this).height()
            if (w>$(window).width())
                w=$(window).width()
            if (h>$(window).height())
                h=$(window).height()
            top =y+$(window).scrollTop() //- h
            //alert($(window).scrollTop())
            left=x+$(window).scrollLeft() //-w
            if (  x>$(window ).height()/2)
                top -= h
            if (x> $(window).width()/2)
                left -= w
            var margin=0
            if (top<$(window).scrollTop())  
                top=$(window).scrollTop()
            if(left<$(window).scrollLeft())
                left=$(window).scrollLeft()
            
            $('#help').css({ position: 'absolute', top: top, left:left }).height(h).width(w)
            $('#help').show()
        })
        $('#help').attr('src', name)
        $('#help').click(function(){
            $(this).hide().replaceWith('<img id=help />')
        })
    }
    function on_mouseover(src) {return 
        var y = event.clientY 
        var x = event.clientX 
        src = $(src)
        var name = src.attr('photo')
        var w, h, top, left 
        $('#help').load(function () {
            w=$(this).width()
            h =$(this).height()
            top =y+$(window).scrollTop() //- h
            left=x+$(window).scrollLeft() //-w
            if (  x>$(window ).height()/2)
                top -= h
            if (x> $(window).width()/2)
                left -= w

            $('#help').css({ position: 'absolute', top: '' + top + 'px', left: '' + left + 'px' })
            $('#help').show()
        })        
        $('#help').attr('src', 'upload/' + name) //+'?'+new Date().getTime())  
    }
    function on_mouseout(src) {return 
        $('#help').hide()
    }
    function on_SubmitFeedback(src0) {
        var src=$(src0)
        var data ='dummy' 
        var features = 'dialogWidth:800px;dialogHeight:300px;center:on';
        var id=getParameterByName('id')
        if (id=='') id=0
        var arr = showModalDialog('sendworkflow.aspx?id='+id+'&WiType='+WiType , data, features);
    }
    function SaveWorkflow( localId, userId, StatusText,comment, action){
        StatusText=MyReplace(StatusText, "'", "''")
        var arr=[localId, userId, StatusText,comment, action, <% =this.RowVersion %>]
        var sql=" usp_saveWorkflow  '"+arr.join("','")+"'"        
        CallAjax('sql', sql, 'runSql', function(data, status, xhr){
            if (data.d.indexOf('The record')!=-1)
                alert('The record has been updated by another user.')
            if (StatusText=="Pending Operations Manager''s Approval to Disable" && action=='Approve')
                window.location='home.aspx'
            else
                window.location=window.location.toString()
        }, function(e){alert(e.responseText)}
        )
    }
    function on_SendWorkflow(src0) {
        var userId='<% =this.UserId %>'
        var src=$(src0)
        var id=src.attr('id')
        var wiid=getParameterByName('id')
        //if (wiid=='')
        //    wiid=getParameterByName('wisId')
        //if (wiid==''){
        //    alert('Please save it first.')
        //    return 
        //}
        var StatusText=$('#StatusText').text()
        //var wiid = location.search.split('id=')[1] ? location.search.split('id=')[1] : '0';
        if (id=='ApproveButton'){
            SaveWorkflow( wiid, userId, StatusText,'', 'Approve')
            return 
        }
        var features = 'dialogWidth:800px;dialogHeight:300px;center:on';
        var arr = showModalDialog('workflow.htm?id='+id, 'data', features);
        if (arr==null)
            return 
        var action
        if (id=='SubmitForApprovalButton')
            action='Submit'
        else if (id=='SubmitForDisableButton')
            action='Disable'
        else//Reject button 
            action='Reject'        
        SaveWorkflow( wiid, userId, StatusText,arr.Comment, action)
    }
    function on_PopupResources(td, data){
        var features = 'dialogWidth:600px;dialogHeight:400px;center:on';
        var arr = showModalDialog('popupPersonnel.htm', data, features);
        $('#resources').attr('xml', arr.xml)
        //escapeXml
        td.html( arr.text)
    }
    function on_Popup(src) {
        if (getParameterByName('id')!='' && getParameterByName('rigId')!=''){
            alert('Please save the Work Instruction first.')
            return 
        }            
        var td = $(src).parent().next()
        var wiid = location.search.split('id=')[1] ? location.search.split('id=')[1] : '0';
        var name = td.attr('id')
        var features = 'dialogWidth:700px;dialogHeight:600px;center:on';
        var url = 'popup.htm?name=' + name  //+ '&wiid=' + wiid
        if (name == 'WI'){
            features = 'dialogWidth:1000px;dialogHeight:600px;center:on';
            url = url.replace('popup.htm', 'popupAWI.aspx')
            var rigName=(WiType=='WIS'? $('#Facility').text():'')
            url+='&id='+getParameterByName('id')+'&WiType='+WiType+'&rigName='+rigName
        }
        var data = $('#' + name).attr('xml')
        if (name=='resources'){
            on_PopupResources(td, data);
            return 
        }
        var arr = showModalDialog(url, data, features);
        if (arr == null) return
        IsDirty = true

        var s = '', xml = '', link = ''
        var arr1 = []
        for (var i = 0; i < arr.length; i++) {
            var name1 = arr[i].name
            name1 = escapeXml(name1)
            var id = arr[i].id
            var number = arr[i].number
            var other = arr[i].other
            var type=arr[i].type
            var other_xml=other
            if (other_xml != null)
                other_xml=escapeXml(other_xml)
            xml += '<' + name + ' id="' + id + '" type="' + type+ '" name="' + name1 + '" number="' + number + '" other="' + other_xml + '" />'
            if (other != null)
                name1=other
            // name1 += '(' + other + ')'
            if (number != null)
                name1 += '(' + number + ')'
            arr1.push(name1)
            link += '<a target=_new href=Create.aspx?id=' + id + '>' + name1 + '</a><br/>'
        }
        $('#' + name).attr('xml', xml)
        var tr = $(src).parent().parent()
        if (name == 'WI')
            td.html(link)
        else
            td.html(arr1.join(', '))
    }
    function GetHazardImage(obj) {
        var Type=obj.Type
        var value=obj.Hazard
        if ($.trim(value)=='')return '<td><td>'
        var arr = value.split(',')
        var s='<td>'
        if (Type!='GlobalStep')
            s+='<td>'
        s += '<table border=0 width=100%>'
        for (var i = 0; i < arr.length; i++) {
            s += '<tr><td align=center  style="border:0"><img onmousedown=on_HazardMousedown(this) onmouseup=on_HazardMouseup(this)  width ="50px" src="images/hazard/' + arr[i] + '" />'
        }
        s+='</table>'
        if (Type=='GlobalStep')
            s+='<td>'
        return s
    }
    function GetHazardImage1(value) {
        if ($.trim(value)=='')return ''
        var arr = value.split(',')
        var s = '<table border=0 width=100%>'
        for (var i = 0; i < arr.length; i++) {
            s += '<tr><td align=center  style="border:0"><img onmousedown=on_HazardMousedown(this) onmouseup=on_HazardMouseup(this)  width ="50px" src="images/hazard/' + arr[i] + '" />'
        }
        return s+'</table>'
    }
    function on_HazardMousedown(src){
        HazardDrag=$(src)
    }
    function on_HazardMouseup(src){
        src=$(src)
        if (src.is(HazardDrag) || !FromSameTable(HazardDrag, src) ||src.prop('tagName' )!='IMG')  {
            HazardDrag = null
            return
        }       
        var img=HazardDrag.attr('src')
        HazardDrag.attr('src', src.attr('src'))
        src.attr('src', img)
        var arr=[]
        src.closest('table').find('img').each(function (){
            var s= $(this).attr('src').split('/')
            s=s[s.length-1]
            arr.push(s)
        })
        var tr=src.closest('table').parent().parent()
        var Key=tr.next().find('td').eq(0).attr('Key')
        Dict[Key].Hazard=arr.join(',')
        
        HazardDrag = null
    }
    function on_Photo(name) {
        __doPostBack('Photo', name);
    }
    function ValidateBeforePreview() {
        var s = window.location.search.toLowerCase()
        if (s.indexOf('?id=') == -1 && s.indexOf('?localid=') == -1  && (s.indexOf('?wisid=') == -1 || s.indexOf('?wisid=0') != -1)) {
            alert('Please save it first.')
            return false
        }
        return true 
    }
    function on_Preview() {
        if (ValidateBeforePreview()) {
            if (IsDirty) {
                var r = confirm("You have unsaved changes. Save them Now?")
                if (r) {
                    on_Save()
                }
            }
            $('input:hidden').prop('disabled', false)
            __doPostBack('Preview', '');
        }
    }
    function on_Convert(){
        $('input:hidden').prop('disabled', false)
        __doPostBack('Convert', '');
    }
    function on_Precaution(src) {
        var td = $(src).parent()
        var features = 'dialogWidth:900px;dialogHeight:500px;center:on';
        var arr = []
        arr.html = td.next().html()
        arr.label=td.text()
        var result = showModalDialog('Precaution.htm', arr, features);
        if (result == null)
            return
        IsDirty = true
        var Precaution = result.Precaution
        td.next().html(Precaution)
    }
    function on_Upload(src) {
        var td=$(src).parent()
        var arr = showModalDialog('upload.aspx', 'dummy', size);
        // alert(arr);return 
        if (arr == null)
            return
        IsDirty = true
        var photo=arr[0].name
        var img = td.find('img')
        if (img.length == 0) {
            img = $("<img  style='height:144px;width:144px' src='upload/" + photo + "' />")
            img.appendTo(td)
        }
        else
            img.attr('src', 'upload/' + photo)
    } 
    function InitAttachments(){
        $('tr', t1).each(function(index){ 
            if (index==0) return 
            var tr0=$(this)       
          //  var arr={'id', 'title', 'Attachment', 'prompt', 'Type', 'counterPart', 'seq'}
            var tr=GetAttachmentTr(GetAttachmentValue(tr0, 'id'),  GetAttachmentValue(tr0, 'Title')
                , GetAttachmentValue(tr0, 'Attachment'), GetAttachmentValue(tr0, 'prompt'),  GetAttachmentValue(tr0, 'Type')
                , GetAttachmentValue(tr0, 'counterPart'), GetAttachmentValue(tr0, 'seq')) 
            tr0.replaceWith(tr)
        })
        var arr=['', 'Figure #', 'Title', 'Attachment']
        for (var i=0;i<arr.length;i++){
            t1.find('th').eq(i).text(arr[i])
        }
        //ReorderAttachment()
        $('th:gt('+(arr.length-1) +')', t1).remove()
        FormatAttachments()
    }
    function ReorderAttachment(){
        if (WiType!='WI') return 
        var arr=$('tr', t1).filter(function(){
            var td=$(this).children('td').eq(0)
            return td.attr('seq')==''
        })
        var arr1=arr.clone()
        arr.remove()
        var seq0=-1
        var total=0
        window.status=arr1.length+' '+$('tr', t1).length
        $('tr', t1).each(function(index){ 
            if (index==0) return 
            var tr=$(this)       
            var td=tr.children('td').eq(0)
            var seq=td.attr('seq')
            var diff=seq-seq0
            if (diff>1){
                var x=arr1.filter(function(index){
                    return  total <=index &&  index <total + diff - 1
                })
                x.insertBefore(tr)
                total+=diff-1
            }
            seq0=seq
            
        })
         
        if (arr1.length>total){
            var x=arr1.filter(function(index){
                return index >=total
            })
            x.appendTo(t1)
        }
    }
    function GetAttachmentValue(tr, name){
        var index=$("th:contains('"+name+"')", t1).index()
        return $.trim( tr.children('td').eq(index).text())
    }
    function GetAttachmentTr(id,title, photo, prompt, type, counterPart,seq){
       // alert(counterPart)
        var thumb=''
        if (photo !='')
            thumb= photo.replace('.', 'thumb.')
        var img= "<img onclick='on_AttachmentClick(this)'  onmouseover='on_mouseover(this)' onmouseout='on_mouseout(this)'  src=upload/" + thumb + " />"
        if (photo.indexOf('.pdf')!=-1){
            img='<a target=_blank href=upload/'+photo+' >'+title+'</a>'
        }
        var s = '<tr><td align=right >'
       // alert(type)
        if (type ==null || WiType!='WI' || type=='Local'|| prompt!='')
            s += '<input type=button onclick=on_AttachmentDelete(this) value=Delete />'
            +'&nbsp<input type=button onclick=on_UploadAttachment(this) value=Edit />'
        s +='<td><td>' + title +'<td>'
        if (photo != '') 
            s+=img
        s += '<font color=blue><i>'+prompt+'</i></font></td>'
        var tr=$(s)
       // alert(prompt)
        tr.children('td').eq(0).attr('id', id).attr('photo', photo).attr('prompt', prompt).attr('counterPart', counterPart).attr('seq', seq)
        return tr
    }
    function on_UploadAttachment(src) {
        var td = $(src) .parent()
        var features = 'dialogWidth:500px;dialogHeight:250px;center:on';
        var url = 'upload.aspx?WiType=' + WiType+'&PDFs='+t1.find('a').length
        var data='dummy'
        var counterPart
        if (td.length!=0){
            //var img=td.next().next().next().children('img').attr('src')
            //if (img!=null)
            //    img=img.replace('upload/', '').replace('thumb', '')
            //else
            //    img=''
            counterPart=td.attr('counterPart')
            var img=td.attr('photo')
            var td1=td.next().next().next()
            var prompt=td1.text()
            //if (td1.find('img').length>0 || td1.find('a').length>0)
            //    prompt=''       
            data={title: td.next().next().text(), prompt:prompt, photo:img}
            //url += '&title=' + td.next().next().text()+ '&prompt=' + td.next().next().next().text()+ '&img=' + td.next().next().next().children('img').length
        }
        var result = showModalDialog(url, data, features);
        if (result == null)
            return
        var photo = result[0].name
        var title = result[0].title 
        var prompt = result[0].prompt
        //if (prompt!= '')
        //    photo=''
        var tr=GetAttachmentTr(td.attr('id'), title, photo, prompt,null, counterPart)
        if (td.length==0)
            tr.appendTo(t1)
        else
            td.parent().replaceWith(tr)
        FormatAttachments()
    }
    function on_AttachmentEdit(src) {
        on_UploadAttachment(src)
    }
    function on_Cancel() {
        if (confirm('Are you sure you want to leave this page?'))
            window.location='home.aspx'
    }
    function on_Duplicate121715() {
        var JobDescriptionId = $('#JobDescription').attr('JobDescriptionId')
        setCookie('wiid', 0)
        setCookie('JobDescriptionId', JobDescriptionId)
        var features = 'dialogWidth:850px;dialogHeight:600px;center:on';
        var xml = showModalDialog('mapping.aspx?Action=Duplicate', JobDescriptionId, features);
        $('#Duplicate').val(1);
        if (xml == null)
            return
        $('input').prop('disabled', false)
        $('select').prop('disabled', false)
        $('input:radio[value="In Progress"]').prop('checked', true)
        IsDirty = true
        $('#MappingXmlHidden').val(xml)

        var Facility = getCookie('Facility')
        $('#Facility').text(Facility)
        var cat = $('#JobDescription').attr('WICategoryName')
        var number = $('#JobDescription').attr('JobDescriptionNumber')
        var WiNo = 'WIT-' + MyReplace(Facility, ',', '/') + '-' + cat + '-' + number
        $('#WiNo').text(WiNo)
    }
    function on_Duplicate() {
        var JobDescriptionId = $('#JobDescription').attr('JobDescriptionId')
        setCookie('wiid', 0)
        setCookie('JobDescriptionId', JobDescriptionId)
        var features = 'dialogWidth:850px;dialogHeight:600px;center:on';
        var xml = showModalDialog('mapping.aspx?Action=Duplicate&WiType='+WiType, JobDescriptionId, features);
        $('#Duplicate').val(1);
        if (xml == null)
            return
        $('input').prop('disabled', false)
        $('select').prop('disabled', false)
        $('input:radio[value="In Progress"]').prop('checked', true)
        IsDirty = true
        $('#MappingXmlHidden').val(xml)

        var Facility = getCookie('Facility')
        $('#Facility').text(Facility)
        var cat = getCookie('WICategoryName')
        var number = getCookie('JobDescriptionNumber')
        var title=getCookie('JobDescriptionName')
        var WiNo = WiType+'-' + MyReplace(Facility, ',', '/') + '-' + cat + '-' + number
        $('#JobDescription').text(title)
        $('#JobDescriptionId').val(getCookie('JobDescriptionId'))
        $('#WiNo').text(WiNo)
        $('#SaveButton').show()
    }
    function on_Facility() {
        var JobDescriptionId = $('#JobDescriptionId').val()//  $('#JobDescription').attr('JobDescriptionId')
        var mappingXml = $('#MappingXmlHidden').val()
        setCookie('JobDescriptionId', JobDescriptionId)
       
        setCookie('wiid',  $('#wiid').val())
        var features = 'dialogWidth:850px;dialogHeight:700px;center:on';
        var url='mapping.aspx?mappingXml=' + mappingXml+'&wiType='+WiType
        var xml = showModalDialog(url, JobDescriptionId, features);
        if (xml == null)
            return 
        IsDirty = true
        $('#MappingXmlHidden').val(xml)

        var Facility= getCookie('Facility')
        $('#Facility').text(Facility)
        var cat = $('#JobDescription').attr('WICategoryName')
        var number = $('#JobDescription').attr('JobDescriptionNumber')        
        var WiNo = WiType+'-'+MyReplace(Facility, ',', '/')+ '-' +cat + '-' + number 
        $('#WiNo').text(WiNo)
    }
    function on_PopupLink(src) {
        var td = $(src).parent().next()
        var name ='WILink'
        var url = 'popupLink.htm?id='+getParameterByName('id')+'&wiType='+WiType
        var data = $('#' + name).attr('xml')
        var elemBody = document.getElementsByTagName("body");
        var features = 'dialogWidth:800px;dialogHeight:400px;center:on';
        var arr = showModalDialog(url, data, features);

        if (arr == null) return
        IsDirty = true

        var s = '', xml = '', link = ''
        var arr1 = []
        for (var i = 0; i < arr.length; i++) {
            var title = arr[i].title
            var link = arr[i].link
            var type = arr[i].type
            title=MyReplace(title, '&','')
            s += '<a target=_new href="' + link + '">' + title + '</a>&nbsp '
            xml += '<' + name + ' title="' + title + '" link="' + link  + '" type="' + type  + '"/>'

        }
        xml= MyReplace(xml, '&', '@!123')
        $('#WILink').attr('xml', xml)
        $('#WILink').html(s)
    }
    function on_PreviewWord() {
        if (ValidateBeforePreview()) {
            if (IsDirty) {
                var r = confirm("You have unsaved changes. Save them Now?")
                if (r) {
                    on_Save()
                }
            }
            __doPostBack('PreviewWord', '');
        }
    }   
    function setCookie(cname, cvalue) {
        //var d = new Date();
        //d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        //var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue //+ "; " + expires;
    }
</script>
