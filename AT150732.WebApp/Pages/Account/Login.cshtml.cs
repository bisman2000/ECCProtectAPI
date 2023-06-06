
namespace AT150732.WebApp.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public LoginViewModel LoginViewModel { get; set; }
    private SignInManager<AppUser> SignInManager { get; }
    private IHttpClientFactory HttpClientFactory { get; }
    private UserManager<AppUser> UserManager { get; }

    public LoginModel(SignInManager<AppUser> signInManager,
        IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager)
    {
        this.SignInManager = signInManager;
        HttpClientFactory = httpClientFactory;
        UserManager = userManager;
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var httpClient = HttpClientFactory.CreateClient("API");
        var privateKey = CryptoWorker.GeneratePrivateKey();
        LoginViewModel.publicKey = CryptoWorker.GetPublicKey(privateKey);
        var response = await httpClient.PostAsJsonAsync<LoginViewModel>("Auth/Login", LoginViewModel);

        string username = "";
        string serverKey = "";

        // get cookie inside response header
        IEnumerable<string> cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
        foreach (var cookie in cookies)
        {
            if (cookie.Contains("X-Username"))
                username = cookie.Split(new string[] { "=", ";" }, StringSplitOptions.None)[1];
            if (cookie.Contains("X-PublicKey"))
                serverKey = cookie.Split(new string[] { "=", ";" }, StringSplitOptions.None)[1];
        }

        var user = await UserManager.FindByNameAsync(username);
        if (response.IsSuccessStatusCode && serverKey !=null && user!=null)
        {
            user.ECDHPrivateKey = privateKey;
            HttpContext.Response.Cookies.Append("X-ServerKey", serverKey, new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                IsEssential = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
            await UserManager.UpdateAsync(user);
            await SignInManager.PasswordSignInAsync(LoginViewModel.Username,
                       LoginViewModel.Password, LoginViewModel.RememberLogin, false);

            return RedirectToPage("/Index");
        }


        return Page();
    }

}


