using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallText : MonoBehaviour
{
    public Text text;
    public Image image;
    public RectTransform rect;

    public Sprite[] sprites;

    void Update()
    {
        transform.localPosition += 100 * Time.deltaTime * Vector3.up;
        text.color = new(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime * 2);
        transform.SetAsLastSibling(); // 가장 위에 표시되게 한다.

        if (text.color.a <= 0) Destroy(gameObject);
    }

    public void SetText(string _text)
    {
        text.text = _text;
        rect.sizeDelta = new(text.preferredWidth + 20, rect.sizeDelta.y);
    }
}
