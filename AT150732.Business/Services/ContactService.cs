
namespace AT150732.Business.Services;

public class ContactService : IContactService
{
    private IMapper Mapper { get; }
    private IBusinessRepos<Contact> ContactRepos { get; }
    private ContactCreateValidator ContactCreateValidator { get; }
    private ContactUpdateValidator ContactUpdateValidator { get; }

    public ContactService(IMapper mapper, IBusinessRepos<Contact> contactRepos,
        ContactCreateValidator contactCreateValidator, ContactUpdateValidator contactUpdateValidator)
    {
        Mapper = mapper;
        ContactRepos = contactRepos;
        ContactCreateValidator = contactCreateValidator;
        ContactUpdateValidator = contactUpdateValidator;
    }

    public async Task<int> CreateContactAsync(ContactCreate contactCreate)
    {
        await ContactCreateValidator.ValidateAndThrowAsync(contactCreate);
        var entity = Mapper.Map<Contact>(contactCreate);
        await ContactRepos.InsertAsync(entity);
        await ContactRepos.SaveChangesAsync();
        return entity.Id;
    }

    public async Task DeleteContactAsync(ContactDelete contactDelete)
    {
        var entity = await ContactRepos.GetByIdAsync(contactDelete.Id);
        if (entity == null)
            throw new ContactNotFoundException(contactDelete.Id);
        if (entity.Employees.Count > 0)
            throw new DependentEmployeesExistException(entity.Employees);
    }

    public async Task<ContactGet> GetContactAsync(int id)
    {
        var enity = await ContactRepos.GetByIdAsync(id);
        if (enity == null)
            throw new ContactNotFoundException(id);
        return Mapper.Map<ContactGet>(enity);
    }

    public async Task<List<ContactGet>> GetContactsAsync()
    {
        var entities = await ContactRepos.GetAsync();
        return Mapper.Map<List<ContactGet>>(entities);
    }

    public async Task UpdateContactAsync(ContactUpdate contactUpdate)
    {
        await ContactUpdateValidator.ValidateAsync(contactUpdate);
        var existContact = await ContactRepos.GetByIdAsync(contactUpdate.Id);
        if (existContact == null)
            throw new ContactNotFoundException(contactUpdate.Id);
        var entity = Mapper.Map<Contact>(contactUpdate);
        ContactRepos.Update(entity, existContact);
        await ContactRepos.SaveChangesAsync();
    }
}