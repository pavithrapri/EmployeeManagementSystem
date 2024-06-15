using DemoEmployeeDb.Models;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetEmployeesAsync();
    Task<Employee> GetEmployeeByIdAsync(string id, string dType);
    Task AddEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(string id, Employee employee);
    Task DeleteEmployeeAsync(string id, string dType); // This should have two parameters
}