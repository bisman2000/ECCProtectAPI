
namespace AT150732.WebApp.Pages.HR;

[Authorize]
public class DeleteEmployeeModel : PageModel
{
    public DeleteEmployeeModel(IHttpClientFactory httpClientFactory,
        UserManager<AppUser> AppUserManager, IConfiguration configuration)
    {
        HttpClientFactory = httpClientFactory;
        this.AppUserManager = AppUserManager;
        Configuration = configuration;
    }

    [BindProperty]
    public EmployeeDelete EmployeeDelete { get; set; }
    private IHttpClientFactory HttpClientFactory { get; }
    private UserManager<AppUser> AppUserManager { get; }
    private IConfiguration Configuration { get; }

    public async Task<IActionResult> OnPostDeleteEmployeeAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["deleteEmp"] = "require id";
            return Page();
        }

        var httpClient = HttpClientFactory.CreateClient("API");
        var user = await AppUserManager.FindByNameAsync(User.Identity.Name);

        //refresh-token
            await httpClient.GetAsync("auth/refresh-token");
        
        //encrypt data
        var data = CryptoWorker.EncryptData(JsonConvert.SerializeObject(EmployeeDelete),
            user.ECDHPrivateKey, Configuration["AESIv"], Request.Cookies["X-ServerKey"]);
        // sent to api
        var res = await httpClient.DeleteAsync($"Employee/Delete/{data}");
        if (res.IsSuccessStatusCode)
        {
            ViewData["deleteEmp"] = "delete success";
        }
        else
            ViewData["deleteEmp"] = "delete fail";
        return Page();
    }

}
