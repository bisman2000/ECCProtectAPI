
namespace AT150732.WebApp.Pages.HR;

[Authorize]
public class CreateEmployee : PageModel
{
    [BindProperty]
    public EmployeeDetailsModel EmployeeDetailsModel { get; set; }

    private IHttpClientFactory HttpClientFactory { get; }
    private IConfiguration Configuration { get; }
    private UserManager<AppUser> UserManager { get; }

    public CreateEmployee(IHttpClientFactory httpClientFactory,
        IConfiguration configuration, UserManager<AppUser> userManager)
    {
        HttpClientFactory = httpClientFactory;
        Configuration = configuration;
        UserManager = userManager;
    }

    public async Task<IActionResult> OnPostCreateEmployeeAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var httpClient = HttpClientFactory.CreateClient("API");
        //refresh-token
            await httpClient.GetAsync("auth/refresh-token");
        // create dto
        var contactCreate = new ContactCreate(EmployeeDetailsModel.Address, EmployeeDetailsModel.Email, EmployeeDetailsModel.Phone);
        int contactId;
        var user = await UserManager.FindByNameAsync(User.Identity.Name);
        //encrypt data
        var encStr = CryptoWorker.EncryptData(JsonConvert.SerializeObject(contactCreate),
            user.ECDHPrivateKey, Configuration["AESIv"], Request.Cookies["X-ServerKey"]);
        // put string into dto
        var strDto1 = new StringDto(encStr);
        var response = await httpClient.PostAsJsonAsync<StringDto>("Contact/Create", strDto1);
        if (response.IsSuccessStatusCode)
            contactId = int.Parse(await response.Content.ReadAsStringAsync());
        else
        {
            ViewData["EmpCreateMess"] = "cant create contact";
            return Page();
        }
        //create Employee
        var employeeCreate = new EmployeeCreate(EmployeeDetailsModel.FullName, EmployeeDetailsModel.JobName, contactId);
        int empId;
        //encrypt data
        var encStrEmployee = CryptoWorker.EncryptData(JsonConvert.SerializeObject(employeeCreate),
            user.ECDHPrivateKey,Configuration["AESIv"], Request.Cookies["X-ServerKey"]);
        // put string into dto
        var strDto2 = new StringDto(encStrEmployee);

        var responseEmp = await httpClient.PostAsJsonAsync<StringDto>("Employee/Create", strDto2);
        if (response.IsSuccessStatusCode)
        {
            empId = int.Parse(await response.Content.ReadAsStringAsync());
            ViewData["EmpCreateMess"] = $"success!!! your id: {empId}";
        }
        else
        {
            ViewData["EmpCreateMess"] = "cant create employee";
            return Page();
        }
        return Page();
    }
}