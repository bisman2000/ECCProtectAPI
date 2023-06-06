


namespace AT150732.Business;

public class DtosMapperProfile : Profile
{

    public DtosMapperProfile()
    {
        //map from <A,B> A->B
        //mapping Contact
        CreateMap<ContactCreate, Contact>()
                //ignore mapping Id
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<ContactUpdate, Contact>();
        CreateMap<Contact, ContactGet>();
        //mapping employee
        CreateMap<EmployeeCreate, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Contact, opt => opt.Ignore());
        CreateMap<Employee, EmployeeDetails>()
          .ForMember(dest => dest.Id, opt => opt.Ignore())
          .ForMember(dest => dest.Contact, opt => opt.Ignore());
        CreateMap<EmployeeUpdate, Employee>()
            .ForMember(dest => dest.Contact, opt => opt.Ignore());

    }


}
