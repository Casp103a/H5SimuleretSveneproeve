using LogicLibrary.Dtos;
using LogicLibrary.Services;

namespace Drocsid.ViewModels
{
    internal class ChatViewModel : BindableBase, INavigationAware
    {
        private readonly IHttpClientService httpClientService;

        public ChatViewModel(IHttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;

            SendCommand = new DelegateCommand(Send);
        }

        private UsersInChatJuncDto chat;
        private string message;

        public string Message
        {
            get => message;
            set
            {
                SetProperty(ref message, value);
            }
        }

        public DelegateCommand SendCommand { get; }

        private async void Send()
        {

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
        }
        #endregion
    }
}
