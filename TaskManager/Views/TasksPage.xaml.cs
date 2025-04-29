using TaskManager.ViewModels;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Services;

namespace TaskManager.Views;

public partial class TasksPage : ContentPage
{
    private readonly TasksViewModel _viewModel;

    public TasksPage(TasksViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CheckAuthenticationAsync();
        await _viewModel.LoadTasksAsync();
    }
} 