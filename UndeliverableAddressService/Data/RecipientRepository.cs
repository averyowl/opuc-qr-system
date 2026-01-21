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
        var tables = getRecipientTables(source);
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
                a.flgBadAddress isBadAddress
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
    /// Helper function distinguishing RecipientSource enum. 
    /// </summary>
    /// <param name="source">RecipientSource enum indicating TDAP or OTAP recipient</param>
    /// <returns>RecipientTables object containing the AddressTable and RecipientTable for their respective programs (OTAP/TDAP)</returns>
    private static RecipientTables getRecipientTables(RecipientSource source) =>
        source switch
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
}
