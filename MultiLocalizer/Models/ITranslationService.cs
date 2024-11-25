using System.Threading;
using System.Threading.Tasks;
using MultiLocalizer.ViewModels;

namespace MultiLocalizer.Models;

public interface ITranslationService {
    Task Translate(string text, string context, Language targetLanguage, CancellationToken token = default);
    Task<bool> Initialize();
}