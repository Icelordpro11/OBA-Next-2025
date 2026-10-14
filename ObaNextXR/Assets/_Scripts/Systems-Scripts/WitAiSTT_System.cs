using _Scripts;
using _Scripts.Managers_Scripts;
using Artem_Library.Attribute_Scripts;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Meta.WitAi.Dictation;
using Meta.WitAi.Dictation.Events;
using Oculus.Voice.Dictation;

public class WitAiSTT_System : Singleton_MonoBehaviour<WitAiSTT_System>
{
    [Header("Meta Voice SDK Reference")]
    [SerializeField] private AppDictationExperience dictationExperience;

    [SerializeField, BoolConverter] private bool isSendingToAI;

    [field: Header("Recognized Text Output")]
    [field: TextArea]
    [field: SerializeField] public string RecognizedText { get; private set; } = "";

    private UniTaskCompletionSource<string> _dictationTcs;

    [ContextMenu("🗣️ Start Voice to Text")]
    public void StartVoiceToText()
    {
        if (dictationExperience is null)
        {
            Debug.LogError("Missing AppDictationExperience reference.");
            return;
        }

        RunVoiceToTextAsync().Forget();
    }

    private async UniTask RunVoiceToTextAsync()
    {
        _dictationTcs = new UniTaskCompletionSource<string>();

        dictationExperience.DictationEvents.OnFullTranscription.AddListener(OnFullTranscription);
        dictationExperience.Activate();

        var result = await _dictationTcs.Task;

        RecognizedText = result;
        Debug.Log("Voice Transcribed: " + result);

        dictationExperience.DictationEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
        dictationExperience.Deactivate();
        if (isSendingToAI) Assistants_Manager.Instance.RunAssistantChain($"{result}").Forget();
    }

    private void OnFullTranscription(string transcription) => _dictationTcs?.TrySetResult(transcription);
}