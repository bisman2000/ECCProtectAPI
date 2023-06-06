
namespace AT150732.Common.Model;

public class Contact : BaseEntity
{
    public string Address { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public List<Employee> Employees { get; set; }
}
