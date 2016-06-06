<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Mapping.aspx.cs" Inherits="Mapping" %>
<%@ Reference Page="WIBase.aspx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
<style>
body {
 /*font-family:Arial;font-size:16;*/ 
 padding:5px;
}
span {
line-height:130%;
}
</style>
<script type="text/javascript" src="js/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="WI.js"></script>
</HEAD>

<body style1='display:none'>
<form id="form1" runat="server">

<div id="headeroffset">
    <br />
</div>

<table width="80%" id="t0" border="1"  style="border-collapse:collapse;">
<colgroup  ><col width="50%"   style="text-align:right"  /><col width="50%" style="vertical-align:top"  /></colgroup>
<tr><td ><b>Select Applicable Facility Type/Rig Design/Rig</b> <td><b>Selected Facility Type/Rig Design/Rig</b>
<tr><td  style="vertical-align:top" ><div style1="border:thin solid" id="RigMapping" runat="server"></div><td style="vertical-align:top" ><div style1="border:thin solid" id="MappingSelected" runat="server" />
</table>
<div id="divJob" >
  <br /><b>  Select Job Category:</b>
   <asp:DropDownList ID="WICategory"   runat="server"  > </asp:DropDownList>
<b> Job Title:</b>   <asp:DropDownList Width="150px" ID="JobDescription"   runat="server"   > </asp:DropDownList>
</div>
<br /><br />
    <input type="button" onclick="on_Submit(this)" value="Create" />  <input type="button" onclick=on_Cancel(this) value="Cancel" /> 


<input id=h1 type=hidden  runat=server/>
    <input id=Action type=hidden  runat=server/>
<div id=holder></div>

