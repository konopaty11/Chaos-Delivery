using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI header;
    [SerializeField] List<CharacterVoiceControl> voiceControls;

    public static DialogManager Instance;

    List<Log> logs = new();

    Coroutine printTextCoroutine;
    public static event UnityAction<string> DialogEnd;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowDialog(string author, PersonID authorID, string content, string contentID, string key = null)
    {
        logs.Add(new Log(author, content, key, authorID, contentID));
        if (printTextCoroutine != null)
            return;

        UIManager.Instance.ShowUI("DialogWindow");
        printTextCoroutine = StartCoroutine(PrintText());
    }

    IEnumerator PrintText()
    {
        float timeFatctor = 0.04f;
        float minTime = 1.5f;

        while (true)
        {
            PlayVoice(logs[0].AuthorID, logs[0].MessageID);

            header.text = logs[0].Author;
            text.text = "";
            foreach (char symbol in logs[0].Content)
            {
                text.text += symbol;
                if (symbol != ' ') 
                    yield return new WaitForSeconds(0.03f);
            }

            int lenght = logs[0].Content.Length;
            yield return new WaitForSeconds(lenght * timeFatctor + minTime);

            if (logs[0].Key != null)
                DialogEnd?.Invoke(logs[0].Key);

            logs.RemoveAt(0);
            if (logs.Count == 0)
                break;
        }

        UIManager.Instance.HideUI("DialogWindow");
        printTextCoroutine = null;
    }

    void PlayVoice(PersonID author, string contentID)
    {
        foreach (CharacterVoiceControl control in voiceControls)
        {
            if (control.Author == author)
            {
                control.PlayVoice(contentID);
            }
        }
    }
}