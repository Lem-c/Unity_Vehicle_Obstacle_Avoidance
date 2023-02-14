using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class BarController : MonoBehaviour
{
    private UIDocument document;

    private VisualElement root;
    private Button SimButton;
    private Button PauseButton;

    private GameObject Vehicle;

    public void OnEnable()
    {
        document = this.GetComponent<UIDocument>();
        root = document.rootVisualElement;

        FindButtons();

        Vehicle = GameObject.FindWithTag("Player");

        SimButton.RegisterCallback<ClickEvent>(ev => OnSimClicked());
        PauseButton.RegisterCallback<ClickEvent>(ev => OnPauseClicked());
    }

    private void OnPauseClicked()
    {
        Vehicle.GetComponent<TestVehicle>().StartSim();
    }

    private void OnSimClicked()
    {
        Vehicle.GetComponent<TestVehicle>().StartSim();
    }

    private void FindButtons()
    {
        if (root is null)
        {
            throw new System.Exception("Null document reference");
        }

        SimButton = root.Q<Button>("Simulate");
        PauseButton = root.Q<Button>("Pause");
    }

}
