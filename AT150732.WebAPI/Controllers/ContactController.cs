using AT150732.Common.Dtos;
using AT150732.Common.Dtos.Contact;
using AT150732.Common.Interface.Services;
using AT150732.WebAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AT150732.WebAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IContactService ContactService { get; }
        private IConfiguration Configuration { get; }

        public ContactController(IContactService contactService, IConfiguration configuration)
        {
            ContactService = contactService;
            Configuration = configuration;
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateContact(StringDto stringDto)
        {
            var data = stringDto.Value;
            var privateKey = Configuration["ECDHPrivkey"];
            var aesIv = Configuration["AESIv"];
            // key for decrypt token
            var secret = Configuration["Secret"];
            Request.Cookies.TryGetValue("X-Access-Token", out var accessToken);
            var str = CryptoWorker.DecryptData(data, privateKey, aesIv, secret, accessToken);
            var contactCreate = JsonConvert.DeserializeObject<ContactCreate>(str);
            var id = await ContactService.CreateContactAsync(contactCreate);
            return Ok(id);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteContact(ContactDelete contactDelete)
        {
            await ContactService.DeleteContactAsync(contactDelete);
            return Ok();
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateContact(StringDto stringDto)
        {
            var data = stringDto.Value;
            var privateKey = Configuration["ECDHPrivkey"];
            var aesIv = Configuration["AESIv"];
            // key for decrypt token
            var secret = Configuration["Secret"];
            Request.Cookies.TryGetValue("X-Access-Token", out var accessToken);
            var str = CryptoWorker.DecryptData(data, privateKey, aesIv, secret, accessToken);
            var contactUpdate = JsonConvert.DeserializeObject<ContactUpdate>(str);
            await ContactService.UpdateContactAsync(contactUpdate);
            return Ok();
        }
        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> GetContact(int id)
        {
            var contact = await ContactService.GetContactAsync(id);
            return Ok(contact);
        }
        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> GetContacts()
        {
            var contact = await ContactService.GetContactsAsync();
            return Ok(contact);
        }
    }
}
