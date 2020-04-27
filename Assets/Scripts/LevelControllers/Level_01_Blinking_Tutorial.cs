using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level_01_Blinking_Tutorial : LevelController
{
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    float lastInstructionDuration;

    [SerializeField]
    TextMeshProUGUI timerText;  // The text for the timer used to finish the level 

    bool timerBegun = false;
    float timer = 0f;

    void Update()
    {
        // This if statement is directly from the LevelController class but is required here because of changes made to the function for this level
        if (!begun)
        {
            coroutine = StartCoroutine(LevelRoutine());
            begun = true;
        }

        timerText.gameObject.SetActive(timerBegun);
        if(timerBegun)
        {
            timer += Time.deltaTime;

            if (gameManager.OxygenLevel < 50f)
                timer = 0f;

            int t = (int)(timer * 100f);
            float d = t / 100f;
            timerText.text = "" + d;
        }
    }

    public override IEnumerator LevelRoutine()
    {
        // Show the tutorial text and wait for the player to blink
        text.GetComponent<Animator>().SetBool("Visible", true);
        yield return new WaitUntil(() => gameManager.GetEyesOpen() == false);
        yield return new WaitUntil(() => gameManager.GetEyesOpen() == true);

        // Change the text to explain why blinking is important
        text.GetComponent<Animator>().SetBool("Visible", false);
        yield return new WaitUntil(() => text.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Invisible"));
        text.text = "Not blinking will make it harder to control your other functions";
        text.GetComponent<Animator>().SetBool("Visible", true);

        yield return new WaitForSeconds(lastInstructionDuration);

        // Show the goal for the level
        text.GetComponent<Animator>().SetBool("Visible", false);
        yield return new WaitUntil(() => text.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Invisible"));
        text.text = "Keep your oxygen above 50% for 15 seconds";
        timerBegun = true;
        text.GetComponent<Animator>().SetBool("Visible", true);
        yield return new WaitUntil(() => text.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Visible"));
        yield return new WaitForSeconds(lastInstructionDuration);
        text.GetComponent<Animator>().SetBool("Visible", false);

        // Wait for the win con to be met
        yield return new WaitUntil(() => timer >= 15f);

        // Level is finished but needs to be implemented
    }
}
