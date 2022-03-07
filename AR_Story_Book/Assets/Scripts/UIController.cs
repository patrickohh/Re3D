using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Button startButton;
    public Button howTo;
    public Label howtoText;
    public Label howtoBox;
    public bool howtoSwitch = true;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        startButton = root.Q<Button>("Start-button");
        howTo = root.Q<Button>("How-to");
        //howtoText = root.Q<Label>("How-to-label");
        howtoBox = root.Q<Label>("How-To-Label");

        startButton.clicked += StartButtonPressed;
        howTo.clicked += HowToButtonPressed;

    }

    void StartButtonPressed()
    {
        SceneManager.LoadScene("ARStorybook");
    }

    void HowToButtonPressed()
    {
        if(howtoSwitch == true)
        {
            howtoBox.text = "How-To\n" +
                            "1. Just click the Start Reading button and point your phone camera at the storybook to see awesome 3D images on every page!\n" +
                            "2. Click on the Page Turning Icon after turning each page to scan.";
            howtoBox.style.display = DisplayStyle.Flex;
            howtoSwitch = false;
        }
        else
        {
            howtoBox.style.display = DisplayStyle.None;
            howtoSwitch = true;
        }
    }
}
