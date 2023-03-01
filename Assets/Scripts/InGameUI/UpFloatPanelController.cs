using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UpFloatPanelController : MonoBehaviour
{
    public float speed = 1000.0f; // moving speed of drawer
    public float finalPositionY;
    public float targetPositionY; // target positio of drawer

    private RectTransform rectTransform;
    /*
     * Replce it using relative final position: find position of visualization panel.
     * The final Y is visualization.RectTransform.rect.height/2 + visualization.y
     */
    private bool isMoving = false;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        finalPositionY = rectTransform.rect.height / 3.6f;
    }

    private void Update()
    {
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, new Vector2(rectTransform.anchoredPosition.x, targetPositionY), step);
            if (rectTransform.anchoredPosition.y == targetPositionY)
            {
                isMoving = false;
            }
        }
    }

    public void OpenDrawer()
    {
        targetPositionY = finalPositionY;
        isMoving = true;
    }

    public void CloseDrawer()
    {
        targetPositionY = rectTransform.rect.height/2.17f;
        isMoving = true;
    }
}
