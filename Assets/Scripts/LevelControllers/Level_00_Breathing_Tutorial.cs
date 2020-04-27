using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level_00_Breathing_Tutorial : LevelController
{
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    float lastInstructionDuration;

    public override IEnumerator LevelRoutine()
    {
        // Show the tutorial text and waits until the lungs are holding
        text.GetComponent<Animator>().SetBool("Visible", true);
        yield return new WaitUntil(() => gameManager.GetCurrentLungState() == GameManager.LungState.HOLDING);

        // Hide the tutorial text and wait for the text to be invisible
        text.GetComponent<Animator>().SetBool("Visible", false);
        yield return new WaitUntil(() => text.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Invisible"));

        // Update and show the text and then wait until the lungs are empty
        text.text = "Press and hold\n'o' key to exhale";
        text.GetComponent<Animator>().SetBool("Visible", true);
        yield return new WaitUntil(() => gameManager.GetCurrentLungState() == GameManager.LungState.EMPTY);

        // Hide the text and wait for it to be invisible
        text.GetComponent<Animator>().SetBool("Visible", false);
        yield return new WaitUntil(() => text.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Invisible"));

        // Update and show the text
        text.text = "Fill up the blue Oxygen meter to complete the level";
        text.GetComponent<Animator>().SetBool("Visible", true);

        // Wait for an amount of time and then hide the text and finish
        yield return new WaitForSeconds(lastInstructionDuration);
        text.GetComponent<Animator>().SetBool("Visible", false);

        yield return new WaitUntil(() => gameManager.OxygenLevel == 100f);

        // Level is finished but needs to be implemented
    }
}
