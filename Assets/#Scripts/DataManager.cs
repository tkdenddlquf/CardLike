using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager _instance;
    void Awake() { _instance = this; }

    private MyData myData;
    private CombineData combines;
    private MissionList missions;
    private DebuffList debuffs;
    private MatchingData matchings;
    private AchievementList achievements;
    private AllCardData allCards;
    private TextData texts;
    private ItemData items;

    private const string MY_DATA_FILE_NAME = "MyData.json";
    private const string GAME_CHAR_FILE_NAME = "Data/Char";
    private const string GAME_TEXTS_FILE_NAME = "Data/Text";
    private const string GAME_CARDS_FILE_NAME = "Data/AllCard";
    private const string GAME_ACHIEVEMENT_FILE_NAME = "Data/Achievement";
    private const string GAME_MATCHING_FILE_NAME = "Data/Matching";
    private const string GAME_DEBUFF_FILE_NAME = "Data/Debuff";
    private const string GAME_MISSION_FILE_NAME = "Data/Mission";
    private const string GAME_COMBINE_FILE_NAME = "Data/Combine";
    private const string GAME_ITEM_FILE_NAME = "Data/Item";

    public void GameDataLoad()
    {
        combines = JsonUtility.FromJson<CombineData>(Resources.Load<TextAsset>(GAME_COMBINE_FILE_NAME).text);
        missions = JsonUtility.FromJson<MissionList>(Resources.Load<TextAsset>(GAME_MISSION_FILE_NAME).text);
        debuffs = JsonUtility.FromJson<DebuffList>(Resources.Load<TextAsset>(GAME_DEBUFF_FILE_NAME).text);
        matchings = JsonUtility.FromJson<MatchingData>(Resources.Load<TextAsset>(GAME_MATCHING_FILE_NAME).text);
        achievements = JsonUtility.FromJson<AchievementList>(Resources.Load<TextAsset>(GAME_ACHIEVEMENT_FILE_NAME).text);
        allCards = JsonUtility.FromJson<AllCardData>(Resources.Load<TextAsset>(GAME_CARDS_FILE_NAME).text);
        texts = JsonUtility.FromJson<TextData>(Resources.Load<TextAsset>(GAME_TEXTS_FILE_NAME).text);
        items = JsonUtility.FromJson<ItemData>(Resources.Load<TextAsset>(GAME_ITEM_FILE_NAME).text);
    }

    public void RemoveGameData()
    {
        combines = null;
        missions = null;
        debuffs = null;
        matchings = null;
        achievements = null;
        allCards = null;
        texts = null;
    }

    public void MyDataSave(MyData _myData)
    {
        File.WriteAllText(Application.persistentDataPath + "/" + MY_DATA_FILE_NAME, JsonUtility.ToJson(_myData, true), System.Text.Encoding.UTF8);
    }

    public void MyDataLoad()
    {
        string path = Application.persistentDataPath + "/" + MY_DATA_FILE_NAME;

        if (File.Exists(path)) myData = JsonUtility.FromJson<MyData>(File.ReadAllText(path, System.Text.Encoding.UTF8));
        else myData = JsonUtility.FromJson<MyData>(Resources.Load<TextAsset>(GAME_CHAR_FILE_NAME).text);
    }

    public MyData GetMyData()
    {
        return myData;
    }

    public ItemData GetItemData()
    {
        return items;
    }

    public Combines[] GetConbineData()
    {
        return combines.data;
    }

    public DebuffList GetDebuffList()
    {
        return debuffs;
    }

    public SimpleNum[] GetMissionList()
    {
        return missions.data;
    }

    public Matching GetMatchingData()
    {
        return matchings.data;
    }

    public Achievements[] GetAchievementList()
    {
        return achievements.data;
    }

    public AllCards[] GetAllCardData()
    {
        return allCards.data;
    }

    public Texts GetTexts(int _num)
    {
        return texts.data[_num];
    }
}

[System.Serializable]
public class MyData
{
    [SerializeField] int hp;
    [SerializeField] int atk;
    [SerializeField] int money;
    [SerializeField] int maxCard;
    [SerializeField] float coolTime;
    [SerializeField] int languege;
    [SerializeField] int bossCount;
    [SerializeField] string[] item;
    [SerializeField] MyAchievement[] achievements;
    [SerializeField] int[] days;
    [SerializeField] ShopItem[] shopItem;
    [SerializeField] MyCard[] myCards;

    public void SetAchievement(string _name, int _num)
    {
        if (GetAchievement(_name) == -1) // 업적이 없는 경우
        {
            List<MyAchievement> _achieve = new(achievements);
            _achieve.Add(new(_name, _num));
            int _length = _achieve.Count;
            achievements = new MyAchievement[_length];

            for (int i = 0; i < _length; i++) achievements[i] = _achieve[i];
        }
        else // 업적이 있는 경우
        {
            int _length = achievements.Length;

            for (int i = 0; i < _length; i++)
            {
                if (achievements[i].name == _name) achievements[i].num = _num;
            }
        }
    }

