using System.Collections;
using UnityEngine;

public class OpeningCutsceneController : MonoBehaviour
{
    [SerializeField] private DialogueNode startingNode;
    [SerializeField] private float totalCutsceneTime;

    private Coroutine startCutscene;
    private bool sendingToGame;

    private void Start()
    {
        startCutscene = StartCoroutine(StartCutscene());
    }

    private void Update()
    {
        if (Keybinds.Instance.controlSkipCutscene.ReadValue<float>() != 0)
            SendToGame();
    }

    private IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(5f);

        yield return DialogueUI.Instance.AddDialogue(startingNode);
        yield return new WaitForSeconds(totalCutsceneTime * (startingNode.dialogue.Length * startingNode.typewriterSpeed));

        SendToGame();
    }

    public void SendToGame()
    {
        if (sendingToGame) return;
        sendingToGame = true;
        if (startCutscene != null) StopCoroutine(startCutscene);

        WorldGeneration.worldSection = "City1";
        WorldGeneration.section = "1";
        TransitionUI.Instance.TransitionTo("Game");
    }
}
