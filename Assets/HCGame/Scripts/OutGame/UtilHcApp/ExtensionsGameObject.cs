using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionsGameObject
{
    public static void Hide(this GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public static void Show(this GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    public static void ChangeAlphaToFloat(this Material material, float value)
    {
        material.color = new Color(1f, 1f, 1f, value);
    }

    public static void ChangeAlphaImageToFloat(this Image image, float value)
    {
        image.color = new Color(1f, 1f, 1f, value);
    }

    public static T Cast<T>(this MonoBehaviour mono) where T : class
    {
        var t = mono as T;
        return t;
    }
}