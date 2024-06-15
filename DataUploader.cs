using DemoEmployeeDb.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DemoEmployeeDb.Services
{
    public class DataUploader
    {
        private readonly Container _container;

        public DataUploader(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task UploadEmployeesAsync(string filePath)
        {
            // Read JSON data from file
            string jsonData = File.ReadAllText(filePath);

            // Deserialize JSON data into a list of Employee objects
            List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(jsonData);

            // Add each employee to the Cosmos DB container
            foreach (var employee in employees)
            {
                await _container.CreateItemAsync(employee, new PartitionKey(employee.DType));
            }

            Console.WriteLine("Data upload completed.");
        }
    }
}
