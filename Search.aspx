<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Search" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
body, table1 {
 font-family:Arial;font-size:10pt;
}
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
<script type="text/javascript" src="js/spin.min.js"></script>
<script type="text/javascript" src="wi.js"></script>
</HEAD>

<body style1='display:none'>
<form id="form1" runat="server">
<center ><h1  id="MyTitle" style="color:rgb(0, 70, 127)" runat="server">Work Instruction Template Search</h1></center>
<table width="100%" id="t" cellpading="3">
<tr>
<td><table>
<tr><td >Job Category:  <td > <asp:DropDownList ID="WICategory"   runat="server"  AutoPostBack="true" OnSelectedIndexChanged="WICategory_SelectedIndexChanged" > </asp:DropDownList>
<tr><td >Job Title:  <td > <asp:DropDownList ID="JobDescription"   runat="server"   > </asp:DropDownList>
<tr><td >Position:  <td > <asp:DropDownList ID="Position"   runat="server"   > </asp:DropDownList>
</table>
<td><table>
<tr><td rowspan="1" >Facility:  <td >
    <table id="FacilityLocal" cellpadding="0" cellspacing="0"><tr><td><input id="RigText" type="text" /><td><img id="RigImg" src="Images/downArrow.png" />  

    </table>
<tr><td >Facility Type:  <td > <asp:DropDownList ID="RigType"   runat="server" AutoPostBack="true" OnSelectedIndexChanged="RigType_SelectedIndexChanged" > </asp:DropDownList>
<tr><td >Rig Design:  <td > <asp:DropDownList ID="RigDesign"   runat="server"   AutoPostBack="true" OnSelectedIndexChanged="RigDesign_SelectedIndexChanged"> </asp:DropDownList>
<tr><td >Facility:  <td > <asp:DropDownList id="Rig" runat="server"  ></asp:DropDownList> 
<tr><td  >Work Instruction Type:  <td> <asp:DropDownList ID="WorkInstructionType"   runat="server"   > </asp:DropDownList>

</table>
<td><table>
<tr><td >Equipment Type:  <td > <asp:DropDownList ID="EquipmentType"   runat="server"  AutoPostBack="true" OnSelectedIndexChanged="EquipmentType_SelectedIndexChanged"  > </asp:DropDownList>
<tr><td >Equipment Make:  <td > <asp:DropDownList ID="EquipmentMake"   runat="server"   > </asp:DropDownList>
</table>
<td >Job Criticality:  <td > <asp:DropDownList ID="Criticality"   runat="server"   > </asp:DropDownList>
<td >Status:  <td > <asp:DropDownList ID="Status"   runat="server"   > </asp:DropDownList>

<tr align="center">
<td colspan="10"><asp:TextBox ID="SearchText" runat="server" />  
<asp:Button ID="Submit" runat="server"  Text="Search" OnClick="Submit_Click"  OnClientClick="on_Submit()"/> 
<asp:Button ID="Clear" runat="server"  Text="Clear" OnClick="Clear_Click" OnClientClick="on_Clear()" /> 
<input type="button" onclick="on_WIS()" value="New Rig-Specific Work Instruction" />
<asp:Button ID="Export" runat="server"  Text="Export" OnClick="Export_Click" OnClientClick="on_Export(this)" /> 

<%--<input type="button" value="Search" onclick="on_Search()" />  
<input type="button" value="Clear" onclick="on_Clear()" />  
<input type="button" value="Export" onclick="on_Export()" />  --%>
</table>
<div id="LocalDiv" > 
<a href="status.aspx">	Status Report  </a>&nbsp
<a href="search.aspx">    Work Instruction Template Library</a>
</div>
    <div id="DivCount"  style="text-align:center"></div>

 <asp:GridView  EnableViewState="false" ID="GridView1" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="11pt" ForeColor=White BackColor= darkblue />
<RowStyle HorizontalAlign="Left"  Font-Size=""/>       
</asp:GridView>  
    <asp:Panel ID="Paging" runat="server" HorizontalAlign="Center"></asp:Panel>
