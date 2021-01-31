using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryDebug : MonoBehaviour
{
    private GameLogicScript gameLogic;
    private BallScript basketball;

    public GoalScript targetGoal;

    private GameObject pointPrefab;

    private float shotA, shotB, shotC;
    private float oldShotA, oldShotB, oldShotC;

    public float xGap;

    [Range(0f, 90f)]
    public float shotAngle = 45f;

    [Range(0.0f, 1.0f)]
    public float distanceFromPlayer;

    private ArrayList pointList;

    // Start is called before the first frame update
    void Start()
    {
        shotA = 0;
        shotB = 0;
        shotC = 0;

        oldShotA = 0;
        oldShotB = 0;
        oldShotC = 0;

        pointList = new ArrayList();

        pointPrefab = Resources.Load("Prefabs/Point") as GameObject;

        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogicScript>();
        //basketball = gameLogic.GetBasketball();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateShootingTrajectory();

        if (shotA != oldShotA || shotB != oldShotB || shotC != oldShotC) {
            ResetPoints();
        }
    }

    private void ResetPoints()
    {
        foreach(GameObject point in pointList) {
            Destroy(point);
        }

        pointList.Clear();

        float x = basketball.transform.position.x;

        while(x <= targetGoal.basketCenter.position.x) {
            float newX = x;
            float newY = (shotA * newX * newX) + (shotB * newX) + shotC;

            pointList.Add(Instantiate(pointPrefab, new Vector2(newX, newY), Quaternion.identity));

            x += xGap;
        }

        oldShotA = shotA;
        oldShotB = shotB;
        oldShotC = shotC;
    }

    //Math credit to: https://www.desmos.com/calculator/lac2i0bgum
    private void CalculateShootingTrajectory()
    {
        //Set variables needed for calculating shot speed
        bool shotRightOfGoal = (targetGoal.basketCenter.position.x < transform.position.x) ? true : false;

        Vector2 point1 = basketball.transform.position;
        Vector2 point3 = targetGoal.basketCenter.position;

        //This is my logic
        float distToGoal = Mathf.Abs(point3.x - point1.x);
        if (shotRightOfGoal == true) distToGoal *= -1;

        float point2X = point1.x + distToGoal * distanceFromPlayer;
        float point2Y = point3.y + (Mathf.Tan((shotAngle)*(Mathf.PI / 180)) * Mathf.Abs(point3.x - point2X));
        Vector2 point2 = new Vector2(point2X, point2Y);

        //Algorithm
        float A1 = -1 * (point1.x * point1.x) + (point2.x * point2.x);
        float B1 = -1 * point1.x + point2.x;
        float D1 = -1 * point1.y + point2.y;

        float A2 = -1 * (point2.x * point2.x) + (point3.x * point3.x);
        float B2 = -1 * point2.x + point3.x;
        float D2 = -1 * point2.y + point3.y;

        float Bmultiplier = -1 * (B2 / B1);
        float A3 = Bmultiplier * A1 + A2;
        float D3 = Bmultiplier * D1 + D2;

        shotA = D3 / A3;
        shotB = (D1 - A1 * shotA) / B1;
        shotC = point1.y - (shotA * (point1.x * point1.x)) - (shotB * point1.x);
    }
}
