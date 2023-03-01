using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerController : MonoBehaviour
{
    public float speed = 1000f; // moving speed of drawer
    public float finalPositionX;
    public float targetPositionX; // target positio of drawer

    private RectTransform rectTransform;
    private bool isMoving = false;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        finalPositionX = -rectTransform.rect.width / 4;
    }

    private void Update()
    {
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, new Vector2(targetPositionX, rectTransform.anchoredPosition.y), step);
            if (rectTransform.anchoredPosition.x == targetPositionX)
            {
                isMoving = false;
            }
        }
    }

    public void OpenDrawer()
    {
        targetPositionX = finalPositionX;
        isMoving = true;
    }

    public void CloseDrawer()
    {
        targetPositionX = -rectTransform.rect.width / 2;
        isMoving = true;
    }
}
