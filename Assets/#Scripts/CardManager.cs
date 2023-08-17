using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    private CardManager firstCard;
    private CardManager enemy;
    private List<CardManager> cardList = new();
    private Vector2 targetPos;
    private int hp = 1;
    private int atk = 1;
    private int moveSpeed = 8;
    private int maxCard = 4;
    private int battlePos = 0;
    private float maxCoolTime = 1;
    private float nowCoolTime = 1;
    private bool safeMove = false;
    private string cardName;

    private const int SPACE = 10;

    public Animator animator;
    public Image cardImage;

    private void Update()
    {
        MoveCard();
    }

    // GET SET
    public void SetEnemy(CardManager _enemy)
    {
        enemy = _enemy;
    }

    public CardManager GetEnemy()
    {
        if (GetFirstCard() != null) return GetFirstCard().GetEnemy(); // 처음 카드가 있는 경우
        else return enemy;
    }

    public bool GetMove()
    {
        if (!gameObject.activeSelf) return false;

        if (Vector2.Distance(transform.localPosition, GetTargetPos()) > 5) return true; // 이동하는 것으로 간주한다

        if (!GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Idle")) return true; // 이동하는 것으로 간주한다

        int _length = GetCardList().Count;

        for (int i = 0; i < _length; i++) // 카드 목록을 확인한다.
        {
            if (GetCardList()[i].GetMove()) return true; // 이동하는 것으로 간주한다
        }

        return false;
    }

    public void SetHp(int _hp, bool _bool) // 체력을 설정한다. true인 경우 증감한다.
    {
        if (_bool)
        {
            NoticeSmallText(0, _hp);
            hp += _hp;

            if (GetHp() == 0) // 죽은 경우
            {
                CardManager _enemy = GetEnemy();

                if (_enemy != null) // 전투중인 경우
                {
                    if (GetFirstCard() == null) // 자신이 처음카드인 경우
                    {
                        _enemy.SetEnemy(null);
                        SetEnemy(null);
                    }
                }

                while (0 < GetCardList().Count) SetCardList(GetCardList().Count - 1, true); // 카드 목록에서 지운다.

                NoticeSmallText(-1, 0);
                GameManager._instance.RemoveCard(this, true);
            }
        }
        else hp = _hp;
    }

    public int GetHp()
    {
        if (hp < 0) hp = 0;

        return hp;
    }

    public void SetAtk(int _atk, bool _bool)
    {
        if (_bool)
        {
            NoticeSmallText(1, _atk);
            atk += _atk;
        }
        else atk = _atk;
    }

    public int GetAtk()
    {
        if (atk < 0) atk = 0;

        return atk;
    }

    public void SetName(string _name)
    {
        cardName = _name;
    }

    public string GetName()
    {
        return cardName;
    }

    public void SetMaxCoolTime(float _coolTime, bool _bool)
    {
        if (_bool)
        {
            maxCoolTime += _coolTime;
            nowCoolTime += _coolTime;
        }
        else
        {
            maxCoolTime = _coolTime;
            nowCoolTime = _coolTime;
        }
    }

    public float GetMaxCoolTime()
    {
        return maxCoolTime;
    }

    public float GetNowCoolTime()
    {
        return nowCoolTime;
    }

    public void SetTargetPos(Vector2 _pos) // 추적할 위치를 갱신한다.
    {
        targetPos = _pos;
    }

    public Vector2 GetTargetPos() // 추척중인 위치를 반환한다.
    {
        if (GetFirstCard() != null) FallowFirstCard(); // 처음의 카드가 있는 경우

        return targetPos;
    }

    public CardManager GetFirstCard() // 처음의 카드를 반환한다.
    {
        return firstCard;
    }

    public void SetFirstCard(CardManager _obj) // 처음의 카드를 갱신한다.
    {
        firstCard = _obj;
    }

    public List<CardManager> GetCardList() // 카드 목록을 반환한다.
    {
        return cardList;
    }

    public void SetDestroyCard(int _num, bool _bool) // 소지한 카드를 전부 파괴한다.
    {
        int _length = GetCardList().Count;

        if (_num == -1)
        {
            for (int i = 0; i < _length; i++) SetDestroyCard(i, true);
        }
        else
        {
            if (_length > 0)
            {
                CardManager _cardManager;

                if (_bool) _cardManager = GetCardList()[Random.Range(0, _length)];
                else _cardManager = GetCardList()[_num];

                _cardManager.NoticeSmallText(-1, 0);
                GameManager._instance.RemoveCard(_cardManager, true); // 소지한 카드를 한개 지운다.
            }
        }
    }

    public void SetCardList(int _num, bool _bool) // 카드를 지운다.
    {
        if (cardList.Count > _num)
        {
            cardList[_num].SetFirstCard(null);

            if (_bool) // true인 경우 랜덤한 방향으로 이동시킨다.
            {
                Vector2 _checkPos = CheckMoveTrue(GetTargetPos() + (Vector2)GameManager._instance.RandomAroundPos()); // 이동 가능한 위치인지 확인한다.

                if (Vector2.Distance(GetTargetPos(), _checkPos) >= GameManager._instance.GetCardSize().x / 2)
                {
                    cardList[_num].SetSafeMove(true);
                    cardList[_num].SetTargetPos(_checkPos); // 랜덤한 방향으로 이동시킨다.
                }
            }

            cardList.RemoveAt(_num);
        }
    }

    public void SetCardList(CardManager _obj) // 카드를 목록에 추가한다.
    {
        cardList.Add(_obj);

        if (GetMaxCard() < GetCardList().Count) SetCardList(GetCardList().Count - 1, true); // 카드를 많이 소지한 경우 버린다.
    }

    public void SetCardList(CardManager _obj, int _num) // 카드를 입력한 자리에 추가한다.
    {
        cardList.Insert(_num, _obj);
    }

    public int GetBattlePos()
    {
        return battlePos;
    }

    public void SetBattlePos(int _num)
    {
        battlePos = _num;
    }

    public int GetMaxCard()
    {
        if (maxCard < 0) maxCard = 0;

        return maxCard;
    }

    public void SetMaxCard(int _num, bool _bool)
    {
        if (_bool) maxCard += _num;
        else maxCard = _num;
    }

    public void SetSafeMove(bool _bool)
    {
        safeMove = _bool;
    }

    public bool GetSafeMove()
    {
        return safeMove;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    // CHECK
    public bool HoldingCheck() // 자신을 잡고있는지 확인한다.
    {
        if (GetFirstCard() != null) // 처음 카드가 있는 경우
        {
            if (GameManager._instance.holdingCard == GetFirstCard()) return true; // 처음카드를 잡고있는 경우
        }
        else
        {
            if (GameManager._instance.holdingCard == this) return true; // 자신을 잡고있는 경우
        }

        return false;
    }

    public bool BattlePosCheck(int _first, int _child, bool _moveFistCard) // 전투가 가능한 위치인지 확인한다.
    {
        Vector2 _myPos = transform.localPosition;
        Vector2 _cardSize = GameManager._instance.GetCardSize();
        Vector2 _moveFirstPos = new();
        Vector2 _moveChildPos;

        if (_moveFistCard) _moveFirstPos = _myPos + new Vector2((_first - 2) * -(_cardSize.x + SPACE) * (_first % 2), (_first - 1) * -(_cardSize.y + SPACE) * ((_first + 1) % 2));

        if (CheckMoveTrue(_moveFirstPos).z == 0) // 처음카드가 이동 가능한지 확인한다.
        {
            int _length = GetCardList().Count;

            for (int i = 0; i < _length; i++) // 카드 목록이 이동 가능한지 확인한다.
            {
                if (_child % 2 == _first % 2) return false;

                _moveChildPos = _moveFirstPos + new Vector2((_child - 2) * -(_cardSize.x + SPACE) * (i + 1) * (_child % 2), (_child - 1) * -(_cardSize.y + SPACE) * (i + 1) * ((_child + 1) % 2));

                if (CheckMoveTrue(_moveChildPos).z != 0) return false;
            }
        }
        else return false;

        return true;
    }

    // BATTLE
    public void StartBattle(CardManager _enemy, int _pos)
    {
        SetEnemy(_enemy);
        Vector3 _dir = new();

        if (_pos == 0) _dir = Vector3.up * (GameManager._instance.GetCardSize().y + SPACE);
        else if (_pos == 1) _dir = Vector3.right * (GameManager._instance.GetCardSize().x + SPACE);
        else if (_pos == 2) _dir = Vector3.down * (GameManager._instance.GetCardSize().y + SPACE);
        else if (_pos == 3) _dir = Vector3.left * (GameManager._instance.GetCardSize().x + SPACE);
        else if (_pos == 4) _dir = new();

        if (GetFirstCard() == null) // 자신이 처음카드인 경우
        {
            SetTargetPos(transform.localPosition + _dir); // 추적할 위치를 갱신한다.
        }
        else
        {
            int _cardIndex = GetFirstCard().GetCardList().IndexOf(this) + 1;
            SetTargetPos((Vector3)GetFirstCard().GetTargetPos() + _dir * _cardIndex); // 추적할 위치를 갱신한다.
        }

        transform.SetAsLastSibling(); // 카드를 가장 위에 표시되게 한다.

        GameManager._instance.SetAchieveRecord("Battle Count", 1, true);

        StartCoroutine(Battle());
    }

    private IEnumerator Battle() // 전투를 진행한다.
    {
        if (GetEnemy() != null) yield return new WaitUntil(() => !GetEnemy().GetMove()); // 적군의 이동이 끝날때 까지 대기한다.

        yield return new WaitUntil(() => !GetMove()); // 이동이 끝날때 까지 대기한다.

        List<CardManager> _enemyCardList = new();

        if (GetEnemy() != null) _enemyCardList = GetEnemy().GetCardList();

        Vector2 _pos = GetTargetPos();
        CardManager _target;

        while (GetEnemy() != null) // 전투중인 경우에만 실행
        {
            yield return new WaitUntil(() => GameManager._instance.GetPlay()); // 게임이 진행중인 경우에만 실행한다.

            if (GetAtk() > 0) // 공격력이 있는 경우에만 실행한다.
            {
                if (GetNowCoolTime() <= 0) // 쿨타임이 다 된 경우
                {
                    if (_enemyCardList.Count > 0) _target = _enemyCardList[^1]; // 하위의 카드가 있는 경우
                    else _target = GetEnemy();

                    if (GetEnemy() != null) // 전투중인 경우에만 실행한다.
                    {
                        SetTargetPos(_target.GetTargetPos());

                        if (Vector2.Distance(transform.localPosition, GetTargetPos()) < 50) // 이동이 끝난 경우
                        {
                            _target.SetHp(-GetAtk(), true);

                            if (_enemyCardList.Count > 0) _target = _enemyCardList[^1]; // 하위의 카드가 있는 경우
                            else _target = GetEnemy();

                            nowCoolTime = GetMaxCoolTime();
                            SetTargetPos(_pos); // 원래의 위치로 이동한다.
                        }
                    }
                }
                else if (!GetMove() && !GetEnemy().GetMove()) nowCoolTime -= Time.deltaTime;
            }



            yield return null;
        }

        SetEnemy(null);
        nowCoolTime += GetMaxCoolTime();

        if (GetFirstCard() == null) // 자신이 처음 카드인 경우
        {
            List<CardManager> _cardManager = GetCardList();
            int _length = _cardManager.Count;

            for (int i = 0; i < _length; i++) _cardManager[i].transform.SetAsLastSibling(); // 마지막 카드를 가장 위에 표시되게 한다.
        }

        SetTargetPos(_pos); // 원래의 위치로 이동한다.
    }

    // MOVE
    public Vector3 CheckMoveTrue(Vector2 _pos) // 이동이 가능한지 확인한다.
    {
        Vector3 _checkPos = new(_pos.x, _pos.y, 0);

        if (_checkPos != transform.localPosition) // 이동하려는 경우에만 확인한다.
        {
            Vector2 _dir = (_checkPos - transform.localPosition).normalized; // 방향 벡터
            float _dist = Vector2.Distance(transform.localPosition, _checkPos);
            RaycastHit2D _hit = Physics2D.BoxCast(transform.position, GameManager._instance.GetCardSize(), 0f, _dir, _dist, 1 << LayerMask.NameToLayer("Wall"));

            if (_hit) // 벽에 닿은 경우
            {
                _checkPos = GameManager._instance.map.transform.InverseTransformPoint(_hit.point);
                _dir = (_checkPos - transform.localPosition).normalized; // 방향 벡터
                _checkPos -= new Vector3(_dir.x * (GameManager._instance.GetCardSize().x / 2 + 1), _dir.y * (GameManager._instance.GetCardSize().y / 2 + 1), 0);
                _checkPos.z = 1; // 이동이 불가능 했던 경우 알려준다.
            }
            else
            {
                Vector2 _mapSize = GameManager._instance.GetMapSize();
                Vector2 _cardSize = GameManager._instance.GetCardSize();

                if (_checkPos.x > _mapSize.x - _cardSize.x * 0.5f) _checkPos = new(_mapSize.x - _cardSize.x * 0.5f, _checkPos.y);

                if (_checkPos.x < -(_mapSize.x - _cardSize.x * 0.5f)) _checkPos = new(-(_mapSize.x - _cardSize.x * 0.5f), _checkPos.y);

                if (_checkPos.y > _mapSize.y - _cardSize.y * 0.5f) _checkPos = new(_checkPos.x, _mapSize.y - _cardSize.y * 0.5f);

                if (_checkPos.y < -(_mapSize.y - _cardSize.y * 0.5f)) _checkPos = new(_checkPos.x, -(_mapSize.y - _cardSize.y * 0.5f));

                if ((Vector3)_pos != _checkPos) _checkPos.z = 1; // 이동이 불가능 했던 경우 알려준다.
            }
        }

        return _checkPos;
    }

    private void FallowFirstCard() // 추적할 위치를 자신의 처음카드로 갱신한다.
    {
        int _cardIndex = GetFirstCard().GetCardList().IndexOf(this);
        Vector3 _dir = Vector3.down * (GameManager._instance.GetCardSize().y * 0.5f - SPACE);

        if (GetEnemy() == null) // 전투중이 아닌경우 추적할 위치를 갱신한다.
        {
            if (_cardIndex == 0) SetTargetPos(GetFirstCard().transform.localPosition + _dir);
            else SetTargetPos(GetFirstCard().GetCardList()[_cardIndex - 1].transform.localPosition + _dir);
        }
    }

    private void MoveCard() // 카드가 움직이게 한다.
    {
        if (GetFirstCard() == null && !GetSafeMove()) SetTargetPos(CheckMoveTrue(GetTargetPos())); // 자신이 처음카드인 경우 이동가능한지 확인한다.

        AnimatorStateInfo _state = GetAnimator().GetCurrentAnimatorStateInfo(0);

        if (Vector2.Distance(transform.localPosition, GetTargetPos()) > 1) // 타겟의 위치와 현재 위치가 멀리있는 경우
        {
            bool _bool = true;

            if (HoldingCheck()) // 자신을 잡고 있는 경우
            {
                if (_state.IsName("Hold"))
                {
                    if (_state.normalizedTime < 1.0f) _bool = false;
                }
                else
                {
                    _bool = false;
                    GetAnimator().Play("Hold");
                    int _length = GetCardList().Count;

                    for (int i = 0; i < _length; i++) GetCardList()[i].GetAnimator().Play("Hold");
                }
            }

            if (_bool)
            {
                if (GetEnemy() != null) transform.localPosition = Vector2.Lerp(transform.localPosition, GetTargetPos(), moveSpeed * 1.5f * Time.deltaTime); // 전투중인 경우 빠르게 이동한다.
                else transform.localPosition = Vector2.Lerp(transform.localPosition, GetTargetPos(), moveSpeed * Time.deltaTime); // 타겟의 위치로 이동한다.
            }

        }
        else if (Vector2.Distance(transform.localPosition, GetTargetPos()) >= 0) // 타겟의 위치와 가까워진 경우
        {
            transform.localPosition = GetTargetPos(); // 타겟의 위치로 바꾼다.

            if (!HoldingCheck()) // 자신을 잡고 있지 않는 경우
            {
                if (_state.IsName("Hold") && _state.normalizedTime >= 1.0f)
                {
                    if (GetFirstCard() == null) GetAnimator().Play("Put");
                    else
                    {
                        _state = GetFirstCard().GetAnimator().GetCurrentAnimatorStateInfo(0);

                        if (_state.IsName("Put") || _state.IsName("Idle")) GetAnimator().Play("Put");
                    }
                }
            }

            if (GetSafeMove()) SetSafeMove(false);
        }
    }

    // OTHER
    public void NoticeSmallText(int _category, int _num)
    {
        GameObject _instant = Instantiate(GameManager._instance.smallText, GameManager._instance.map.transform); // 메시지를 출력한다.
        SmallText _smallText = _instant.GetComponent<SmallText>();

        _smallText.SetText(_num.ToString());
        _instant.transform.localPosition = transform.localPosition + new Vector3(0, GameManager._instance.GetCardSize().y / 2 - 5, 0);

        if (_num == 0)
        {
            if (_category == 2) // 돈 출력시 파괴 메시지를 띄우지 않는다.
            {
                _smallText.text.color = new(0, 0, 0);
                _smallText.SetText(0.ToString());
            }
            else
            {
                _instant.transform.localPosition += new Vector3(0, -20, 0);
                _smallText.SetText(GameManager._instance.GetDestroyText()); // 파괴
            }
        }
        else if (_num > 0) _smallText.text.color = new(0, 0, 255); // 증가한 경우 파랑색으로 변경

        if (_category == -1) _smallText.image.gameObject.SetActive(false);
        else _smallText.image.sprite = _smallText.sprites[_category];
    }
}
