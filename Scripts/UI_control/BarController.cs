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
    private Toggle IsCameraOn;

    private GameObject Vehicle;
    private GameObject VehicleCamera; 
    private int choice = 0;

    public void OnEnable()
    {
        document = this.GetComponent<UIDocument>();
        root = document.rootVisualElement;

        FindButtons();

        Vehicle = GameObject.FindWithTag("Player");
        VehicleCamera = GameObject.FindGameObjectWithTag("MainCamera");

        SimButton.RegisterCallback<ClickEvent>(ev_sim => OnSimClicked());
        PauseButton.RegisterCallback<ClickEvent>(ev_pause => OnPauseClicked());
        ChangePos.RegisterCallback<ClickEvent>(ev_pos => ChangeStartPosition());
        IsCameraOn.RegisterValueChangedCallback(ev_cam => OnToggleClicked());
        
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

    private void OnToggleClicked()
    {
        VehicleCamera.SetActive(IsCameraOn.value);
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

    /// <summary>
    /// Find all elements in the top bar
    /// </summary>
    /// <exception cref="System.Exception"></exception>
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

        IsCameraOn = root.Q<Toggle>("IsCameraOn");
        IsCameraOn.value = true;
    }

}
