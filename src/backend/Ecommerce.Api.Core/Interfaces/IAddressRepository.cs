using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Core.Interfaces;

public interface IAddressRepository
{
    Address? GetById(int id);
    IEnumerable<Address> GetByCustomerId(int customerId);
    void Add(Address entity);
    void Update(Address entity);
    void Delete(Address entity);
}
