using UnityEngine;

[CreateAssetMenu(fileName = "New Ball Params", menuName = "Ball Params")]
public class BallPhysicsAttributesSO : ScriptableObject
{
    [Header("Loose Ball Fields")]
    [SerializeField] private float minBounceVelocity;
    public float MinBounceVelocity => minBounceVelocity;

    [SerializeField] private float bounceFactor;
    public float BounceFactor => bounceFactor;

    [SerializeField] private float reboundFloorMargin;
    public float ReboundFloorMargin => reboundFloorMargin;

    [SerializeField] private float minBlockVelocityX;
    public float MinBlockVelocityX => minBlockVelocityX;

    [SerializeField] private float maxBlockVelocityX;
    public float MaxBlockVelocityX => maxBlockVelocityX;

    [SerializeField] private float blockVelocityY;
    public float BlockVelocityY => blockVelocityY;

    [Header("Shoot Fields")]
    [SerializeField] private float maxRegShotSpeed;
    public float MaxRegShotSpeed => maxRegShotSpeed;

    [SerializeField] private float minShotSpeed;
    public float MinShotSpeed => minShotSpeed;

    [SerializeField] private float maxShotSpeed;
    public float MaxShotSpeed => maxShotSpeed;

    [SerializeField] private float shotSpeedDamper;
    public float ShotSpeedDamper => shotSpeedDamper;

    [SerializeField] private float shotAngleDegrees;
    public float ShotAngle => shotAngleDegrees * (Mathf.PI / 180);

    [SerializeField] private float shotAngleXPercent;
    public float ShotAngleXPercent => shotAngleXPercent;

    [SerializeField] private float shotLowerSpeedMod;
    public float ShotLowerSpeedMod => shotLowerSpeedMod;

    [SerializeField] private float shotUpperSpeedMod;
    public float ShotUpperSpeedMod => shotUpperSpeedMod;

    [SerializeField] private float shotHalfCourtPos;
    public float ShotHalfCourtPos => shotHalfCourtPos;

    [SerializeField] private float shotFarthestPos;
    public float ShotFarthestPos => shotFarthestPos;

    [SerializeField] private float maxBlockableAngle;
    public float MaxBlockableAngle => maxBlockableAngle;

    [SerializeField] private float maxBlockableDistance;
    public float MaxBlockableDistance => maxBlockableDistance;

    [Header("Pass Fields")]
    [SerializeField] private float passingSpeed;
    public float PassingSpeed => passingSpeed;
}
