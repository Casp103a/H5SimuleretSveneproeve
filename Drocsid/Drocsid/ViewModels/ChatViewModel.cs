using Drocsid.Messages.Events;
using Drocsid.Messages.Query;
using Drocsid.Service;
using LogicLibrary.Dtos;
using LogicLibrary.Services;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Drocsid.ViewModels
{
    internal class ChatViewModel : BindableBase, INavigationAware
    {
        private readonly IHttpClientService httpClientService;
        private readonly IUserInfoService userInfoService;
        private readonly IEventAggregator eventAggregator;

        public ChatViewModel(IHttpClientService httpClientService, IUserInfoService userInfoService, IEventAggregator eventAggregator)
        {
            this.httpClientService = httpClientService;
            this.userInfoService = userInfoService;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.GetEvent<DisposeEvent>().Subscribe(Dispose);
            this.eventAggregator.GetEvent<ChatGotNewMessagesEvent>().Subscribe(GotNewMessages);
            this.eventAggregator.GetEvent<GetLatestMessageQuery>().Subscribe(GetLatestMessage);

            SendCommand = new DelegateCommand(Send);
        }

        private UsersInChatJuncDto chat;
        private string message;
        private ObservableCollection<ConversationDto> chats;

        public string Message
        {
            get => message;
            set
            {
                SetProperty(ref message, value);
            }
        }

        public ObservableCollection<ConversationDto> Chats
        {
            get => chats;
            set
            {
                SetProperty(ref chats, value);
            }
        }


        public DelegateCommand SendCommand { get; }

        private async void Send()
        {
            string endpoint = "/api/chat/sendMessage";

            if (userInfoService.CurrentUser == null)
                return;

            var conversation = new ConversationDto()
            {
                Message = Message,
                SenderId = userInfoService.CurrentUser.Id,
                SenderName = userInfoService.CurrentUser.Username!,
                Sent = DateTime.Now
            };

            var updateMessage = new UpdateMessageDto()
            {
                Conversation = conversation,
                UsersInChatJunc = chat
            };

            var json = JsonSerializer.Serialize(updateMessage);

            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await httpClientService.PostAsync(endpoint, stringContent);

            if (result.IsSuccessStatusCode)
            {
                Chats.Add(conversation);
                Message = string.Empty;
            }
            else
            {
                var error = await result.Content.ReadAsStringAsync();
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ObservableCollection<ConversationDto> FetchChat()
        {
            string endPoint = $"/api/chat/getConversationMessages?conversationId={chat.Id}";

            var result = httpClientService.GetAsync(endPoint).GetAwaiter().GetResult();

            if (result.IsSuccessStatusCode)
            {
                var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var chats = JsonSerializer.Deserialize<ObservableCollection<ConversationDto>>(content, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (chats != null)
                    return chats;
            }
            else
            {
                var error = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return [];
        }

        private void GotNewMessages(ChatGotNewMessageArgs args)
        {
            if (args.ChatId != chat.Id)
                return;

            string endPoint = $"/api/chat/getLatestMessage?id={Chats.Last().Id}&Conversation={args.ChatGuid}";

            var result = httpClientService.GetAsync(endPoint).GetAwaiter().GetResult();

            if (result.IsSuccessStatusCode)
            {
                var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var chats = JsonSerializer.Deserialize<ObservableCollection<ConversationDto>>(content, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (chats == null)
                    return;

                foreach (var chat in chats)
                {
                    Chats.Add(chat);
                }
            }
            else
            {
                var error = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetLatestMessage(GetLatestMessageQueryArgs args)
        {
            if (args.ChatId != chat.Id)
                return;

            args.Message = Chats.Last();

            return;
        }

        private void Dispose()
        {
            eventAggregator.GetEvent<ChatGotNewMessagesEvent>().Unsubscribe(GotNewMessages);
            eventAggregator.GetEvent<GetLatestMessageQuery>().Unsubscribe(GetLatestMessage);
            eventAggregator.GetEvent<DisposeEvent>().Unsubscribe(Dispose);
        }

        #region navigation
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var content = navigationContext.Parameters["chat"];
            if (content != null && content is UsersInChatJuncDto chat)
                this.chat = chat;

            Chats = FetchChat();
        }
        #endregion
    }
}
