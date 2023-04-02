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
    private Foldout Map;

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
        json = new JsonAssist(Application.dataPath+"/PlayerData/");

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnPauseClicked()
    {
        if (Vehicle.GetComponent<LightCar>().enabled)
        {
            Vehicle.GetComponent<LightCar>().ChangeStartState();
            return;
        }

        /* TODO: There is no NULL ref check! */
        if (Vehicle.GetComponent<NAVCar>().enabled)
        {
            Vehicle.GetComponent<NAVCar>().ChangeStartState();
            return;
        }

        if(Vehicle.GetComponent<DWACar>().enabled)
        {
            Vehicle.GetComponent <DWACar>().ChangeStartState();
            return;
        }
    }

    private void OnToggleClicked()
    {
        VehicleCamera.SetActive(IsCameraOn.value);
    }

    private void UpdateSpeed()
    {
        Vehicle.GetComponent<LightCar>().SetScale(0.1f);
        Vehicle.GetComponent<DWACar>().SetScale(0.05f);
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

    private void OnMap01Clicked()
    {
        SceneManager.LoadScene(1);
    }

    private void OnMap02Clicked()
    {
        SceneManager.LoadScene(2);
    }

    private void OnMap03Clicked()
    {
        SceneManager.LoadScene(3);
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
        Map = root.Q<Foldout>("MapControl");
        SetFoldOut();
    }

    private void SetFoldOut()
    {
        Label save = new Label("Save");
        Label load = new Label("Load");
        Label randomPos = new Label("Next");
        Label scene_1 = new Label("Map01");
        Label scene_2 = new Label("Map02");
        Label scene_3 = new Label("Map03");

        File.Add(save);
        File.Add(load);
        Position.Add(randomPos);
        Map.Add(scene_1);
        Map.Add(scene_2);
        Map.Add(scene_3);

        save.RegisterCallback<ClickEvent>(ev_save => OnSaveClicked());
        load.RegisterCallback<ClickEvent>(ev_pos => OnLoadClicked());
        randomPos.RegisterCallback<ClickEvent>(ev_pos => ChangeStartPosition());
        scene_1.RegisterCallback<ClickEvent>(ev_load01 => OnMap01Clicked());
        scene_2.RegisterCallback<ClickEvent>(ev_load02 => OnMap02Clicked());
        scene_3.RegisterCallback<ClickEvent>(ev_load02 => OnMap03Clicked());
    }
}
