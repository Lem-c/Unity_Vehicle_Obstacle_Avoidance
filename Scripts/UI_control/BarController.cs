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
    private int choice = 0;

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
        Vehicle.GetComponent<LightCar>().ChangeStartState();
    }

    private void UpdateSpeed()
    {
        Vehicle.GetComponent<LightCar>().SetScale(SpeedScale.value * 0.1f);
    }

    private void ChangeStartPosition()
    {
        if (choice >= 3)
        {
            choice = 0;
        }
        else
        {
            choice += 1;
        }

        switch (choice)
        {
            case 0:
                Vehicle.GetComponent<LightCar>().ChangeStartPosition(-4.54f, -7.2f ); break;
            case 1:
                Vehicle.GetComponent<LightCar>().ChangeStartPosition(0.36f, -7.2f); break;
            case 2:
                Vehicle.GetComponent<LightCar>().ChangeStartPosition(5.33f, -7.2f); break;
            default:
                Vehicle.GetComponent<LightCar>().ChangeStartPosition(-7.76f, -7.2f); break;
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
