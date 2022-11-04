using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Interfaces.DAL
{
    public interface ICustomerDataManager
    {
        Task<string> CreateCustomer(Customer customer);

        Task<Customer> GetCustomerDetails(string customerId);

        Task<bool> UpdateCustomer(Customer customer);

        Task<bool> DeleteCustomer(string customerId);
    }
}
