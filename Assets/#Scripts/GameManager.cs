using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    void Awake()
    {
        _instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }

    [Header("--PUBLIC--")]
    public Camera cam;
    public GameObject map;
    public List<Animator> canvas;

    [Header("--MAIN UI--")]
    public List<Animator> panels;
    public List<Animator> panels_Button;
    public List<Animator> setAndExitPanel;
    public List<Animator> buttons;

    [Header("--ACHIEVEMENT--")]
    public GameObject achievementUI;
    public Text achievementInfo_Title;
    public Image achievementInfo_Image;
    public Text achievementInfo_Detail;

    [Header("--DECK--")]
    public GameObject deckUI;
    public Text deck_Title;
    public Text deckInfo_Title;
    public Image deckInfo_Image;
    public Text deckInfo_Detail;
    public Button deckInfo_Button;
    public Text deckInfo_Button_Text;
    public Button deckInfo_Change_Button;
    public Text deckInfo_Change_Button_Text;

    [Header("--ITEM--")]
    public Text itemInfo_Title;
    public Image itemInfo_Image;
    public Text itemInfo_Detail;
    public Button itemInfo_Button;
    public Text itemInfo_Button_Text;

    [Header("--SHOP--")]
    public Text[] shop_Title;
    public List<Shop_Item> shop_Items;
    public Shop_Item shop_Items_Special;

    [Header("--DEFAULT UI--")]
    public Text myMoney;
    public Text daysText;
    public Image daysPercentImage;
    public Animator defaultUI;

    [Header("--GAME UI--")]
    public Image noticeUI;
    public Text noticeText;
    public Text missionText;
    public RectTransform missionRect;
    public GameObject loadingObj;
    public Animator resultsUI;
    public Text results_Title;
    public Text results_Text;
    public Animator choiceCardUI;
    public Transform choiceCard_Trans;
    public Transform cardInfo_Trans_Right;
    public Transform cardInfo_Trans_Left;

    [Header("--INFO--")]
    public GameObject InfoUI;
    public GameObject[] InfoUI_Panel;
    public List<ChoiceCard> choiceCards;
    public List<CardInfo> cardInfos_Right;
    public List<CardInfo> cardInfos_Left;

    [Header("--PREPEBS--")]
    public GameObject smallText;
    public GameObject choiceCard;
    public GameObject cardInfo;
    public GameObject deckCard;
    public GameObject achievement_Title;
    public GameObject achievement_Mask;
    public GameObject achievement_Item;

    [Header("--CARD DATA--")]
    public CardManager holdingCard;
    public CardManager holdingCard_FCard;
    public List<CardManager> holdingCard_CList;
    public List<CardManager> allCardsList;

#if (UNITY_ANDROID) // 안드로이드인 경우
    private readonly int DEVICE = 1;
#else
    private readonly int DEVICE = 0;
