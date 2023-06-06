namespace AT150732.Common.Interface.Services
{
    public interface IContactService
    {
        Task<int> CreateContactAsync(ContactCreate contactCreate);
        Task UpdateContactAsync(ContactUpdate contactUpdate);
        Task DeleteContactAsync(ContactDelete contactDelete);
        Task<ContactGet> GetContactAsync(int id);
        Task<List<ContactGet>> GetContactsAsync();
    }
}
