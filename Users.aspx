<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Users" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
    .highlight{
       background-color:darkblue;
       color:white;
    }
        .normal{
       background-color:white;
       color:black;       
    }
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
<script type="text/javascript" src="wi.js"></script>
</HEAD>
<body style1='display:none'>
<form id="form1" runat="server">
<div id="div00" style="position:absolute">
<table cellpadding="7">
<tr colspan="3"><td >User Id: <span id="UserId" />  ( <span id="UserName" /> )
<tr><td >        Role <br /><div id="SelectedRole" style="border:1px solid black;"></div>
<td >        All Roles<br /> <select id="AllRole" style="width:50px;height" runat="server" multiple="true"></select>
<td >        All Rigs <br /><select id="AllRig" runat="server"  multiple="true"></select>
<tr coldiv="3"><td align="middle" >    
     <input type="button" onclick="on_Save()" value="Save" /> 
    <input type="button" onclick="on_Cancel(this)" value="Cancel" />

</table>
</div>

<div id=div01 class="selected"   style="width:100%"  >
<table width="50%" id="t">
<colgroup  ><col width="10%"   style="text-align:right"  /><col /></colgroup>
<tr><td >User Id:  <td > <asp:TextBox ID="SearchText" runat="server"></asp:TextBox>
<tr><td >  <td >
<input type="button" onclick="    on_Search()" value="Search" /> <input type="button" onclick="    on_Add(this)" value="Add" />
    <input type="button" value="Reset" onclick="on_Reset()" />
    <asp:Button ID="SyncUser" runat="server"  Text="Sync Users"  onclick="SyncUser_Click" />
    <asp:Button ID="SearchAD" runat="server"  Text="Search AD"  onclick="SearchAD_Click"  OnClientClick="on_SearchAD()"/>

</table>
    <asp:Literal ID="l" runat="server"  ></asp:Literal>
<div id="div1" class="selected"   style="width:100%"  ></div>
</div>


