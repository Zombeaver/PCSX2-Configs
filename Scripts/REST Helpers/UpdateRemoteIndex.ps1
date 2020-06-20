$Uri = "https://api.github.com/repos/Zombeaver/PCSX2-Configs/dispatches"
$Headers = @{"Authorization" = "token " + $env:GITHUB_TOKEN }
$Content = @{ event_type = "update-index" }
Invoke-RestMethod -Method POST -Uri $Uri -Headers $Headers -Body ($Content | ConvertTo-Json)