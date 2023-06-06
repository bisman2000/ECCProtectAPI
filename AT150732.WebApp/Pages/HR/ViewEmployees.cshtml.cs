
namespace AT150732.WebApp.Pages.HR;


[Authorize]
public class ViewEmployees : PageModel
{
    public ViewEmployees(IHttpClientFactory httpClientFactory,
        UserManager<AppUser> appUserManager, IConfiguration configuration)
    {
        HttpClientFactory = httpClientFactory;
        AppUserManager = appUserManager;
        Configuration = configuration;
    }
    [BindProperty]
    public List<EmployeeDetails> Employees { get; set; }
    private IHttpClientFactory HttpClientFactory { get; }
    private UserManager<AppUser> AppUserManager { get; }
    private IConfiguration Configuration { get; }

    public async Task OnGet()
    {
        var client = HttpClientFactory.CreateClient("API");
        var user = await AppUserManager.FindByNameAsync(User.Identity.Name);
            await client.GetAsync("auth/refresh-token");

        var str = await client.GetStringAsync("employee/get");
        var decryptStrJson = CryptoWorker.DecryptData(str, Request.Cookies["X-ServerKey"],
             user.ECDHPrivateKey, Configuration["AESIv"]);
        Employees = JsonConvert.DeserializeObject<List<EmployeeDetails>>(decryptStrJson);
    }
}
