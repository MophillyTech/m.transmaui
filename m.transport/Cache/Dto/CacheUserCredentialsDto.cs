using m.transport.Domain;
using m.transport.ServiceInterface;

namespace m.transport.Dto
{
    public class CacheUserCredentialsDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Truck { get; set; }
        public string DispatcherCode { get; set; }

        public CompanyInfo CompanyInfo { get; set; }

        public LoginResult LoginResult { get; set; }
    }
}