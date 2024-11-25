using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MultiLocalizer.ViewModels;

namespace MultiLocalizer.Views;

public partial class MainWindow : Window {
    public static event Action<TranslationService>? OnTranslationServiceChanged;
    public static MainWindow? Instance { get; private set; }

    public MainWindow() {
        Instance = this;
        InitializeComponent();
    }

    private void OpenAiButton_OnClick(object? sender, RoutedEventArgs e) {
        OnTranslationServiceChanged?.Invoke(TranslationService.OpenAi);
    }

    public Task CopyToClipboard(string text) {
        return TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(text) ?? Task.CompletedTask;
    }
}