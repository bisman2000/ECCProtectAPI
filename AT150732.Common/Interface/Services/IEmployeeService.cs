
using AT150732.Common.Dtos.Employee;

namespace AT150732.Common.Interface.Services;

public interface IEmployeeService
{
    Task<int> CreateEmployeeAsync(EmployeeCreate employeeCreate);
    Task UpdateEmployeeAsync(EmployeeUpdate employeeUpdate);
    Task<List<EmployeeDetails>> GetEmployeesAsnyc();
    Task<EmployeeDetails> GetEmployeeAsync(int id);
    Task DeleteEmployeeAsync(EmployeeDelete employeeDelete);
}
