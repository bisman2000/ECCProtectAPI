

namespace AT150732.Common.Model;

public class LoginViewModel
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
    public string? publicKey { get; set; }
    public bool RememberLogin { get; set; } = false;
}
