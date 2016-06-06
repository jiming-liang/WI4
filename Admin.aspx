<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
body {
}
.selected {
     font-weight: bold;
     font-size:20;
     border-bottom-width:thick;
     background-color:rgb(226,239,218) ;
     color1:white;
 }
.normal {
     background-color:;
     border-bottom-width:thin;
     font-size:15;
     border-spacing:10px;
     
 }
span{
    border-spacing:50px;
    border-left-color:white
}
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
<script type="text/javascript" src="wi.js"></script>
</HEAD>

<body style1='display:none'>
<form id="form1" runat="server">
<center ><h1  style="color:rgb(0, 70, 127)">WIMS Administration</h1></center>
    <span class="selected"   > &nbsp Mapping Administration &nbsp</span><span class="normal" > &nbsp Change WIT Status &nbsp</span>
    <br />
<div id="div0" class="selected"   style="width:100%"  >
    <br />
<table width="100%" id="t">
<colgroup  ><col width="10%"   style="text-align:right"  /><col /></colgroup>

<tr><td >List  <td > <asp:DropDownList ID="Entity"   runat="server"  > </asp:DropDownList>
<tr><td >Title:  <td > <input id=Name type="text" />
<tr><td >  <td ><input type="button" onclick="on_Submit()" value="Add" />
    <input type="button" onclick="    on_Search()" value="Search" />
</table>
    <div id="div00" class="selected"   style="width:100%"  ></div>
</div>

    <div id="div1"  class="normal"  style="width:100%" >
