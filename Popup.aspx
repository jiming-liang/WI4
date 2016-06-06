<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Popup.aspx.cs" Inherits="Popup" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
body {font-family:Arial;}
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
</HEAD>
<body style1='display:none'>
<form id="form1" runat="server">
<h1 runat="server"  id="MyTitle">Resources</h1>
    <div id="div0" >
    </div>
<table id=t  width="100%" >
<colgroup  ><col width="50%"   style="text-align: right"  /><col /></colgroup>
<tr><td>
</table>
    <div id="Holder" runat="server" ></div>
    <input type="button" onclick="on_Submit(this)"  value="Save" /> 
    <td><input type="button" onclick="on_Cancel(this)"  value="Cancel" /> 
</form>
</body>
</html>
<script>
    var theForm = document.forms['form1']
    var t=$('#t')
    $(document).ready(function () {
        var xml = window.dialogArguments
        xml = '<r>' + xml + '</r>'
        if ($('input[type=checkbox]').length == 0) {
            alert("There is no Work Instruction available. ")
            window.close()
            return 
        }
        $('input[type=checkbox]').each(function (index) {
            var src=$(this)
             $($.parseXML(xml)).find('r').each(function (index, element) {
                 $(this).children().each(function () {
                     if (src.attr('id') == $(this).attr('id')) {
                         src.prop('checked', true)
                         var tb = src.parent().next().next().find('input:text').eq(0)
                         var other_tb = src.parent().next().find('input:text').eq(0)
                         if ($.trim(src.parent().next().text()) == 'Other') {
                             $(this).attr('other')
                             other_tb.val($(this).attr('other'))
                         }
                         if (tb.prop('tagName')=='INPUT')
                             tb.val($(this).attr('number'))

                     }
                 })

             });
        })
        $('input[type=text]').keyup(function (e) {
            var src = $(this)
            if (src.attr('id') != 'resources')
                return 
            //if (src.prev().length != 0 && src.prev().prop('tagName') == 'INPUT')
            //    return 
            if (/\D/g.test(this.value)) {
                // Filter non-digits from input value.
                this.value = this.value.replace(/\D/g, '');
            }
        });
    })
    
    function on_Cancel() {
        window.close()
        return
    }
    function on_Submit() {
        var arr = [];
        var validate=true
        $('input[type=checkbox]:checked').each(function (index) {
            var id = $(this).attr('id')
            var other_tb = $(this).parent().next().find('input:text').eq(0)
            var other 
            if (other_tb != null && other_tb.prop('tagName') == 'INPUT') {
                other = other_tb.val()
                other = $.trim(other)
                var ck = other_tb.prev()
                if ( other == '') {
                    alert('Please enter text for Other.')
                    validate = false
                    return
                }
            }
            var name = $(this).attr('name')
            if ($('#MyTitle').text() == 'Minimum Personnel Required') {
                var tb = $(this).parent().next().next().find('input:text').eq(0)
                var number = tb.val()

                if ((tb.prop('tagName') == 'INPUT' && tb.parent().text() != 'Other') && (isNaN(number) || number < 1 || number > 10)) {
                    alert('Please enter a positive number(max 10).')
                    validate = false
                    return
                }
            }
            arr.push({ id: id, name: name, number: number, other: other });
        })
        if (!validate) {
            return
        }
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
