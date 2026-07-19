# Diagnostic script: reproduces a Card Builder SAVE POST exactly like the browser.
# Usage (run from PowerShell):  .\trace-save.ps1
# It will prompt for the admin password (secure input), then:
#   1. POST /admin-secure-panel/Login  (establish session)
#   2. GET  /admin-secure-panel/Cards/Builder (extract __RequestVerificationToken + anti-forgery cookie)
#   3. POST /admin-secure-panel/Cards/Builder with a full KPI card payload (action=save)
# All server-side logs go to project-control/logs/app-trace.log (the running app writes there).
# This script only reports HTTP status + redirects so we can correlate with the server log.

$base = "http://localhost:5000"
$loginUrl = "$base/admin-secure-panel/Login"
$builderUrl = "$base/admin-secure-panel/Cards/Builder"

# Cookie jar for session + anti-forgery
$jar = New-Item -ItemType File -Force -Path "$env:TEMP\wb-cookies.txt" -Value ""

# 1) Login
$secure = Read-Host -Prompt "Enter admin password" -AsSecureString
$ptr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
$pw = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($ptr)
[System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr)

Write-Host "==> Step 1: Login" -ForegroundColor Cyan
$loginBody = "Password=$pw"
$resp = curl.exe -s -i -c $jar -X POST $loginUrl `
    -H "Content-Type: application/x-www-form-urlencoded" `
    --data $loginBody
$loginStatus = ($resp | Select-String "^HTTP/").Line
Write-Host "Login response: $loginStatus"

# 2) GET Builder page to extract anti-forgery token
Write-Host "==> Step 2: GET Builder (extract token)" -ForegroundColor Cyan
$builderHtml = curl.exe -s -b $jar -c $jar "$builderUrl"
$tokenMatch = [regex]::Match($builderHtml, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"')
if (-not $tokenMatch.Success) {
    # try input tag variant
    $tokenMatch = [regex]::Match($builderHtml, '__RequestVerificationToken"[^>]*value="([^"]+)"')
}
if (-not $tokenMatch.Success) {
    $tokenMatch = [regex]::Match($builderHtml, 'value="([^"]+)"[^>]*name="__RequestVerificationToken"')
}
$token = if ($tokenMatch.Success) { $tokenMatch.Groups[1].Value } else { "" }
Write-Host "Token extracted: $(if($token){'YES ('+$token.Length+' chars)'}else{'NO'})"

# 3) Build the SAVE POST payload (mirrors card-builder.js submitForm + syncHiddenInputs)
# Simulate a KPI card from a SqlTable source, same shape the user reported failing.
$saveFields = @(
    "__RequestVerificationToken=$token",
    "action=save",
    "cardType=KPI",
    "sourceType=SqlTable",
    "sourceId=stg_ST_UNITS",
    "customSql=",
    "title=TEST_TRACE_CARD",
    "description=",
    "gridWidth=4",
    "gridHeight=2",
    "gridX=",
    "gridY=",
    "colorPalette=info",
    "refreshInterval=300",
    "sqlQuery=SELECT SUM([UNIT_CODE]) AS [UNIT_CODE] FROM [stg_ST_UNITS]",
    "valueColumn=UNIT_CODE",
    "dateColumn=",
    "categoryColumn=",
    "showChange=false",
    "changeSource=previousPeriod",
    "showSparkline=false",
    "sparklineMonths=6",
    "showGrandTotal=false",
    "grandTotalSource=sameTable",
    "dateFilterMode=dashboard",
    "fixedStartDate=",
    "fixedEndDate=",
    "relativeDays=30",
    "aggregationType=Count",
    "kpiMode=simple",
    "originalSourceType=SqlTable",
    "originalSourceId=stg_ST_UNITS"
)
$body = ($saveFields -join "&") -replace " ", "%20"

Write-Host "==> Step 3: POST SAVE (action=save)" -ForegroundColor Cyan
$saveResp = curl.exe -s -i -b $jar -c $jar -X POST $builderUrl `
    -H "Content-Type: application/x-www-form-urlencoded" `
    --data $body
$saveStatus = ($saveResp | Select-String "^HTTP/").Line
Write-Host "SAVE response: $saveStatus"
# Show location header if redirect
$loc = ($saveResp | Select-String "^location:").Line
if ($loc) { Write-Host "Redirect: $loc" }

Write-Host ""
Write-Host "DONE. Now check project-control/logs/app-trace.log for 'Card Builder POST' entries." -ForegroundColor Green
