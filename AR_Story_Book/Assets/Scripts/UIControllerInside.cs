using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIControllerInside : MonoBehaviour
{
    public Button backButton;
    public Button turnPageButton;
    SimpleCloudRecoEventHandler cloudObj;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        backButton = root.Q<Button>("Back-to-main");
        turnPageButton = root.Q<Button>("Next-Page-Button");

        backButton.clicked += BackButtonPressed;
        turnPageButton.clicked += TurnPageButtonPressed;

    }

    void BackButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void TurnPageButtonPressed()
    {
        cloudObj = GameObject.FindGameObjectWithTag("cloud").GetComponent<SimpleCloudRecoEventHandler>();
        cloudObj.ResetScanning();
    }
}
