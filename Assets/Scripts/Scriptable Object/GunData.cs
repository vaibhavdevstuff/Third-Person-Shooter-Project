using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class GunData : ScriptableObject
{
    [Header("Gun Stats")]
    public string lable;
    public GameObject model;
    public AudioClip gunShotAudio;

    [Header("Boolean Data")]
    [HideInInspector] public bool allowButtonHold;

    [Header("Bullet Data")]
    public int damage;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;
    public float timeBetweenShooting;

    public int bulletShot;
    public int magazineSize;
    public int bulletPerTap;









}//class