    public int GetAchievement(string _name)
    {
        int _length = achievements.Length;

        for (int i = 0; i < _length; i++)
        {
            if (achievements[i].name == _name)
            {
                return achievements[i].num;
            }
        }

        return -1;
    }

    public void SetCards(int _index, int _num)
    {
        List<MyCard> _myCards = new(myCards);

        if (_index > 0)
        {
            if (_num == -1) _myCards.RemoveAt(_index);
            else _myCards[_index].card = _num;
        }
        else
        {
            if (_index == -1) _myCards.Add(new MyCard(_num));
            else if (_index == -2) _myCards.Insert(0, new MyCard(_num));
        }

        myCards = _myCards.ToArray();
    }

    public void SetDeck(int _index, bool _deck)
    {
        myCards[_index].deck = _deck;
    }

    public MyCard[] GetCards()
    {
        return myCards;
    }

    public void SetCoolTime(float _num, bool _bool)
    {
        if (_bool) coolTime += _num;
        else coolTime = _num;
    }

    public float GetCoolTime()
    {
        return coolTime;
    }

    public void SetItem(int _index, string _name)
    {
        List<string> _items = new(item);

        if (_index == -1)
        {
            _items.Add(_name);
            item = _items.ToArray();
        }
        else
        {
            if (_name == null)
            {
                _items.RemoveAt(_index);
                item = _items.ToArray();
            }
            else item[_index] = _name;
        }
    }

    public string[] GetItem()
    {
        return item;
    }

    public void SetShopItemNum(int _index, int _num)
    {
        shopItem[_index].num = _num;
    }

    public void SetShopItemMoney(int _index, int _num)
    {
        shopItem[_index].money = _num;
    }

    public ShopItem[] GetShopItem()
    {
        return shopItem;
    }

    public void SetBossCount(int _num, bool _bool)
    {
        if (_bool) bossCount += _num;
        else bossCount = _num;

        if (bossCount < 0) bossCount = 0;
    }

    public int GetBossCount()
    {
        return bossCount;
    }

    public void SetLanguege(int _num)
    {
        languege = _num;
    }

    public int GetLanguege()
    {
        return languege;
    }

    public void SetMoney(int _money, bool _bool)
    {
        if (_bool) money += _money;
        else money = _money;

        if (money < 0) money = 0;
    }

    public int GetMoney()
    {
        return money;
    }

    public void SetHp(int _hp, bool _bool)
    {
        if (_bool) hp += _hp;
        else hp = _hp;

        if (hp < 0) hp = 0;
    }

    public int GetHp()
    {
        return hp;
    }

    public void SetAtk(int _atk, bool _bool)
    {
        if (_bool) atk += _atk;
        else atk = _atk;

        if (atk < 0) atk = 0;
    }

    public int GetAtk()
    {
        return atk;
    }

    public void SetMaxCard(int _maxCard, bool _bool)
    {
        if (_bool) maxCard += _maxCard;
        else maxCard = _maxCard;
    }

    public int GetMaxCard()
    {
        return maxCard;
    }

    public void SetDays(int _days, int _percent)
    {
        days[0] = _days;
        days[1] = _percent;
    }

    public int[] GetDays()
    {
        return days;
    }
}

public class ItemData
{
    public Items[] items;
}

public class CombineData
{
    public Combines[] data;
}

public class DebuffList
{
    public SimpleNum[] title;
    public Debuffs[] data;
}

public class MissionList
{
    public SimpleNum[] data;
}

public class MatchingData
{
    public Matching data;
}

public class AchievementList
{
    public Achievements[] data;
}

public class AllCardData
{
    public AllCards[] data;
}

public class TextData
{
    public Texts[] data;
}

[System.Serializable]
public class MyCard
{
    public int card;
    public bool deck = false;

    public MyCard(int _card)
    {
        card = _card;
    }
}

[System.Serializable]
public class MyAchievement
{
    public string name;
    public int num;

    public MyAchievement(string _name, int _num)
    {
        name = _name;
        num = _num;
    }
}

[System.Serializable]
public class Items
{
    public string name;
    public int num;
    public string sprite;
    public string[] objStrings;
    public string type;

    public object[] obj = new object[2];
    public Sprite spriteImage;

    public void SetData()
    {
        obj[0] = int.Parse(objStrings[0]);

        if (objStrings[1] == "true" || objStrings[1] == "false") obj[1] = bool.Parse(objStrings[1]);
        else
        {
            if (int.TryParse(objStrings[1], out int _num)) obj[1] = _num;
            else obj[1] = objStrings[1];
        }

        if (type == "Cards")
        {
            AllCards _allCards = GameManager._instance.GetCardData((int)obj[1]);
            name = _allCards.name;
            spriteImage = _allCards.spriteImage;
        }
        else
        {
            spriteImage = Resources.Load<Sprite>(sprite);
            name = GameManager._instance.TransName(name, num);
        }
    }
}

