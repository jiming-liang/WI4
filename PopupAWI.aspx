<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="PopupAWI.aspx.cs" Inherits="PopupAWI" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
    <style>
        body {
            font-family: Arial;    font-size: 10pt;
        }
    </style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="wi.js"></script>
</HEAD>
<body style1='display:none'>
<form id="form1" runat="server">

        <table id=t border1="1" width="100%" style="border-collapse:collapse;" cellpadding="4">
<tr><td colspan="4">        <b>Select all associated work instructions for this job:</b>
<tr><td  align="right">WI Category:  <td > <asp:DropDownList ID="WICategory"   runat="server"  AutoPostBack="false" OnSelectedIndexChanged="WICategory_SelectedIndexChanged" > </asp:DropDownList>
  <td align="right">  Equipment Type:  <td > <asp:DropDownList ID="EquipmentType"   runat="server"  AutoPostBack="false" OnSelectedIndexChanged="EquipmentType_SelectedIndexChanged"  > </asp:DropDownList>
<tr><td align="right">Job Title:  <td > <asp:DropDownList ID="JobDescription"   runat="server"   > </asp:DropDownList>

<td align="right">Equipment Make:  <td > <asp:DropDownList ID="EquipmentMake"   runat="server"   > </asp:DropDownList>
<tr><td colspan="4"><asp:TextBox ID="SearchText" runat="server"></asp:TextBox>
    <input type="button"    onclick="on_Search()" value="Search" />
<tr><td colspan="4">
    <input type="button"    onclick="on_Save()" value="Add" />
    <input type="button"  onclick=" window.close()"  value="Cancel" />
    </table>

          <table  id=t0 border1="1" width="100%"></table>
        <br />
        <table  id=t1 border1="1" width="100%"></table>
</form>
</body>
</html>
<script>
    var theForm = document.forms['form1']
    var t = $('#t')
    var t0 = $('#t0')
    var t1 = $('#t1')
    var Id = getParameterByName('Id')
    var RigName = getParameterByName('RigName')
    var Type = ''
    var WiType = getParameterByName('WiType').toUpperCase()
    $(document).ready(function () {
        if (Id == '')
            Id=0
        if (WiType!='WIT')
            Type='Local'
        var xml = window.dialogArguments
        xml = '<r>' + xml + '</r>'
        $($.parseXML(xml)).find('WI').each(function (index, element) {
            var src = $(this)
            var type = src.attr('type')
            var disabled = 'disabled'
            if (src.attr('type') == Type)
                disabled=''
            var s = '<tr><td ><input checked '+disabled+' type=checkbox id=' + src.attr('id') + ' />' + src.attr('name') + '</td></tr>'
            $(s).appendTo(t1)
        });
        $('select').change(function () {
            var src = $(this)
            var id = src.attr('id')
            if (id == 'WICategory')
                FillDropdown('WICategory', 'JobDescription')
            if (id == 'EquipmentType')
                FillDropdown('EquipmentType', 'EquipmentMake')
            Init()
        })
        $('input').keypress(function (e) {
            if (e.which == 13) {
                on_Search()
                return false
            }
        });
        Init()
        $(document).prop('title', 'Associated Work Instructions' + $(document).prop('title'))
    })
    function FillDropdown(id1, id2) {
        var id = $('#' + id1).val()
        var src = $('#' + id2)
        src.html('')
        if (id == '')
            return
        var sql = " select * from " + id2 + " where " + id1 + "id=" + id
        CallAjax('sql', sql, 'runSql', function (xml) {
            $('<option ></option>').appendTo(src)
            $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                var field = $(element)
                var name = field.find('Name').text()
                var id = field.find('id').text()
                var s='<option value='+id+'>'+name+'</option>'
                $(s).appendTo(src)
            });
        },
        function (e) {
            alert(e.responseText)
        }
       )
    }
    function Init() {
        $('select', t).each(function () {
            var src = $(this)
            var td = src.parent()
            var tr=td.parent()
            if (src.find('option').length == 0) {
                var src0 = tr.prev().children('td').eq(td.index())
                src.width(src0.find('select').eq(0).width())
            }
        })
        $('#__VIEWSTATE').parent().remove()
        $('#__VIEWSTATEGENERATOR').parent().remove()
    }
    function on_Cancel() {
        window.close()
        return
    }
    function on_Search() {
        var sp = 'usp_searchAWI'
        var items = [{ where: GetWhere() }];
        items.push({ id: Id})
        if (Type == 'Local') {
            items.push({  RigName: RigName })
            sp = 'usp_searchAWIlocal'
        }
        items.push({sp:sp})
        var data = "{'items':" + JSON.stringify(items) + "}"
        CallAjax2(data, 'runSP', function (xml) {
            //alert(xml.d)
            t0.html('')
            $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                var field = $(element)
                var name = field.find('Name').text()
                var id = field.find('id').text()
                if ($('#'+id).length!=0)
                    return 
                var s = '<tr><td ><input  type=checkbox id=' + id + ' />' + name + '</td></tr>'
                $(s).appendTo(t0)
            });
        },
        function (e) {
            alert(e.responseText)
        }
       )
    }
    function GetWhere() {
        var where = "  where 1=1";
        $('select').each(function () {
            var src = $(this)
            var value = src.val()
            if (value != null && value != '')
                where += " and " + src.attr('id') + "Id in (" + value + ")";

        })
        var text = $.trim($('#SearchText').val())
        if (text != '')
            where += " and j.name like '%" + text + "%'";
        //alert(where)
        return (where)
    }
    function on_Save() {
        var arr = [];
        var validate = true
        $('input[type=checkbox]:checked').each(function (index) {
            var src = $(this)
            var id = src.attr('id')
            var name = src.parent().text()
            var type='Local'
            if (Type=='' || src.prop('disabled'))
                type=''
            arr.push({ id: id, name: name, type:type });
        })
        if (!validate) {
            return
        }
        arr.sort(function (a, b) {
            if (b.name == 'Other')
                return -1
            if (a.name == 'Other')
                return 1
            if (a.name < b.name)
                return -1
            if (a.name > b.name)
                return 1
            if (a.name == b.name)
                return 0
        })
        window.returnValue = arr;
        window.close();
        return
    }
    function __doPostBack(eventTarget, eventArgument) {
        if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
            theForm.__EVENTTARGET.value = eventTarget;
            theForm.__EVENTARGUMENT.value = eventArgument;
            theForm.submit();
        }
    }
</script>
