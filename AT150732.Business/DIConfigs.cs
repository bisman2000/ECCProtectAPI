
namespace AT150732.Business;

public static class DIConfigs
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DtosMapperProfile));
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        services.AddScoped<ContactCreateValidator>();
        services.AddScoped<ContactUpdateValidator>();
        services.AddScoped<EmployeeCreateValidator>();
        services.AddScoped<EmployeeUpdateValidator>();

    }
}




