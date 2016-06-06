<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeFile="Canvas.aspx.cs" Inherits="Canvas" %>
<html>
<head>
  <title>
    Desk Canvas 
  </title>
  <style media="screen" type="text/css">
body{
    font-family:Arial;
}
.header{
    font-size:11pt;
    font-weight:bold;
}
  </style>
</head>
<body>
    <div id="header" class="header"></div>
    <div id="div1"></div>
  <pre id="context"></pre>
  <!-- Include the Desk Canvas library, hosted on a CDN for your convenience -->
  <script src="https://ajax.deskapi.com/libs/desk/canvas/1.0.0/desk-canvas-all.js"></script>
    <script type="text/javascript" src="js/jquery-1.10.2.js"></script>

  <script>
      Desk.canvas(function () {
        Desk.canvas.client.refreshSignedRequest(function (data) {
        var signedRequest = data.payload.response;
        var decodedRequest = Desk.canvas.decode(signedRequest.split('.')[1]);
        var contextObj = JSON.parse(decodedRequest);
        var contextElem = Desk.canvas.byId('context');
        var cust = contextObj.context.environment.customer
        //$('#context').html(JSON.stringify(cust.phoneNumbers, undefined, 2))
        // We can also store this so we can reference it later without having to use a global variable.
            // Desk.canvas.client.signedrequest(contextObj);
        var phones = cust.phoneNumbers
        var s=''
        //for (var i = 0; i < phones.length; i++) {
        //    if (s != '')
        //        s+=', '
        //    s+="'"+phones[i].value+"'"
        //}
       // $('#context').html(s)
        var empId=getParameterByName('empId')
        GetData(s,  empId)
      });
    });
      function GetData( phones, empId) {
        $.ajax({
            type: "POST",
            url: "canvas.aspx/GetData",
            data: '{empId: "' + empId + '", phones: "' + phones + '"  }', 
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                //alert(data.d)
                var s = '<table width=100% border=1 cellspacing="0" cellpadding="0" style="border-collapse:collapse;">'
                $($.parseXML(data.d)).find('Table').children().each(function (index, element) {
                    var field = $(element)
                    var src = $(this)
                    s +='<tr><td>'+ this.tagName  +'<td>'+ src.text() 
                });
                s+='</table>'
                s = MyReplace(s, '_x0020_', ' ')
                if ($($.parseXML(data.d)).find('Table').children().length==0)
                    s='No record.'
                $('#div1').html(s)
            },
            failure: function (response) {
                alert(response.d);
            }
        });
    }
    function getParameterByName(name) {
        name = name.toLowerCase()
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search.toLowerCase());
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
function MyReplace(str, s1, s2) {
    if (str==null)  return ''
    var re = new RegExp(s1, 'g');
    return str.replace(re, s2)//.replace(s2, s1);
}
  </script>

</body>
</html>

