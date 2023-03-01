using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    Button settingBtn;
    Button startBtn;
    Button quitBtn;

    GameObject drawer;
    bool isClikced;

    void Start()
    {
        BindObjects();
        isClikced = false;
    }

    private void BindObjects()
    {
        startBtn = this.GetComponentsInChildren<Button>()[0];
        settingBtn = this.GetComponentsInChildren<Button>()[1];
        quitBtn = this.GetComponentsInChildren<Button>()[2];

        drawer = GameObject.FindGameObjectWithTag("Drawer");

        startBtn.onClick.AddListener(OnStartClick);
        settingBtn.onClick.AddListener(OnSettingClick);
        quitBtn.onClick.AddListener(OnQuitClick);
    }

    private void OnSettingClick()
    {
        isClikced = !isClikced;

        if(isClikced)
        {
            drawer.GetComponent<DrawerController>().OpenDrawer();
        }
        else
        {
            drawer.GetComponent<DrawerController>().CloseDrawer();
        }
    }

    private void OnStartClick()
    {
        SceneManager.LoadScene(1);
    }

    private void OnQuitClick()
    {
        Application.Quit();
    }
}
