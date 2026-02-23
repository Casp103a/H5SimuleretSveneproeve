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
        private readonly IPasswordService passwordService;

        public RegisterUserViewModel(IRegionManager regionManager, IHttpClientService httpClientService, IPasswordService passwordService)
        {
            this.regionManager = regionManager;
            this.httpClientService = httpClientService;
            this.passwordService = passwordService;

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
            string url = "http://192.168.10.25:8080/api/user/register";
            var user = new LoginRegisterUserDto()
            {
                Password = passwordService.EncodePassword(Password),
                Username = Username,
            };
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = httpClientService.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                //Navigate to overview for chats.
                regionManager.RequestNavigate("main", nameof(ChatOverviewView));
            }
            else
            {
                //Display the error
                var error = result.Content.ReadAsStringAsync().Result;
            }
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
