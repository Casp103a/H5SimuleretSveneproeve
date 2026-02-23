using LogicLibrary.Interface;

namespace Drocsid
{
    internal class ApplicationSpecific : IApplicationSpecific
    {
        public Action SignUserOut => throw new NotImplementedException();

        public Action<string> SaveTokenInSecureStorage => throw new NotImplementedException();

        public string? BaseUrl => "http://192.168.10.25:8080";

        public string GetToken()
        {
            throw new NotImplementedException();
        }
    }
}
