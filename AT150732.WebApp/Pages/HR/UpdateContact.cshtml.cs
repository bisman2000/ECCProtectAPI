

namespace AT150732.WebApp.Pages.HR;

public class UpdateContactModel : PageModel
{
    [BindProperty]
    public ContactUpdate ContactUpdate { get; set; }
    private IHttpClientFactory HttpClientFactory { get; }
    private UserManager<AppUser> AppUserManager { get; }
    private IConfiguration Configuration { get; }

    public UpdateContactModel(IHttpClientFactory httpClientFactory,
        UserManager<AppUser> appUserManager, IConfiguration configuration)
    {
        HttpClientFactory = httpClientFactory;
        AppUserManager = appUserManager;
        Configuration = configuration;
    }
    public async Task<IActionResult> OnPostAsync()
    {
        var httpClient = HttpClientFactory.CreateClient("API");
        if (ModelState.IsValid)
        {
            var user = await AppUserManager.FindByNameAsync(User.Identity.Name);
            //refresh-token
                await httpClient.GetAsync("auth/refresh-token");

            //encrypt data
            var data = CryptoWorker.EncryptData(JsonConvert.SerializeObject(ContactUpdate),
                user.ECDHPrivateKey, Configuration["AESIv"], Request.Cookies["X-ServerKey"]);

            var res = await httpClient.PutAsJsonAsync<StringDto>("contact/update", new StringDto(data));
            if (res.IsSuccessStatusCode)
            {
                ViewData["updateContact"] = " update success";
            }
            ViewData["updatContact"] = " update fail";
        }
        else
            ViewData["updateContact"] = " pls fill up all field";
        return Page();
    }



}
