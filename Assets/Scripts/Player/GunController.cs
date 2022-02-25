using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class GunController : MonoBehaviour
{
    public Transform gunBodyParent;

    public GunData[] guns;

    public LayerMask EnemyLayer;
    public int currentGunIndex;
    public Transform gunPoint;
    [Space(10)]
    public GameObject muzzelFlash;

    [Header("Reload Animation Time")]
    public float aimReloadTime;
    public float standReloadTime;

    public GameObject bulletHole;
    private RaycastHit rayCastHit;

    private bool CameraIsSet = false;

    private GunRecoil _gunRecoil;
    private StarterAssetsInputs _input;

    private Camera _cam;
    private AudioSource _audioSource;
    private PlayerAudioHandler _audioHandler;
    private ThirdPersonController _thirdPersonController;
    private ThirdPersonShooterController _thirdPersonShooterController;

    #region GunData Private Variables

    GameObject gunModel;
    AudioClip gunShotAudio;

    [Space(10)]
    [SerializeField] private bool shooting;
    [SerializeField] private bool reloading;
    [SerializeField] private bool readyToShoot;

    private int damage;
    private int bulletShot;
    private int magazineSize;
    private int bulletPerTap;
    private int bulletsLeft;

    private float spread;
    private float range;
    private float timeBetweenShots;
    private float timeBetweenShooting;

    #endregion

    private void Awake()
    {
        _cam = Camera.main;
        _gunRecoil = GetComponent<GunRecoil>();
        _input = GetComponent<StarterAssetsInputs>();
        _audioSource = GetComponentInChildren<AudioSource>();
        _audioHandler = GetComponent<PlayerAudioHandler>();

        _thirdPersonController = GetComponent<ThirdPersonController>();
        _thirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
    }

    private void Start()
    {

        SetGunData();
        GetGunModel();

        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        Input();
    }

    void SetGunData()
    {
        gunModel = guns[currentGunIndex].model;
        gunShotAudio = guns[currentGunIndex].gunShotAudio;

        damage = guns[currentGunIndex].damage;
        bulletShot = guns[currentGunIndex].bulletShot;
        magazineSize = guns[currentGunIndex].magazineSize;
        bulletPerTap = guns[currentGunIndex].bulletPerTap;

        spread = guns[currentGunIndex].spread;
        range = guns[currentGunIndex].range;
        timeBetweenShots = guns[currentGunIndex].timeBetweenShots;
        timeBetweenShooting = guns[currentGunIndex].timeBetweenShooting;
}

    void GetGunModel()
    {
        GameObject gunObject = Instantiate(gunModel, gunBodyParent.position, Quaternion.identity);
        gunObject.transform.SetParent(gunBodyParent);
        gunObject.transform.localPosition = Vector3.zero;
        gunObject.transform.localRotation = Quaternion.identity;
    }

    private void Input()
    {
        shooting = _input.shoot;

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
            Shoot();

        //Reloading
        if (_input.reload && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (bulletsLeft == 0 && !reloading)
            Reload();

        if(!_input.shoot && !CameraIsSet)
        {
            CameraIsSet = true;
            _thirdPersonController.SetCameraAngle();
        }
        
    }

    private void Shoot()
    {
        readyToShoot = false;
        CameraIsSet = false;

        bulletsLeft--;

        PlaySoundAndParticle();

        //Ray Casting
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        //Recoil
        _gunRecoil.GenerateRecoil();
        _thirdPersonController.SetCameraOverride(_gunRecoil.verticalRecoil);

        Physics.Raycast(ray, out rayCastHit, range, EnemyLayer);
        Instantiate(bulletHole, rayCastHit.point, Quaternion.identity);

        if (Physics.Raycast(ray, out rayCastHit, range, EnemyLayer))
        {
            //if (rayCastHit.collider.CompareTag("Enemy"))
            //    Debug.Log("<color=cyan> EnemyHit </color>");
        }

        Invoke("ResetShot", timeBetweenShooting);
    }

    private void ResetShot()
    {
        //Call by Shoot via Invoke Function

        readyToShoot = true;
        _gunRecoil.ResetFrequency();
    }

    private void Reload()
    {
        reloading = true;

        StartCoroutine(FinishReloading());

    }

    IEnumerator FinishReloading()
    {
        float reloadTime;

        if (_input.aim || _input.shoot)
            reloadTime = aimReloadTime;
        else
            reloadTime = standReloadTime;

        _thirdPersonShooterController.PlayReloadAnimation(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        bulletsLeft = magazineSize;
        reloading = false;
    }

    private void PlaySoundAndParticle()
    {
        GameObject muzzelFlashClone = Instantiate(muzzelFlash, gunPoint, false);
        Destroy(muzzelFlashClone, 0.3f);

        _audioHandler.PlayGunSound(gunShotAudio);
    }













}//class
