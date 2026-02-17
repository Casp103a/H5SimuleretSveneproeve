namespace LogicLibrary.Interface
{
    public interface IApplicationSpecific
    {
        public Action SignUserOut { get; }
        public Action<string> SaveTokenInSecureStorage { get; }
        public string GetToken();
        public string? BaseUrl { get; }
    }
}
