using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BonusGame;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoinFlyAnimation : ManualSingletonMono<CoinFlyAnimation>
{
    [SerializeField] private RectTransform goldPos;
    [SerializeField] private RectTransform tokenPos;
    [SerializeField] private RectTransform ticketPos;

    [SerializeField] private Image goldPrefab;
    [SerializeField] private Image tokenPrefab;
    [SerializeField] private Image ticketPrefab;
    [SerializeField] private Image addTurnWheelPrefab;

    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float timeInterval = 0.5f;
    [SerializeField] private RectTransform holderCoin;
    public Ease ease;

    private Vector2 _localGoldPos = new Vector2(-250, 850);
    private Vector2 _localTokenPos = new Vector2(0, 850);
    private Vector2 _localTicketPos = new Vector2(250, 850);
    private Vector3[] _path;

    private void Start()
    {
        for (int i = holderCoin.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holderCoin.GetChild(i));
        }

        _localGoldPos = goldPos.anchoredPosition;
        _localTokenPos = tokenPos.anchoredPosition;
        _localTicketPos = ticketPos.anchoredPosition;
    }


    [Button]
    public void SpawnListBonusGameRewardClaim(List<wheelRewardType> listReward, Vector2 startPosition,
        Action onCompleteMove = null)
    {
        SetDefault();
        for (int i = 0; i < listReward.Count; i++)
        {
            switch (listReward[i])
            {
                case wheelRewardType.gold:
                    SpawnReward(10, startPosition,
                        _localGoldPos,
                        RewardType.Gold);
                    break;
                case wheelRewardType.hc_token:
                    SpawnReward(10, startPosition,
                        _localTokenPos,
                        RewardType.Token);
                    break;
                case wheelRewardType.ticket:
                    SpawnReward(10, startPosition,
                        _localTicketPos,
                        RewardType.Ticket);
                    break;
                default:
                    break;
            }
        }
    }


    [Button]
    public void Test()
    {
        ListReward tmp = new ListReward();
        tmp.Reward.Add(new Reward()
        {
            Reward_ = 10,
            RewardType = 1
        });
        tmp.Reward.Add(new Reward()
        {
            Reward_ = 10,
            RewardType = 2
        });
        SpawnListRewardClaim(tmp, new Vector2(0, 0));
    }

    [Button]
    public void SpawnListRewardClaim(ListReward listReward, Vector2 startPosition, Action onCompleteMove = null)
    {
        SetDefault();
        var tmpListAvailable = listReward.Reward.Where(s => s.Reward_ > 0).ToList();
        for (int i = 0; i < tmpListAvailable.Count; i++)
        {
            switch (tmpListAvailable[i].RewardType)
            {
                case 1:
                    SpawnReward(10, startPosition,
                        _localGoldPos,
                        RewardType.Gold);
                    break;
                case 2:
                    SpawnReward(10, startPosition,
                        _localTokenPos,
                        RewardType.Token);
                    break;
                case 3:
                    SpawnReward(10, startPosition,
                        _localTicketPos,
                        RewardType.Ticket);
                    break;
                default:
                    break;
            }
        }
    }


    public void SpawnRewardClaim(Reward reward, Vector2 startPosition, Action onCompleteMove = null)
    {
        SetDefault();
        switch (reward.RewardType)
        {
            case 1:
                SpawnReward(5, startPosition, _localGoldPos
                    ,
                    RewardType.Gold);
                break;
            case 2:
                SpawnReward(5, startPosition,
                    _localTokenPos,
                    RewardType.Token);
                break;
            case 3:
                SpawnReward(5, startPosition,
                    _localTicketPos,
                    RewardType.Ticket);
                break;
            default:
                break;
        }
    }

    private void SetDefault()
    {
        for (int i = holderCoin.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holderCoin.GetChild(i));
        }
    }


    [Button]
    public void SpawnReward(int countSpawn, Vector2 startPosition, Vector2 endPosition, RewardType type,
        Action onCompleteMove = null)
    {
        switch (type)
        {
            case RewardType.Gold:
                StartCoroutine(CoinFly(countSpawn, holderCoin, startPosition, endPosition, onCompleteMove));
                break;
            case RewardType.Token:
                StartCoroutine(TokenFly(countSpawn, holderCoin, startPosition, endPosition, onCompleteMove));
                break;
            case RewardType.Ticket:
                StartCoroutine(TicketFly(countSpawn, holderCoin, startPosition, endPosition, onCompleteMove));
                break;
            default:
                break;
        }
    }

    //TAT CA VI TRI THE GIOI
    private IEnumerator CoinFly(int countSpawn, Transform parent, Vector2 startPosition, Vector2 endPosition,
        Action onComplete = null)
    {
        for (int i = 0; i < countSpawn; i++)
        {
            var coin = BonusPool.Spawn(goldPrefab, parent);
            MoveCoin(coin, startPosition, endPosition, onComplete);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }


    private IEnumerator TokenFly(int countSpawn, Transform parent, Vector2 startPosition, Vector2 endPosition,
        Action onComplete = null)
    {
        for (int i = 0; i < countSpawn; i++)
        {
            var coin = BonusPool.Spawn(tokenPrefab, parent);
            MoveCoin(coin, startPosition, endPosition, onComplete);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }

    private IEnumerator TicketFly(int countSpawn, Transform parent, Vector2 startPosition, Vector2 endPosition,
        Action onComplete = null)
    {
        for (int i = 0; i < countSpawn; i++)
        {
            var coin = BonusPool.Spawn(ticketPrefab, parent);
            MoveCoin(coin, startPosition, endPosition, onComplete);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }

    private IEnumerator AddTurnWheelFly(int countSpawn, Transform parent, Vector2 startPosition, Vector2 endPosition,
        Action onComplete = null)
    {
        for (int i = 0; i < countSpawn; i++)
        {
            var coin = BonusPool.Spawn(addTurnWheelPrefab, parent);
            MoveCoin(coin, startPosition, endPosition, onComplete);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }

    private bool BelowVector(Vector3 p1, Vector3 p2, Vector3 checkPoint)
    {
        var v1 = p2 - p1;
        var v2 = checkPoint - p1;
        var cross = Vector3.Cross(v1, v2);
        return cross.z <= 0;
    }

    private void MoveCoin(Image coin, Vector2 startPosition, Vector2 endPosition, Action onComplete = null)
    {
        var coinTrs = coin.rectTransform;
        coinTrs.localScale = Vector3.zero;
        coinTrs.anchoredPosition = startPosition;
        var dir = Random.insideUnitCircle - Vector2.zero;
        var position = dir.normalized * Random.Range(100f, 150f) + startPosition;
        var dis = Vector3.Distance(startPosition, endPosition);
        var dur = dis / 100 * duration;
        var deltaMid = dis / 1000 * 200;
        var se = DOTween.Sequence();
        var isBelow = BelowVector(startPosition, endPosition, position);
        var path = CreatePath(coinTrs.anchoredPosition, endPosition, deltaMid, isBelow);
        var size = Random.Range(0.65f, 0.8f);
        se.Append(coinTrs.DOAnchorPos(position, 0.2f))
            .Join(coinTrs.DOScale(size + 0.1f, 0.2f))
            .Append(coinTrs.DOScale(size, 0.02f))
            .AppendInterval(timeInterval)
            //.Append(coinTrs.DOAnchorPos(endPosition, 5f)
            .Append(coinTrs.DOLocalPath(path, dur, PathType.CatmullRom).SetEase(ease))
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                BonusPool.DeSpawn(coin);
            });
    }

    private Vector3[] CreatePath(Vector3 start, Vector3 end, float deltaMid, bool isBelow)
    {
        var p = end - start;
        var n = isBelow ? new Vector3(p.y, -p.x, 0) : new Vector3(-p.y, p.x);
        var midPoint = (start + end) / 2;
        var midPath = n.normalized * Random.Range(-deltaMid, deltaMid) + midPoint;
        _path = new[] { start, midPath, end };
        return _path;
    }
}