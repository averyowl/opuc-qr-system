$ScriptName = "ComposeSendEmail.ps1"
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$ScriptPath = Join-Path -Path $ScriptDirectory -ChildPath $ScriptName

if (-Not (Test-Path -Path $ScriptPath)) {
    Write-Error "$ScriptName not found: $ScriptPath. Exiting script."
    exit 1
}

$Trigger = New-ScheduledTaskTrigger -Daily -At 12:00PM 
# $Trigger = New-ScheduledTaskTrigger -Weekly -DaysOfWeek Thursday -At 9:00AM

$TaskActionArguments = @{
    Execute = "powershell.exe"
    WorkingDirectory = $ScriptDirectory
    Argument = "-NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -File `"$ScriptPath`""
}

$Action = New-ScheduledTaskAction @TaskActionArguments
$Settings = New-ScheduledTaskSettingsSet -StartWhenAvailable

$RegisterTaskArguments = @{
    TaskName    = "Compose and send invalidated recipient id email"
    Description = "Sends email once per day"
    Trigger     = $Trigger
    Action      = $Action
    Settings    = $Settings
    RunLevel    = "Highest"
    Force       = $true
}

Register-ScheduledTask @RegisterTaskArguments
