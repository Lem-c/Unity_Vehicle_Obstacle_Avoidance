using UnityEngine;
using UnityEngine.UI;

public class FloatController : MonoBehaviour
{

    // Find arrow icon
    private GameObject down;
    private GameObject up;
    private Button floatBtn;

    GameObject drawer;

    private bool isClicked;
    private void Awake()
    {
        FindArrows();
        isClicked = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        drawer = GameObject.FindGameObjectWithTag("Drawer");
        floatBtn.onClick.AddListener(OnFloatClick);
    }


    private void FindArrows()
    {
        down = GameObject.Find("arrow_down");
        up = GameObject.Find("arrow_up");

        down.SetActive(true);
        up.SetActive(false);

        floatBtn = GetComponentInChildren<Button>();
    }
    private void OnFloatClick()
    {
        isClicked = !isClicked;

        down.SetActive(!isClicked);
        up.SetActive(isClicked);

        if (isClicked)
        {
            drawer.GetComponent<UpFloatPanelController>().OpenDrawer();
        }
        else
        {
            drawer.GetComponent<UpFloatPanelController>().CloseDrawer();
        }
    }
}
