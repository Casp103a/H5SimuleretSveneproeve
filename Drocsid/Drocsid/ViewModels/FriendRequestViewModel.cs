using Drocsid.Views;
using LogicLibrary.Dtos;
using LogicLibrary.Services;

namespace Drocsid.ViewModels
{
    internal class FriendRequestViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IHttpClientService httpClientService;

        public FriendRequestViewModel(IRegionManager regionManager, IHttpClientService httpClientService)
        {
            this.regionManager = regionManager;
            this.httpClientService = httpClientService;

            BackCommand = new DelegateCommand(Back);
            AcceptRequestCommand = new DelegateCommand<FriendRequestDto>(AcceptRequest);
            RejectRequestCommand = new DelegateCommand<FriendRequestDto>(RejectRequest);
        }

        public DelegateCommand BackCommand { get; }
        public DelegateCommand<FriendRequestDto> AcceptRequestCommand { get; }
        public DelegateCommand<FriendRequestDto> RejectRequestCommand { get; }


        private List<FriendRequestDto>? listOfFriendRequest;

        public List<FriendRequestDto>? ListOfFriendRequest
        {
            get => listOfFriendRequest;
            set
            {
                SetProperty(ref listOfFriendRequest, value);
            }
        }


        private void Back()
        {
            regionManager.RequestNavigate("main", nameof(ChatOverviewView));
        }

        private void AcceptRequest(FriendRequestDto requestDto)
        {

        }

        private void RejectRequest(FriendRequestDto requestDto)
        {

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