<table id="GridView11" border="1"   width="100%" style="border-color: black; width: 100%; border-collapse: collapse;" cellpadding="4" cellspacing="0"></table>
<input id=h1 type=hidden  runat=server/>
<div id=holder></div>
<%--<iframe2  id="f1"  frameBorder="0" scrolling1="no" width="100%" ></iframe2>--%>
<iframe id="f" > </iframe>
        <asp:ListBox  runat="server"  SelectionMode="Multiple"  Height="100" ID ="RigListBox" ></asp:ListBox> 
</form>
</body>
</html>
<script>
    var t = $('#GridView1')
    var t1 = $('#GridView112')
    var BinaryIndex
    var sort
    var IsLocal
    var theForm = document.forms['form1']
    var f = $('#f')
    $(document).ready(function () {        
        if (getParameterByName ('islocal') != '')
            IsLocal = true
        else
            $('#LocalDiv').hide()
        $('select').width($('#WICategory').width())

        $('select').change(function () {
            if (!IsLocal)
                return 
            var src = $(this)
            var index = src.closest('td').index()
            var tr = src.closest('tr')
            var id = src.attr('id')
            if (id == 'JobDescription')
                return 
            var value = src.val()
            tr = tr.next()
            if (tr.length != 0 && tr.find('td').eq(index).find('select').length != 0) {
                var select = tr.find('td').eq(index).find('select')
                if (select.length != 0)
                    select.find('option').remove()
                if (value != '') {
                    var sql = ' select * from ' + select.attr('id') + ' where ' + id + 'id=' + value
                    select.append('<option value="" ></option>')
                    CallAjax('sql', sql, 'RunSql'
                    , function (xml) {
                        $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                            var field = $(element)
                            var name = field.find('Name').text()
                            var id = field.find('id').text()
                            select.append('<option value="' + id + '" >' + name + '</option>')
                        });
                    },
                    function (e) {
                        alert(e.responseText)
                        alert(sql)
                    })
                }
                tr = tr.next()
                while (tr.length != 0) {
                    tr.find('option').remove()
                    tr = tr.next()
                }
            }
        })
       // on_Search()
        Init()
        InitBreadCrumb()
        InitFacility()
        NoWrap()
        $('th:contains("Updated on")', t).click()
    })
    function on_Submit() {
        var doc = $("#f").contents()[0];
        var arr=[]
        $(doc.body).find('div').each(function () {
            var src = $(this)
           // if (src.attr('id').indexOf('All')>-1) return 
            if (src.find('input:checkbox:checked').length>0  )
                arr.push(src.find('input').attr('id'))
        })
        var s = arr.join()
        setCookie('SearchRigList', s)
        setCookie('CheckAll', $('input:radio:checked', $(doc.body)).val())
        if (s!='')  
            s = ' and a.rigId in (' + s + ') '
        $('#h1').val(s)
    }
    function setCookie(cname, cvalue) {
        document.cookie = cname + "=" + cvalue //+ "; " + expires;
    }
    function InitFacility() {
        var t=$('#t')
        if (IsLocal) {
            $('td:nth-child(2)', t).find('tr:gt(1):lt(3)').remove()
            $('#WorkInstructionType').parent().css('text-align', 'left')
            $('#WorkInstructionType').find('option').eq(1).val('WIS')
            $('#WorkInstructionType').find('option').eq(2).val('WI')
            if (Roles.length==0)
                $('input[type="button"][value="New Rig-Specific Work Instruction"]').hide()
        } else {
            $('td:nth-child(2)', t).find('tr:lt(2)').remove()
            $('#RigListBox').remove()
            f.hide()
            $('#WorkInstructionType').parent().parent().hide()
            $('input[type="button"][value="New Rig-Specific Work Instruction"]').hide()
            return 
        }
        var doc = f.contents()[0];
        f.width(210).height(400)
        var cookie = getCookie('SearchRigList')
        var cookieCheckAll = getCookie('CheckAll')
        var s = '<div style="background-color:lightblue"><input '+(cookieCheckAll==1?'checked':'')+' type=radio name=CheckAll value=1 />Check All <input '+(cookieCheckAll==0?'checked':'')+' type=radio name=CheckAll value=0 />Uncheck All</div>'
        var arr
        if (cookie!=null)
            arr = cookie.split(',')
        $('#RigListBox').find('option').each(function () {
            var src = $(this)
            var id = src.attr('value')
            var checked = ''
            if (cookie != null && arr.indexOf(id)>-1)
                checked = ' checked '
            else
                if (src.prop('selected'))
                    checked = ' checked '
            s += '<div><input type=checkbox ' + checked + ' id=' + id + ' />' + src.text() + '</div>'
        })
        $('#RigListBox').remove()
        $(doc.body).html(s)
        if ($(doc.body).find('input:checkbox:checked').length != $(doc.body).find('input:checkbox').length)
            $('input:radio', $(doc.body)).eq(0).prop('checked', false)
        f.hide()
        var rigImg = $('#RigImg')
        var rigText = $('#RigText')
        rigImg.height(rigText.height() + 5)
        rigImg.position().top = rigText.position().top
        rigImg.on('click', function () {
            ShowRig()
        })
        var X = 0, Y = 0
        var len = $('input:checkbox', $(doc.body)).length
        $(document).mousemove(function (e) {
            var x0 = f.position().left
            var x1 = x0 + f.width()
            var y0 = f.position().top
            var y1 = y0 + f.height()
            if ((e.clientX < X && e.clientX < x0) || (e.clientX > X && e.clientX > x1)
                || (e.clientY < Y && e.clientY < y0) || (e.clientY > Y && e.clientY > y1)) {
                var selected = $('input:checkbox:checked', $(doc.body)).length
                if (selected == len) selected = 'All'
                rigText.val(selected + ' selected')
                f.hide()
            }
            X = e.clientX;
            Y = e.clientY;
        })
        rigText.keypress(function (e) {
            if (e.which != 13) return
            e.preventDefault()
            var text = $.trim($(this).val())
            $(doc.body).find('input').prop('checked', false)
            $(doc.body).find('div').each(function () {
                var src = $(this)
                if (src.find('input:radio').length>0)return 
                if (src.text().indexOf(text) == -1)
                    src.hide()
                else
                    src.show()
            })
            ShowRig()
        })
        $('input:radio', $(doc.body)).change(function () {
            if ($(this).val()==1)   
                $('input:checkbox', $(doc.body).find('div:visible')).prop('checked', true)
            else
                $('input:checkbox', $(doc.body)).prop('checked', false )
        })
        $('input:checkbox', $(doc.body)).change(function () {
            $('input:radio', $(doc.body)).prop('checked', false)
        })
        //        if (cookieCheckAll==null &&  ( IsRole('Admin')||IsRole('Global')||IsRole('Public')))
        if (cookieCheckAll == null && !IsRole('Local') && !IsRole('User'))
            $('input:radio', $(doc.body)).eq(0).click()
        $(doc.body).css('font-family', 'arial').css('font-size', '10pt')
    }
    function ShowRig() {
        var src = $('#RigText')
        var w, h, top, left
        w = src.width()
        h = src.height()
        top = src.position().top + h + 2 
        left = src.position().left  
        f.css({ position: 'absolute', top: '' + top + 'px', left: '' + left + 'px' })
        f.css('background-color', 'white');
        f.show()
    }
    function InitBreadCrumb() {
        if (!IsLocal) return
        var span = $('table').eq(1).find('span').eq(0)
        span.find('span').eq(2).text('Work Instruction Library')
    }
    function Init() {
        if (!IsRole('Admin') || IsLocal)
            RemoveColumn('Flag', t)
        $('td:even', $('#t').find('table')).css('text-align', 'right')
        $('td', $('#t')).css('vertical-align', 'top')
        $('td:odd:not(:last)', $('#t')).css('text-align', 'right')
        InitSort()
        InitApproved()
        HideColumn('Date')
       // $('th:contains("Updated on")', t).click()
        $("th:contains('Flag')", t).text('Active/Inactive')
        var arr = ['id', 'RigId', 'wiid', 'createdBy', 'la']
        for (var i in arr)
            RemoveColumn(arr[i], t)
    }
    function InitDateColumn() {
        if ($('tr', t).length == 0) return
        var index = GetColumnIndex('Updated on')
        $('td:nth-child(' + index + ')', t).each(function (i) {
        })
    }
    function IsLocalRig(RigId) {
        for(var i = 0; i < Roles.length; i++) {
            if ( (Roles[i][0]=='Local' || Roles[i][0]=='User') &&  Roles[i][1]==RigId)
                return true 
        }
        return false 
    }
    function GetColumnIndex(name) {
        var index 
        $('th', t).each(function (i) {
            var src = $(this)
            if (src.text() == name)
                index=  i + 1
        })
        return index 
    }
    function InitApproved() {
        if ($('tr', t).length == 0) return
        var index = GetColumnIndex('la')
        var indexRigId = GetColumnIndex('RigId')
        var RigId
        $('td:nth-child(' + index + ')', t).each(function (i) {
            var src = $(this)
            if (IsLocal)
                RigId = $('td:nth-child(' + indexRigId + ')', src.parent()).text()
            var readOnly = true
            if ((IsLocal && (IsLocalRig(RigId) || IsRole('Admin'))) || (!IsLocal && (IsRole('Admin') || IsRole('Global'))))
                readOnly = false
            if ($.trim(src.text()) != '0') {
                var td = src.parent().children('td').eq(0)
                var a = td.find('a')
                var a1 = a.clone()
                var url = a.attr('href')
                if (readOnly)
                    url += '&action=approved'
                a.attr('href', url)
            } 
        })
    }
    function InitSort() {
        $('th', t).each(function () {
            var src = $(this)
            var img = src.find('img')
            if (img.length == 0) {
                img = $('<img  width=20px src=images/sort_neutral.png />')
                src.append(img)
            }
        })
        $('th', t).click(function (index) {
            var src = $(this);
            var ths = document.getElementsByTagName('th');
            var max = ths.length;
            var currIndex;
            for (var i = 0; i < max; i++) {
                var theader = $(ths[i]);
                var img = theader.find('img');

                if (i != src[0].cellIndex) { // Reset to default if not selected
                    img.attr('src', 'images/sort_neutral.png');
                }
            }

            var img = src.find('img');
            if (img.length > 0) {
                if (img.attr('src').indexOf('desc') == -1) {
                    img.attr('src', 'images/sort_desc.png')
                    sort = 'desc'
                } else {
                    img.attr('src', 'images/sort_asc.png')
                    sort = 'asc'
                }
            }
            var col = src.index()
            $('tr', t).each(function (index) {
                var src = $(this)
                if (index < 2) return

                var i = 1
                // while (i < index && Compare(CellText(src, col), CellText(t.find('tr').eq(i), col), sort)) i++
                BinarySearch(src, i, index - 1, col)
                if (BinaryIndex != index)
                    src.insertBefore(t.find('tr').eq(BinaryIndex))
            })
        })
    }
    function on_HistoryLog(id,wiType) {
        var features = 'dialogWidth:900px;dialogHeight:500px;center:on';
        var result = showModalDialog('HistoryLog.aspx?id=' + id + '&wiType=' + wiType, null, features);
        if (result != null)
            window.location=result
    }
    function HideColumn(name) {
        $('th', t).each(function (index) {
            var src = $(this)
            if (src.text().toLowerCase() == name.toLowerCase()) {
                src.hide()
                $('td:nth-child(' + (index + 1) + ')', t).hide()
            }
        })
    }
    function on_Flag(src0, id) {
        var src = $(src0)
        var flag = src.val()
        var canceled = false
        var items = [{ id: id }, { sp: 'usp_getWiNoAffected' }];
        var data = "{'items':" + JSON.stringify(items) + "}"

        if (flag == "Active")
            CallAjax2(data, 'runSP', function (xml, status, xhr) {
                var arr = []
                $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                    var field = $(element)
                    var name = field.find('Name').text()
                    arr.push(name)
                });
                if (arr.length != 0) {
                    var s = 'Are you sure you want to deactivate this work instruction template? The following WIs will be deactivated:\n' + arr.join(', ')
                    if (confirm(s))
                        Update_Flag(src0, id, true)
                } else
                    Update_Flag(src0, id)
            }, function (e) {
                alert(e.responseText)
            }, 'false')
        else
            Update_Flag(src0, id)
    }
    function Update_Flag(src0, id, confirmed) {
        var src = $(src0)
        var flag = src.val()
        if (!confirmed && !confirm('Are you sure to make changes to this record?'))
            return
        flag= (flag == "Inactive"? 'Active':'Inactive')
        src.val(flag)
        //var sql = "update wi set  flag='" + flag + "' where id= " + id
        var items = [{ id: id }, { sp: 'usp_updateFlag' }];
        var data = "{'items':" + JSON.stringify(items) + "}"
        CallAjax2(data, 'runSP', function (xml, status, xhr) {
            //  alert('The status was updated successfully. ')
        }, function (e) {
            alert(e.responseText)
        })
    }
    function BinarySearch(src, i, j, col) {
        var k = (i + j) / 2
        k = parseInt(k)

        if (i + 1 >= j) {
            if (Compare(CellText(src, col), CellText(t.find('tr').eq(j), col)))
                BinaryIndex = j + 1
            else if (!Compare(CellText(src, col), CellText(t.find('tr').eq(i), col)))
                BinaryIndex = i
            else
                BinaryIndex = j
            return
        }
        if (Compare(CellText(src, col), CellText(t.find('tr').eq(k), col)))
            BinarySearch(src, k, j, col)
        else
            BinarySearch(src, i, k, col)
    }
    function Compare(s1, s2) {
        s1 = GetDate(s1)
        s2 = GetDate(s2)
        if (sort == 'asc')
            return s1 > s2
        else
            return s1 < s2
    }
    function GetDate(s) {
        try {
            var arr = s.split('-')
            var d = new Date(arr[1] + ' ' + arr[0] + ' ' + (2000 + parseInt(arr[2])))
            if (d=='Invalid Date')
                return s
            else
                return d 
        } catch (e) {
            return s 
        }
    }
    function CellText(tr, i) {
        var s = $.trim(tr.find('td').eq(i).text())
        var index = $('th:contains("Date")', t).index()
        if (t.find('th').eq(i).text == 'Updated on')
            s = $.trim(tr.find('td').eq(index).text())
        return s
    }
    function NoWrap() {
        var arr = ['Equipment Type', 'Updated by', 'Updated on']
        $('th', t).each(function () {
            var src = $(this)
            for( var name in arr){
                if (name==src.text())
                    src.css('white-space', 'nowrap')

            }
        })
    }
    function RemoveColumn(name, tt) {
        var th = $('th', tt).filter(function () {
            return $(this).text() === name
        })
        if (th.length==0)return 
        var index = th.index()
        $('td:nth-child(' + (index + 1) + ')', tt).remove()
        th.remove()
    }
    function ReplaceColumn(name, tt) {
        var th = $('th', tt).filter(function () {
            return $(this).text() === name
        })
        if (th.length == 0) return
        var index = th.index()
        $('td:nth-child(' + (index + 1) + ')', tt).each(function () {
            var td = $(this)
            td.html(td.find('input').val())
        })
    }
    function on_Paging(src0) {
        var src = $(src0);
        __doPostBack('Paging', src.text())
    }
    function on_Export() {
        var tt = t.clone()
        var arr = ['la', 'id','Date']
        for (var i in arr) {
            RemoveColumn(arr[i], tt)
        }
        $('a', tt).each(function () {
            var a = $(this)
            var td = a.parent()
            var url = GetSiteUrl() + '/'+a.attr('href')
            var index = td.index()
            if (tt.find('th').eq(index).text() == 'Updated on')
                td.text(a.text())
            else 
                a.attr('href', url)

        })
       tt.find('img').remove()
       ReplaceColumn('Active/Inactive', tt)
        var s = tt[0].outerHTML
        s=escapeXml(s)
        $('#h1').val(s)
        SetTabIndex()
        return 
        __doPostBack('Export', s)
    }
    function SetTabIndex() {
        $('input,select,a ').each(function () {
            var $input = $(this);
            $input.attr("tabindex", -1);
        });

    }
    function GetWhere() {
        var where=' where 1=1 '
        $('select ').each(function (){
            var src=$(this)
            if (src.val()=='' || src.find('option').length==0) return 
            var id0=src.attr('id')
            var id='a.'+id0
            if (id0 == 'WICategory' || id0 == 'EquipmentType')
                id=id0
            if (id0 == 'Criticality' || id0 == 'Status')
                where += " and "+id+" in ('"+src.val()+"')"
            else
                where += " and "+id+ "Id in (" + src.val() + ")";
        })
        var s=$.trim($('#SearchText').val())
        if (s != "") {
            var  like=" like '%" + s + "%'";
            where += " and (j.name "+like +" or a.WiNo "+like+")";
        }
        return where 
    }
    function on_Spin(cursor) {
        if ($("body").css("cursor")==cursor) return 
        $("body").css("cursor", cursor);
        $("input").css("cursor", cursor);
        return 
        var src = f.attr('src')
        if (src == 'spin.htm')
            f.attr('src', '')
        else
            f.attr('src', 'spin.htm')
    }
    function on_Search() {
        t.html('')
        on_Spin('wait')
        $("body").css("cursor", "wait");
        //__doPostBack('Search', '')
        var sp = 'usp_searchWI'
        if (IsLocal)
            sp = 'usp_searchWIlocal'
        var items = [{ sp: sp },
       { where: GetWhere() },
       { userId:  UserId}];
        var data = "{'items':" + JSON.stringify(items) + "}"
        CallAjax2(data, 'RunSP'
                    , function (xml) {
                        $($.parseXML(xml.d)).find('Table').each(function (index) {
                            var src = $(this)
                            var s = '<tr>'
                            var header = '<tr style="color: white; font-size: 14pt; font-weight: normal; background-color: rgb(0, 70, 127);">'
                            src.children().each(function () {
                                var td = $(this)
                                if (index == 0)
                                    header+='<th>'+MyReplace( td[0].tagName, '_x0020_', ' ')+'</th>'
                                var text=td.text()
                                if (text==undefined )text=''
                                s+='<td>'+ text+'</td>'
                            })
                            if (index == 0)
                                $(header).appendTo(t)
                           $(s+'</tr>').appendTo(t)
                        });
                        FormatGridView()
                        Init()
                        on_Spin('default')
                    },
                    function (e) {
                        $('#holder').text(e.responseText)
                        alert( GetWhere())
                    })

    }
    function  FormatGridView() {
        $('tr', t).each(function (index){
            if (index==0)return 
            var tr=$(this)
            var id=GetCell(tr, 'id').text()
            var para = "id=" + id
            var td = GetCell(tr, "Work Instruction Template No.");
            if (IsLocal) {
                var rigId = GetCell(tr, 'RigId').text()
                if (GetCell(tr, 'Status').text()=='New')
                    para+='&rigId='+rigId
                else
                    para='localId='+id
                td=GetCell(tr, "Work Instruction No.");
            }
            td.html("<a href=Create.aspx?" + para + " >" + td.text() + "</a>")

            td = GetCell(tr, "Updated on");
            td.html("<a href=# onclick=on_HistoryLog(" + id + ") >" + td.text() + "</a>")

            td = GetCell(tr, "Flag");
            if (td == null) return;
            var flag = td.text();
            if (flag != "Inactive")
                flag = "Active";
            td.html( "<input type=button value=" + flag + " id=" + id + " onclick='on_Flag(this, " + id +")' />")
        })
    }
    function GetCell(tr, name) {
        var index
        var th = $('th:contains("' + name + '")', t)
        if (th.length>0)
            index = th.index()
        else
            return null
        return tr.children('td').eq(index)
    }
    function on_Clear() {
        setCookie('SearchRigList', null)
        setCookie('CheckAll', null )
        return            
        t.html('')
        $('input:text').val('')
        $('tr:gt(0)', $('#t')).find('select').find('option').remove()
        $('tr:lt(1)', $('#t')).find('select').each(function(){
            var src=$(this)
            src.val($("option:first", src).val());
        })
    }
    function on_WIS() {
        window.location='mapping.aspx?WiType=WIS'
    }
</script>