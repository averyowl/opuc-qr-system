Assumes file of invalidated recipients is "invalidated-recipients.csv".
If ParentDirectory is empty, assumes temporary directory.

# Required packages

```dotnet add package Microsoft.Extensions.Configuration```
```dotnet add package MimeKit```
```dotnet add package MailKit```

# Testing
This application was tested using the local SMTP server, [MailHog](https://github.com/mailhog/MailHog). 