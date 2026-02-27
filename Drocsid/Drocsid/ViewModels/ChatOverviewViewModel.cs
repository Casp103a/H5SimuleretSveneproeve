using Drocsid.Dialogs.Views;
using Drocsid.Views;

namespace Drocsid.ViewModels
{
    internal class ChatOverviewViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public ChatOverviewViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;

            AddFriendCommand = new DelegateCommand(AddFriend);
            LogoutCommand = new DelegateCommand(Logout);
            ViewFriendRequestCommand = new DelegateCommand(ViewFriendRequests);
        }

        public DelegateCommand AddFriendCommand { get; }
        public DelegateCommand LogoutCommand { get; }
        public DelegateCommand ViewFriendRequestCommand { get; }

        private void Logout()
        {
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

        }
        #endregion
    }
}
