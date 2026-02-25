using LogicLibrary.Dtos;
using LogicLibrary.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Drocsid.Dialogs.ViewModels
{
    internal class AddFriendDialogViewModel : BindableBase, IDialogAware
    {
        private readonly IHttpClientService httpClientService;

        public AddFriendDialogViewModel(IHttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;

            CloseCommand = new DelegateCommand(Close);
            AddFriendCommand = new DelegateCommand(AddFriend);
        }

        public DialogCloseListener RequestClose { get; }
        public DelegateCommand CloseCommand { get; }
        public DelegateCommand AddFriendCommand { get; }

        private string usernameToAdd;

        public string UsernameToAdd
        {
            get => usernameToAdd;
            set 
            { 
                SetProperty(ref usernameToAdd, value); 
            }
        }


        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }

        private void AddFriend()
        {
            string url = "http://192.168.10.25:8080/api/friend/Create";

            var dto = new AddFriendRequestDto()
            {
                SenderId = 8, // TODO: need to be change to get the active users id
                ReceiverUsername = usernameToAdd,
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = httpClientService.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                RequestClose.Invoke();
            }
            else
            {
                // Display the error
                var error = result.Content.ReadAsStringAsync().Result;
            }
        }

        private void Close()
        {
            RequestClose.Invoke();
        }
    }
}
