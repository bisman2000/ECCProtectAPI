
namespace AT150732.WebApp.Pages.HR;

[Authorize]
public class UpdateEmployeeModel : PageModel
{
    [BindProperty]
    public EmployeeUpdate EmployeeUpdate { get; set; }
    private IHttpClientFactory HttpClientFactory { get; }
    private UserManager<AppUser> AppUserManager { get; }
    private IConfiguration Configuration { get; }

    public UpdateEmployeeModel(IHttpClientFactory httpClientFactory,
        UserManager<AppUser> AppUserManager, IConfiguration configuration)
    {
        HttpClientFactory = httpClientFactory;
        this.AppUserManager = AppUserManager;
        Configuration = configuration;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var httpClient = HttpClientFactory.CreateClient("API");
        await httpClient.GetAsync("auth/refresh-token");
        if (ModelState.IsValid)
        {
            var user = await AppUserManager.FindByNameAsync(User.Identity.Name);

            //refresh-token
                await httpClient.GetAsync("auth/refresh-token");

            //encrypt data
            var data = CryptoWorker.EncryptData(JsonConvert.SerializeObject(EmployeeUpdate),
                user.ECDHPrivateKey, Configuration["AESIv"], Request.Cookies["X-ServerKey"]);

            var res = await httpClient.PutAsJsonAsync<StringDto>("employee/update", new StringDto(data));
            if (res.IsSuccessStatusCode)
            {
                ViewData["updateEmp"] = " update success";
            }
            else
                ViewData["updateEmp"] = " update fail";
        }
        else
            ViewData["updateEmp"] = " pls fill up all field";
        return Page();
    }
}