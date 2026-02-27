using Drocsid.Views;
using LogicLibrary.Dtos;
using LogicLibrary.Interface;
using LogicLibrary.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Drocsid.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    internal class RegisterUserViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IApplicationSpecific applicationSpecific;
        private readonly IPasswordService passwordService;

        public RegisterUserViewModel(IRegionManager regionManager, IApplicationSpecific applicationSpecific, IPasswordService passwordService)
        {
            this.regionManager = regionManager;
            this.applicationSpecific = applicationSpecific;
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
            string endpoint = "/api/user/register";
            var user = new LoginRegisterUserDto()
            {
                Password = passwordService.Encrypt(Password),
                Username = Username,
            };
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpclient = new HttpClient();
            var result = httpclient.PostAsync(applicationSpecific.BaseUrl + endpoint, content).Result;

            if (result.IsSuccessStatusCode)
            {
                //Navigate to overview for chats.
                Back();
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