</form>
</body>
</html>
<script>
    var theForm = document.forms['form1']
    var div0 = $('#RigMapping'), div1 = $('#MappingSelected')
    var JobDescriptionId
    var IsNew = true
    var WiType = getParameterByName('WiType').toUpperCase()
    var IsDuplicate = false
    var Timer
    var count = 0
    $(document).ready(function () {
        if (WiType == '')
            WiType='WIT'
        if ($('#Action').val() == 'Duplicate') {
            IsDuplicate = true
        }
        JobDescriptionId = window.dialogArguments
        if (JobDescriptionId != null || IsDuplicate) {
            if (!IsDuplicate)
                $('#divJob').hide()
            IsNew = false
            $('#breadcrumb').hide()
            $('#headeroffset').hide()
        }
        $('img', div0).each(function () {
            var src = $(this)
            src.next().next().hide()
        })
        if (!IsNew) {
            $('span', div0).each(function () {
                var span = $(this)
                $('div', div1).each(function () {
                    if ($.trim($(this).text()) == $.trim(span.text())) {
                        //span.click()
                        while (span.parent() != null && (span.parent().prop('tagName') == 'SPAN' || span.parent().prop('tagName') == 'DIV')) {
                            span.parent().show()
                            var img = span.parent().prevUntil('img').prev()
                            img.attr('src', 'images/minus.gif')
                            span = span.parent()
                        }
                    }
                })
            })
        }
        $('img', div0).click(function () {
            var src = $(this)
            if (src.attr('src') == 'images/plus.gif') {
                src.attr('src', 'images/minus.gif')
                src.next().next().show()
            } else {
                src.attr('src', 'images/plus.gif')
                src.next().next().hide()
            }
            event.stopPropagation();
        })
        $('span', div0).mouseover(function () {
            $(this).css('cursor', 'pointer');
        }).mouseout(function () {
            $(this).css('cursor', 'default');
        })
        $('span', div0).click(function (event) {
            event.stopPropagation();
            var src = $(this)
            if (src.children().length != 0)
                return
            var name = src.text()
            if (WiType=='WIS' && name.indexOf('ENSCO') == -1) {
                return 
            }
            var p = src.parent().prev().text()
            if (!FindParent(src)) {
                var s = src[0].outerHTML
                s = MyReplace(s, 'span', 'div')
                $(s).appendTo(div1)
                RemoveChildren(src)
            }
        })
        div1.click(function (event) {
            var src = $(event.target)
            src.remove()
        })
        $("#WICategory").prepend("<option value=''></option>").val('');
        $('#WICategory').on('change', function () {
            var src = $(this)
            if (src.val() == '') {
                $('#JobDescription').empty().width(150)
                return
            }
            var name = src.attr('id')
            var id = src.val()
            GetLookup(name, id)
        })
        if (!IsNew) {
            $('select').on('change', function () {
                var name = $(this).attr('id')
                var id = $(this).val()
                GetLookup(name, id)
            })
            FormatAllRigs()
        }
        InitJobTitle()
    })
    function InitJobTitle() {
        if (!IsDuplicate) return
        var sql = ' select cat.id from JobDescription j join  Wicategory cat on j.WicategoryId=cat.id where j.id= ' + JobDescriptionId
        CallAjax('sql', sql, 'RunSql'
        , function (xml) {
            var catId = $($.parseXML(xml.d)).find('Table').find('id').text()
            $('#WICategory').val(catId).change()
        },
        function (e) { alert(e.responseText) }, false)
        Timeout()

    }
    function Timeout() {
        $('#JobDescription').val(JobDescriptionId)
        //var src = $('#JobDescription option[value="' + JobDescriptionId + '"]')
        var src = $('#JobDescription option:selected')
        if (src.val() == JobDescriptionId) { //0 && !src.prop('selected')){
            src.prop('selected', true)
            clearTimeout(Timer)
            return 
        }
        else {
            Timer = setTimeout('Timeout()', 100);
        }
    }
    function FormatAllRigs() {
        if ($.trim(div1.text()) == 'ALL_RIGS') {
            div1.find('div').remove()
            div0.children('span[RigRootId=1][xml]').each(function () {
                $(this).click()
            })
        }
    }
    function MyReplace(str, s1, s2) {
        var re = new RegExp(s1, 'g');
        return str.replace(re, s2)//.replace(s2, s1);
    }
    function FindParent(src) {
        var found = false
        while (src.length != 0 && src.prop('tagName') == 'SPAN') {
            var name = $.trim(src.text())
            $('div', div1).each(function () {
                var src = $(this)
                var s = $.trim(src.text())
                if (name == s) {
                    found = true
                    return false
                }
            })
            if (found)
                return true
            src = src.parent().prev()
        }
        return found
    }
    function RemoveChildren(src) {
        if (src.length == 0 || src.prop('tagName') != 'SPAN' || src.next().length == 0 || src.next().prop('tagName') != 'SPAN')
            return
        src.next().children().each(function () {
            var src = $(this)
            RemoveChild(src.html())
            RemoveChildren(src)
        })
    }
    function RemoveChild(name) {
        name = MyReplace(name, '&amp;', '&')
        $('div', div1).each(function () {
            var src = $(this)
            //alert(src.text()+' '+name)
            if ($.trim(src.text()) == $.trim(name))
                src.remove()
        })
    }
    function RefreshDropDown(data) {
        var srcName
        $($.parseXML(data)).find('Table1').each(function (index, element) {
            var field = $(element)
            srcName = field.find('Name').text()
        });
        if (srcName == 'RigDesign')
            $('#Rig').empty()
        var src = $('#' + srcName)
        src.empty()
        src.append('<option value="" ></option>')
        $($.parseXML(data)).find('Table').each(function (index, element) {
            var field = $(element)
            var name = field.find('Name').text()
            var id = field.find('id').text()
            var number = field.find('number').text()
            src.append('<option value="' + id + '" number="' + number + '"  >' + name + '</option>')
            // there is no number for mapping2
            //            src.append('<option value="' + id + '" >' + name + '</option>') 
        });
        src.width('auto')
    }

    function GetLookup(name, id) {
        if (name=='JobDescription')return
        var parameters = "{'name':'" + name + "', 'id':'" + id + "' }"
        $.ajax({
            type: 'POST',
            url: GetSiteUrl() + '/webservice.asmx/GetWILookup',
            data: parameters,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (xml) {
                RefreshDropDown(xml.d)
            },
            error: function (e) {
                //  alert(e.responseText)
                $('#holder').html('error');
            }
        });
    }

    function setCookie(cname, cvalue) {
        document.cookie = cname + "=" + cvalue //+ "; " + expires;
    }
    function Validate(xml) {
        var wiid = 0
        JobDescriptionId = $('#JobDescription').val()
        if (!IsNew) { //facility, duplicate 
            //if (JobDescriptionId == null || JobDescriptionId == '')
            if (!IsNew && !IsDuplicate)//facility
                JobDescriptionId = getCookie('JobDescriptionId')
            wiid = getCookie('wiid')
            if (IsDuplicate || wiid == null || wiid == '')
                wiid = 0
        }
        if (xml == '') {
            alert('Please select a Rig Mapping.')
            return false
        }
        if (JobDescriptionId == null || JobDescriptionId == '') {
            alert('Please select a Job Title.')
            return false
        }
        //xml = '<MappingXml >' + xml + '</MappingXml >'
        var parameters = "{'xml':'" + xml + "', 'JobDescriptionId':'" + JobDescriptionId + "', 'wiid':'" + wiid + "', 'userId':'" + UserId + "'}"
        var canCreate = false
        //  alert(xml.d)
        if (!HasPermission(parameters)) {
            alert('Permission Denied.')
            return false
        }
        $.ajax({
            type: 'POST',
            url: GetSiteUrl() + '/webservice.asmx/ValidateWI',
            data: parameters,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (xml) {                
                var arr = []
                $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                    var field = $(element)
                    var name = field.find('Name').text()
                    arr.push(name)
                });
                if (arr.length != 0) {
                    var s = arr.join(',') + ' already have the WI with the same job title.'
                    alert(s)
                    return false
                }
                else
                    canCreate = true
            },
            error: function (e) {
               // $('#holder').html(e.responseText);
            }
        });
        return canCreate
    }
    function HasPermission(parameters) {
        var r=true
        $.ajax({
            type: 'POST',
            url: GetSiteUrl() + '/webservice.asmx/ValidatePermission',
            data: parameters,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (xml) {
                var arr = []
                if ($($.parseXML(xml.d)).find('Column1').text()==0) {
                    r=false
                };
            },
            error: function (e) {}
        });
        return r
    }
    function IsAllRigs(arr) {
        var arr0 = []
        if (arr.length != div0.children('span[RigRootId=1][xml]').length)
            return false
        div0.children('span[RigRootId=1][code]').each(function () {
            var src = $(this)
            arr0.push(src.attr('code'))
        })
        arr0.sort()
        arr.sort()
        if (arr.join(',') == arr0.join(','))
            return true
        else
            return false
    }
    function on_Submit() {
        var xml = '', s = ''
        var arr = []
        if ( WiType=='WIS' && ($('div', div1).length !=1 || $('div', div1).text().indexOf('ENSCO')==-1)) {
            alert('Please select one rig only.')
            return 
        }
        $('div', div1).each(function () {
            var src = $(this)
            xml += src.attr('xml')
            arr.push(src.attr('Code'))
        })
        if (IsAllRigs(arr)) {
            xml = '<RigRoot id="1" name="ALL_RIGS" code="ALL_RIGS"  />'
            xml = htmlEscape(xml)
            xml = encodeURIComponent(xml);
            arr = []
            arr.push('ALL_RIGS')
        }
        if (!Validate(xml))
            return
        if (IsNew || IsDuplicate) {
            var id = $('#WICategory option:selected').attr('value')
            var sql=' select code from JobDescription j join  Wicategory cat on j.WicategoryId=cat.id where j.id= ' +JobDescriptionId
            var code=$('#WICategory option:selected').text()
            CallAjax('sql', sql, 'RunSql'
            , function (xml) {
                $($.parseXML(xml.d)).find('Table').each(function (index, element) {
                    var field = $(element)
                    if (field.find('code').text().trim()!='')
                        code = field.find('code').text()
                });

            },
            function (e) {
                alert(e.responseText)
            }
            ,false
            )
            setCookie('WICategoryName', code)
            setCookie('JobDescriptionId', $('#JobDescription').val())
            setCookie('JobDescriptionName', $('#JobDescription option:selected').text())
            setCookie('JobDescriptionNumber', $('#JobDescription option:selected').attr('number'))
            //  setCookie('JobDescriptionNumber', $('#JobDescription option:selected').index())
            setCookie('xml', xml)
            if (IsDuplicate) {
                setCookie('Facility', arr.join('/'))
                window.returnValue = xml;
                window.close();
                return 
            }            
            $(theForm).attr('action', 'Create.aspx?WiType='+WiType)
            __doPostBack('Create', xml)
        } else {
            setCookie('Facility', arr.join('/'))
            window.returnValue = xml;
            window.close();
        }
    }
    function on_Cancel(src0) {
        if (window.dialogArguments != null)
            window.close()
        else 
            window.location='home.aspx'        
    }
</script>
