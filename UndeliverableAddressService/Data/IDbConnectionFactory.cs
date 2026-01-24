using System.Data;
namespace UndeliverableAddressService.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
