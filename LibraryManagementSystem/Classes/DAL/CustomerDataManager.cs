using Dapper;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using LibraryManagementSystem.Interfaces.DAL;
using LibraryManagementSystem.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Classes.DAL
{
    public class CustomerDataManager : ICustomerDataManager
    {
        // private readonly string connectionString = ConfigurationManager.ConnectionStrings["lms-localDB"].ConnectionString;

        private readonly string connectionString;

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "Nfjq4tAqxBmSO8zzrafd10JJVtPUSiIaXIh6mr2J",
            BasePath = "https://library-a972e-default-rtdb.firebaseio.com/"

        };

        IFirebaseClient client;

        public bool CreateConnection()
        {
            client = new FireSharp.FirebaseClient(config);
            if (client != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> CreateCustomer(Customer customer)
        {
            string customerId = null;
            try
            {         
                client = new FireSharp.FirebaseClient(config);
                var data = customer;
                PushResponse response = client.Push("customers/", data);
                data.CustomerId = response.Result.name;
                customerId = data.CustomerId;
                SetResponse setResponse = client.Set("customers/" + data.CustomerId, data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return customerId;
        }

        public async Task<Customer> GetCustomerDetails(string customerId)
        {
            Customer data = null;

            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("customers/" + customerId);
                data = JsonConvert.DeserializeObject<Customer>(response.Body);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return data;
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            bool success = true;

            try
            {
                client = new FireSharp.FirebaseClient(config);
                SetResponse response = client.Set("customers/" + customer.CustomerId, customer);
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(e.Message);
            }

            return success;
        }

        public async Task<bool> DeleteCustomer(string customerId)
        {
            bool success = true;

            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Delete("customers/" + customerId);
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(e.Message);
            }

            return success;
        }
    }
}