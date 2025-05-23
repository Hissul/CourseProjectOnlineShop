
using Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopLib;
using System.Text.Json;

namespace Client.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly LoginService _loginService;

        public LoginModel (LoginService apiService) {
            _loginService = apiService;
        }   

        [BindProperty]
        public ShopLib.LoginModel LogModel { get; set; }


        public void OnGet() {}


        public async Task<IActionResult> OnPost() {

            UserModel? authResponse = await _loginService.LoginAsync (LogModel.Email, LogModel.Password);

            if (authResponse == null) {
                ModelState.AddModelError ("", "������ �����. ��������� email � ������.");
                return Page ();
            }

            // ��������� ����� � ���� � ������
            HttpContext.Session.SetString ("user_id", authResponse.Id);
            HttpContext.Session.SetString ("user_name", authResponse.FullName);
            HttpContext.Session.SetString ("auth_token", authResponse.Token);
            HttpContext.Session.SetString ("user_role", string.Join (",", authResponse.Roles));

            if (authResponse.Roles.Contains ("Admin")) {
                return RedirectToPage ("/Admin/Statistics/Statistic");
            }
           

            return RedirectToPage ("/Index");
        }


    }
}
