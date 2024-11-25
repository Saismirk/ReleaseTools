using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Notification;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiLocalizer.Models;
using MultiLocalizer.Views;
using OpenAI.Chat;
using LLama.Common;
using LLama;
using LLama.Sampling;

namespace MultiLocalizer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public static INotificationMessageManager Manager { get; } = new NotificationMessageManager();

    [ObservableProperty] private string _englishInput = string.Empty;
    [ObservableProperty] private string _contextInput = string.Empty;
    [ObservableProperty] private string? _spanishInput = string.Empty;
    [ObservableProperty] private string? _frenchInput = string.Empty;
    [ObservableProperty] private string? _germanInput = string.Empty;
    [ObservableProperty] private string? _italianInput = string.Empty;
    [ObservableProperty] private string? _japaneseInput = string.Empty;
    [ObservableProperty] private string? _chineseSimplifiedInput = string.Empty;
    [ObservableProperty] private string? _chineseTraditionalInput = string.Empty;
    [ObservableProperty] private string? _russianInput = string.Empty;
    [ObservableProperty] private OpenAiModel _openAiModel = OpenAiModel.Gpt35Turbo;
    [ObservableProperty] private TranslationService _translationServiceSelection;


    private ITranslationService? _translationService;
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _isTranslationRunning = false;
    private bool _isServiceInitialized = false;

    public MainWindowViewModel() {
        ContextInput = "Mobile Game localization";
        SetTranslationService(TranslationService.OpenAi);
        MainWindow.OnTranslationServiceChanged += OnTranslationServiceChanged;
    }

    [RelayCommand]
    private void OnTranslate() {
        if (_translationService is null) {
            Console.WriteLine("Translation Service is not initialized");
            return;
        }
        
        var value = EnglishInput;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new();
        foreach (var language in Enum.GetValues<Language>()) {
            _translationService.Translate(value, ContextInput, language, _cancellationTokenSource.Token);
        }
    }
    
    private async void SetTranslationService(TranslationService service) {
        _isServiceInitialized = false;
        Debug.Log($"Setting Translation Service: {service}");
        _translationService = GetTranslationService(service);
        _isServiceInitialized = await _translationService.Initialize();
        Debug.Log(_isServiceInitialized ? "Translation Service Initialized" : "Translation Service Failed to Initialize");
    }

    private ITranslationService GetTranslationService(TranslationService service) => service switch {
        TranslationService.OpenAi => new OpenAiManager(OpenAiModel, SetLanguage),
        // TranslationService.Llama => new LlamaManager(),
        _ => throw new ArgumentOutOfRangeException(nameof(service), service, null)
    };

    private Action<string> SetLanguage(Language language) => language switch {
        Language.Spanish => result => MainWindow.Instance!.SpanishInput.Input = result,
        Language.French => result => MainWindow.Instance!.FrenchInput.Input = result,
        Language.German => result => MainWindow.Instance!.GermanInput.Input = result,
        Language.Italian => result => MainWindow.Instance!.ItalianInput.Input = result,
        Language.Japanese => result => MainWindow.Instance!.JapaneseInput.Input = result,
        Language.ChineseSimplified => result => MainWindow.Instance!.ChineseSimplifiedInput.Input = result,
        Language.ChineseTraditional => result => MainWindow.Instance!.ChineseTraditionalInput.Input = result,
        Language.Russian => result => MainWindow.Instance!.RussianInput.Input = result,
        _ => _ => { },
    };
    
    private void OnTranslationServiceChanged(TranslationService service) => Task.Run(() => SetTranslationService(service));
}

public enum Language {
    // English,
    Spanish,
    French,
    German,
    Japanese,
    Italian,
    ChineseSimplified,
    ChineseTraditional,
    Russian,
}

public enum TranslationService {
    OpenAi,
    Llama,
}

public record struct TranslationResult(Language TargetLanguage, string Original, string Localized);