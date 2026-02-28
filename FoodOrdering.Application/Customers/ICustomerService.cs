using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Customers;

public interface ICustomerService
{
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request);
    Task<IReadOnlyList<CustomerDto>> GetCustomersAsync();
    Task<bool> UpdateCustomerInfo(int customerId,CustomerDto customerDto);
}
