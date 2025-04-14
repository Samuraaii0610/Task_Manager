using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskManager.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool isBusy;
    }
} 