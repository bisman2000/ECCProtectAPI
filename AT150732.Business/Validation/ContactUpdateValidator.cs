namespace AT150732.Business.Validation;

public class ContactUpdateValidator : AbstractValidator<ContactUpdate>
{
    public ContactUpdateValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(c => c.Address).NotEmpty();
        RuleFor(c => c.Phone).MaximumLength(32);
    }
}