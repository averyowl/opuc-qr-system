namespace UndeliverableAddressService.Models;
public class Recipient
{
    public int RecipientId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string MiddleName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string StateCode { get; init; } = string.Empty;
    public string ZIPCode { get; init; } = string.Empty;
    public int isBadAddress { get; init; }
}
