using Drocsid.Views;
using LogicLibrary.Dtos;
using LogicLibrary.Services;

namespace Drocsid.ViewModels
{
    internal class StartViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly ILoginService loginService;
        private readonly IPasswordService passwordService;

        public StartViewModel(IRegionManager regionManager, ILoginService loginService, IPasswordService passwordService)
        {
            this.regionManager = regionManager;
            this.loginService = loginService;
            this.passwordService = passwordService;

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
            var pass = passwordService.EncodePassword(Password);
            loginDto.Password = pass;

            var (result, message) = await loginService.Login(loginDto);
            
            if (result)
            {
                //Navigate to overview of chats
                regionManager.RequestNavigate("main", nameof(ChatOverviewView));
            }
            else
            {
                //show what failed
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
