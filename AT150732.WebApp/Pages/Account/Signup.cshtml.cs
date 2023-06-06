

namespace AT150732.WebApp.Pages.Account
{
    public class SignupModel : PageModel
    {

        [BindProperty]
        public SignupViewModel SignupViewModel { get; set; }
        private UserManager<AppUser> UserManager { get; }
        private IHttpClientFactory HttpClientFactory { get; }

        public SignupModel(UserManager<AppUser> userManager, IHttpClientFactory httpClientFactory)
        {
            UserManager = userManager;
            HttpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var httpClient = HttpClientFactory.CreateClient("API");
            var response = await httpClient.PostAsJsonAsync<SignupViewModel>("auth/register", SignupViewModel);

            if (response.IsSuccessStatusCode)
            {

                return RedirectToPage("/Account/Login");
            }


            return Page();
        }

    }
}
