using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Core.Interfaces;

public interface ICustomerRepository
{
    Customer? GetById(int id);
    Customer? GetByEmail(string email);
    Customer? GetByIdentityUserId(string identityUserId);
    void Add(Customer entity);
    void Update(Customer entity);
    void Delete(Customer entity);
}