<asp:GridView ID="GridView1"   ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle HorizontalAlign="Left"  Font-Size=""/>       
</asp:GridView>  
</div>
<input id=h1 type=hidden  runat=server/>
<div id=holder></div>
</form>
</body>
</html>
<script>
    var t = $('#t')
    var div00 = $('#div00')
    var parent
    var theForm = document.forms['form1']
    $(document).ready(function () {
        $('#GridView1').width('100%')
        $('td:nth-child(1)', t).css('text-align', 'right')
        //$('#Entity').val('WI Category')
        $('select').on('change', function () {
            $('#Name').val('')
        })
        $('#Entity').change(function () {
            var name = $(this).val()
            GetDependence(name)
            div1.html('')
        })
        $('#div1').hide()
        //$('span').height(500).width(1200)
        $('span').click(function () {
            var src = $(this)
            var src2
            if (src.next().prop('tagName') == 'SPAN') {
                index = 0
                src2 = src.next()
            } else {
                index = 1
                src2 = src.prev()
            }
            src.addClass('selected').removeClass('normal')
            src2.addClass('normal').removeClass('selected')
            //  alert(index)
            $('#div' + index).show().addClass('selected').removeClass('normal')
            $('#div' + (index + 1) % 2).hide()

        })


    })
    function on_Search() {
        var table = $('#Entity').val()
        var sql = " select * from " + table + ' where 1=1 '
        var name = $.trim($('input[type=text]').val())
        if (name != '') {
            sql += " and name like '%" + name + "%'"
        }
        if (parent != null) {
            var parentId = $('#' + parent).val()
            sql += " and  " + parent + "id= " + parentId
        }
        sql += ' order by name '
        div00.html('')
        CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                var s = '<table border =1 width=60% cellspacing=0 cellpadding=5 >'
                $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                    var field = $(element)
                    var name = field.find('Name').text()
                    var id = field.find('id').text()
                    var ch = ''
                    if (field.find('disabled') && field.find('disabled').text() == 'true')
                        ch = ' checked '
                    s += '<tr><td width=1% nowrap ><span>Disabled&nbsp&nbsp</span><input  onclick=on_Disable(this) type=checkbox ' + ch + ' /><input id= ' + id + ' type=button onclick=on_Edit(this) value=Edit />'
                    s += '    &nbsp&nbsp<input type=button class=cancel onclick=on_Cancel(this) value=Cancel /> <td name="' + name + '">' + name
                });
                s += '</table>'
                div00.append($(s))
                // sql=sql.toLowerCase()
                if (sql.indexOf('WICategory') > -1 || sql.indexOf('JobDescription') > -1) {
                    var ck = $('input:checkbox')
                    ck.prev().text('')
                    ck.hide()
                }
                $('input.cancel').hide()
            },
            function (e) {
                alert(e.responseText)
            }
        )
    }
    function on_Disable(src0) {
        var src = $(src0)
        var flag = 0
        if (src.prop('checked'))
            flag = 1
        var id = src.next().attr('id')
        var table = $('#Entity').val()
        var sql = ' update ' + table + ' set disabled=' + flag + ' where id=' + id
        CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                alert('The record was updated successfully.')
            },
            function (e) {
                alert(e.responseText)
            }
        )
    }
    function on_Cancel(src0) {
        var src = $(src0)
        src.prev().val('Edit')
        var td = src.parent().next()
        td.text(td.attr('name'))
        src.hide()
    }
    function on_Edit(src0) {
        var src = $(src0)
        var id = src.attr('id')
        var td = src.parent().next()
        if (src.val() == 'Edit') {
            src.val('Save')
            var s = '<input type=text value="' + td.text() + '" />'
            td.html(s)
            src.next().show()
        } else {
            var s = td.find('input').val()
            if ($.trim(s) == '') {
                alert('The field cannot be blank.')
                return
            }
            src.val('Edit')
            var table = $('#Entity').val()
            var sql = " if exists(select * from " + table + " where name='" + name + "') select 0 else  update " + table + " set name='" + s + "' where id=" + id
            CallAjax('sql', sql, 'RunSql'
                , function (xml) {
                    if ($($.parseXML(xml.d)).find('Table').length == 0)
                        s = s //alert('The record saved successfully')
                    else
                        alert('The name already exists')
                },
                function (e) {
                    alert(e.responseText)
                }
            )
            td.html(s)
            src.next().hide()
            td.attr('name', s)
        }
    }
    function on_Submit() {
        var name = $.trim($('input[type=text]').val())
        if (name == '') {
            alert('Please enter a name')
            return
        }
        var table = $('#Entity').val()
        var sql = " if exists(select * from " + table + " where name='" + name + "' {0}) select 0 else insert " + table + " (name {1} ) select '" + name + "' {2}"
        var sql0='', sql1='', sql2=''
        if (parent != null) {
            var pId=parent + "Id"
            var pValue=$('#' + parent).val()
            sql0 = ' and ' + pId + '=' + pValue
            sql1 = ', ' + pId
            sql2=', '+pValue
        }
        sql=sql.format([sql0, sql1, sql2])
        CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                if ($($.parseXML(xml.d)).find('Table').length == 0)
                    alert('The record saved successfully')
                else
                    alert('The name already exists')
            },
            function (e) {
                alert(e.responseText)
            }
        )
    }
    function RefreshDropDown(data) {
        parent = null
        $($.parseXML(data)).find('Table').each(function (index, element) {
            var field = $(element)
            parent = field.find('Name').text()
        });
        var count = $('tr', t).length
        $("tr", t).each(function (index) {
            if (index != 0 && index < count - 2) {
                $(this).remove()
            }
        })
        if (parent != null) {
            var tr = $('tr:last-child', t)
            tr = tr.prev()
            var parent_label = parent
            if (parent_label == 'WICategory')
                parent_label = 'Job Category'
            var src = $('<tr><td style="text-align:right">' + parent_label + '<td><select  id=' + parent + ' /> ')

            src.insertBefore(tr)
            src = $('select', src)
            //  alert(tr.html())
            $($.parseXML(data)).find('Table1').each(function (index, element) {
                var field = $(element)
                var name = field.find('Name').text()
                var id = field.find('id').text()
                src.append('<option value="' + id + '" >' + name + '</option>')
            });
        }
        div00.html('')
    }

    function GetDependence(name) {
        CallAjax('name', name, 'GetDependence', function (xml) {
            RefreshDropDown(xml.d)
        },
            function (e) {
                alert(e.responseText)
            }
       )
    }
    function on_Save(name, id) {
        var status = $('input[name=' + name + ']:checked').val()
        //        var sql = "update wi set STGstatus=case when  '" + status + "'='Reviewed' then 'Complete' else STGstatus end   , status='" + status + "' where id= " + id
        var sql = "update wi set  status='" + status + "' where id= " + id
        CallAjax('sql', sql, 'runSql', function (data, status, xhr) {
            alert('The status was updated successfully. ')
        },
        function (e) {
            alert(e.responseText)
        }
       )
    }
</script>
