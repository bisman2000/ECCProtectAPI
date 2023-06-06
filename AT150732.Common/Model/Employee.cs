

namespace AT150732.Common.Model;

public class Employee : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string JobName { get; set; } = default!;
    public Contact Contact { get; set; } = default!;
}
