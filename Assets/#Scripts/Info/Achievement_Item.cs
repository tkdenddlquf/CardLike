using UnityEngine;
using UnityEngine.UI;

public class Achievement_Item : MonoBehaviour
{
    public Image image;
    public Text itemName;

    private string detail;

    public void SetData(string _name, Sprite _sprite, string _detail)
    {
        itemName.text = _name;
        image.sprite = _sprite;
        detail = _detail;
    }

    public void NoticeInfo()
    {
        GameManager._instance.achievementInfo_Title.text = itemName.text;
        GameManager._instance.achievementInfo_Image.sprite = image.sprite;
        GameManager._instance.achievementInfo_Detail.text = detail;
        GameManager._instance.OpenInfoUI(0, transform.position.x);
    }
}
