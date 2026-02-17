using Drocsid.Views;
using LogicLibrary.Dtos;
using LogicLibrary.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Drocsid.ViewModels
{
    internal class RegisterUserViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IHttpClientService httpClientService;

        public RegisterUserViewModel(IRegionManager regionManager, IHttpClientService httpClientService)
        {
            this.regionManager = regionManager;
            this.httpClientService = httpClientService;

            RegisterUserCommand = new DelegateCommand(RegisterUser);
            BackCommand = new DelegateCommand(Back);
        }

        public DelegateCommand RegisterUserCommand { get; }
        public DelegateCommand BackCommand { get; }

        private string username;

        public string Username
        {
            get => username;
            set
            {
                SetProperty(ref username, value);
            }
        }

        private string password;

        public string Password
        {
            get => password;
            set
            {
                SetProperty(ref password, value);
            }
        }

        private void RegisterUser()
        {
            string url = "https://192.168.10.139:7257/api/user/register";
            var user = new UserDto()
            {
                Password = password,
                Username = username,
            };
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = httpClientService.PostAsync(url, content).Result;
        }

        private void Back()
        {
            regionManager.RequestNavigate("main", nameof(StartView));
        }

        #region Navigation
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
