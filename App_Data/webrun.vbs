
dim URL, oArgs, objXML
Set oArgs = WScript.Arguments
URL = "http://localhost/wi3/copyWI.aspx" 'oArgs(0)
URL="https://wims-dev.enscoplc.com/wi/copyWI.aspx"
'URL = oArgs(0)
on error resume next

Set objXML = CreateObject("Microsoft.XMLDOM")
objXML.async = "false"
objXML.load(URL)
Set objXML = Nothing  