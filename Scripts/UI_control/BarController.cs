using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class BarController : MonoBehaviour
{
    private UIDocument document;

    private VisualElement root;
    private Button PauseButton;
    private Button ResetButton;
    private Toggle IsCameraOn;
    private Foldout File;
    private Foldout Position;

    private GameObject Vehicle;
    private GameObject VehicleCamera; 
    private int choice = 0;

    // Serialize
    private JsonAssist json;

    public void OnEnable()
    {
        document = this.GetComponent<UIDocument>();
        root = document.rootVisualElement;
        // init json class
        json = new JsonAssist("built/");

        DefaultSetUp();

        Vehicle = GameObject.FindWithTag("Player");
        VehicleCamera = GameObject.FindGameObjectWithTag("MainCamera");

        PauseButton.RegisterCallback<ClickEvent>(ev_sim => OnPauseClicked());
        ResetButton.RegisterCallback<ClickEvent>(ev_pause => OnResetClicked());
        IsCameraOn.RegisterValueChangedCallback(ev_cam => OnToggleClicked());
    }

    public void FixedUpdate()
    {
        UpdateSpeed();
    }

    private void OnResetClicked()
    {
        SceneManager.LoadScene(1);
    }

    private void OnPauseClicked()
    {
        Vehicle.GetComponent<LightCar>().ChangeStartState();
    }

    private void OnToggleClicked()
    {
        VehicleCamera.SetActive(IsCameraOn.value);
    }

    private void UpdateSpeed()
    {
        Vehicle.GetComponent<LightCar>().SetScale(0.1f);
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

    private void OnSaveClicked()
    {
        json.SaveFile(Vehicle.GetComponent<LightCar>());
    }

    private void OnLoadClicked()
    {
        json.LoadFile();
    }

    /// <summary>
    /// Find all elements in the top bar
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void DefaultSetUp()
    {
        if (root is null)
        {
            throw new System.Exception("Null document reference");
        }
        // bind buttons
        PauseButton = root.Q<Button>("Pause");
        ResetButton = root.Q<Button>("Restart");
        // bind toggle
        IsCameraOn = root.Q<Toggle>("IsCameraOn");
        IsCameraOn.value = true;
        // bind fold out
        File = root.Q<Foldout>("FileControl");
        Position = root.Q<Foldout>("PosControl");
        SetFoldOut();
    }

    private void SetFoldOut()
    {
        Label save = new Label("Save");
        Label load = new Label("Load");
        Label randomPos = new Label("Next");

        File.Add(save);
        File.Add(load);
        Position.Add(randomPos);

        save.RegisterCallback<ClickEvent>(ev_save => OnSaveClicked());
        load.RegisterCallback<ClickEvent>(ev_pos => OnLoadClicked());
        randomPos.RegisterCallback<ClickEvent>(ev_pos => ChangeStartPosition());
    }
}
