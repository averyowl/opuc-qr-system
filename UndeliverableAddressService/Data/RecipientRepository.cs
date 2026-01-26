using Dapper;
using UndeliverableAddressService.Models;
namespace UndeliverableAddressService.Data;

public class RecipientRepository : IRecipientRepository
{
    private sealed record RecipientTables(
        string AddressTable,
        string RecipientTable
    );

    private readonly IDbConnectionFactory _connectionFactory;

    public RecipientRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Selects matching recipientId participating in the TDAP program
    /// </summary>
    /// <param name="recpientId">Integer ID</param>
    /// <returns>Returns Recipient object or null if no match</returns>
    public async Task<Recipient?> GetTdapRecipientById(int recipientId)
    {
        return await GetRecipientByIDAndSource(recipientId, RecipientSource.TDAP);
    }
    
    /// <summary>
    /// Selects matching recipientId participating in the OTAP program
    /// </summary>
    /// <param name="recpientId">Integer ID</param>
    /// <returns>Returns Recipient object or null if no match</returns>
    public async Task<Recipient?> GetOtapRecipientById(int recipientId)
    {
        return await GetRecipientByIDAndSource(recipientId, RecipientSource.OTAP);
    }

    /// <summary>
    /// Selects matching recipientId participating in the specified program
    /// </summary>
    /// <param name="recpientId">Integer ID</param>
    /// <param name="source">RecipientSource enum indicating TDAP or OTAP recipient</param>
    /// <returns>Returns Recipient object or null if no match</returns>
    public async Task<Recipient?> GetRecipientByIDAndSource(int recipientId, RecipientSource source)
    {
        var tables = source switch
        {
            RecipientSource.OTAP => new RecipientTables(
                AddressTable: "PUC.dbo.tblOTAPRecipientAddress",
                RecipientTable: "PUC.dbo.tblOTAPRecipient"
            ),

            RecipientSource.TDAP => new RecipientTables(
                AddressTable: "PUC.dbo.tblTDAPRecipientAddress",
                RecipientTable: "PUC.dbo.tblTDAPRecipient"
            ),

            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
        string sql = $"""
            select
                t.RecipientId,
                t.txtFirstName FirstName, 
                t.txtMiddleName MiddleName,
                t.txtLastName LastName,
                a.txtLine1 AddressLine1,
                tc.txtDescription City,
                a.StateCode,
                tcz.ZIPCode,
                a.flgBadAddress FlagBadAddress
            from {tables.AddressTable} a
            left join {tables.RecipientTable} t
                on a.RecipientID = t.RecipientId
            left join PUC.dbo.tblCity tc
                on a.CountryCode = tc.CountryCode 
               and a.StateCode = tc.StateCode 
               and a.CountyCode = tc.CountyCode  
               and a.CityCode = tc.CityCode 
            left join PUC.dbo.tblCityZIP tcz
                on a.CountryCode = tcz.CountryCode 
               and a.StateCode = tcz.StateCode 
               and a.CountyCode = tcz.CountyCode 
               and a.CityCode = tcz.CityCode 
               and LEFT(LTRIM(RTRIM(a.txtZIP)), 5) = tcz.ZIPCode
            where t.RecipientId = @RecipientId
        """;

        using var connection = _connectionFactory.CreateConnection();
        var recipient = await connection.QuerySingleOrDefaultAsync<Recipient>(
                sql,
                new { RecipientId = recipientId }
        );
        return recipient;
    }
    
    /// <summary>
    /// Inserts a new event for the recipient into the appropriate event table
    /// </summary>
    /// <param name="recipientId">Recipient ID</param>
    /// <param name="source">RecipientSource enum indicating TDAP or OTAP recipient</param>
    /// <param name="eventTypeCode">4-character event type code</param>
    /// <param name="note">Optional note for the event</param>
    /// <param name="updatedBy">User who created the event</param>
    /// <returns>True if insert succeeded, false otherwise</returns>
    public async Task<bool> InsertRecipientEvent(int recipientId, RecipientSource source, string eventTypeCode, string note, string updatedBy)
    {
        string eventTable = source switch
        {
            RecipientSource.OTAP => "PUC.dbo.tblOTAPRecipientEvent",
            RecipientSource.TDAP => "PUC.dbo.tblTDAPRecipientEvent",
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };

        string getSequenceSql = $"SELECT ISNULL(MAX(numSequence), 0) + 1 FROM {eventTable} WHERE RecipientId = @RecipientId";
        
        string insertSql = $"""
            INSERT INTO {eventTable} 
                (RecipientId, numSequence, EventTypeCode, memNote, datEventDate, txtLastUpdatedBy, datLastUpdatedOn)
            VALUES 
                (@RecipientId, @Sequence, @EventTypeCode, @Note, CURRENT_TIMESTAMP, @UpdatedBy, CURRENT_TIMESTAMP)
        """;

        using var connection = _connectionFactory.CreateConnection();
        try
        {
            var sequence = await connection.QuerySingleAsync<int>(getSequenceSql, new { RecipientId = recipientId });
            
            var rowsAffected = await connection.ExecuteAsync(insertSql, new
            {
                RecipientId = recipientId,
                Sequence = sequence,
                EventTypeCode = eventTypeCode,
                Note = note,
                UpdatedBy = updatedBy,
            });

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting recipient event: {ex.Message}");
            return false;
        }
    }
}
