using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BallSpawner : MonoBehaviour
{
    public NetworkPhysicsObject BallPrefab;
    public BallPhysicConfig ballPhysicConfig;
    public Transform groupBallPos;
    public Transform whiteBallPos;
    public List<Texture> ballTextures = new List<Texture>();


    //public const int WHITE_BALL_ID = 49;

    private Vector3[] cachedPos = new Vector3[16];
    [HideInInspector]
    public List<NetworkPhysicsObject> balls = new List<NetworkPhysicsObject>();
    public NetworkPhysicsObject[] Spawn(NetworkPhysicsObject whiteBall)
    {
        List<NetworkPhysicsObject> balls = new List<NetworkPhysicsObject>() { whiteBall };
        whiteBall.ballID = EightBallGameSystem.WHITE_BALL_ID;
        int counter = 1;
        int row = 5;
        // var whiteBall = Instantiate(BallPrefab);
        // whiteBall.name = "WhiteBall";
        // whiteBall.CreateBody(ballTextures[0]);
        // balls.Add(whiteBall);
        whiteBall.transform.position = whiteBallPos.position;
        return balls.ToArray();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                var ball = Instantiate(BallPrefab);
                ball.ballID = counter;
                ball.name = "Ball_" + counter.ToString();
                ball.CreateBody(ballTextures[counter], ballPhysicConfig);
                ball.GetComponent<Rigidbody>().maxAngularVelocity = 150;
                balls.Add(ball);
                var radius = ball.GetComponent<SphereCollider>().radius * ball.transform.localScale.x;
                var pos = new Vector3((i * radius * 2), (j * radius * 2) - radius, 0);
                // shift x by every row
                pos.y += (row - i) * radius;
                // shif z to fix into padding
                //Debug.Log($" ball {counter} z = {pos.z} --- offset padding = {pos.z - radius * 0.25}");
                pos.x += (row - i) * (radius * 0.25f);
                // shift to center
                var offset = (radius * row) - radius;
                pos += new Vector3(-offset, -offset, 0);
                // offset to this transform
                pos += groupBallPos.position;
                ball.transform.position = pos;
                counter++;
            }
        }
        return balls.ToArray();
    }

    public NetworkPhysicsObject[] Spawn_V2(NetworkPhysicsObject whiteBall)
    {
        balls.Clear();
        balls.Add(whiteBall);
        whiteBall.ballID = EightBallGameSystem.WHITE_BALL_ID;
        whiteBall.transform.position = whiteBallPos.position;
        whiteBall.Rigidbody.maxAngularVelocity = 150f;
        cachedPos[0] = whiteBallPos.position;
        int counter = 1;
        int row = 5;
        float noise = Random.Range(0.05f, 0.09f);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                var ball = Instantiate(BallPrefab);
                ball.ballID = counter;
                ball.name = "Ball_" + counter.ToString();
                ball.CreateBody(ballTextures[counter], ballPhysicConfig);
                ball.GetComponent<Rigidbody>().maxAngularVelocity = 150;
                balls.Add(ball);
                var ballR = ball.GetComponent<SphereCollider>().radius * ball.transform.localScale.x;
                var offset = i * ballR;
                var left = ballR * 2 * j - offset + (j * ballR * noise);
                var forward = ballR * 2 * i * 0.9f;
                var pos = new Vector3(forward, left, 0);
                //Debug.Log($"Ball id = {counter} - pos = {pos}");
                ball.transform.position = groupBallPos.TransformPoint(pos);
                cachedPos[counter] = ball.transform.position;
                counter++;
            }
        }
        return balls.ToArray();
    }

    public NetworkPhysicsObject[] Spawn_V3(NetworkPhysicsObject whiteBall, int[] ballIds)
    {
        balls.Clear();
        balls.Add(whiteBall);
        whiteBall.CreateBody(ballPhysicConfig);
        whiteBall.ballID = EightBallGameSystem.WHITE_BALL_ID;
        whiteBall.transform.position = whiteBallPos.position;
        whiteBall.Rigidbody.maxAngularVelocity = 150f;
        cachedPos[0] = whiteBallPos.position;
        var ballR = whiteBall.GetComponent<SphereCollider>().radius * whiteBall.transform.localScale.x;
        int counter = 1;
        int row = 5;
        float noise = Random.Range(-0.01f, 0.01f);
        // create formation position
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                var offset = i * ballR;
                var left = ballR * 2 * j - offset + (j * ballR * .1f);
                var forward = ballR * 2 * i * 0.95f;
                var pos = new Vector3(forward, left, 0);
                var worldPos = groupBallPos.TransformPoint(pos);
                if (counter == 1)
                {
                    //Debug.Log($"Counter = {counter} noise = {noise} wp = {worldPos}");
                    worldPos.x += noise;
                    //Debug.Log($"Counter = {counter} noise = {noise} ===> wp = {worldPos}");
                }
                
                cachedPos[counter] = worldPos;
                counter++;
            }
        }

        for (int i = 0; i < ballIds.Length; i++)
        {
            var ball = Instantiate(BallPrefab);
            ball.ballID = ballIds[i];
            ball.name = "Ball_" + ball.ballID.ToString();
            ball.CreateBody(ballTextures[ball.ballID], ballPhysicConfig);
            ball.GetComponent<Rigidbody>().maxAngularVelocity = 150;
            ball.transform.position = cachedPos[i + 1];
            balls.Add(ball);
        }

        return balls.ToArray();
    }

    public NetworkPhysicsObject[] SpawnByServer(NetworkPhysicsObject whiteBall, Dictionary<int, Vector3> ballPos)
    {
        //Spawn single
        balls.Clear();
        balls.Add(whiteBall);
        whiteBall.gameObject.SetActive(false);
        whiteBall.ballID = EightBallGameSystem.WHITE_BALL_ID;
        whiteBall.Rigidbody.maxAngularVelocity = 150f;
        foreach (var ballInfo in ballPos)
        {
            if (ballInfo.Key == EightBallGameSystem.WHITE_BALL_ID)
            {
                whiteBall.gameObject.SetActive(true);
                whiteBall.CreateBody(ballPhysicConfig);
                whiteBall.transform.position = new Vector3(ballInfo.Value.x, ballInfo.Value.y, ballInfo.Value.z);
                cachedPos[0] = ballInfo.Value;
                continue;
            }
            var ball = Instantiate(BallPrefab);
            ball.ballID = ballInfo.Key;
            ball.name = "Ball_" + ball.ballID.ToString();
            //ball id 8 is marked ball
            var ballTexture = ball.ballID == 8 ? ballTextures[5] : ballTextures[8];
            ball.CreateBody(ballTexture, ballPhysicConfig);
            ball.GetComponent<Rigidbody>().maxAngularVelocity = 150;
            cachedPos[ball.ballID] = new Vector3(ballInfo.Value.x, ballInfo.Value.y, ballInfo.Value.z);
            ball.transform.position = cachedPos[ball.ballID];
            Debug.Log("Final ball pos : " + ball.name + "===" + ball.transform.position);
            balls.Add(ball);
        }
        return balls.ToArray();
    }

    void SwapBallPos(int indexA, int indexB)
    {
        NetworkPhysicsObject temp = balls[indexA];
        balls[indexA] = balls[indexB];
        balls[indexB] = temp;
    }

    public void ReSpawn()
    {
        balls[0].ResetRBState();
        balls[0].transform.position = whiteBallPos.position;
        for (int i = 1; i < balls.Count; i++)
        {
            if (!balls[i].gameObject.activeSelf)
                balls[i].gameObject.SetActive(true);
            balls[i].ResetRBState();
            balls[i].transform.position = cachedPos[i];
            balls[i].transform.rotation = Quaternion.identity;
        }
    }
}
