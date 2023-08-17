using UnityEngine;
using UnityEngine.UI;

public class Shop_Item : MonoBehaviour
{
    public Button button;
    public RectTransform rect;
    public Text money;
    public Image image;
    public Text itemName;

    private int thisNum;
    private int thisMoney;
    private string thisType;
    private string detail;
    private object[] obj;

    public void SetData(Items _items, string _detail, int _money, int _index)
    {
        itemName.text = _items.name;
        image.sprite = _items.spriteImage;
        detail = _detail;
        thisNum = _index;
        thisMoney = _money;
        money.text = _money.ToString();
        rect.sizeDelta = new(money.preferredWidth + 20, rect.sizeDelta.y);
        obj = _items.obj;
        thisType = _items.type;
    }

    public void NoticeInfo()
    {
        GameManager._instance.itemInfo_Title.text = itemName.text;
        GameManager._instance.itemInfo_Image.sprite = image.sprite;
        GameManager._instance.itemInfo_Detail.text = detail;
        GameManager._instance.SetShopButtonText();
        GameManager._instance.itemInfo_Button.onClick.RemoveAllListeners();
        GameManager._instance.itemInfo_Button.onClick.AddListener(() => GameManager._instance.BuyShopItem(thisMoney, thisNum, thisType, obj));
        GameManager._instance.OpenInfoUI(1, transform.position.x);
    }
}
