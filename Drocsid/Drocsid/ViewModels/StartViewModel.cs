using Drocsid.Service;
using Drocsid.Views;
using LogicLibrary.Dtos;
using LogicLibrary.Interface;
using LogicLibrary.Services;
using System.Windows;

namespace Drocsid.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    internal class StartViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly ILoginService loginService;
        private readonly IPasswordService passwordService;
        private readonly IApplicationSpecific applicationSpecific;
        private readonly IUserInfoService userInfoService;

        public StartViewModel(IRegionManager regionManager, ILoginService loginService, IPasswordService passwordService, 
            IApplicationSpecific applicationSpecific, IUserInfoService userInfoService)
        {
            this.regionManager = regionManager;
            this.loginService = loginService;
            this.passwordService = passwordService;
            this.applicationSpecific = applicationSpecific;
            this.userInfoService = userInfoService;

            LoginCommand = new DelegateCommand(Login);
            RegisterUserCommand = new DelegateCommand(RegisterUser);
        }

        public DelegateCommand LoginCommand { get; }
        public DelegateCommand RegisterUserCommand { get; }

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

        private bool rememberMe;

        public bool RememberMe
        {
            get => rememberMe;
            set
            {
                SetProperty(ref rememberMe, value);
            }
        }


        private async void Login()
        {
            var loginDto = new LoginRegisterUserDto();
            loginDto.Username = Username;
            var pass = passwordService.Encrypt(Password);
            loginDto.Password = pass;

            var result = await loginService.Login(loginDto);
            
            if (result.Success)
            {
                userInfoService.CurrentUser = result.User;
                applicationSpecific.SaveTokenInSecureStorage.Invoke(result.Token);

                //Navigate to overview of chats
                regionManager.RequestNavigate("main", nameof(ChatOverviewView));
            }
            else
            {
                //show what failed
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterUser()
        {
            regionManager.RequestNavigate("main", nameof(RegisterUserView));
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
