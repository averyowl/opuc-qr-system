$InvalidatedRecipientIDsPath = $env:TEMP + "\invalidated-recipients*"
if (-Not (Test-Path -Path $InvalidatedRecipientIDsPath)) {
    Write-Host "Invalidated recipient IDs file not found at path: $InvalidatedRecipientIDsPath. Exiting script."
    exit 0
}
$InvalidatedRecipientIDs = Get-Content -Path $InvalidatedRecipientIDsPath -Raw
if ([string]::IsNullOrWhiteSpace($InvalidatedRecipientIDs)) {
    Write-Host "No invalidated recipient IDs found. Exiting script."
    exit 0
}

$SmtpUsername = $env:SMTP_USERNAME
$SmtpPassword = $env:SMTP_PASSWORD

$MailMessageParameters = @{
    From        = "no-reply@puc.oregon.gov" # Replace with actual sender email address
    To          = "puc.rspf@puc.oregon.gov"
    Subject     = "Recipient ID Invalidation Notification $(Get-Date -Format 'MM-dd-yyyy')"
    Body        = "The following recipient IDs have been invalidated:`n$InvalidatedRecipientIDs"
    SmtpServer  = "localhost" # Replace with actual SMTP server
    Port        = 1025 # Replace with actual SMTP port if different
    ErrorAction = "Stop"
}

if (-Not [string]::IsNullOrWhiteSpace($SmtpUsername) -and -Not [string]::IsNullOrWhiteSpace($SmtpPassword)) {
    $SecurePassword = ConvertTo-SecureString $SmtpPassword -AsPlainText -Force
    $Credential = New-Object System.Management.Automation.PSCredential($SmtpUsername, $SecurePassword)
    $MailMessageParameters.Add("Credential", $Credential)
    Write-Host "Using SMTP authentication with username: $SmtpUsername"
} else {
    Write-Host "No SMTP credentials provided. Attempting to send without authentication."
}

try {
    Send-MailMessage @MailMessageParameters
    Write-Host "Email sent successfully to $($MailMessageParameters.To)."
}catch {
    Write-Error "Error connecting to SMTP server: $($_.Exception.Message). Exiting script."
    exit 1
}

Remove-Item -Path $InvalidatedRecipientIDsPath