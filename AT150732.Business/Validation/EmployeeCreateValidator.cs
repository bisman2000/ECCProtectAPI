

namespace AT150732.Business.Validation;

public class EmployeeCreateValidator : AbstractValidator<EmployeeCreate>
{
    public EmployeeCreateValidator()
    {
        RuleFor(employeeCreate => employeeCreate.FullName).NotEmpty();
        RuleFor(employeeCreate => employeeCreate.JobName).NotEmpty();
    }
}