using DemoEmployeeDb.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DemoEmployeeDb.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly Container _container;

        public EmployeeService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            var query = _container.GetItemQueryIterator<Employee>();
            List<Employee> results = new List<Employee>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<Employee> GetEmployeeByIdAsync(string id, string dType)
        {
            try
            {
                ItemResponse<Employee> response = await _container.ReadItemAsync<Employee>(id, new PartitionKey(dType));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _container.CreateItemAsync(employee, new PartitionKey(employee.DType));
        }

        public async Task<Employee> GetEmployeeByIdAsync(string id)
        {
            try
            {
                ItemResponse<Employee> response = await _container.ReadItemAsync<Employee>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(string id, Employee employee)
        {
            try
            {
                await _container.UpsertItemAsync(employee, new PartitionKey(employee.DType));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task DeleteEmployeeAsync(string id, string dType)
        {
            await _container.DeleteItemAsync<Employee>(id, new PartitionKey(dType));
        }


    }
}
