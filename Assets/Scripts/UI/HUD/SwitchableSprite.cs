using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SwitchableSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private int len;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    
    // switch to the `index` sprite
    public void Switch(int index)
    {
        if (index < len)
            image.sprite = sprites[index];
    }
}