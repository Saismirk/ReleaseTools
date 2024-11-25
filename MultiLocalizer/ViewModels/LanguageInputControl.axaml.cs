using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiLocalizer.Views;

namespace MultiLocalizer.ViewModels;

public partial class LanguageInputControl : UserControl {
    public static readonly StyledProperty<string> LanguageProperty =
        AvaloniaProperty.Register<LanguageInputControl, string>(nameof(Language));

    public static readonly StyledProperty<string> InputProperty =
        AvaloniaProperty.Register<LanguageInputControl, string>(nameof(Input));

    public string Language {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public string Input {
        get => GetValue(InputProperty);
        set => Dispatcher.UIThread.Invoke(() => SetValue(InputProperty, value));
    }

    public LanguageInputControl() {
        DataContext = this;
        InitializeComponent();
    }

    [RelayCommand]
    private async Task CopyToClipboard() {
        var input = await Dispatcher.UIThread.InvokeAsync(() => Input);
        await MainWindow.Instance?.CopyToClipboard(input)!;
    }
}