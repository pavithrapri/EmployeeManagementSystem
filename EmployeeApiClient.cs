using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class EmployeeApiClient
{
    private readonly HttpClient _httpClient;

    public EmployeeApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task CreateEmployeeAsync()
    {
        // Define the data for the new employee
        var newEmployeeData = @"
        {
          ""id"": ""unique_id_here"", // Replace ""unique_id_here"" with the actual unique identifier for the employee
          ""dType"": ""string"",
          ""civilId"": ""string"",
          ""fileNumber"": ""string"",
          ""fullName"": ""string"",
          ""jobName"": ""string"",
          ""generalDepartment"": ""string"",
          ""department"": ""string"",
          ""branch"": ""string"",
          ""address"": ""string"",
          ""tell"": ""string"",
          ""country"": ""string"",
          ""city"": ""string"",
          ""town"": ""string"",
          ""photo"": ""string"",
          ""vacations"": [
            {
              ""vacationType"": ""string"",
              ""startDate"": ""2024-06-04T02:09:17.632Z"",
              ""endDate"": ""2024-06-04T02:09:17.632Z""
            }
          ],
          ""overtimes"": [
            {
              ""daysWorked"": 0
            }
          ],
          ""doctors"": [
            {
              ""doctorVisitReason"": ""string"",
              ""visitDate"": ""2024-06-04T02:09:17.632Z""
            }
          ],
          ""sanctions"": [
            {
              ""sanctionType"": ""string"",
              ""sanctionDate"": ""2024-06-04T02:09:17.632Z""
            }
          ]
        }";

        // Create the HttpContent for the POST request
        var content = new StringContent(newEmployeeData, Encoding.UTF8, "application/json");

        // Make the POST request to create the new employee
        var response = await _httpClient.PostAsync("https://localhost:7035/api/Employee", content);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("New employee created successfully: " + responseBody);
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error creating new employee: " + errorResponse);
        }
    }
}
