using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    private UIDocument document;

    private VisualElement root;
    private Button StartButton;
    private Button QuitButton;

    public void OnEnable()
    {
        document = this.GetComponent<UIDocument>();
        root = document.rootVisualElement;

        FindButtons();

        StartButton.RegisterCallback<ClickEvent>(ev => OnStartClicked());
        QuitButton.RegisterCallback<ClickEvent>(ev=> OnQuitClicked());
    }

    private void FindButtons()
    {
        if(root is null)
        {
            throw new System.Exception("Null document reference");
        }

        StartButton = root.Q<Button>("Start");
        QuitButton = root.Q<Button>("Quit");
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene(1);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }
}
