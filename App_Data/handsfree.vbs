
dim URL, oArgs, objXML
Set oArgs = WScript.Arguments
URL = "http://localhost/wi4/sso/fileWatch.aspx" 'oArgs(0)
'URL = oArgs(0)
on error resume next

Set objXML = CreateObject("Microsoft.XMLDOM")
objXML.async = "false"
objXML.load(URL)
Set objXML = Nothing  