using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HcGames;
using TMPro;

public class TutorialController : MonoBehaviour
{
    public Camera cameraTutorial;
    public Canvas canvasTutorial;
    public GameObject arrow;
    public RectTransform handTutorialCue;
    public GameObject txtGoodLuck;
    public TMP_Text txtSwipe;
    public TMP_Text txtGoodJob;
    public EndTutorial8Ball endTutorial8Ball;
    private NetworkPhysicsObject[] balls;
    public Dictionary<int, Vector3> ballsPos;
    public int ballIdTarget;
    public int ballIdOnpoted = -1;
    private int countShoot;

    public void ActiveBall()
    {
        if (balls == null || balls.Length < 1) return;
        foreach(var ball in balls)
        {
            if (ball.ballID != ballIdTarget && ball.ballID != EightBallGameSystem.WHITE_BALL_ID)
                continue;
            ball.ChangeLayer("tutorial");
        }
    }
    public void ResetBall()
    {
        if (balls == null || balls.Length < 1) return;
        foreach (var ball in balls)
        {
            ball.ChangeLayer("Default");
        }
    }
    public void HideTutorial()
    {
        cameraTutorial.enabled = false;
        canvasTutorial.gameObject.SetActive(false);
    }
    public void ShowTutorial()
    {
        cameraTutorial.enabled = true;
        canvasTutorial.gameObject.SetActive(true);
    }
    public void StartTutorial(NetworkPhysicsObject[] _balls, Dictionary<int, Vector3> _ballsPos)
    {
        ballsPos = _ballsPos;
        balls = _balls;
        ballIdTarget = balls[1].ballID;
        txtSwipe.text = "";
        handTutorialCue.gameObject.SetActive(false);
        ShowStep1();
    }
    public void EndShoot()
    {
        Debug.Log($"EndShoot ballIdTarget = {ballIdTarget} ballIdOnpoted = {ballIdOnpoted}");
        if (ballIdTarget == ballIdOnpoted) {
            countShoot += 1;
            Debug.Log($"EndShoot countShoot = {countShoot} ballsPos.Count = {ballsPos.Count}");
            if (countShoot < ballsPos.Count-1)
            {
                NextTarget();
            }
            else
            {
                EndTutorial();
                return;
            }
        }
        GameController.Instance.ResetBall();
        ballIdOnpoted = -1;
        ShowStep1();
    }
    public void ShowStep1()//
    {
        ShowTutorial();
        if (txtGoodLuck != null) txtGoodLuck.SetActive(false);
        ActiveBall();
        txtSwipe.text = "Drag your finger to adjust \n the ball direction.";
        arrow.SetActive(true);
        arrow.transform.DOLocalMoveX(1.5f, 1f).SetEase(Ease.Linear).SetLoops(-1);
    }
    public void CompleteStep1()
    {
        ResetBall();
        arrow.SetActive(false);
        HideTutorial();
    }
    public void ShowStep2()
    {
        ActiveBall();
        ShowTutorial();
        arrow.transform.DOKill();
        arrow.SetActive(false);
        txtSwipe.text = "Swipe your finger to \n adjust the hitting force.";
        handTutorialCue.DOAnchorPosY(-40,1f).SetEase(Ease.Linear).SetLoops(-1);
        handTutorialCue.gameObject.SetActive(true);
        GameController.Instance.refreshTurnUI();
    }
    public void NextTarget()
    {
        ballIdTarget = balls[2].ballID;
        Debug.Log($"NextTarget ballIdTarget = {ballIdTarget}");
    }
    public void CompleteStep2()
    {
        ResetBall();
        HideTutorial();
        arrow.SetActive(false);
        handTutorialCue.DOKill();
        handTutorialCue.gameObject.SetActive(false);
    }
    public void EndTutorial()
    {
        StartCoroutine(EndTutorialView());
    }
    private IEnumerator EndTutorialView()
    {
        Debug.Log("EndTutorial");
        ShowTutorial();
        txtSwipe.text = "";
        txtGoodJob.gameObject.SetActive(true);
        txtGoodJob.text = "GOOD JOB";
        handTutorialCue.gameObject.SetActive(false);
        arrow.SetActive(false);
        //show anim character
        yield return new WaitForSeconds(2);
        endTutorial8Ball.Show();
    }
}
