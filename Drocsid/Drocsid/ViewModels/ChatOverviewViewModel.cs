using Drocsid.Dialogs.Views;
using Drocsid.Messages.Events;
using Drocsid.Service;
using Drocsid.Views;

namespace Drocsid.ViewModels
{
    internal class ChatOverviewViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;
        private readonly IUserInfoService userInfoService;

        public ChatOverviewViewModel(IRegionManager regionManager, IDialogService dialogService, IEventAggregator eventAggregator, IUserInfoService userInfoService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;
            this.userInfoService = userInfoService;

            AddFriendCommand = new DelegateCommand(AddFriend);
            LogoutCommand = new DelegateCommand(Logout);
            ViewFriendRequestCommand = new DelegateCommand(ViewFriendRequests);
        }

        public DelegateCommand AddFriendCommand { get; }
        public DelegateCommand LogoutCommand { get; }
        public DelegateCommand ViewFriendRequestCommand { get; }

        private string greetUser;

        public string GreetUser
        {
            get => greetUser;
            set 
            { 
                SetProperty(ref greetUser, value);
            }
        }


        private void Logout()
        {
            eventAggregator.GetEvent<SignoutEvent>().Publish();
            regionManager.RequestNavigate("main", nameof(StartView));
        }

        private void AddFriend()
        {
            dialogService.Show(nameof(AddFriendDialog));
        }

        private void ViewFriendRequests()
        {
            regionManager.RequestNavigate("main", nameof(FriendRequestView));
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
            GreetUser = "Hi " + userInfoService.CurrentUser?.Username;
        }
        #endregion
    }
}
