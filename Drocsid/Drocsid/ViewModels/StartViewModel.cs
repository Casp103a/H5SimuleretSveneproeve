using Drocsid.Views;

namespace Drocsid.ViewModels
{
    internal class StartViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;

        public StartViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

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


        private void Login()
        {

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
