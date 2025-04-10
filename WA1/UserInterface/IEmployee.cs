using WA1.Entities;

namespace WA1.UserInterface
{
    public interface IEmployee
    {
        Task AddEmployeeAsync(Employee employee);

        Task<Employee> GetEmployeeByIdAsync(int employeeId);

        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        Task UpdateEmployeeAsync(Employee employee);

        Task DeleteEmployeeAsync(int employeeId);
    }
}
