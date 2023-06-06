using AT150732.Common.Dtos;
using AT150732.Common.Dtos.Employee;
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
    public class EmployeeController : ControllerBase
    {
        private IEmployeeService EmployeeService { get; }
        private IConfiguration Configuration { get; }

        public EmployeeController(IEmployeeService employeeService, IConfiguration configuration)
        {
            EmployeeService = employeeService;
            Configuration = configuration;
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateEmployee(StringDto stringDto)
        {
            var data = stringDto.Value;
            var privateKey = Configuration["ECDHPrivkey"];
            var aesIv = Configuration["AESIv"];
            // key for decrypt token
            var secret = Configuration["Secret"];
            Request.Cookies.TryGetValue("X-Access-Token", out var accessToken);
            var str = CryptoWorker.DecryptData(data, privateKey, aesIv, secret, accessToken);
            var employeeCreate = JsonConvert.DeserializeObject<EmployeeCreate>(str);
            var id = await EmployeeService.CreateEmployeeAsync(employeeCreate);
            return Ok(id);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateEmployee(StringDto stringDto)
        {
            var data = stringDto.Value;
            var privateKey = Configuration["ECDHPrivkey"];
            var aesIv = Configuration["AESIv"];
            // key for decrypt token
            var secret = Configuration["Secret"];
            Request.Cookies.TryGetValue("X-Access-Token", out var accessToken);
            var str = CryptoWorker.DecryptData(data, privateKey, aesIv, secret, accessToken);
            var employeeUpdate = JsonConvert.DeserializeObject<EmployeeUpdate>(str);
            await EmployeeService.UpdateEmployeeAsync(employeeUpdate);
            return Ok();
        }

        [HttpDelete]
        [Route("Delete/{data}")]
        public async Task<IActionResult> DeleteEmployee(string data)
        {
            var privateKey = Configuration["ECDHPrivkey"];
            var aesIv = Configuration["AESIv"];
            // key for decrypt token
            var secret = Configuration["Secret"];
            Request.Cookies.TryGetValue("X-Access-Token", out var accessToken);
            var str = CryptoWorker.DecryptData(data, privateKey, aesIv, secret, accessToken);
            var employeeDelete = JsonConvert.DeserializeObject<EmployeeDelete>(str);
            await EmployeeService.DeleteEmployeeAsync(employeeDelete);
            return Ok();
        }

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await EmployeeService.GetEmployeeAsync(id);
            return Ok(employee);
        }
        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await EmployeeService.GetEmployeesAsnyc();
            var stringJson = JsonConvert.SerializeObject(employees);
            var privateKey = Configuration["ECDHPrivkey"];
            var aesIv = Configuration["AESIv"];
            // key for decrypt token
            var secret = Configuration["Secret"];
            Request.Cookies.TryGetValue("X-Access-Token", out var accessToken);
            var enc = CryptoWorker.EncryptData(stringJson, privateKey, aesIv, secret, accessToken);
            return Ok(enc);
        }
    }
}
