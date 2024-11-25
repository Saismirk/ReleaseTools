using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MultiLocalizer.ViewModels;
using OpenAI.Chat;

namespace MultiLocalizer.Models;

public enum OpenAiModel {
    Gpt35Turbo,
    Gpt4O,
    Gpt4OTurbo,
    Gpt4OMini,
}

public class OpenAiManager : ITranslationService {
    private const string PROMPT_FORMAT =
        "Text to Translate:\n{0}\n\nTranslate to {1}. Show ONLY the translation result with no other text. If translation is not possible, show the original text. Translation context: {2}";

    private ChatClient? _openAi;
    private readonly Func<Language, Action<string>> _onTranslationResult;
    private readonly OpenAiModel _model;

    public OpenAiManager(OpenAiModel model, Func<Language, Action<string>> onTranslationResult) {
        _onTranslationResult = onTranslationResult;
        _model = model;
    }
    
    private static string GetOpenAiModel(OpenAiModel model) => model switch {
        OpenAiModel.Gpt35Turbo => "gpt-3.5-turbo",
        OpenAiModel.Gpt4O => "gpt-4o",
        OpenAiModel.Gpt4OTurbo => "gpt-4o-turbo",
        OpenAiModel.Gpt4OMini => "gpt-4o-mini",
        _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
    };
    
    public async Task Translate(string text, string context, Language targetLanguage, CancellationToken token = default) => 
        TranslateAsync(text, context, targetLanguage, token).ContinueWith(t => _onTranslationResult.Invoke(targetLanguage)(t.Result.Localized), token);

    public Task<bool> Initialize() {
        Console.WriteLine("Initializing OpenAI Chat Client");
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey)) {
            Debug.Log("OPENAI_API_KEY is not set");
            return Task.FromResult(false);
        }

        try {
            _openAi = new ChatClient(model: GetOpenAiModel(_model), apiKey: apiKey);
        }
        catch (Exception e) {
            Debug.Log($"Failed to Initialize:\n{e.Message}");
            return Task.FromResult(false);
        }
        
        return Task.FromResult(true);
    }

    private async Task<TranslationResult> TranslateAsync(string text, string context, Language targetLanguage, CancellationToken token = default) {
        if (string.IsNullOrWhiteSpace(text) || _openAi is null) return new TranslationResult(targetLanguage, string.Empty, string.Empty);
        await Task.Delay(100, token);
        var message = new List<ChatMessage> {
            string.Format(PROMPT_FORMAT, text, targetLanguage, context)
        };
        var completionOptions = new ChatCompletionOptions();
        try {
            var completion = await _openAi.CompleteChatAsync(message, completionOptions, token);
            return new TranslationResult(targetLanguage, text, completion.Value.Content[0].Text);
        }
        catch (Exception e) {
            Debug.Log($"Failed to translate:\n{e.Message}");
            return new TranslationResult(targetLanguage, text, string.Empty);
        }
    }
}