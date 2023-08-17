using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelButton : MonoBehaviour
{
    public Text text;
    public RectTransform rect;

    public void SetText(string _text)
    {
        text.text = _text;
        rect.sizeDelta = new(text.preferredWidth + 20, rect.sizeDelta.y);
    }
}
