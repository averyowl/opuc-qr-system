$TaskName = "Compose and send invalidated recipient id email"
$Description = "Sends email once per day"
$Execute = "" # Required - Executable
$WorkingDirectory = "" # Required - Working directory to InvalidatedRecipientIdEmail
$Argument = " "
$Trigger = New-ScheduledTaskTrigger -Daily -At 12:00PM
#$Trigger = New-ScheduledTaskTrigger -Weekly -DaysOfWeek Thursday -At 9:00AM
$Action = New-ScheduledTaskAction -Execute $Execute -Argument $Argument -WorkingDirectory $WorkingDirectory
$Settings = New-ScheduledTaskSettingsSet -StartWhenAvailable

Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
Register-ScheduledTask -TaskName $TaskName -Description $Description -Trigger $Trigger -Action $Action -Settings $Settings -RunLevel Highest -Force
