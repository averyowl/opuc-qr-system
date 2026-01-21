# ScheduleRecurringTask.ps1
This script schedules the `ComposeSendEmail.ps1` in Task Scheduler to run daily. This script is for QOL preliminary step before emails can be sent out.
* Assumes `ComposeSendEmail.ps1` is in the same working directory as `ScheduleRecurringTask.ps1`.
* By default the task will run daily at 12:00PM.

# ComposeSendEmail.ps1
This script will ingests a temporary file full of recipient ids that were marked as undelivered, composes, and sends the email. Deletes temp file after.
* Will not send an email if `invalidated-recipients` is empty or missing.
* `invalidated-recipients` is a text file full of newline delimited list of recipient ids. Ignores file extension. Expects file to be located in the system's temporary directory.
* Assumes the enviroment variables `SMTP_USERNAME` and `SMTP_PASSWORD` contains necessary credentials. If none are found, will attempt to send mail without.
* Some settings such as SMTP server and port need to be configured.
* Deletes `invalidated-recipients` on success.