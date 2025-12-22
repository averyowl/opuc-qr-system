/*
    Authoring team: 
    Hunter Gambee-Iddings - SE
    Jacob Purvis - Team Lead
    Natalya Langford - SE
    Nathan Waggoner - SE
    Philip Grazhdan - SE
    Sean McCoy - SE
    Wenkang Yu Zhen - SE

    Author: Sean McCoy
*/

using Microsoft.Extensions.Configuration;
using AppConfigurations.EmailSettings;
using AppConfigurations.RecipientFileSettings;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

public record RecipientIdResult(
    bool Success,
    string[] Ids,
    string? Error
);

public record EmailSendResult(
    bool Success,
    string? Error
);

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var recipientFileSettingsSection = config.GetSection("RecipientFileSettings");
        var recipientFileSettings = recipientFileSettingsSection.Get<RecipientFileSettings>() ?? new RecipientFileSettings();

        RecipientIdResult results = GetInvalidatedRecipientIds(recipientFileSettings);
        if (!results.Success)
        {
            Console.WriteLine("Error retrieving recipient IDs: " + results.Error);
            return 1;
        }
        if (results.Ids.Length < 1)
        {
            Console.WriteLine("No invalidated recipient IDs found.");
            return 0;
        }

        var emailSettingsSection = config.GetSection("EmailSettings");
        var emailSettings = emailSettingsSection.Get<EmailSettings>() ?? new EmailSettings();

        var sendResult = await ComposeAndSendEmails(results.Ids, emailSettings);

        if (!sendResult.Success)
        {
            Console.WriteLine("Error sending email: " + sendResult.Error);
            return 1;
        }

        Console.WriteLine($"Email sent successfully to {emailSettings.RecipientEmail}.");

        return 0;
    }

    public static RecipientIdResult GetInvalidatedRecipientIds(RecipientFileSettings recipientFileSettings)
    {
        if (string.IsNullOrEmpty(recipientFileSettings.Name))
            return new RecipientIdResult(
                Success: false,
                Ids: Array.Empty<string>(),
                Error: "File name is null or empty."
            );

        try
        {
            if (string.IsNullOrEmpty(recipientFileSettings.ParentDirectory))
                recipientFileSettings.ParentDirectory = Path.GetTempPath();

            var path = Path.Combine(recipientFileSettings.ParentDirectory, recipientFileSettings.Name);
            if (!File.Exists(path))
                return new RecipientIdResult(
                    Success: false,
                    Ids: Array.Empty<string>(),
                    Error: $"File not found: {path}"
                );

            string[] recipientIds = File.ReadAllLines(path);

            return new RecipientIdResult(
                Success: true,
                Ids: recipientIds,
                Error: null
            );
        }
        catch (Exception ex)
        {
            return new RecipientIdResult(
                Success: false,
                Ids: Array.Empty<string>(),
                Error: ex.Message
            );
        }
    }

    public static async Task<EmailSendResult> ComposeAndSendEmails(string[] recipientIds, EmailSettings emailSettings)
    {
        try
        {
            string htmlBody = (emailSettings.Body ?? string.Empty) + "<br>" + string.Join("<br>", recipientIds);

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(emailSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(emailSettings.RecipientEmail));
            message.Subject = emailSettings.Subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            var secure = emailSettings.SmtpPort switch
            {
                465 => SecureSocketOptions.SslOnConnect,
                587 => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.Auto
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.SmtpPort, secure);

            if (!string.IsNullOrEmpty(emailSettings.SenderPassword))
            {
                await client.AuthenticateAsync(emailSettings.SenderEmail, emailSettings.SenderPassword);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return new EmailSendResult(true, null);
        }
        catch (Exception ex)
        {
            return new EmailSendResult(false, ex.Message);
        }
    }

}
