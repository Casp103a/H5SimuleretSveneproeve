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

        public ChatViewModel(IHttpClientService httpClientService, IUserInfoService userInfoService)
        {
            this.httpClientService = httpClientService;
            this.userInfoService = userInfoService;

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
                Sent = DateTime.Now,
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
                chats.Add(conversation);
            }
            else
            {
                var error = await result.Content.ReadAsStringAsync();
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<ObservableCollection<ConversationDto>> FetchChat()
        {
            string endPoint = $"/api/chat/getConversationMessages?conversationId={chat.Id}";

            var result = await httpClientService.GetAsync(endPoint);

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var chats = JsonSerializer.Deserialize<ObservableCollection<ConversationDto>>(content);
                if(chats != null)
                    return chats;
            }
            else
            {
                var error = await result.Content.ReadAsStringAsync();
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return [];
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

            chats = FetchChat().GetAwaiter().GetResult();
        }
        #endregion
    }
}
