using UnityEngine;
using UnityEngine.UI;

public class DeckCard : MonoBehaviour
{
    public Image image;
    public Text boolText;

    private string cardName;
    private string detail;
    private int cardIndex;
    private bool deck;

    public void SetDeck(bool _deck)
    {
        deck = _deck;

        if (deck) boolText.text = "D";
        else boolText.text = "";
    }

    public void SetData(string _name, Sprite _sprite, string _detail, int _cardIndex)
    {
        cardName = _name;
        image.sprite = _sprite;
        detail = _detail;
        cardIndex = _cardIndex;
    }

    public void NoticeInfo()
    {
        GameManager._instance.deckInfo_Title.text = cardName;
        GameManager._instance.deckInfo_Image.sprite = image.sprite;
        GameManager._instance.deckInfo_Detail.text = detail;
        GameManager._instance.deckInfo_Button.onClick.RemoveAllListeners();
        GameManager._instance.deckInfo_Button.onClick.AddListener(() => GameManager._instance.ToggleDeckCard(cardIndex));
        GameManager._instance.deckInfo_Change_Button.onClick.RemoveAllListeners();
        GameManager._instance.SetDeckButtonText(cardIndex);
        GameManager._instance.OpenInfoUI(2, transform.position.x);
    }
}
