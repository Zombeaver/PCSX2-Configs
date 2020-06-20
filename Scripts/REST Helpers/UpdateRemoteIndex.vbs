dim URL
set wshShell = CreateObject("WScript.Shell")
set xmlHttp = CreateObject("MSXML2.XMLHTTP.6.0")
URL = "https://api.github.com/repos/Zombeaver/PCSX2-Configs/dispatches"

xmlHttp.open "POST", URL, false
xmlHttp.setRequestHeader "Authorization", ("token " & wshShell.ExpandEnvironmentStrings("%GITHUB_TOKEN%"))
On Error Resume Next
xmlHttp.send "{ ""event_type"": ""update-index"" }"