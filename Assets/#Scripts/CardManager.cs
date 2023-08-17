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
        if (GetFirstCard() != null) return GetFirstCard().GetEnemy(); // ó�� ī�尡 �ִ� ���
        else return enemy;
    }

    public bool GetMove()
    {
        if (!gameObject.activeSelf) return false;

        if (Vector2.Distance(transform.localPosition, GetTargetPos()) > 5) return true; // �̵��ϴ� ������ �����Ѵ�

        if (!GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Idle")) return true; // �̵��ϴ� ������ �����Ѵ�

        int _length = GetCardList().Count;

        for (int i = 0; i < _length; i++) // ī�� ����� Ȯ���Ѵ�.
        {
            if (GetCardList()[i].GetMove()) return true; // �̵��ϴ� ������ �����Ѵ�
        }

        return false;
    }

    public void SetHp(int _hp, bool _bool) // ü���� �����Ѵ�. true�� ��� �����Ѵ�.
    {
        if (_bool)
        {
            NoticeSmallText(0, _hp);
            hp += _hp;

            if (GetHp() == 0) // ���� ���
            {
                CardManager _enemy = GetEnemy();

                if (_enemy != null) // �������� ���
                {
                    if (GetFirstCard() == null) // �ڽ��� ó��ī���� ���
                    {
                        _enemy.SetEnemy(null);
                        SetEnemy(null);
                    }
                }

                while (0 < GetCardList().Count) SetCardList(GetCardList().Count - 1, true); // ī�� ��Ͽ��� �����.

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

    public void SetTargetPos(Vector2 _pos) // ������ ��ġ�� �����Ѵ�.
    {
        targetPos = _pos;
    }

    public Vector2 GetTargetPos() // ��ô���� ��ġ�� ��ȯ�Ѵ�.
    {
        if (GetFirstCard() != null) FallowFirstCard(); // ó���� ī�尡 �ִ� ���

        return targetPos;
    }

    public CardManager GetFirstCard() // ó���� ī�带 ��ȯ�Ѵ�.
    {
        return firstCard;
    }

    public void SetFirstCard(CardManager _obj) // ó���� ī�带 �����Ѵ�.
    {
        firstCard = _obj;
    }

    public List<CardManager> GetCardList() // ī�� ����� ��ȯ�Ѵ�.
    {
        return cardList;
    }

    public void SetDestroyCard(int _num, bool _bool) // ������ ī�带 ���� �ı��Ѵ�.
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
                GameManager._instance.RemoveCard(_cardManager, true); // ������ ī�带 �Ѱ� �����.
            }
        }
    }

    public void SetCardList(int _num, bool _bool) // ī�带 �����.
    {
        if (cardList.Count > _num)
        {
            cardList[_num].SetFirstCard(null);

            if (_bool) // true�� ��� ������ �������� �̵���Ų��.
            {
                Vector2 _checkPos = CheckMoveTrue(GetTargetPos() + (Vector2)GameManager._instance.RandomAroundPos()); // �̵� ������ ��ġ���� Ȯ���Ѵ�.

                if (Vector2.Distance(GetTargetPos(), _checkPos) >= GameManager._instance.GetCardSize().x / 2)
                {
                    cardList[_num].SetSafeMove(true);
                    cardList[_num].SetTargetPos(_checkPos); // ������ �������� �̵���Ų��.
                }
            }

            cardList.RemoveAt(_num);
        }
    }

    public void SetCardList(CardManager _obj) // ī�带 ��Ͽ� �߰��Ѵ�.
    {
        cardList.Add(_obj);

        if (GetMaxCard() < GetCardList().Count) SetCardList(GetCardList().Count - 1, true); // ī�带 ���� ������ ��� ������.
    }

    public void SetCardList(CardManager _obj, int _num) // ī�带 �Է��� �ڸ��� �߰��Ѵ�.
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
    public bool HoldingCheck() // �ڽ��� ����ִ��� Ȯ���Ѵ�.
    {
        if (GetFirstCard() != null) // ó�� ī�尡 �ִ� ���
        {
            if (GameManager._instance.holdingCard == GetFirstCard()) return true; // ó��ī�带 ����ִ� ���
        }
        else
        {
            if (GameManager._instance.holdingCard == this) return true; // �ڽ��� ����ִ� ���
        }

        return false;
    }

    public bool BattlePosCheck(int _first, int _child, bool _moveFistCard) // ������ ������ ��ġ���� Ȯ���Ѵ�.
    {
        Vector2 _myPos = transform.localPosition;
        Vector2 _cardSize = GameManager._instance.GetCardSize();
        Vector2 _moveFirstPos = new();
        Vector2 _moveChildPos;

        if (_moveFistCard) _moveFirstPos = _myPos + new Vector2((_first - 2) * -(_cardSize.x + SPACE) * (_first % 2), (_first - 1) * -(_cardSize.y + SPACE) * ((_first + 1) % 2));

        if (CheckMoveTrue(_moveFirstPos).z == 0) // ó��ī�尡 �̵� �������� Ȯ���Ѵ�.
        {
            int _length = GetCardList().Count;

            for (int i = 0; i < _length; i++) // ī�� ����� �̵� �������� Ȯ���Ѵ�.
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

        if (GetFirstCard() == null) // �ڽ��� ó��ī���� ���
        {
            SetTargetPos(transform.localPosition + _dir); // ������ ��ġ�� �����Ѵ�.
        }
        else
        {
            int _cardIndex = GetFirstCard().GetCardList().IndexOf(this) + 1;
            SetTargetPos((Vector3)GetFirstCard().GetTargetPos() + _dir * _cardIndex); // ������ ��ġ�� �����Ѵ�.
        }

        transform.SetAsLastSibling(); // ī�带 ���� ���� ǥ�õǰ� �Ѵ�.

        GameManager._instance.SetAchieveRecord("Battle Count", 1, true);

        StartCoroutine(Battle());
    }

    private IEnumerator Battle() // ������ �����Ѵ�.
    {
        if (GetEnemy() != null) yield return new WaitUntil(() => !GetEnemy().GetMove()); // ������ �̵��� ������ ���� ����Ѵ�.

        yield return new WaitUntil(() => !GetMove()); // �̵��� ������ ���� ����Ѵ�.

        List<CardManager> _enemyCardList = new();

        if (GetEnemy() != null) _enemyCardList = GetEnemy().GetCardList();

        Vector2 _pos = GetTargetPos();
        CardManager _target;

        while (GetEnemy() != null) // �������� ��쿡�� ����
        {
            yield return new WaitUntil(() => GameManager._instance.GetPlay()); // ������ �������� ��쿡�� �����Ѵ�.

            if (GetAtk() > 0) // ���ݷ��� �ִ� ��쿡�� �����Ѵ�.
            {
                if (GetNowCoolTime() <= 0) // ��Ÿ���� �� �� ���
                {
                    if (_enemyCardList.Count > 0) _target = _enemyCardList[^1]; // ������ ī�尡 �ִ� ���
                    else _target = GetEnemy();

                    if (GetEnemy() != null) // �������� ��쿡�� �����Ѵ�.
                    {
                        SetTargetPos(_target.GetTargetPos());

                        if (Vector2.Distance(transform.localPosition, GetTargetPos()) < 50) // �̵��� ���� ���
                        {
                            _target.SetHp(-GetAtk(), true);

                            if (_enemyCardList.Count > 0) _target = _enemyCardList[^1]; // ������ ī�尡 �ִ� ���
                            else _target = GetEnemy();

                            nowCoolTime = GetMaxCoolTime();
                            SetTargetPos(_pos); // ������ ��ġ�� �̵��Ѵ�.
                        }
                    }
                }
                else if (!GetMove() && !GetEnemy().GetMove()) nowCoolTime -= Time.deltaTime;
            }



            yield return null;
        }

        SetEnemy(null);
        nowCoolTime += GetMaxCoolTime();

        if (GetFirstCard() == null) // �ڽ��� ó�� ī���� ���
        {
            List<CardManager> _cardManager = GetCardList();
            int _length = _cardManager.Count;

            for (int i = 0; i < _length; i++) _cardManager[i].transform.SetAsLastSibling(); // ������ ī�带 ���� ���� ǥ�õǰ� �Ѵ�.
        }

        SetTargetPos(_pos); // ������ ��ġ�� �̵��Ѵ�.
    }

    // MOVE
    public Vector3 CheckMoveTrue(Vector2 _pos) // �̵��� �������� Ȯ���Ѵ�.
    {
        Vector3 _checkPos = new(_pos.x, _pos.y, 0);

        if (_checkPos != transform.localPosition) // �̵��Ϸ��� ��쿡�� Ȯ���Ѵ�.
        {
            Vector2 _dir = (_checkPos - transform.localPosition).normalized; // ���� ����
            float _dist = Vector2.Distance(transform.localPosition, _checkPos);
            RaycastHit2D _hit = Physics2D.BoxCast(transform.position, GameManager._instance.GetCardSize(), 0f, _dir, _dist, 1 << LayerMask.NameToLayer("Wall"));

            if (_hit) // ���� ���� ���
            {
                _checkPos = GameManager._instance.map.transform.InverseTransformPoint(_hit.point);
                _dir = (_checkPos - transform.localPosition).normalized; // ���� ����
                _checkPos -= new Vector3(_dir.x * (GameManager._instance.GetCardSize().x / 2 + 1), _dir.y * (GameManager._instance.GetCardSize().y / 2 + 1), 0);
                _checkPos.z = 1; // �̵��� �Ұ��� �ߴ� ��� �˷��ش�.
            }
            else
            {
                Vector2 _mapSize = GameManager._instance.GetMapSize();
                Vector2 _cardSize = GameManager._instance.GetCardSize();

                if (_checkPos.x > _mapSize.x - _cardSize.x * 0.5f) _checkPos = new(_mapSize.x - _cardSize.x * 0.5f, _checkPos.y);

                if (_checkPos.x < -(_mapSize.x - _cardSize.x * 0.5f)) _checkPos = new(-(_mapSize.x - _cardSize.x * 0.5f), _checkPos.y);

                if (_checkPos.y > _mapSize.y - _cardSize.y * 0.5f) _checkPos = new(_checkPos.x, _mapSize.y - _cardSize.y * 0.5f);

                if (_checkPos.y < -(_mapSize.y - _cardSize.y * 0.5f)) _checkPos = new(_checkPos.x, -(_mapSize.y - _cardSize.y * 0.5f));

                if ((Vector3)_pos != _checkPos) _checkPos.z = 1; // �̵��� �Ұ��� �ߴ� ��� �˷��ش�.
            }
        }

        return _checkPos;
    }

    private void FallowFirstCard() // ������ ��ġ�� �ڽ��� ó��ī��� �����Ѵ�.
    {
        int _cardIndex = GetFirstCard().GetCardList().IndexOf(this);
        Vector3 _dir = Vector3.down * (GameManager._instance.GetCardSize().y * 0.5f - SPACE);

        if (GetEnemy() == null) // �������� �ƴѰ�� ������ ��ġ�� �����Ѵ�.
        {
            if (_cardIndex == 0) SetTargetPos(GetFirstCard().transform.localPosition + _dir);
            else SetTargetPos(GetFirstCard().GetCardList()[_cardIndex - 1].transform.localPosition + _dir);
        }
    }

    private void MoveCard() // ī�尡 �����̰� �Ѵ�.
    {
        if (GetFirstCard() == null && !GetSafeMove()) SetTargetPos(CheckMoveTrue(GetTargetPos())); // �ڽ��� ó��ī���� ��� �̵��������� Ȯ���Ѵ�.

        AnimatorStateInfo _state = GetAnimator().GetCurrentAnimatorStateInfo(0);

        if (Vector2.Distance(transform.localPosition, GetTargetPos()) > 1) // Ÿ���� ��ġ�� ���� ��ġ�� �ָ��ִ� ���
        {
            bool _bool = true;

            if (HoldingCheck()) // �ڽ��� ��� �ִ� ���
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
                if (GetEnemy() != null) transform.localPosition = Vector2.Lerp(transform.localPosition, GetTargetPos(), moveSpeed * 1.5f * Time.deltaTime); // �������� ��� ������ �̵��Ѵ�.
                else transform.localPosition = Vector2.Lerp(transform.localPosition, GetTargetPos(), moveSpeed * Time.deltaTime); // Ÿ���� ��ġ�� �̵��Ѵ�.
            }

        }
        else if (Vector2.Distance(transform.localPosition, GetTargetPos()) >= 0) // Ÿ���� ��ġ�� ������� ���
        {
            transform.localPosition = GetTargetPos(); // Ÿ���� ��ġ�� �ٲ۴�.

            if (!HoldingCheck()) // �ڽ��� ��� ���� �ʴ� ���
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
        GameObject _instant = Instantiate(GameManager._instance.smallText, GameManager._instance.map.transform); // �޽����� ����Ѵ�.
        SmallText _smallText = _instant.GetComponent<SmallText>();

        _smallText.SetText(_num.ToString());
        _instant.transform.localPosition = transform.localPosition + new Vector3(0, GameManager._instance.GetCardSize().y / 2 - 5, 0);

        if (_num == 0)
        {
            if (_category == 2) // �� ��½� �ı� �޽����� ����� �ʴ´�.
            {
                _smallText.text.color = new(0, 0, 0);
                _smallText.SetText(0.ToString());
            }
            else
            {
                _instant.transform.localPosition += new Vector3(0, -20, 0);
                _smallText.SetText(GameManager._instance.GetDestroyText()); // �ı�
            }
        }
        else if (_num > 0) _smallText.text.color = new(0, 0, 255); // ������ ��� �Ķ������� ����

        if (_category == -1) _smallText.image.gameObject.SetActive(false);
        else _smallText.image.sprite = _smallText.sprites[_category];
    }
}
