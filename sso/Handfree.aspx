<%@ Page Language="C#" enableEventValidation="false" ValidateRequest="false"  EnableViewStateMac="false" AutoEventWireup="true" CodeFile="Handfree.aspx.cs" Inherits="Handfree" %>
<%@ Reference Page="../WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
    body {
    padding:10px;
    font-size: 10pt;
    font-family: arial;
}

    table, select, input {
    font-size: 10pt;
    font-family: arial;
    margin:2px;
}

td{
    padding-left:1px;
    border-left: 1px solid #CCC;
    border-right: 1px solid #CCC;
    border-top: 1px solid #CCC;
    border-bottom: 1px solid #CCC;
}

</style>
<script type="text/javascript" src="../js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="../WI.js"></script>



<body style1='display:none'>
<form id="form1" runat="server">

<input type="button"    value="Add" onclick="on_Add() " />
<asp:GridView ID="GridView1" ShowHeaderWhenEmpty Width="100%" runat="server"   Caption="" Font-Size1 ="14pt"  CellPadding="5" BorderColor="Black">
<HeaderStyle  Font-Bold=false  Font-Size="14pt" ForeColor=White BackColor= darkblue />
<RowStyle  Font-Size=""/>     
<AlternatingRowStyle  />  
</asp:GridView>  

</form>
</body>
</html>
<script>
    var t = $('#GridView1')
    $(document).ready(function () {
        Init()
       // on_Add()
        t.on('click', 'input',   function () {
            var src = $(this)
            var td = src.parent()
            var tr=td.parent()
            var name = src.val()
            var id = td.attr('id')
            var sql
            switch (name) {
                case 'Edit':
                    $('td:gt(0)', tr).each(function () {
                        var src = $(this)
                        var s = '<input style="width:100%" type=text value="' + src.text() + '" />'
                        src.html(s)
                    })
                    ShowSave(tr, true)
                    break
                case 'Delete':
                    if (id == null)
                        return 
                     sql = ' delete  FileWatchList where id='+id
                     Save(sql)
                    tr.remove()
                    break;
                case 'Save':
                    var arr0 = [], arr1 = [], arr2=[]
                    $('td:gt(0)', tr).each(function (index) {
                        var src = $(this)
                        var field= t.find('th').eq(index+1).text()
                        var value =  src.find('input').val() 
                        arr0.push(field + "='" + value + "'")
                        arr1.push(field)
                        arr2.push("'" + value + "'")
                        src.html(value)
                    })
                    if (id == null)
                        sql = ' insert FileWatchList ( ' + arr1.toString() + ') select ' + arr2.toString()
                    else
                        sql = ' update FileWatchList set ' + arr0.toString() + ' where id=' + id
                    sql = sql.replace(/\\/g, '\\\\');
                  //  alert(sql)
                    Save(sql)
                    window.location=window.location
                    break;
                case 'Cancel':
                    window.location = window.location
                    break;
            }            
        })

    })
    function ShowSave(tr, b) {
        if (b) {
            $("input[value*='Save'] , input[value*='Cancel']", tr).show()
            $("input[value*='Delete'] , input[value*='Edit']", tr).hide()
        } else {
            $("input[value*='Save'] , input[value*='Cancel']", tr).hide()
            $("input[value*='Delete'] , input[value*='Edit']", tr).show()
        }
    }
    function Init() {
        var arr=['Delete', 'Edit', 'Save', 'Cancel']
        var s = ''
        for (var i in arr) {
            s+=GetButton(arr[i])
        }
        $('td:nth-child(1)', t).each(function () {
            var src = $(this)
            src.attr('id', src.text())
        })
        $('td:nth-child(1)', t).html(s)
        t.find('th').eq(0).width('1%')
        $("input[value*='Save'] , input[value*='Cancel']").hide()
    }
    function GetButton(name) {
        return '<input type=button value='+name+' />'
    }
    function on_Add() {
        t.find('tbody').append($('<tr/>'))
        var tr = t.find('tr').last()
        tr.append(t.find('tr').eq(0).clone().find('th').wrapInner('<td />').contents().unwrap())
        tr.find('td').html('<input type=text style="width:100%" />')
        var arr = [ 'Save', 'Cancel']
        var s = ''
        for (var i in arr) {
            s += GetButton(arr[i])
        }
        tr.find('td').eq(0).html(s)
    }
    function Save(sql) {
        CallAjax('sql', sql, 'runSql', function (data, status, xhr) {
        },
        function (e) { alert(e.responseText); alert(s) }
        ,false
        )
    }
</script>
    