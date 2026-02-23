namespace Drocsid.Dialogs.ViewModels
{
    internal class AddFriendDialogViewModel : BindableBase, IDialogAware
    {
        public AddFriendDialogViewModel()
        {
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
            bool added = false;
            //If friend added

            if (added)
                RequestClose.Invoke();
        }

        private void Close()
        {
            RequestClose.Invoke();
        }
    }
}
