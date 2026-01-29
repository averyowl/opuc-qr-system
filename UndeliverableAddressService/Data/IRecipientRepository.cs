using UndeliverableAddressService.Models;
namespace UndeliverableAddressService.Data;

public interface IRecipientRepository
{
    Task<Recipient?> GetTdapRecipientById(int recipientId);
    Task<Recipient?> GetOtapRecipientById(int recipientId);
    Task<Recipient?> GetRecipientByIDAndSource(int recipientId, RecipientSource source);
    Task<bool> InsertRecipientEvent(int recipientId, RecipientSource source, string eventTypeCode, string note, string updatedBy);
}
