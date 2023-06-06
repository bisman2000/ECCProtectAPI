

namespace AT150732.Business.Services;

public class EmployeeService : IEmployeeService
{
    private IBusinessRepos<Employee> EmployeeRepos { get; }
    private IBusinessRepos<Contact> ContactRepos { get; }
    private IMapper Mapper { get; }
    private EmployeeCreateValidator EmployeeCreateValidator { get; }
    private EmployeeUpdateValidator EmployeeUpdateValidator { get; }

    public EmployeeService(IBusinessRepos<Employee> employeeRepos, IBusinessRepos<Contact> contactRepos,
        IMapper mapper, EmployeeCreateValidator employeeCreateValidator,
        EmployeeUpdateValidator employeeUpdateValidator)
    {
        EmployeeRepos = employeeRepos;
        ContactRepos = contactRepos;
        Mapper = mapper;
        EmployeeCreateValidator = employeeCreateValidator;
        EmployeeUpdateValidator = employeeUpdateValidator;
    }

    public async Task<int> CreateEmployeeAsync(EmployeeCreate employeeCreate)
    {
        await EmployeeCreateValidator.ValidateAndThrowAsync(employeeCreate);
        var contact = await ContactRepos.GetByIdAsync(employeeCreate.ContactId);
        if (contact == null)
            throw new ContactNotFoundException(employeeCreate.ContactId);
        var entity = Mapper.Map<Employee>(employeeCreate);
        entity.Contact = contact;
        await EmployeeRepos.InsertAsync(entity);
        await EmployeeRepos.SaveChangesAsync();
        return entity.Id;
    }

    public async Task DeleteEmployeeAsync(EmployeeDelete employeeDelete)
    {
        var entity = await EmployeeRepos.GetByIdAsync(employeeDelete.Id);
        if (entity == null)
            throw new EmployeeNotFoundException(employeeDelete.Id);
        EmployeeRepos.Delete(entity);
        await EmployeeRepos.SaveChangesAsync();
    }

    public async Task<EmployeeDetails> GetEmployeeAsync(int id)
    {
        var entity = await EmployeeRepos.GetByIdAsync(id, e => e.Contact);
        if (entity == null)
            throw new EmployeeNotFoundException(id);
        return Mapper.Map<EmployeeDetails>(entity);

    }

    public async Task<List<EmployeeDetails>> GetEmployeesAsnyc()
    {
        var entities = await EmployeeRepos.GetAsync(e => e.Contact);
        return Mapper.Map<List<EmployeeDetails>>(entities);
    }

    public async Task UpdateEmployeeAsync(EmployeeUpdate employeeUpdate)
    {
        await EmployeeUpdateValidator.ValidateAsync(employeeUpdate);
        var contact = await ContactRepos.GetByIdAsync(employeeUpdate.Id);
        if (contact == null)
            throw new ContactNotFoundException(employeeUpdate.Id);
        var existingEmployee = await EmployeeRepos.GetByIdAsync(employeeUpdate.Id);
        if (existingEmployee == null)
            throw new EmployeeNotFoundException(employeeUpdate.Id);
        var entity = Mapper.Map<Employee>(employeeUpdate);
        entity.Contact = contact;
        EmployeeRepos.Update(entity, existingEmployee);
        await EmployeeRepos.SaveChangesAsync();
    }
}