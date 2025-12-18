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

public record RecipientIdResult(
    bool Success,
    string[] Ids,
    string? Error
);

public class Program
{
    public static int Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        RecipientIdResult results = GetInvalidatedRecipientIds(
            config["RecipientTemporaryFile:ParentDirectory"],
            config["RecipientTemporaryFile:Name"]
            );

        if (!results.Success)
        {
            Console.WriteLine("Error retrieving recipient IDs: " + results.Error);
            return 1;
        }
        if (results.Ids.Length > 0) //temp until email composition and sending is implemented
            Console.WriteLine("Recipient IDs: " + string.Join(", ", results.Ids));

        return 0;
    }

    public static RecipientIdResult GetInvalidatedRecipientIds(string? parentDirectory, string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return new RecipientIdResult(
                Success: false,
                Ids: Array.Empty<string>(),
                Error: "File name is null or empty."
            );

        try
        {
            if (string.IsNullOrEmpty(parentDirectory))
                parentDirectory = Path.GetTempPath();

            var path = Path.Combine(parentDirectory, fileName);
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

}