</form>
</body>
</html>
<script>
    var div00 = $('#div00')
    var div01 = $('#div01')
    var div0 = $('#SelectedRole')
    var div1 = $('#div1')
    var select0 = $('#AllRole')
    var select1 = $('#AllRig')
    var Arr = ['Local', 'User', 'OIM', 'Rig Manager', 'Operations Manager']
    var UserName 
    $(document).ready(function () {
        var h = 200, w = 150
        div00.hide()
        div0.width(w)
        select0.height(h).width(w)
        select1.height(h).width(w)
        div0.closest('table').find('td').css('vertical-align', 'top')
        if (UserId != '011311') {
            $('#SyncUser').hide()
            $('#SearchAD').hide()
        }
        select0.click(function () {
            var option = select0.children('option:selected')
            var value = option.text()
            var html='<div >'+option.text()+'</div>'
            var d=$(html)
            d.prop('id', value).appendTo(div0)
            Highlight(d)
            option.wrap('<div/>')
        })
        select1.click(function () {
            var d0 = $('.highlight', div0).eq(0)
            var name = d0.clone().children().remove().end().text();
            //alert(name)
            if (!Arr.contains(name))
                return
           
            var option = select1.children('option:selected').eq(0)
            MoveOption(option, d0)
            return 
            var text = option.text()
            var id = option.val()
            var html = '<div >' + text  + '</div>'
            var d = $(html)
            d.prop('id', id).css('padding-left', 10).addClass('normal').appendTo(d0)
            option.wrap('<div/>')
        })
        div0.dblclick(function () {
            var target = (event.target) ? event.target : event.srcElement;
            var d = $(target)
            var id = d.clone().children().remove().end().attr('id')
           // alert(id)
            $('option[value="' + id + '"]').unwrap()
            if (isNaN(id))
                UnWrapAll()
            else
                d.parent().addClass('highlight')
            d.remove()
        })
        div0.click(function () {
            var target = (event.target) ? event.target : event.srcElement;
            var d = $(target)
           // alert(d.html())
            if (!isNaN(d.attr('id')))
                Highlight(d.parent())
            else
                Highlight(d)
        })
       // SetupRole('011311')
        $('input[value="Search" ][type="button"]').click()
        $('input[type="text"]').on('keypress', function (e) {
            var code = e.keyCode || e.which;
            if (code == 13) {
                on_Search()
                e.preventDefault();
                return false;
            }
        });
        $('a', $('#TableSearchAD')).click(function () {
            var src = $(this)
            var userId = src.parent().next().text()
           // alert(userId)
            CheckUserId(userId, $('input[type=text]').eq(0))
            src.closest('table').remove()
        })
    })
    function on_Add(src0) {
        var src = $('input[type=text]').eq(0)
        var userId = src.val()
        userId = $.trim(userId)
        if (userId == '') {
            alert('Please enter a User Id')
            return
        }
        CheckUserId(userId, src)
    }
    function CheckUserId(userId, src) {
        $.ajax({
            type: "POST",
            url: "users.aspx/ValidateUser",
            data: '{userId: "' + userId + '" }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.d == '')
                    alert('Invalid user.')
                else {
                    if (div01.text().indexOf(userId) != -1) {
                        alert('The user already exists.')
                        return 
                    }
                    SaveUseProfile(data.d)
                    
                    SetupRole(userId, src)
                }
            },
            failure: function (response) {
                alert(response.d);
            }
        });
    }
    function SaveUseProfile(data) {
        var arr = eval(data)
        UserName = arr[0].Name
        var sql = " if not exists(select * from users where userid='" + arr[0].Id + "' ) insert users select '" + arr[0].Id + "','" + arr[0].Name + "', '" + arr[0].Email + "'"
        CallAjax('sql', sql, 'RunSql'
            ,function(){}
            ,function(){}
            )
    }
    function SetupRole(userId, name, src) {
        var sql = "select * from userRole where userId='" + userId + "'"
        div00.show()
        div01.hide()
        div0.html('')
        //$('#Rig').html('')
        //$('#AllRig').html('')
        //$('#Role1').html('')
        $('#UserId').text(userId)
       // alert(sql)
        CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                    var field = $(element)
                    var role = field.find('role').text()
                    var rigid = field.find('rigId').text()
                    var html = '<div >' + role + '</div>'
                    var d = $('div[id="' + role + '"]', div0)
                    if (!(d.length > 0 && div0.has(d))) {
                        d = $(html)
                        d.prop('id', role).appendTo(div0)
                        $('option[value="'+role+'"]').wrap('<div/>')
                    }
                    if (!isNaN(rigid) && rigid > 0) {
                        UnWrapAll()
                        var option = select1.children('option[value="' + rigid + '"]')
                        //html = option[0].outerHTML
                        //html = MyReplace(html, 'option', 'div')
                        //alert(html)
                        //$(html).appendTo(d)
                        MoveOption(option, d)
                    }
                    Highlight(d)
                    d.children('div').sort(function (a, b) {
                        var aa = $(a).text();
                        var bb = $(b).text();
                        return aa >bb;
                    });

                });
            },
            function (e) {
                alert(e.responseText)
            }
        )
    }
    function MoveOption(option, d0) {
        var id = option.val()
        var html = option[0].outerHTML
        html = MyReplace(html, 'option', 'div')
        var d = $(html)
        d.prop('id', id).css('padding-left', 10).addClass('normal').appendTo(d0)
        option.prop('selected', false).wrap('<div/>')
    }
    function on_Search() {    
        var sql = " select distinct ur.userId, name from userRole ur left join Users u on ur.userId=u.userId where 1=1 "
        var name = $.trim($('input[type=text]').val())
        if (name != '') {
            sql += " and ur.userId like '%" + name + "%'"
        }
        sql += ' order by name '
        div1.html('')
        CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                var s = '<table border =1 width=30% cellspacing=0 cellpadding=5 >'
                $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                    var field = $(element)
                    var userId = field.find('userId').text()
                    var name = field.find('name').text()
                    var ch = ''
                    if (field.find('disabled') && field.find('disabled').text() == 'true')
                        ch = ' checked '
                    s += '<tr><td width=1% nowrap ><input  type=button onclick=on_Delete(this) value=Delete />&nbsp<input  type=button onclick=on_Edit(this) value=Edit />'
                    s += '    &nbsp&nbsp<input type=button class=cancel onclick=on_Cancel(this) value=Cancel /> <td name="' + userId + '">' + userId
                    s += '<td>' + name
                });
                s += '</table>'
                div1.append($(s))
                $('input.cancel').hide()
            },
            function (e) {
                alert(e.responseText)
            }
        )
    }
    function on_Delete(src0) {
        var td = $(src0).parent()
        var userId = td.next().text()
        var sql = " delete userRole where userId= '" + userId + "'"
        CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                //alert('The record was deleted successfully.')
            },
            function (e) {
                alert(e.responseText)
            }
        )
        $('td:contains("' + userId + '")', div01 ).parent().remove()
        $('input[text]').text()
    }
    function on_Cancel(src0) {
        Reset()
        return 
        location.reload()
        return 
        div00.hide()
        div01.show()
    }
    function on_Edit(src0) {
        var src = $(src0)
        var td = src.parent()
        var userId = td.next().text()
        var name = td.next().next().text()
        SetupRole(userId, name, td.next())
    }

    function on_Save2() {
        var userId = $('#UserId').text()
        var role = $('#Role1').find('option:selected').text()
        var sql = "delete userRole where userId='" + userId + "' delete userAccess where  userId='" + userId + "'  insert userRole select '" + userId + "', '" + role + "'"

        switch (role) {
            case 'Local':
            case 'User':
            case 'OIM':
            case 'Rig Manager':
            case 'Operations Manager':
                var rigs = $.map($('#Rig option'), function (ele) {
                    return ele.value;
                });
                if (rigs.toString().trim() == '') {
                    alert('Please select a rig.')
                    return
                }
                var arr = rigs.toString().split(',');
                if (role != 'Local' && role != 'User')
                    sql = " delete approverRig where userId= '" + userId + "' and role='" + role + "'"
                $.each(arr, function (index) {
                    if (role == 'Local' || role == 'User')
                        sql += "  insert userAccess select '" + userId + "', " + arr[index]
                    else
                        sql += "  insert approverRig select '" + userId + "','" + role + "'," + arr[index]
                })
                break
            default:
        }
        CallAjax('sql', sql, 'runSql', function (data, status, xhr) {
            //alert('The record was saved successfully. ')
        }, function (e) { alert(e.responseText) }
       )
        div0.hide()
        div.show()
    }
    function Highlight(d) {
        $('div', div0).removeClass('highlight')
        d.addClass('highlight')
        UnWrapAll()
        d.children('div').each(function () {
            var src=$(this)
            var rigId = src.attr('id')
            //alert(rigId)
            $('option[value="'+rigId+'"]').wrap('<div/>')
        })
        window.status=d.children('div').length
    }
    function UnWrapAll() {
        $('option', select1).each(function () {
            var src = $(this)
            if (src.parent().prop('tagName')=='DIV')
                src.unwrap()
        })
    }
    function Reset() {
        window.location = location.href.replace(location.hash, "")
        return 
        $('option', select0).each(function () {
            var src = $(this)
            if (src.parent().prop('tagName') == 'DIV')
                src.unwrap()
        })
        UnWrapAll()
        div00.hide()
        div01.show()
    }
    function on_Save() {
        if (div0.children('div').length == 0) {
            //alert('Please select a role.')
            //return 
        }
        var done = false
        var UserId = $('#UserId').text()
        var s = " delete userRole where userId= '" + UserId + "'"
        div0.children('div').each(function () {
            if (done)return 
            var d0 = $(this)
            var role = d0.attr('id')
            if (role=='') return 
            var len = d0.children('div').length
            if (Arr.contains(role) && len == 0) {
                alert('Please select rigs for ' + role)
                done=true
                return 
            }
            if (len== 0)
                s += " insert userRole select '" + UserId + "', '" + role + "', null"
            d0.children('div').each(function () {
                var d = $(this)
                var id = d.attr('id')
                s+=" insert userRole select '"+UserId+"', '"+role+"', "+id
            })
            s+=" if not exists(select * from users where userId='"+UserId+"') insert users select '"+UserId+"', null, null"
        })
       if (done) return
        CallAjax('sql', s, 'runSql', function (data, status, xhr) {
            if ($('td:contains("' + UserId + '")', div01).length == 0) {
                var tr0 = div01.find('table').find('tr').last()
                var tr = tr0.clone()
                tr.children('td').eq(1).text(UserId)
                tr.children('td').eq(2).text(UserName)
                tr.insertAfter(tr0)
            }
           Reset()// location.reload()
        }, function (e) { alert(e.responseText);alert(s) }
        )
    }
    Array.prototype.contains = function (obj) {
        var i = this.length;
        while (i--) {
            if (this[i] === obj) {
                return true;
            }
        }
        return false;
    }
    function on_Reset() {
        window.location=window.location.toString()
    }
  </script>
