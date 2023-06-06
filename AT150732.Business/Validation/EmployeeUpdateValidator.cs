namespace AT150732.Business.Validation
{
    public class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdate>
    {
        public EmployeeUpdateValidator()
        {
            RuleFor(employeeCreate => employeeCreate.FullName).NotEmpty();
            RuleFor(employeeCreate => employeeCreate.JobName).NotEmpty();
        }
    }
}