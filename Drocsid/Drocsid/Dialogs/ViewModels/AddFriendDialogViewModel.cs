using Drocsid.Service;
using LogicLibrary.Dtos;
using LogicLibrary.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Drocsid.Dialogs.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    internal class AddFriendDialogViewModel : BindableBase, IDialogAware
    {
        private readonly IHttpClientService httpClientService;
        private readonly IUserInfoService userInfoService;

        public AddFriendDialogViewModel(IHttpClientService httpClientService, IUserInfoService userInfoService)
        {
            this.httpClientService = httpClientService;
            this.userInfoService = userInfoService;

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

        private async void AddFriend()
        {
            string endpoint = "/api/friend/Create";

            if (userInfoService.CurrentUser == null)
                return;

            var dto = new AddFriendRequestDto()
            {
                SenderId = userInfoService.CurrentUser.Id,
                ReceiverUsername = usernameToAdd,
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var result = await httpClientService.PostAsync(endpoint, content);

                if (result.IsSuccessStatusCode)
                {
                    RequestClose.Invoke();
                }
                else
                {
                    // Display the error
                    var error = await result.Content.ReadAsStringAsync();
                    MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                RequestClose.Invoke();
            }
        }

        private void Close()
        {
            RequestClose.Invoke();
        }
    }
}
