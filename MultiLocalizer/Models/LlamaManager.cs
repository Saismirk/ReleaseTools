using System.Collections.Generic;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace MultiLocalizer.Models;

public class LlamaManager {
    private const string LLAMA_MODEL_PATH = "E:\\Meta-Llama-3.1-8B-Instruct-Q6_K.gguf";
    
    private LLamaWeights _weights;
    private LLamaContext _context;
    private InteractiveExecutor? _executor;

    private readonly InferenceParams _inferenceParams = new() {
        MaxTokens = 256,
        AntiPrompts = new List<string>() { "User:" },
        SamplingPipeline = new DefaultSamplingPipeline()
    };

    private ChatSession _chatSession;
    
    private LlamaManager(string LlamaManager) {
        if (string.IsNullOrEmpty(LlamaManager)) {
            Debug.Log("Failed to Initialize Llama Manager");
            return;
        }
        
        var parameters = new ModelParams(LlamaManager) {
            ContextSize = 2048, 
            GpuLayerCount = 33
        };

        _weights = LLamaWeights.LoadFromFile(parameters);
        _context = _weights.CreateContext(parameters);
        _executor = new InteractiveExecutor(_context);

        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System,
                               $"You are a helpful translator. that translates text from English to any language."
                               + $"The translation is based on the following context: . The user will only write the text that needs to be translated. "
                               + $"You ONLY reply with the translated text, and nothing else. Do not write anything else.");
        chatHistory.AddMessage(AuthorRole.Assistant, "What do you wish to translate?");
        _chatSession = new ChatSession(_executor, chatHistory);
        _chatSession.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
                                             new[] { "User:", "Assistant:", "System:" },
                                             redundancyLength: 20));
    }
}