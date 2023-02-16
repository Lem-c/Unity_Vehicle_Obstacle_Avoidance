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
    private Button ChangePos;
    private Slider SpeedScale;

    private GameObject Vehicle;

    public void OnEnable()
    {
        document = this.GetComponent<UIDocument>();
        root = document.rootVisualElement;

        FindButtons();

        Vehicle = GameObject.FindWithTag("Player");

        SimButton.RegisterCallback<ClickEvent>(ev => OnSimClicked());
        PauseButton.RegisterCallback<ClickEvent>(ev => OnPauseClicked());
        ChangePos.RegisterCallback<ClickEvent>(ev => ChangeStartPosition());
    }

    public void FixedUpdate()
    {
        UpdateSpeed();
    }

    private void OnPauseClicked()
    {
        SceneManager.LoadScene(1);
    }

    private void OnSimClicked()
    {
        Vehicle.GetComponent<TestVehicle>().StartSim();
    }

    private void UpdateSpeed()
    {
        Vehicle.GetComponent<TestVehicle>().SetScale(SpeedScale.value * 0.1f);
    }

    private void ChangeStartPosition()
    {
        int choice = UnityEngine.Random.Range(0, 3);

        switch(choice)
        {
            case 0:
                Vehicle.GetComponent<TestVehicle>().StartPosition(-4.62f); break;
            case 1:
                Vehicle.GetComponent<TestVehicle>().StartPosition(4.82f, -3.5f); break;
            case 2: 
                Vehicle.GetComponent<TestVehicle>().StartPosition(); break;
            default:
                Vehicle.GetComponent<TestVehicle>().StartPosition(-2.9f); break;   
        }
    }

    private void FindButtons()
    {
        if (root is null)
        {
            throw new System.Exception("Null document reference");
        }

        SimButton = root.Q<Button>("Simulate");
        PauseButton = root.Q<Button>("Pause");
        ChangePos = root.Q<Button>("ChangePos");

        SpeedScale = root.Q<Slider>("SpeedScale");
        SpeedScale.value = 1f;
    }

}
