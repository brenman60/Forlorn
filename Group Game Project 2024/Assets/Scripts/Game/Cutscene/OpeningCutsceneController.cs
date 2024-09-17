using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpeningCutsceneController : MonoBehaviour
{
    [SerializeField] private List<StartingCutsceneText> cutsceneTexts;
    [SerializeField] private DialogueUI cutsceneDialogue;
    [SerializeField] private float typewriteSpeed = 0.025f;

    private Coroutine startCutscene;
    private bool sendingToGame;

    private void Start()
    {
        cutsceneDialogue = DialogueUI.Instance;
        startCutscene = StartCoroutine(StartCutscene());
    }

    private void Update()
    {
        if (Input.GetKeyDown(Keybinds.GetKeybind(KeyType.CutsceneSkip)))
            SendToGame();
    }

    private IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(5f);

        foreach (StartingCutsceneText cutsceneText in cutsceneTexts)
        {
            DialogueProperties dialogueProperties = new DialogueProperties(cutsceneText.cutsceneText, Color.white, new Color(0, 0, 0, 0.5f), 10f, true, typewriteSpeed, new Dictionary<char, float>()
            {
                [' '] = 0.035f,
            });
            Task dialogueTask = cutsceneDialogue.AddDialogue(dialogueProperties);
            yield return new WaitUntil(() => dialogueTask.IsCompleted);
            yield return new WaitForSeconds((cutsceneText.waitTime + 10) * 1.5f);
        }

        SendToGame();
    }

    private void SendToGame()
    {
        if (sendingToGame) return;
        sendingToGame = true;
        if (startCutscene != null) StopCoroutine(startCutscene);

        WorldGeneration.worldSection = "City1";
        WorldGeneration.section = "1";
        TransitionUI.Instance.TransitionTo("Game");
    }

    [Serializable]
    private struct StartingCutsceneText
    {
        [TextArea] public string cutsceneText;
        public float waitTime;
    }
}
