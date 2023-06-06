
namespace AT150732.WebApp.Pages.Account;

public class LogoutModel : PageModel
{
    private SignInManager<AppUser> SignInManager { get; }

    public LogoutModel(SignInManager<AppUser> signInManager)
    {
        SignInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await SignInManager.SignOutAsync();
        // delete all cookie when signout even cookie create from asp.net form (antiforge) and from identity

        /*if (HttpContext.Request.Cookies.Count > 0)
        {
            var siteCookies = HttpContext.Request.Cookies.Where(c =>c.Key != null);
            foreach (var cookie in siteCookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
        }*/
        // delete cookie authentication
        //Response.Cookies.Delete();
        // clear session
        return RedirectToPage("/Index");
    }

}