[System.Serializable]
public class Combines
{
    public string result;
    public int result_Num;
    public string[] use;
    public int[] use_Num;

    public void SetData()
    {
        result = GameManager._instance.GetCardData(result_Num).name;
        int _length = use_Num.Length;
        use = new string[_length];

        for (int i = 0; i < _length; i++) use[i] = GameManager._instance.GetCardData(use_Num[i]).name;
    }
}

[System.Serializable]
public class Debuffs
{
    public string type;
    public bool increase;
    public bool data;
    public int value;
}

[System.Serializable]
public class SimpleNum
{
    public int[] num;
}

[System.Serializable]
public class ShopItem
{
    public int num;
    public int money;
}

[System.Serializable]
public class Matching
{
    public MissionData[] mission;
    public string[] result;
}

[System.Serializable]
public class MissionData
{
    public string name;
    public int[] num;
    public int max = 0;
}

[System.Serializable]
public class Achievements
{
    public string name;
    public int category;
    public string sprite;
    public int max;

    public Sprite spriteImage;

    public void SetData()
    {
        spriteImage = Resources.Load<Sprite>(sprite);
    }
}

[System.Serializable]
public class AllCards
{
    public string category;
    public int atk;
    public int hp;
    public int money = 0;
    public int maxCard = 4;
    public float coolTime;
    public string name;
    public int num;
    public string sprite;

    public Sprite spriteImage;

    public void SetData()
    {
        spriteImage = Resources.Load<Sprite>(sprite);
        name = GameManager._instance.TransName(name, num);
    }

    public void Clone(AllCards _allCards)
    {
        category = _allCards.category;
        atk = _allCards.atk;
        hp = _allCards.hp;
        money = _allCards.money;
        maxCard = _allCards.maxCard;
        coolTime = _allCards.coolTime;
        name = _allCards.name;
        num = _allCards.num;
        sprite = _allCards.sprite;
        spriteImage = _allCards.spriteImage;
    }
}

[System.Serializable]
public class Texts
{
    public string MISSION_NAME_TEXT;
    public string ACHIEVEMENT_NONE_TEXT;
    public string SHOP_NONE_TEXT;
    public string DECK_ADD_TEXT;
    public string DECK_DEL_TEXT;
    public string SET_MAIN_CARD_TEXT;
    public string SHOP_BUY_TEXT;
    public string MY_CARDS_TEXT;

    // 이름
    public string[] STAT_NAME;
    public string[] ITEM_CATEGORY_NAME;
    public string[] PANELS_NAME;
    public string[] SHOP_TITLE;
    public string[] SHOP_ITEM_NAME; // 아이템
    public string[] FRIEND_CARD_NAME; // 아군
    public string[] BOSS_CARD_NAME; // 보스
    public string[] SPECIAL_CARD_NAME; // 특수 카드
    public string[] NPC_CARD_NAME; // NPC 카드
    public string[] FOOD_CARD_NAME; // 음식
    public string[] OTHER_CARD_NAME; // 기타
    public string[] SHIELD_CARD_NAME; // 방패
    public string[] HELMET_CARD_NAME; // 투구
    public string[] WEAPON_CARD_NAME; // 무기
    public string[] ARMOR_CARD_NAME; // 갑옷
    public string[] ENEMY_CARD_NAME; // 적군

    // 임무
    public string[] MISSION_TITLE;
    public string[] MISSION_NAME;

    // 보상
    public string[] DEBUFF_TITLE;
    public string[] DEBUFF_NAME;

    // 결과
    public string[] RESULT_NAME;

    // 업적
    public string[] ACHIEVEMENT_CATEGORY;
    public string[] ACHIEVEMENT_TITLE;
    public string[] ACHIEVEMENT_DETAIL;

    // 시스템 메시지
    public string DIE_MASSEGE;
    public string GAME_OVER_MASSEGE;
    public string GAME_CLEAR_MASSEGE;
    public string LOADING_MASSEGE;
    public string DESTROY_MASSEGE;
    public string START_SHUFFLE_CARD;
    public string START_MISSION_CHOICE;
    public string PASSES_DAY;
    public string DECREASES_HP;
    public string USE_ITEM_MASSEGE;
    public string CHOICE_REWARD;
    public string RE_SHUFFLE_USE_CARD;
    public string BOSS_APPEARS;
    public string NONE_MAIN_CARD_MASSEGE;
    public string DECK_OVER_MASSEGE;
    public string BOOS_ALLIVE_MASSEGE;
    public string SHOP_FAIL_MASSEGE;
}