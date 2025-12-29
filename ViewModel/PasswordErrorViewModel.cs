using CommunityToolkit.Mvvm.ComponentModel;

namespace CrocoManager.ViewModel
{
    public partial class PasswordErrorViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage;

        public void SetError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        public void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }
    }
}
