using Drocsid.Views;
using LogicLibrary.Dtos;
using LogicLibrary.Services;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

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


        private ObservableCollection<FriendRequestDto>? listOfFriendRequest;

        public ObservableCollection<FriendRequestDto>? ListOfFriendRequest
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

        private async void AcceptRequest(FriendRequestDto requestDto)
        {
            string url = "http://192.168.10.25:8080/api/friend/accept";

            var json = JsonSerializer.Serialize(requestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = httpClientService.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                if (ListOfFriendRequest != null)
                {
                    ListOfFriendRequest.Remove(requestDto);
                }
            }
            else
            {
                // Display the error
                var error = result.Content.ReadAsStringAsync().Result;
            }
        }

        private void RejectRequest(FriendRequestDto requestDto)
        {
            string url = "http://192.168.10.25:8080/api/friend/reject";

            var json = JsonSerializer.Serialize(requestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = httpClientService.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                if (ListOfFriendRequest != null)
                {
                    ListOfFriendRequest.Remove(requestDto);
                    RaisePropertyChanged(nameof(ListOfFriendRequest));
                }
            }
            else
            {
                // Display the error
                var error = result.Content.ReadAsStringAsync().Result;
            }
        }

        private ObservableCollection<FriendRequestDto> GetFriendRequests(int userId)
        {
            string url = $"http://192.168.10.25:8080/api/friend/GetUserFriendRequest?userId={userId}";

            var result = httpClientService.GetAsync(url).Result;

            if (result.IsSuccessStatusCode)
            {
                var json = result.Content.ReadAsStringAsync().Result;
                var friendRequests = JsonSerializer.Deserialize<ObservableCollection<FriendRequestDto>>(json, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
                return friendRequests ?? new ObservableCollection<FriendRequestDto>();
            }
            else
            {
                // Display the error
                var error = result.Content.ReadAsStringAsync().Result;
                return new ObservableCollection<FriendRequestDto>();
            }
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
            int currentUserId = 9;

            ListOfFriendRequest = GetFriendRequests(currentUserId);
        }
        #endregion
    }
}
