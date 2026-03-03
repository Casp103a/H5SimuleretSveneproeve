using Drocsid.Dialogs.Views;
using Drocsid.Factory;
using Drocsid.Messages.Events;
using Drocsid.Service;
using Drocsid.Views;
using LogicLibrary.Dtos;
using LogicLibrary.Services;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;

namespace Drocsid.ViewModels
{
    internal class ChatOverviewViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;
        private readonly IUserInfoService userInfoService;
        private readonly IHttpClientService httpClientService;
        private readonly IApiPollerCoordinatorService coordinatorService;

        public ChatOverviewViewModel(IRegionManager regionManager, IDialogService dialogService,
            IEventAggregator eventAggregator, IUserInfoService userInfoService, IHttpClientService httpClientService,
            IApiPollerCoordinatorService coordinatorService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;
            this.userInfoService = userInfoService;
            this.httpClientService = httpClientService;
            this.coordinatorService = coordinatorService;

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

        private ObservableCollection<UsersInChatJuncDto> listOfChats;

        public ObservableCollection<UsersInChatJuncDto> ListOfChats
        {
            get => listOfChats;
            set
            {
                SetProperty(ref listOfChats, value);
            }
        }

        private UsersInChatJuncDto selectedChat;

        public UsersInChatJuncDto SelectedChat
        {
            get => selectedChat;
            set
            {
                if (value == selectedChat)
                    return;

                SetProperty(ref selectedChat, value);
                ChangeChatView(SelectedChat);
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

        private void ChangeChatView(UsersInChatJuncDto chat)
        {
            var param = new NavigationParameters
            {
                { "chat", chat }
            };

            regionManager.RequestNavigate("chat", nameof(ChatView), param);
        }

        private ObservableCollection<UsersInChatJuncDto> GetChats()
        {
            string endpoint = $"/api/chat/getUserChats?userId={userInfoService.CurrentUser?.Id}";

            var result = httpClientService.GetAsync(endpoint).GetAwaiter().GetResult();

            if (result.IsSuccessStatusCode)
            {
                var json = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var chats = JsonSerializer.Deserialize<ObservableCollection<UsersInChatJuncDto>>(json, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                return chats ?? new ObservableCollection<UsersInChatJuncDto>();
            }
            else
            {
                // Display the error
                var error = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<UsersInChatJuncDto>();
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
            GreetUser = "Hi " + userInfoService.CurrentUser?.Username;
            ListOfChats = GetChats();

            foreach (var chat in listOfChats)
            {
                var config = new PollerConfig()
                {
                    ChatId = chat.Id,
                    PollerId = chat.ConversationName,
                    Interval = new TimeSpan(),
                    Url = $"/api/getIsThisLatestMessage?id={chat.Id}&conversationName={chat.ConversationName}"
                };

                coordinatorService.CreateAndStartPoller(config);
            }
        }
        #endregion
    }
}