#endif

    private int language = -1;
    private MyData myData = null;
    private Texts texts = null;
    private ItemData items = null;
    private AllCards[] allCards = null;
    private SimpleNum[] missions = null;
    private DebuffList debuffs = null;
    private Matching matching = null;
    private Achievements[] achievements = null;
    private Combines[] combines = null;
    private List<Achievement> activeAchieve = new();
    private List<DeckCard> myDeckCards = new();
    private List<string> removeCardList = new();
    private List<int> debuff_NumList = new();
    private Dictionary<string, int> achieveRecord = new();
    private CardManager cardInfoTarget = null;
    private LayerMask cardLayerMask = new();
    private Vector2 bPos = new();
    private Vector2 mPos = new();
    private Vector2 mapPos = new();
    private int missionNum = -1;
    private int debuffNum = -1;
    private int thisCanvas = 0;
    private int thisPanels = 0;
    private int[] days = new int[3] { 0, 0, 0 };
    private int[] spawnCards = new int[2] { 0, 30 };
    private bool play = false;
    private bool noticeResult = false;
    private bool viewAllMissions = false;
    private bool moveMap = false;

    private const int ZERO = 0;
    private const float MOVE_PERCENT = 0.03f;
    private const string FADE_IN = "Active";
    private const string FADE_OUT = "Inactive";
    private const string PUT = "Put";
    private readonly Vector2 SCREEN_SIZE = new(1920, 1080);
    private readonly Vector2 MAP_SIZE = new(2950 * 0.5f, 1958 * 0.5f);
    private readonly Vector2 CARD_SIZE = new(118.5f, 182.25f);

    private void Start()
    {
        DataManager._instance.MyDataLoad();
        myData = DataManager._instance.GetMyData();
        SetDays(myData.GetDays()[0], myData.GetDays()[1], false);
        SetMoneyText(myData.GetMoney());
        SetLanguage(myData.GetLanguege());
        cardLayerMask = 1 << LayerMask.NameToLayer("Card");
        StartCoroutine(CardInteraction());
        canvas[ZERO].Play(FADE_IN);
        panels[ZERO].Play(FADE_IN);
        panels_Button[ZERO].Play(FADE_IN);
        defaultUI.Play(FADE_IN);
        DataManager._instance.MyDataSave(myData);
    }

    // GET SET
    public Vector2 GetMapSize()
    {
        return MAP_SIZE;
    }

    public Vector2 GetScreenSize()
    {
        return SCREEN_SIZE;
    }

    public Vector2 GetCardSize()
    {
        return CARD_SIZE;
    }

    public bool GetPlay()
    {
        return play;
    }

    public void SetPlay(bool _bool)
    {
        play = _bool;
    }

    private Vector2 GetMapPos()
    {
        return mapPos;
    }
    
    private void SetMapPos(Vector2 _pos)
    {
        mapPos = _pos;
    }

    public void SetDays(int _day, int _percent, bool _bool)
    {
        if (_bool)
        {
            days[0] += _day;
            days[1] += _percent;
        }
        else
        {
            days[0] = _day;
            days[1] = _percent;
        }

        daysText.text = days[0].ToString();
        daysPercentImage.fillAmount = days[1] * 0.01f;
    }

    private CardManager GetInfoTarget()
    {
        return cardInfoTarget;
    }

    private void SetInfoTarget(CardManager _cardManager)
    {
        cardInfoTarget = _cardManager;
    }

    public int GetMissionCount(int _num)
    {
        return GetAchieveRecord(matching.mission[_num].name);
    }

    public int GetResultCount(int _num)
    {
        return GetAchieveRecord(matching.result[_num]);
    }

    public int GetCategoryCount(string _category)
    {
        int _count = 0;

        for (int i = 0; i < achievements.Length; i++)
        {
            if (texts.ACHIEVEMENT_CATEGORY[achievements[i].category] == _category) _count++;
        }

        return _count;
    }

    public void SetAchieveRecord(string _name, int _num, bool _bool) // 업적에 사용될 내역들을 기록한다.
    {
        if (CheckAchieveRecord(_name)) // 이름이 있는 경우
        {
            if (_bool) achieveRecord[_name] += _num;
            else
            {
                if (achieveRecord[_name] < _num) achieveRecord[_name] = _num; // 기존보다 큰 경우 대입한다.
            }
        }
        else achieveRecord.Add(_name, _num); // 이름이 없는 경우
    }

    public int GetAchieveRecord(string _name)
    {
        if (CheckAchieveRecord(_name)) return achieveRecord[_name]; // 이름이 있는 경우
        else return 0; // 이름이 없는 경우
    }

    public string GetDestroyText()
    {
        return texts.DESTROY_MASSEGE;
    }

    public void SetLanguage(int _num) // 언어를 설정한다.
    {
        if (language != _num)
        {
            language = _num;

            DataManager._instance.GameDataLoad();

            texts = DataManager._instance.GetTexts(GetLanguage());
            allCards = DataManager._instance.GetAllCardData();
            missions = DataManager._instance.GetMissionList();
            debuffs = DataManager._instance.GetDebuffList();
            matching = DataManager._instance.GetMatchingData();
            achievements = DataManager._instance.GetAchievementList();
            combines = DataManager._instance.GetConbineData();
            items = DataManager._instance.GetItemData();

            int _length = allCards.Length;

            for (int i = 0; i < _length; i++) allCards[i].SetData();

            _length = combines.Length;

            for (int i = 0; i < _length; i++) combines[i].SetData();

            _length = achievements.Length;

            for (int i = 0; i < _length; i++) achievements[i].SetData();

            _length = items.items.Length;

            for (int i = 0; i < _length; i++) items.items[i].SetData();

            DataManager._instance.RemoveGameData();

            _length = panels_Button.Count;

            for (int i = 0; i < _length; i++) panels_Button[i].GetComponent<PanelButton>().SetText(texts.PANELS_NAME[i]);
        }
    }

    public int GetLanguage()
    {
        return language;
    }

    public void SetMoneyText(int _num)
    {
        myMoney.text = _num.ToString();
    }

    public AllCards GetCardData(int _num)
    {
        return allCards[_num];
    }

    // BUTTON
    public void CanvasChange(int _num)
    {
        HideInfo();

        if (_num == 1)
        {
            StartCoroutine(CanvasChangeWait(_num));
        }
        else StartCoroutine(CanvasChangeWait(_num));
    }

    public void LaunchURL(string _url)
    {
        Application.OpenURL(_url);
    }

    public void PanelsChange(int _num) // 패널을 변경한다.
    {
        HideInfo();

        if (_num != thisPanels)
        {
            panels[thisPanels].Play(FADE_OUT);
            panels[_num].Play(FADE_IN);
            panels_Button[thisPanels].Play(FADE_OUT);
            panels_Button[_num].Play(FADE_IN);
            thisPanels = _num;

            if (thisPanels == 1) CheckAchievement();
            else if (thisPanels == 2) CheckShopItem();
            else if (thisPanels == 3) CheckMyCards();
        }
    }

    public void SetAndExitPanelChange(int _num) // 세팅창 또는 종료창을 열거나 닫는다.
    {
        if (setAndExitPanel[_num].GetComponent<CanvasGroup>().alpha == 1) setAndExitPanel[_num].Play(FADE_OUT);
        else setAndExitPanel[_num].Play(FADE_IN);
    }

    public void PlayButtonAnim(int _num) // 버튼 상호작용
    {
        buttons[_num].Play(FADE_IN);
    }

    public void ExitGame() // 게임을 종료한다.
    {
        Application.Quit();
    }

    public void ChoiceMission(int _num) // 미션을 선택한다.
    {
        missionNum = _num;
    }

    public void ChoiceReward(int _num) // 보상을 선택한다.
    {
        debuffNum = _num;
    }

    public void ToggleMissionsView() // 미션이 보이는 범위를 설정한다.
    {
        viewAllMissions = !viewAllMissions;
        CheckMissionClear();
    }

    public void OpenInfoUI(int _num, float _pos) // 정보창 화면을 전환한다.
    {
        if (!InfoUI_Panel[_num].activeSelf)
        {
            int _length = InfoUI_Panel.Length;

            for (int i = 0; i < _length; i++) InfoUI_Panel[i].SetActive(false);

            InfoUI_Panel[_num].SetActive(true);
        }

        if (_pos > 0)
        {
            RectTransform _rect = InfoUI.GetComponent<RectTransform>();
            Vector2 _vector2 = new(0, 0.5f);

            _rect.pivot = _vector2;
            _rect.anchorMin = _vector2;
            _rect.anchorMax = _vector2;
            InfoUI.transform.localPosition = new(100 - GetScreenSize().x / 2, 0);
        }
        else
        {
            RectTransform _rect = InfoUI.GetComponent<RectTransform>();
            Vector2 _vector2 = new(1, 0.5f);

            _rect.pivot = _vector2;
            _rect.anchorMin = _vector2;
            _rect.anchorMax = _vector2;
            InfoUI.transform.localPosition = new(-100 + GetScreenSize().x / 2, 0);
        }

        InfoUI.SetActive(true);
    }

    public void HideInfo() // 정보창을 닫는다.
    {
        InfoUI.SetActive(false);
    }

    // CHECK
    private Vector2 CheckMoveTrue(Vector2 _pos) // 맵을 이동할수 있는지 확인한다.
    {
        Vector2 _mapSize = GetMapSize();
        Vector2 _screenSize = new(GetScreenSize().x * 0.5f, GetScreenSize().y * 0.5f);

        if (_pos.x > _mapSize.x - _screenSize.x) _pos = new(_mapSize.x - _screenSize.x, _pos.y);

        if (_pos.x < -(_mapSize.x - _screenSize.x)) _pos = new(-(_mapSize.x - _screenSize.x), _pos.y);

        if (_pos.y > _mapSize.y - _screenSize.y) _pos = new(_pos.x, _mapSize.y - _screenSize.y);

        if (_pos.y < -(_mapSize.y - _screenSize.y)) _pos = new(_pos.x, -(_mapSize.y - _screenSize.y));

        if (_mapSize.x <= _screenSize.x ) _pos.x = 0;

        if (_mapSize.y <= _screenSize.y) _pos.y = 0;

        return _pos;
    }

    private CardManager GetHitCard(RaycastHit2D[] _hit) // 클릭한 카드들의 우선순위를 구분하여 반환한다.
    {
        GameObject _hitCard = null; // 클릭한 카드를 기록한다.
        int _sibNum = -1; // 카드의 순서를 기록한다.
        int _length = _hit.Length;

        for (int i = 0; i < _length; i++) // 클릭한 카드를 전부 확인한다.
        {
            if (_hit[i].transform.GetSiblingIndex() > _sibNum) // 클릭한 카드의 순서가 더 앞인경우
            {
                if (holdingCard == null)
                {
                    _sibNum = _hit[i].transform.GetSiblingIndex(); // 카드의 순서를 기록한다.
                    _hitCard = _hit[i].transform.gameObject; // 클릭한 카드로 간주한다.
                }
                else if (_hit[i].transform.gameObject != holdingCard.gameObject) // 클릭한 카드가 잡고있던 카드가 아닌 경우
                {
                    if (holdingCard_CList.Count > 0)
                    {
                        if (!holdingCard_CList.Contains(_hit[i].transform.GetComponent<CardManager>()))
                        {
                            _sibNum = _hit[i].transform.GetSiblingIndex(); // 카드의 순서를 기록한다.
                            _hitCard = _hit[i].transform.gameObject; // 클릭한 카드로 간주한다.
                        }
                    }
                    else
                    {
                        _sibNum = _hit[i].transform.GetSiblingIndex(); // 카드의 순서를 기록한다.
                        _hitCard = _hit[i].transform.gameObject; // 클릭한 카드로 간주한다.
                    }
                }
            }
        }

        if (_hitCard != null) return _hitCard.transform.GetComponent<CardManager>(); // 카드를 반환한다.
        else return null;
    }

    private CardManager GetHitCard(Vector2 _pos) // 잡고있는 카드에 닿아있는 카드를 반환한다.
    {
        CardManager _hitCard = null; // 카드를 기록한다.
        CardManager _card; // 카드를 기록한다.
        Vector2 _size = new(GetCardSize().x / 2 - 5, GetCardSize().y / 2 - 5);
        Vector2[] _nearPos = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(_size.x, _size.y),
            new Vector2(-_size.x, _size.y),
            new Vector2(-_size.x, -_size.y),
            new Vector2(_size.x, -_size.y)
        };

        for (int i = 0; i < 5; i++) // 주변을 전부 확인한다.
        {
            RaycastHit2D[] _hit = Physics2D.RaycastAll(_pos + _nearPos[i], Vector2.zero, 1, cardLayerMask);

            if (_hitCard == null) _hitCard = GetHitCard(_hit);
            else
            {
                _card = GetHitCard(_hit);

                if (_card != null)
                {
                    if (Vector2.Distance(_hitCard.GetTargetPos(), _pos) > Vector2.Distance(_card.GetTargetPos(), _pos)) _hitCard = _card;
                }
            }
        }

        return _hitCard; // 카드를 반환한다.
    }

    private bool CheckMissionClear() // 미션 클리어 여부를 확인한다.
    {
        bool _success = true;
        int _nowCondition = 0, _allCondition = 0, _length;

        if (missionNum == -1) return false;

        if (viewAllMissions)
        {
            missionText.text = "";

            _length = missions[missionNum].num.Length;

            for (int i = 0; i < _length; i++)
            {
                int _num = missions[missionNum].num[i];

                if (matching.mission[_num].max > 0) // 진행중인 미션인 경우
                {
                    _allCondition = matching.mission[_num].max;

                    if (matching.mission[_num].max >= GetMissionCount(_num)) _nowCondition = GetMissionCount(_num);
                    else _nowCondition = matching.mission[_num].max;

                    if (missionText.text == "") missionText.text = texts.MISSION_NAME[_num] + " ( " + _nowCondition + " / " +_allCondition + " )";
                    else missionText.text += "\n" + texts.MISSION_NAME[_num] + " ( " + _nowCondition + " / " + _allCondition + " )";

                    if (_nowCondition < _allCondition) _success = false;
                }
            }
        }
        else
        {
            _length = missions[missionNum].num.Length;

            for (int i = 0; i < _length; i++)
            {
                int _num = missions[missionNum].num[i];

                if (matching.mission[_num].max > 0) // 진행중인 미션인 경우
                {
                    _allCondition += matching.mission[_num].max;

                    if (matching.mission[_num].max > GetMissionCount(_num)) _nowCondition += GetMissionCount(_num);
                    else _nowCondition += matching.mission[_num].max;
                }
            }

            missionText.text = texts.MISSION_TITLE[missionNum] + " ( " + (int)((float)_nowCondition / _allCondition * 100) + "% )";

            if (_nowCondition < _allCondition) _success = false;
        }
        
        missionRect.sizeDelta = new(missionText.preferredWidth + 40, missionRect.sizeDelta.y);
        missionRect.sizeDelta = new(missionRect.sizeDelta.x, missionText.preferredHeight + 27);

        return _success;
    }

    private bool CheckMyItems(string _name) // 아이템을 보유중인지 확인한다.
    {
        string[] _items = myData.GetItem();
        int _length = _items.Length;

        for (int i = 0; i < _length; i++)
        {
            if (_items[i] == _name)
            {
                StartCoroutine(Notice(texts.USE_ITEM_MASSEGE));
                myData.SetItem(i, null);
                DataManager._instance.MyDataSave(myData);
                SetAchieveRecord(_name, 1, true);
                return true;
            }
        }

        return false;
    }

    // IENUMERATOR
    private IEnumerator CanvasChangeWait(int _num)
    {
        if (_num != thisCanvas)
        {
            if (thisCanvas == 0) panels[thisPanels].Play(FADE_OUT);
            else if (thisCanvas == 1)
            {
                resultsUI.Play(FADE_OUT);
                PanelsChange(0);

                yield return new WaitUntil(() => resultsUI.GetCurrentAnimatorStateInfo(0).IsName("Inactive"));
                yield return new WaitUntil(() => resultsUI.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f); // 결과창이 사라질때 까지 기다린다.
            }

            canvas[thisCanvas].Play(FADE_OUT);
            canvas[_num].Play(FADE_IN);
            thisCanvas = _num;

            if (thisCanvas == 1)
            {
                StartCoroutine(MoveMap());
                StartCoroutine(StartGame());
            }
        }

        yield return null;
    }

    private IEnumerator CardInteraction() // 카드를 상호작용하는 경우 진행
    {
        while (true)
        {
            if (GetPlay() && !noticeResult) // 게임을 진행중인 경우에만 실행한다.
            {
                int _tCount;
                Vector3 _posType = Input.mousePosition;

                if (DEVICE == 0) // 데스크탑인 경우
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (Input.GetMouseButtonDown(1)) _tCount = 2;
                        else _tCount = 1;
                    }
                    else _tCount = 0;
                }
                else // 안드로이드인 경우
                {
                    _tCount = Input.touchCount;

                    if (_tCount > 0) _posType = Input.GetTouch(0).position;
                }

                if (CheckMissionClear()) EndGame(true);

                if (moveMap) SetMapPos(-(cam.ScreenToWorldPoint(_posType) - map.transform.localPosition));

                SetCardInfo(true); // 카드 정보를 출력한다.

                if (holdingCard != null) // 카드를 잡고있는 경우
                {
                    holdingCard.SetTargetPos(cam.ScreenToWorldPoint(_posType) - map.transform.localPosition); // 손 위치로 타겟을 설정 해준다.
                    mPos = holdingCard.CheckMoveTrue(holdingCard.GetTargetPos());
                    CardManager _hitCard = GetHitCard(cam.ScreenToWorldPoint(_posType)); // 손 위치 주변의 카드를 기록한다.

                    if (_tCount > 0) // 카드를 이동중인 경우
                    {
                        if (_hitCard != null) // 손 위치에 자신이 아닌 카드가 있는 경우
                        {
                            CardManager _firstCard = _hitCard.GetFirstCard() ?? _hitCard; // 카드의 처음 카드를 가져온다.

                            UpdateDays(days[2] + (int)(Vector2.Distance(bPos, _firstCard.GetTargetPos()) * MOVE_PERCENT), false); // 자신이 아이템이 아닌 경우 날짜의 변경을 보여준다.
                        }
                        else UpdateDays(days[2] + (int)(Vector2.Distance(bPos, mPos) * MOVE_PERCENT), false); // 날짜의 변경을 보여준다.
                    }

                    if (_tCount == 0) // 손을 뗀 경우
                    {
                        UpdateDays(days[1], true); // 날짜를 변경한다.
                        CardManager _holdingCard = holdingCard;
                        CardManager _holdingCard_FCard = holdingCard_FCard;
                        List<CardManager> _holdingCard_CList = holdingCard_CList;
                        Vector2 _bPos = bPos;
                        Vector2 _mPos = mPos;
                        ResetHoldCardData(); // 잡고있었던 카드의 정보를 초기화 한다.

                        yield return new WaitUntil(() => GetPlay()); // 게임이 진행될 때까지 대기한다.

                        if (_hitCard != null) // 자신을 클릭하지 않은 경우
                        {
                            CardManager _firstCard = _hitCard.GetFirstCard(); // 클릭한 카드의 처음 카드를 가져온다.

                            if (_firstCard == null) _firstCard = _hitCard; // 클릭한 카드의 처음 카드가 없다면

                            _holdingCard.SetTargetPos(_firstCard.GetTargetPos());

                            yield return new WaitUntil(() => !_holdingCard.GetMove()); // 이동이 끝날때 까지 대기한다.

                            switch (_firstCard.name)
                            {
                                case "Friend":
                                    TradeCardList(_firstCard, _holdingCard);
                                    break;

                                case "Food":
                                    if (_holdingCard.name == "Friend") // 아군 카드인 경우
                                    {
                                        List<CardManager> _cardList = _firstCard.GetCardList();
                                        int _length = _cardList.Count;

                                        for (int i = 0; i < _length; i++) _cardList[i].SetTargetPos(_firstCard.GetTargetPos()); // 중앙으로 이동 시킨다.

                                        yield return new WaitUntil(() => !_firstCard.GetMove()); // 이동이 끝날때 까지 대기한다.

                                        for (int i = 0; i < _length; i++) // 카드 목록을 확인한다.
                                        {
                                            if (_cardList[i].name == "Food") // 음식인 경우 먹는다.
                                            {
                                                SetAchieveRecord("Eat Food", 1, true);
                                                _holdingCard.SetHp(_cardList[i].GetHp(), true); // 메인 카드의 체력을 증가시킨다.
                                                _holdingCard.SetAtk(_cardList[i].GetAtk(), true); // 메인 카드의 체력을 증가시킨다.
                                                RemoveCard(_cardList[i], true); // 음식 카드를 지운다.
                                                i--;
                                                _length--;
                                            }
                                            else _firstCard.SetCardList(i, true); // 카드 목록에서 지운다.
                                        }

                                        SetAchieveRecord("Eat Food", 1, true);
                                        _holdingCard.SetHp(_firstCard.GetHp(), true); // 메인 카드의 체력을 증가시킨다.
                                        _holdingCard.SetAtk(_firstCard.GetAtk(), true); // 메인 카드의 체력을 증가시킨다.
                                        RemoveCard(_firstCard, true); // 음식 카드를 지운다.
                                    }
                                    else TradeCardList(_holdingCard, _firstCard);
                                    break;

                                case "Item": // 클릭한 카드가 아이템인 경우
                                    TradeCardList(_firstCard, _holdingCard);

                                    while (true) // 아이템 조합을 실행한다.
                                    {
                                        string _combineName = null;
                                        List<CardManager> _checkCardList = new();

                                        for (int i = 0; i < combines.Length; i++) // 조합식 목록을 확인한다.
                                        {
                                            List<string> _combineNameList = new();
                                            int _length = combines[i].use.Length;

                                            for (int j = 0; j < _length; j++) _combineNameList.Add(combines[i].use[j]); // 조합식 목록을 불러온다.

                                            if (_combineNameList.Contains(_holdingCard.GetName())) // 처음 카드가 조합 목록에 있는 경우
                                            {
                                                _checkCardList.Add(_holdingCard);
                                                _combineNameList.RemoveAt(_combineNameList.IndexOf(_holdingCard.GetName()));
                                            }

                                            for (int j = 0; j < _holdingCard_CList.Count; j++) // 카드 목록을 확인한다.
                                            {
                                                _length = _combineNameList.Count;

                                                for (int k = 0; k < _length; k++) // 조합식을 확인한다.
                                                {
                                                    if (_combineNameList[k] == _holdingCard_CList[j].GetName()) // 조합에 필요한 아이템인 경우
                                                    {
                                                        _checkCardList.Add(_holdingCard_CList[j]);
                                                        _combineNameList.RemoveAt(k);

                                                        break;
                                                    }
                                                }

                                                if (_combineNameList.Count == 0) break; // 조합이 가능한 경우
                                            }

                                            if (_combineNameList.Count == 0) // 조합이 가능한 경우
                                            {
                                                _combineName = combines[i].result;

                                                break;
                                            }
                                            else _checkCardList.Clear(); // 조합이 불가능한 경우
                                        }

                                        if (_combineName != null) // 조합이 가능한 경우
                                        {
                                            yield return new WaitForSecondsRealtime(0.2f);
                                            yield return new WaitUntil(() => !_holdingCard.GetMove()); // 이동이 끝날때 까지 대기한다.

                                            bool _comThisCard = false;
                                            int _length = _checkCardList.Count;

                                            for (int i = 0; i < _length; i++) // 조합에 사용할 카드의 목록을 확인한다.
                                            {
                                                if (_checkCardList[i] != _holdingCard) // 조합할 카드가 자신이 아닌 경우
                                                {
                                                    _checkCardList[i].SetFirstCard(null);
                                                    _holdingCard_CList.Remove(_checkCardList[i]); // 카드 목록에서 지운다.
                                                    _checkCardList[i].SetSafeMove(true);
                                                    _checkCardList[i].SetTargetPos(_holdingCard.GetTargetPos()); // 처음 카드에게 이동 시킨다.
                                                }
                                                else _comThisCard = true; // 조합할 카드가 자신인 경우
                                            }

                                            _length = _checkCardList.Count;

                                            for (int i = 0; i < _length; i++) yield return new WaitUntil(() => !_checkCardList[i].GetMove()); // 이동이 끝날때 까지 대기한다.

                                            for (int i = 0; i < allCards.Length; i++) // 조합 결과물 목록을 확인한다.
                                            {
                                                if (allCards[i].name == _combineName) // 조합 결과물을 찾은 경우
                                                {
                                                    _length = _checkCardList.Count;
                                                    AllCards _allCards = new();
                                                    _allCards.Clone(allCards[i]);
                                                    _allCards.hp = 0;
                                                    _allCards.atk = 0;

                                                    if (!_comThisCard) // 자신이 조합할 카드가 아닌 경우
                                                    {
                                                        for (int j = 0; j < _length - 1; j++)
                                                        {
                                                            _allCards.hp += _checkCardList[j].GetHp();
                                                            _allCards.atk += _checkCardList[j].GetAtk();
                                                            RemoveCard(_checkCardList[j], false); // 조합에 사용할 카드의 목록을 확인한다.
                                                        }

                                                        _allCards.hp += _checkCardList[^1].GetHp() + 1;
                                                        _allCards.atk += _checkCardList[^1].GetAtk() + 1;
                                                        SetCardData(_checkCardList[^1], _allCards); // 조합에 사용할 카드의 마지막 카드를 결과물로 바꾼다.
                                                        _holdingCard.SetCardList(_checkCardList[^1]); // 카드 목록에 추가한다.
                                                        _checkCardList[^1].SetFirstCard(_holdingCard);
                                                    }
                                                    else // 자신이 조합할 카드인 경우
                                                    {
                                                        for (int j = 0; j < _length; j++)
                                                        {
                                                            if (_checkCardList[j] != _holdingCard)
                                                            {
                                                                _allCards.hp += _checkCardList[j].GetHp();
                                                                _allCards.atk += _checkCardList[j].GetAtk();
                                                                RemoveCard(_checkCardList[j], false); // 조합에 사용할 카드의 목록을 확인한다.
                                                            }
                                                        }

                                                        _allCards.hp += _holdingCard.GetHp() + 1;
                                                        _allCards.atk += _holdingCard.GetAtk() + 1;
                                                        SetCardData(_holdingCard, _allCards); // 카드를 결과물로 바꾼다.
                                                    }

                                                    SetAchieveRecord("Combine Count", 1, true);
                                                    break;
                                                }
                                            }
                                        }
                                        else break; // 더이상 조합이 불가능한 경우
                                    }
                                    break;

                                case "Special": // 특수 카드인 경우
                                    string _cardName = _firstCard.GetName();

                                    if (_cardName == texts.SPECIAL_CARD_NAME[0]) // 셔플 카드인 경우
                                    {
                                        TradeCardList(_holdingCard, _firstCard);
                                        List<CardManager> _cardList = _firstCard.GetCardList();
                                        int _length = _cardList.Count;

                                        for (int i = 0; i < _length; i++) _cardList[i].SetTargetPos(_firstCard.GetTargetPos()); // 중앙으로 이동 시킨다.

                                        yield return new WaitUntil(() => !_firstCard.GetMove()); // 이동이 끝날때 까지 대기한다.

                                        while (0 < _cardList.Count) // 카드 목록을 확인한다.
                                        {
                                            if (_firstCard.GetHp() > 0) // 셔플 카드의 채력이 남아있는 경우
                                            {
                                                SetCardData(_cardList[^1], allCards[RandomCardNum("Boss", false)]);
                                                _firstCard.SetCardList(_cardList.Count - 1, true); // 카드를 버린다.
                                                _firstCard.SetHp(-1, true); // 셔플 카드의 채력을 감소시킨다.
                                            }
                                        }
                                    }
                                    break;

                                case "Enemy":
                                case "Boss": // 적군 카드인 경우
                                    if (_holdingCard.name == "Friend") // 아군 카드인 경우에만 실행
                                    {
                                        for (int i = 0; i < 4; i++) // 전투 가능한 위치를 확인한다.
                                        {
                                            for (int j = 0; j < 4; j++)
                                            {
                                                if (_firstCard.BattlePosCheck(i, j, false) && _holdingCard.BattlePosCheck(i, j, true)) // 카드배치가 가능한 경우
                                                {
                                                    _firstCard.StartBattle(_holdingCard, 4);
                                                    _holdingCard.StartBattle(_firstCard, i);
                                                    int _length = _firstCard.GetCardList().Count;

                                                    for (int k = 0; k < _length; k++)
                                                    {
                                                        _firstCard.GetCardList()[k].StartBattle(null, j);
                                                        _firstCard.GetCardList()[k].transform.SetAsLastSibling(); // 가장 위에 표시되게 설정한다.
                                                    }

                                                    _length = _holdingCard_CList.Count;

                                                    for (int k = 0; k < _length; k++)
                                                    {
                                                        _holdingCard_CList[k].StartBattle(null, j);
                                                        _holdingCard_CList[k].transform.SetAsLastSibling(); // 가장 위에 표시되게 설정한다.
                                                    }

                                                    i = 4; // 반복문 종료
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else // 아군 카드가 아닌 경우
                                    {
                                        while (0 < _holdingCard.GetCardList().Count) _holdingCard.SetCardList(_holdingCard.GetCardList().Count - 1, true); // 카드 목록에서 지운다.

                                        _holdingCard.SetTargetPos(_holdingCard.GetTargetPos() + (Vector2)RandomAroundPos()); // 자신을 랜덤한 방향으로 이동시킨다.
                                    }
                                    break;

                                case "NPC":
                                    TradeCardList(_firstCard, _holdingCard);
                                    break;
                            }
                        }
                    }
                    else if (_tCount == 2) // 우클릭한 경우
                    {
                        UpdateDays(days[2], false); // 날짜의 변경을 원래상태로 돌린다.

                        if (holdingCard_FCard != null) TradeCardList(holdingCard, holdingCard_FCard); // 처음의 카드가 있었던 경우
                        else holdingCard.SetTargetPos(bPos); // 처음의 카드가 없었던 경우 처음 클릭 했을때의 위치로 타겟을 설정 해준다.

                        ResetHoldCardData(); // 잡고있었던 카드의 정보를 초기화 한다.
                        SetInfoTarget(null); // 카드 정보를 변경한다.
                    }
                }
                else // 카드를 잡고있지 않는 경우
                {
                    if (_tCount == 1 && moveMap == false) // 맵을 이동중이지 않을 때 좌클릭한 경우
                    {
                        CardManager _hitCard = GetHitCard(Physics2D.RaycastAll(cam.ScreenToWorldPoint(_posType), Vector2.zero, 1, cardLayerMask)); // 자신이 클릭한 카드를 기록한다.

                        if (_hitCard != null) // 오브젝트가 있는 경우
                        {
                            moveMap = false;
                            holdingCard = _hitCard; // 잡은 카드의 CardManager를 기록한다.
                            holdingCard_FCard = holdingCard.GetFirstCard(); // 처음의 카드를 가져온다.
                            holdingCard_CList = holdingCard.GetCardList(); // 자신이 가지고 있는 카드 목록을 가져온다.
                            bPos = holdingCard.GetTargetPos(); // 처음 클릭한 위치를 기록한다.
                            SetInfoTarget(holdingCard); // 카드 정보를 변경한다.

                            if (holdingCard.GetMove()) ResetHoldCardData(); // 이동중인 경우
                            else if (holdingCard.GetEnemy() != null) ResetHoldCardData(); // 전투중인 경우 카드를 잡을 수 없다.
                            else _hitCard.transform.SetAsLastSibling(); // 클릭한 카드를 가장 위에 표시한다.

                            if (holdingCard != null) // 움직일 수 있는 카드인 경우
                            {
                                if (holdingCard_FCard != null) // 처음의 카드가 있는 경우
                                {
                                    List<CardManager> _fcardList = holdingCard_FCard.GetCardList(); // 처음 카드가 가지고있는 카드 목록을 가져온다.
                                    bPos = holdingCard_FCard.GetTargetPos(); // 처음 클릭한 위치를 처음카드의 위치로 기록한다.
                                    holdingCard.SetFirstCard(null); // 처음의 타겟을 초기화 해준다.

                                    for (int i = 0; i < _fcardList.Count; i++) // 오브젝트를 전부 확인한다.
                                    {
                                        if (_fcardList[i] == _hitCard) // 오브젝트가 자신이라면
                                        {
                                            holdingCard_FCard.SetCardList(i, false); // 자신을 처음카드의 카드 목록에서 제거한다.

                                            while (i < _fcardList.Count) // 자신 이후의 오브젝트를 전부 확인한다.
                                            {
                                                holdingCard.SetCardList(_fcardList[i]); // 자신의 카드 목록에 추가한다.
                                                holdingCard_FCard.SetCardList(i, false); // 처음카드의 카드 목록에서 제거한다.
                                            }
                                        }
                                    }
                                }

                                int _length = holdingCard_CList.Count;

                                for (int i = 0; i < _length; i++) // 카드 목록을 전부 확인한다.
                                {
                                    holdingCard_CList[i].SetFirstCard(_hitCard); // 처음 카드를 설정한다.
                                    holdingCard_CList[i].transform.SetAsLastSibling(); // 카드를 가장 위에 표시한다.
                                }

                                days[2] = days[1]; // 잡았을때의 daysPercent를 기록한다.
                            }
                        }
                        else // 오브젝트가 없는 경우
                        {
                            SetInfoTarget(null); // 카드 정보를 변경한다.
                            moveMap = true; // 맵을 이동시킨다.
                        }
                    }
                    else if (_tCount == 0) // 손을 뗀 경우
                    {
                        moveMap = false; // 맵 이동을 멈춘다.
                    }
                }
            }

            yield return null;
        }
    }

    private IEnumerator MoveMap() // 맵을 이동시킨다.
    {
        int _mapSpeed = 6;
        float _dist;
    
        while (thisCanvas == 1)
        {
            SetMapPos(CheckMoveTrue(GetMapPos()));
            _dist = Vector2.Distance(map.transform.localPosition, GetMapPos());
    
            if (holdingCard != null) _mapSpeed = 2;
    
            if (_dist > 1) // 타겟의 위치와 현재 위치가 멀리있는 경우
            {
                map.transform.localPosition = Vector2.Lerp(map.transform.localPosition, GetMapPos(), _mapSpeed * Time.deltaTime); // 타겟의 위치로 이동한다.
            }
            else if (_dist > 0) // 타겟의 위치와 가까워진 경우
            {
                map.transform.localPosition = GetMapPos(); // 타겟의 위치로 바꾼다.
                _mapSpeed = 6;
            }
    
            yield return null;
        }
    }

    private IEnumerator ResultGame(bool _win) // 결과창을 출력한다.
    {
        yield return new WaitUntil(() => GetPlay()); // 게임이 진행중일 때까지 기다린다.

        missionText.GetComponent<Animator>().Play(FADE_OUT);
        int _length = allCardsList.Count;

        for (int i = 0; i < _length; i++)
        {
            if (!allCardsList[i].gameObject.activeSelf) yield return new WaitUntil(() => !allCardsList[i].GetMove());
        }

        SetPlay(false);
        SetInfoTarget(null); // 카드 정보를 변경한다.
        SetCardInfo(true); // 카드 정보를 정리한다.
        SetCardInfo(false); // 카드 정보를 정리한다.

        results_Text.text = texts.MISSION_NAME_TEXT + " : " + texts.MISSION_TITLE[missionNum];

        _length = texts.RESULT_NAME.Length;

        for (int i = 0; i < _length; i++) results_Text.text += "\n" + texts.RESULT_NAME[i] + " : " + GetResultCount(i);

        if (_win) // 이겼을 경우
        {
            StartCoroutine(Notice(texts.GAME_CLEAR_MASSEGE));
            results_Title.text = texts.GAME_CLEAR_MASSEGE;
            myData.SetMoney(myData.GetMoney(), false);
            SetMoneyText(myData.GetMoney());
            myData.SetDays(days[0], days[1]);
            myData.SetBossCount(GetAchieveRecord("Kill Boss"), true);

            // List<CardManager> _cardManager = DataManager._instance.mainCard.GetCardList();
            // _length = _cardManager.Count;
            // 
            // for (int i = 0; i < _length; i++)
            // {
            //     string _name = _cardManager[i].GetName();
            // 
            //     if (_name == texts.NPC_CARD_NAME[0])
            //     {
            //         myData.SetShopItemNum(6, Random.Range(0, items.items.Length)); // 특수 상점 항목을 추가한다.
            //         myData.SetShopItemMoney(6, Random.Range(30, 51)); // 아이템 금액을 설정한다.
            //         SetAchieveRecord("Contect NPC", 1, true); // 상인 카드를 소지중인 경우
            //         removeCardList.Add(_name);
            //         break;
            //     }
            // }

            _length = debuff_NumList.Count;

            for (int i = 0; i < _length; i++) // 디버프 목록을 정산한다.
            {
                object[] _obj = new object[2] { debuffs.data[i].value, debuffs.data[i].increase };
                SetFieldValue(myData, "Set" + debuffs.data[i].type, _obj);
            }

            for (int i = 0; i < removeCardList.Count; i++) // 파괴된 카드들을 확인한다.
            {
                MyCard[] _myCards = myData.GetCards();
                _length = _myCards.Length;

                for (int j = 0; j < _length; j++) // 카드 목록에서 제거
                {
                    if (allCards[_myCards[j].card].name == removeCardList[i] && _myCards[j].deck)
                    {
                        myData.SetCards(j, -1);
                        break;
                    }
                }
            }

            SetAchieveRecord("Win", 1, true);
        }
        else // 졌을 경우
        {
            StartCoroutine(Notice(texts.GAME_OVER_MASSEGE));
            results_Title.text = texts.GAME_OVER_MASSEGE;
            SetDays(myData.GetDays()[0], myData.GetDays()[1], false);
            achieveRecord.Clear(); // 패배한 경우 업적을 초기화 한다.
            SetAchieveRecord("Lose", 1, true);
        }

        _length = achievements.Length;

        for (int i = 0; i < _length; i++) // 달성한 업적을 기록한다.
        {
            int _count = GetAchieveRecord(achievements[i].name);

            if (_count > 0) myData.SetAchievement(achievements[i].name, _count);
        }

        _length = matching.mission.Length;

        for (int i = 0; i < _length; i++) matching.mission[i].max = ZERO;

        _length = allCardsList.Count;

        for (int i = 0; i < _length; i++) RemoveCard(allCardsList[i], false); // 카드를 정리한다.

        _length = choiceCards.Count;

        for (int i = 0; i < _length; i++) choiceCards[i].button.onClick.RemoveAllListeners();

        spawnCards[ZERO] = ZERO;
        myData.SetShopItemNum(6, -1); // 특수 상점을 초기화한다.
        SetMapPos(new(ZERO, ZERO));
        resultsUI.Play(FADE_IN);
        missionNum = -1;
        viewAllMissions = false;
        noticeResult = false;
        achieveRecord.Clear();
        removeCardList.Clear();
        debuff_NumList.Clear();
        DataManager._instance.MyDataSave(myData);

        yield break;
    }

    private IEnumerator StartGame() // 게임 시작시 카드를 배치한다.
    {
        yield return new WaitForSecondsRealtime(0.5f);

        StartCoroutine(Notice(texts.START_MISSION_CHOICE));
        choiceCardUI.Play(FADE_IN);
        List<int> _randNum = RandNum(0, texts.MISSION_TITLE.Length, choiceCards.Count);
        int _length;

        for (int i = 0; i < choiceCards.Count; i++) // 미션 카드들을 랜덤으로 보여준다.
        {
            choiceCards[i].button.onClick.RemoveAllListeners();
            choiceCards[i].title.text = texts.MISSION_TITLE[_randNum[i]];
            _length = missions[_randNum[i]].num.Length;

            for (int j = 0; j < _length; j++)
            {
                if (j == 0) choiceCards[i].info.text = texts.MISSION_NAME[missions[_randNum[i]].num[j]];
                else choiceCards[i].info.text += "\n" + texts.MISSION_NAME[missions[_randNum[i]].num[j]];
            }
            int _num = _randNum[i];

            choiceCards[i].button.onClick.AddListener(() => ChoiceMission(_num));
        }

        yield return new WaitUntil(() => missionNum != -1); // 미션을 선택할 때까지 대기한다.

        choiceCardUI.Play(FADE_OUT);
        SetMission();
        missionText.GetComponent<Animator>().Play(FADE_IN);
        StartCoroutine(Notice(texts.START_SHUFFLE_CARD));

        MyCard[] _myCards = myData.GetCards();
        _length = _myCards.Length;
        _randNum = RandNum(0, _length, _length);

        for (int i = 0; i < _length; i++) // 나의 카드 목록을 확인한다.
        {
            yield return new WaitForSecondsRealtime(0.15f);

            MyCard _card = _myCards[_randNum[i]];

            if (_card.deck) SpawnCard(_card.card, spawnCards[0]++); // 덱에 있는 카드인 경우

            if (spawnCards[0] == spawnCards[1]) break;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (days[0] / 5 > myData.GetBossCount())
        {
            StartCoroutine(Notice(texts.BOSS_APPEARS));
            SpawnCard(RandomCardNum("Boss", true), spawnCards[1]);
        }

        SetPlay(true);

        yield break;
    }

    private IEnumerator NextDays() // 하루가 넘어갈때 마다 실행한다.
    {
        SetPlay(false); // 진행중 조작하지 못하게 한다.

        while (days[1] >= 100)
        {
            SetDays(1, -100, true);
            StartCoroutine(Notice(texts.PASSES_DAY));

            yield return new WaitUntil(() => Vector2.Distance(map.transform.localPosition, GetMapPos()) == 0); // 이동이 끝날때 까지 대기한다.

            StartCoroutine(Notice(texts.CHOICE_REWARD));
            choiceCardUI.Play(FADE_IN);
            List<int> _randNum = RandNum(0, texts.DEBUFF_TITLE.Length, choiceCards.Count);

            for (int i = 0; i < choiceCards.Count; i++) // 미션 카드들을 랜덤으로 보여준다.
            {
                choiceCards[i].button.onClick.RemoveAllListeners();
                choiceCards[i].title.text = texts.DEBUFF_TITLE[_randNum[i]];

                int _length = debuffs.title[_randNum[i]].num.Length;

                for (int j = 0; j < _length; j++)
                {
                    if (j == 0) choiceCards[i].info.text = texts.DEBUFF_NAME[debuffs.title[_randNum[i]].num[j]];
                    else choiceCards[i].info.text += "\n" + texts.DEBUFF_NAME[debuffs.title[_randNum[i]].num[j]];
                }

                int _num = _randNum[i];

                choiceCards[i].button.onClick.AddListener(() => ChoiceReward(_num));
            }

            yield return new WaitUntil(() => debuffNum != -1); // 보상을 선택할 때까지 대기한다.

            choiceCardUI.Play(FADE_OUT);
            SetDebuff();

            if (days[0] / 5 > myData.GetBossCount()) // 5일차 마다 보스를 소환한다.
            {
                int _length = allCardsList.Count;
                bool _boss = false;

                for (int i = 0; i < _length; i++) // 카드 데이터를 확인한다.
                {
                    if (allCardsList[i].name == "Boss" && allCardsList[i].gameObject.activeSelf) // 보스가 살아있는 경우
                    {
                        if (days[0] / 5 > myData.GetBossCount() + 1) // 5일이 지난 경우 패배한다.
                        {
                            StartCoroutine(Notice(texts.BOOS_ALLIVE_MASSEGE));
                            EndGame(false);
                        }

                        _boss = true;
                    }
                }

                yield return new WaitForSecondsRealtime(0.2f);

                if (!_boss) // 보스가 없는 경우 소환한다.
                {
                    StartCoroutine(Notice(texts.BOSS_APPEARS));
                    SpawnCard(RandomCardNum("Boss", true), spawnCards[1]);
                }
            }
        }

        SetPlay(true);

        yield break;
    }

    private IEnumerator Notice(string _str) // 안내 창을 출력한다.
    {
        noticeText.text = _str;
        noticeUI.color = new(noticeUI.color.r, noticeUI.color.g, noticeUI.color.b, 0.7f);
        noticeText.color = new(noticeText.color.r, noticeText.color.g, noticeText.color.b, 2);

        if (noticeUI.gameObject.activeSelf) yield break;

        noticeUI.gameObject.SetActive(true);

        while (noticeUI.color.a > 0)
        {
            noticeText.color = new(noticeText.color.r, noticeText.color.g, noticeText.color.b, noticeText.color.a - Time.deltaTime);

            if (noticeText.color.a < 1) noticeUI.color = new(noticeUI.color.r, noticeUI.color.g, noticeUI.color.b, noticeUI.color.a - Time.deltaTime);

            yield return null;
        }

        noticeUI.gameObject.SetActive(false);
    }

    // RANDOM
    private List<int> RandNum(int _min, int _max, int _length) // 중복되지 않는 랜덤한 수를 배열로 반환한다.
    {
        List<int> _rand = new();

        while (_rand.Count < _length)
        {
            int _num = Random.Range(_min, _max);

            if (!_rand.Contains(_num)) _rand.Add(_num);
        }

        return _rand;
    }

    private Vector2 RandomMapPos() // 맵에서의 랜덤한 좌표를 반환한다.
    {
        return new(Random.Range(-GetMapSize().x, GetMapSize().x), Random.Range(-GetMapSize().y, GetMapSize().y));
    }

    public Vector3 RandomAroundPos() // 주변으로의 랜덤한 좌표를 반환한다.
    {
        Quaternion _rot;
        Vector3 _dir, _rotDir;

        _rot = Quaternion.Euler(0f, 0f, Random.Range(0, 360));  // 회전각
        _dir = Vector3.up * 150f; // 이동시킬 벡터
        _rotDir = _rot * _dir;

        return _rotDir;
    }

    public int RandomCardNum(string _category, bool _bool) // 랜덤한 카드 번호를 반환한다. false인 경우 제외한 번호를 반환한다.
    {
        List<int> _card = new();
        int _length = allCards.Length;

        for (int i = 0; i < _length; i++)
        {
            if (_bool)
            {
                if (allCards[i].category == _category) _card.Add(i);
            }
            else
            {
                if (allCards[i].category != _category) _card.Add(i);
            }
        }

        return _card[Random.Range(0, _card.Count)];
    }

    // REMOVE
    private void ResetHoldCardData() // 잡고있는 카드의 정보를 초기화 한다.
    {
        holdingCard = null;
        holdingCard_FCard = null;
        holdingCard_CList = new();
        bPos = new Vector2(0, 0);
        mPos = new Vector2(0, 0);
    }

    public void RemoveCard(CardManager _card, bool _bool) // 카드를 지운다. true인 경우 카운트한다.
    {
        if (_bool)
        {
            switch (_card.name) // 카드의 종류를 확인한다.
            {
                case "Boss":
                    SetAchieveRecord("Kill Enemy", 1, true);
                    SetAchieveRecord("Kill Boss", 1, true);
                    break;

                case "Enemy":
                    SetAchieveRecord("Kill Enemy", 1, true);
                    break;

                case "Special":
                    SetAchieveRecord("Use Special", 1, true);
                    break;
            }

            SetAchieveRecord("Destroy Card", 1, true);

            if (_card.name != "Boss") removeCardList.Add(_card.GetName());
        }

        _card.SetEnemy(null);
        int _length = _card.GetCardList().Count;

        if (_card.GetFirstCard() != null) // 처음 카드가 있는 경우
        {
            _card.GetFirstCard().GetCardList().Remove(_card);
            _card.SetFirstCard(null);
        }
        else while (0 < _length) _card.SetCardList(0, false); // 카드 목록을 정리한다.

        _card.SetTargetPos(new(0, 0));
        _card.transform.localPosition = new(0, 0, 0);
        _card.gameObject.SetActive(false);
        _card.name = "Card";
        _card.SetHp(1, false);
        _card.SetAtk(1, false);
        _card.SetMaxCard(4, false);
    }

    public void EndGame(bool _win) // 게임이 끝났을때 실행한다.
    {
        if (!noticeResult)
        {
            if (CheckMyItems("Rebirth"))
            {
                // SpawnCard(myData.GetCards()[GetMainCard()].card, spawnCards[1] - 1); // 메인카드를 다시 소환한다.
                // SetMapPos(-DataManager._instance.mainCard.GetTargetPos()); // 맵을 메인 카드가 있는 곳으로 이동시킨다.
            }
            else
            {
                noticeResult = true;
                StartCoroutine(ResultGame(_win));
            }
        }
    }

    // STETTING
    private void SetDebuff() // 디버프를 선택한다.
    {
        int _length = debuffs.title[debuffNum].num.Length;

        for (int i = 0; i < _length; i++)
        {
            int _debuffNum = debuffs.title[debuffNum].num[i];

            if (debuffs.data[_debuffNum].type != "")
            {
                object[] _obj = new object[2] { debuffs.data[_debuffNum].value, debuffs.data[_debuffNum].increase };

                SetFieldValue(allCardsList[RandomCardNum("Boss", false)], "Set" + debuffs.data[_debuffNum].type, _obj);

                if (debuffs.data[_debuffNum].data) debuff_NumList.Add(_debuffNum);
            }
        }

        debuffNum = -1;
    }

    private void SetMission() // 미션을 설정한다.
    {
        int _num;
        int _length = missions[missionNum].num.Length;

        for (int i = 0; i < _length; i++)
        {
            _num = missions[missionNum].num[i];
            matching.mission[_num].max = Random.Range(matching.mission[_num].num[0], matching.mission[_num].num[1] + 1);
        }

        CheckMissionClear();
    }

    private void SetCardData(CardManager _card, AllCards _allCards) // 카드의 정보를 변경한다.
    {
        _card.gameObject.SetActive(false);
        _card.name = _allCards.category;
        _card.cardImage.sprite = _allCards.spriteImage;
        _card.SetName(_allCards.name);
        _card.SetAtk(_allCards.atk, false);
        _card.SetHp(_allCards.hp, false);
        _card.SetMaxCard(_allCards.maxCard, false);
        _card.SetMaxCoolTime(_allCards.coolTime, false);
        _card.gameObject.SetActive(true);
    }

    private void SetCardInfo(bool _bool) // 추적중인 카드의 정보를 출력한다. false인 경우 적의 정보를 출력한다.
    {
        List<CardInfo> _cardInfos;
        CardManager _infoManager = GetInfoTarget();

        if (_bool) _cardInfos = cardInfos_Right;
        else
        {
            _cardInfos = cardInfos_Left;

            if (_infoManager != null) _infoManager = _infoManager.GetEnemy();
        }

        int _length = _cardInfos.Count;

        if (_infoManager == null || !_infoManager.gameObject.activeSelf) // 추적할 카드가 없는 경우
        {
            if (_bool) SetInfoTarget(null);

            for (int i = 0; i < _length; i++)
            {
                if (i > 5) // 정보칸이 많은 경우 제거한다.
                {
                    CardInfo _cardInfo = _cardInfos[i];

                    _cardInfos.Remove(_cardInfo);
                    Destroy(_cardInfo);
                    _length--;
                    i--;
                }
                else SetCardInfo(i, "", 0, 0, _bool);
            }
        }
        else
        {
            List<CardManager> _cardList = _infoManager.GetCardList();

            for (int i = 0; i < _length; i++)
            {
                if (i == 0) SetCardInfo(0, _infoManager.GetName(), _infoManager.GetHp(), _infoManager.GetAtk(), _bool);
                else if (i > _cardList.Count)
                {
                    if (i > 5) // 정보칸이 많은 경우 제거한다.
                    {
                        CardInfo _cardInfo = _cardInfos[i];

                        _cardInfos.Remove(_cardInfo);
                        Destroy(_cardInfo);
                        _length--;
                        i--;
                    }
                    else SetCardInfo(i, "", 0, 0, _bool);
                }
                else
                {
                    CardManager _cardManager = _cardList[i - 1];
                    SetCardInfo(i, _cardManager.GetName(), _cardManager.GetHp(), _cardManager.GetAtk(), _bool);
                }
            }
        }

        if (GetInfoTarget() != null && _bool) SetCardInfo(false);
    }

    private void SetCardInfo(int _num, string _name, int _hp, int _atk, bool _bool) // 카드 정보를 출력한다. true인 경우 오른쪽
    {
        List<CardInfo> _cardInfos;
        Transform _cardInfo_Trans;

        if (_bool)
        {
            _cardInfos = cardInfos_Right;
            _cardInfo_Trans = cardInfo_Trans_Right;
        }
        else
        {
            _cardInfos = cardInfos_Left;
            _cardInfo_Trans = cardInfo_Trans_Left;
        }

        if (_name != "") // 카드의 이름이 있는 경우에만 출력한다.
        {
            if (_num > _cardInfos.Count) _cardInfos.Add(Instantiate(cardInfo, _cardInfo_Trans).GetComponent<CardInfo>()); // 정보칸을 추가한다.

            RectTransform _nameRect = _cardInfos[_num].cardName.GetComponent<RectTransform>();
            RectTransform _objRect = _cardInfos[_num].GetComponent<RectTransform>();
            int _size = 0;

            _cardInfos[_num].gameObject.SetActive(true);
            _cardInfos[_num].cardName.text = _name;
            _cardInfos[_num].hp.text = _hp.ToString();
            _cardInfos[_num].atk.text = _atk.ToString();
            _nameRect.sizeDelta = new(_cardInfos[_num].cardName.preferredWidth + 20, _nameRect.sizeDelta.y);

            if (_hp > 0)
            {
                _cardInfos[_num].hp_Image.SetActive(true);
                _cardInfos[_num].hp.gameObject.SetActive(true);
                _size += 80;
            }
            else
            {
                _cardInfos[_num].hp_Image.SetActive(false);
                _cardInfos[_num].hp.gameObject.SetActive(false);
            }

            if (_atk > 0)
            {
                _cardInfos[_num].atk_Image.SetActive(true);
                _cardInfos[_num].atk.gameObject.SetActive(true);
                _size += 80;
            }
            else
            {
                _cardInfos[_num].atk_Image.SetActive(false);
                _cardInfos[_num].atk.gameObject.SetActive(false);
            }

            _objRect.sizeDelta = new(_cardInfos[_num].cardName.preferredWidth + _size + 40, _objRect.sizeDelta.y);
        }
        else _cardInfos[_num].gameObject.SetActive(false); // 이름이 없는경우 숨긴다.
    }

    // ACHIEVEMENT
    private void CheckAchievement() // 업적을 확인한다.
    {
        while (0 < activeAchieve.Count) // 기존에 있던 업적들을 지운다.
        {
            Destroy(activeAchieve[0].mask);
            Destroy(activeAchieve[0].gameObject);
            activeAchieve.RemoveAt(0);
        }

        for (int i = 0; i < achievements.Length; i++) // 모든 업적을 확인한다.
        {
            if (myData.GetAchievement(achievements[i].name) >= achievements[i].max) // 업적을 달성한 경우
            {
                if (activeAchieve.Count == 0) // 활성화된 카테고리가 없는 경우
                {
                    Achievement _achievement = Instantiate(achievement_Title, achievementUI.transform).GetComponent<Achievement>();
                    _achievement.mask = Instantiate(achievement_Mask, achievementUI.transform);
                    _achievement.itemList.Add(Instantiate(achievement_Item, _achievement.mask.transform).GetComponent<Achievement_Item>());
                    _achievement.itemList[^1].SetData(texts.ACHIEVEMENT_TITLE[i], achievements[i].spriteImage, texts.ACHIEVEMENT_DETAIL[i]);
                    _achievement.SetCategory(texts.ACHIEVEMENT_CATEGORY[achievements[i].category]);
                    activeAchieve.Add(_achievement); // 업적을 추가한다.
                }
                else // 활성화된 카테고리가 있는 경우
                {
                    int _length = activeAchieve.Count;

                    for (int j = 0; j < _length; j++) // 활성화된 모든 카테고리를 확인한다.
                    {
                        if (activeAchieve[j].GetCategory() != texts.ACHIEVEMENT_CATEGORY[achievements[i].category]) // 해당 카테고리가 활성화가 안된 경우
                        {
                            Achievement _achievement = Instantiate(achievement_Title, achievementUI.transform).GetComponent<Achievement>();
                            _achievement.mask = Instantiate(achievement_Mask, achievementUI.transform);
                            _achievement.itemList.Add(Instantiate(achievement_Item, _achievement.mask.transform).GetComponent<Achievement_Item>());
                            _achievement.itemList[^1].SetData(texts.ACHIEVEMENT_TITLE[i], achievements[i].spriteImage, texts.ACHIEVEMENT_DETAIL[i]);
                            _achievement.SetCategory(texts.ACHIEVEMENT_CATEGORY[achievements[i].category]);
                            activeAchieve.Add(_achievement); // 업적을 추가한다.
                        }
                        else // 해당 업적이 활성화된 경우
                        {
                            activeAchieve[j].itemList.Add(Instantiate(achievement_Item, activeAchieve[j].mask.transform).GetComponent<Achievement_Item>());
                            activeAchieve[j].itemList[^1].SetData(texts.ACHIEVEMENT_TITLE[i], achievements[i].spriteImage, texts.ACHIEVEMENT_DETAIL[i]);
                            activeAchieve[j].SetCategory(texts.ACHIEVEMENT_CATEGORY[achievements[i].category]);
                        }
                    }
                }
            }
        }

        if (activeAchieve.Count == 0) panels[1].GetComponent<Text>().text = texts.ACHIEVEMENT_NONE_TEXT;
        else panels[1].GetComponent<Text>().text = "";
    }

    private bool CheckAchieveRecord(string _name)
    {
        List<string> _names = new(achieveRecord.Keys);
        int _length = _names.Count;

        for (int i = 0; i < _length; i++)
        {
            if (_names[i] == _name) return true;
        }

        return false;
    }

    // SHOP
    public void SetShopButtonText()
    {
        itemInfo_Button_Text.text = texts.SHOP_BUY_TEXT;
    }

    public void BuyShopItem(int _money, int _num, string _name, object[] _obj) // 아이템 구매를 처리한다.
    {
        int _myMoney = myData.GetMoney();

        if (_myMoney >= _money)
        {
            myData.SetMoney(-_money, true);
            SetMoneyText(_myMoney - _money);
            SetFieldValue(myData, "Set" + _name, _obj);
            DataManager._instance.MyDataSave(myData);

            if (_num == 6)
            {
                myData.SetShopItemNum(_num, -1);
                shop_Title[2].text = texts.SHOP_NONE_TEXT;
                shop_Items_Special.gameObject.SetActive(false);
                HideInfo();
            }
            else
            {
                ShopItem[] _shopItems = myData.GetShopItem();
                Items _items = new();
                _items.objStrings = new string[2] { "-1", _shopItems[_num].num.ToString() };
                _items.type = "Cards";
                _items.SetData();
                SetRandShopItemNum(_num);
                shop_Items[_num].SetData(_items, GetShopItemDetail(_items), _shopItems[_num].money, _num);
                itemInfo_Title.text = _items.name;
                itemInfo_Image.sprite = _items.spriteImage;
                itemInfo_Detail.text = GetShopItemDetail(_items);
                SetShopButtonText();
                itemInfo_Button.onClick.RemoveAllListeners();
                itemInfo_Button.onClick.AddListener(() => BuyShopItem(_shopItems[_num].money, _num, _items.type, _items.obj));
            }
        }
        else StartCoroutine(Notice(texts.SHOP_FAIL_MASSEGE));
    }

    private string GetShopItemDetail(Items _items)
    {
        string _detail;

        if (_items.type == "Cards")
        {
            AllCards _card = allCards[(int)_items.obj[1]];
            _detail = texts.STAT_NAME[0] + " : " + ChangeCategory(_card.category) + " " + ChangeCategory(_items.type) + "\n";
            _detail += texts.STAT_NAME[1] + " : " + _card.hp + "\n";
            _detail += texts.STAT_NAME[2] + " : " + _card.atk + "\n";
            _detail += texts.STAT_NAME[3] + " : " + _card.coolTime;
        }
        else
        {
            _detail = texts.STAT_NAME[0] + " : " + ChangeCategory(_items.type) + "\n";

            if (_items.type == "Hp") _detail += texts.STAT_NAME[1] + " : " + _items.obj[0];
            else if (_items.type == "Atk") _detail += texts.STAT_NAME[2] + " : " + _items.obj[0];
            else _detail += _items.name;
        }

        return _detail;
    }

    private void SetRandShopItemNum(int _index)
    {
        int _cardNum = -1;

        switch (Random.Range(0, 3))
        {
            //case 0:
            //    while (true)
            //    {
            //        _cardNum = RandomCardNum("Friend", true);
            //
            //        if (_cardNum != 0) break;
            //    }
            //    break;

            case 1:
                _cardNum = RandomCardNum("Enemy", true);
                break;

            case 2:
                _cardNum = RandomCardNum("Food", true);
                break;

            default:
                int _length = combines.Length;

                while (_cardNum == -1)
                {
                    _cardNum = RandomCardNum("Item", true);

                    for (int i = 0; i < _length; i++)
                    {
                        if (combines[i].result_Num == _cardNum)
                        {
                            _cardNum = -1;
                            break;
                        }
                    }
                }

                break;
        }

        myData.SetShopItemNum(_index, _cardNum); // 아이템 항목을 갱신한다.
        myData.SetShopItemMoney(_index, Random.Range(5, 11)); // 아이템 금액을 갱신한다.
        DataManager._instance.MyDataSave(myData);
    }

    private void CheckShopItem() // 상점 아이템을 확인한다. 
    {
        ShopItem[] _shopItems = myData.GetShopItem();
        int _length = _shopItems.Length - 1;
        Items _items;

        shop_Title[0].text = texts.SHOP_TITLE[0];
        shop_Title[1].text = texts.SHOP_TITLE[1];
        shop_Title[2].text = texts.SHOP_NONE_TEXT;

        for (int i = 0; i < _length; i++)
        {
            if (_shopItems[i].num == -1) SetRandShopItemNum(i); // 아이템이 없는 경우
        }

        for (int i = 0; i < _length; i++)
        {
            _items = new();
            _items.objStrings = new string[2] { "-1", _shopItems[i].num.ToString() };
            _items.type = "Cards";
            _items.SetData();
            shop_Items[i].SetData(_items, GetShopItemDetail(_items), _shopItems[i].money, i);
        }

        if (_shopItems[6].num > -1)
        {
            shop_Title[2].text = "";
            shop_Items_Special.gameObject.SetActive(true);
            _items = items.items[_shopItems[6].num];
            shop_Items_Special.SetData(_items, GetShopItemDetail(_items), _shopItems[6].money, 6);
        }
        else shop_Items_Special.gameObject.SetActive(false);
    }

    // MY CARDS
    public void SetDeckButtonText(int _cardIndex)
    {
        MyCard[] _myCards = myData.GetCards();

        if (!_myCards[_cardIndex].deck) deckInfo_Button_Text.text = texts.DECK_ADD_TEXT;
        else deckInfo_Button_Text.text = texts.DECK_DEL_TEXT;

        deckInfo_Change_Button_Text.gameObject.SetActive(false);

        GetDeckCount();
        DataManager._instance.MyDataSave(myData);
    }

    public void ToggleDeckCard(int _cardIndex) // 덱 등록 여부를 변경한다.
    {
        MyCard[] _myCards = myData.GetCards();

        if (_myCards[_cardIndex].deck) // 덱에 등록되어 있다면
        {
            myData.SetDeck(_cardIndex, false);
            myDeckCards[_cardIndex].SetDeck(false);
        }
        else
        {
            if (GetDeckCount() < spawnCards[1])
            {
                myData.SetDeck(_cardIndex, true);
                myDeckCards[_cardIndex].SetDeck(true);
            }
            else StartCoroutine(Notice(texts.DECK_OVER_MASSEGE)); // 30개 이상의 카드를 덱에 넣을 수 없다.
        }

        SetDeckButtonText(_cardIndex);
    }

    private void CheckMyCards() // 나의 카드들을 확인한다.
    {
        while (0 < myDeckCards.Count) // 기존에 있던 카드들을 지운다.
        {
            Destroy(myDeckCards[0].gameObject);
            myDeckCards.RemoveAt(0);
        }

        MyCard[] _myCards = myData.GetCards();
        int _length = _myCards.Length;

        for (int i = 0; i < _length; i++)
        {
            DeckCard _deckCard = Instantiate(deckCard, deckUI.transform).GetComponent<DeckCard>();
            AllCards _card = allCards[_myCards[i].card];
            string _detail = texts.STAT_NAME[0] + " : " + ChangeCategory(_card.category) + "\n";
            _detail += texts.STAT_NAME[1] + " : " + _card.hp + "\n";
            _detail += texts.STAT_NAME[2] + " : " + _card.atk + "\n";
            _detail += texts.STAT_NAME[3] + " : " + _card.coolTime;
            _deckCard.SetData(_card.name, _card.spriteImage, _detail, i);
            myDeckCards.Add(_deckCard); // 덱카드를 추가한다.
            myDeckCards[i].SetDeck(_myCards[i].deck);
        }

        GetDeckCount();
    }

    private int GetDeckCount()
    {
        MyCard[] _myCards = myData.GetCards();
        int _length = _myCards.Length;
        int _num = 0;

        for (int i = 0; i < _length; i++)
        {
            if (_myCards[i].deck) _num++;
        }

        deck_Title.text = texts.MY_CARDS_TEXT + " ( " + _num + " / " + spawnCards[1] + " )";

        return _num;
    }

    // OTHER
    private string ChangeCategory(string _category)
    {
        if (_category == "Friend") return texts.ITEM_CATEGORY_NAME[1];
        else if (_category == "Enemy") return texts.ITEM_CATEGORY_NAME[2];
        else if (_category == "Food") return texts.ITEM_CATEGORY_NAME[3];
        else if (_category == "Item") return texts.ITEM_CATEGORY_NAME[4];
        else if (_category == "Special") return texts.ITEM_CATEGORY_NAME[5];
        else if (_category == "NPC") return texts.ITEM_CATEGORY_NAME[6];
        else if (_category == "Cards") return texts.ITEM_CATEGORY_NAME[7];
        else return texts.ITEM_CATEGORY_NAME[0];
    }

    public string TransName(string _name, int _num)
    {
        string transName;

        var _texts = texts.GetType().GetField(_name).GetValue(texts);

        if (_texts.GetType() == typeof(string)) transName = _texts as string;
        else transName = (_texts as string[])[_num];

        return transName;
    }

    public void SetFieldValue<T>(T _field, string _name, object[] _obj)
    {
        _field.GetType().GetMethod(_name)?.Invoke(_field, _obj);
    }

    private void UpdateDays(int _num, bool _bool) // 날짜를 갱신한다.
    {
        days[1] = _num;

        if (_bool) // true를 입력한 경우에만 날짜를 변경한다.
        {
            SetAchieveRecord("Passed Days", days[1] - days[2], true);

            if (days[1] >= 100) StartCoroutine(NextDays());
        }

        daysPercentImage.fillAmount = days[1] * 0.01f;
    }

    private void TradeCardList(CardManager _obj_1, CardManager _obj_2) // _obj_1의 카드 목록을 _obj_2에 넣는다.
    {
        List<CardManager> _cardList = _obj_1.GetCardList(); // _obj_1의 카드 목록을 가져온다.
        _obj_1.SetFirstCard(_obj_2); // _obj_1의 처음 카드를 _obj_2로 설정한다.
        _obj_1.transform.SetAsLastSibling(); // 가장 위에 표시되게 설정한다.
        _obj_2.SetCardList(_obj_1); // _obj_2의 카드 목록에 자신을 추가한다.

        while (0 < _cardList.Count) // _obj_1의 카드 목록을 전부 확인한다.
        {
            CardManager _card = _cardList[0];

            _obj_1.SetCardList(0, false); // _obj_1의 카드 목록에서 제거한다.
            _obj_2.SetCardList(_card); // _obj_2의 카드 목록에 _obj_1의 카드 목록을 추가한다.
            _card.SetFirstCard(_obj_2); // _obj_1의 카드 목록에 있는 카드들의 처음 카드를 설정한다.
            _card.transform.SetAsLastSibling(); // 가장 위에 표시되게 설정한다.
        }
    }

    private void SpawnCard(int _num, int _index) // 카드들을 소환한다.
    {
        allCardsList[_index].name = allCards[_num].name;
        SetCardData(allCardsList[_index], allCards[_num]);
        allCardsList[_index].transform.SetAsLastSibling(); // 카드를 가장 위에 표시되게 설정한다.
        Vector2 _dir = allCardsList[_index].CheckMoveTrue(RandomMapPos());  // 랜덤한 위치
        CardManager _hitCard = GetHitCard(Physics2D.RaycastAll(_dir, Vector2.zero, 1, cardLayerMask)); // 자신이 클릭한 카드를 기록한다.

        if (_hitCard != null) // 오브젝트가 있는 경우
        {
            CardManager _firstCard = _hitCard.GetFirstCard(); // 카드의 처음 카드를 가져온다.

            if (_firstCard == null) _firstCard = _hitCard; // 카드의 처음 카드가 없다면

            if (_firstCard != allCardsList[_index]) // 자신이 아닌경우만 실행
            {
                if (allCardsList[_index].name == _firstCard.name) // 같은 종류의 카드인 경우합친다.
                {
                    TradeCardList(allCardsList[_index], _firstCard);
                    _dir = allCardsList[_index].GetTargetPos();
                }
            }
        }

        allCardsList[_index].SetTargetPos(_dir);
        allCardsList[_index].transform.localPosition = allCardsList[_index].CheckMoveTrue(allCardsList[_index].GetTargetPos());
        allCardsList[_index].GetAnimator().Play(PUT);
    }
}