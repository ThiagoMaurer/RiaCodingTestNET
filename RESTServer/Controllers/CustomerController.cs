using Microsoft.AspNetCore.Mvc;
using RESTServer.Models;
using System.Text.Json;

namespace RESTServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private static List<Customer> customers = LoadCustomers();

        [HttpPost]
        public IActionResult AddCustomers(List<Customer> newCustomers)
        {
            if (newCustomers == null || newCustomers.Count == 0)
            {
                return BadRequest("No customers provided.");
            }

            foreach (var customer in newCustomers)
            {
                // check if any field is missing
                if (string.IsNullOrWhiteSpace(customer.FirstName) ||
                    string.IsNullOrWhiteSpace(customer.LastName) ||
                    customer.Age <= 18 ||
                    customers.Any(c => c.Id == customer.Id))
                {
                    return BadRequest($"Invalid customer. ({customer.Id})");
                }
            }

            // add each customer to the sorted list
            foreach (var customer in newCustomers)
            {
                AddCustomerSorted(customer);
            }

            SaveCustomers(); // persist the changes

            return Ok("Customers added successfully.");
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(customers);
        }

        [HttpDelete]
        [Route("clear-database")]
        public IActionResult ClearCustomers()
        {
            try { 
                string filePath = "customers.json";
                System.IO.File.WriteAllText(filePath, "[]");

                return Ok("database cleared");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static List<Customer> LoadCustomers()
        {
            try
            {
                string filePath = "customers.json";
                if (System.IO.File.Exists(filePath))
                {
                    string json = System.IO.File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<List<Customer>>(json);
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                Console.WriteLine(ex.Message);
            }
            return new List<Customer>();
        }

        private static void SaveCustomers()
        {
            try
            {
                string filePath = "customers.json";
                string json = JsonSerializer.Serialize(customers);
                System.IO.File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Log or handle exception
                Console.WriteLine(ex.Message);
            }
        }

        private void AddCustomerSorted(Customer newCustomer)
        {
            int index = 0;
            while (index < customers.Count &&
                IsLessThanIgnoreCase(customers[index].LastName, newCustomer.LastName))
            {
                index++;
            }

            while (index < customers.Count &&
                IsLessThanIgnoreCase(customers[index].FirstName, newCustomer.FirstName) &&
                customers[index].LastName.ToLower() != newCustomer.LastName.ToLower())
            {
                index++;
            }

            customers.Insert(index, newCustomer);
        }

        private bool IsLessThanIgnoreCase(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;

            int minLen = len1 < len2 ? len1 : len2;

            str1 = str1.ToLower();
            str2 = str2.ToLower();

            // loop through the characters of both strings
            for (int i = 0; i < minLen; i++)
            {
                char char1 = str1[i];
                char char2 = str2[i];

                // if the characters are not equal, return the result
                if (char1 != char2)
                {
                    return char1 < char2;
                }
            }

            // if the loop finishes and the strings are equal up to minLen,
            // then the shorter string should be considered less than the longer string
            return len1 < len2;
        }
    }
}
