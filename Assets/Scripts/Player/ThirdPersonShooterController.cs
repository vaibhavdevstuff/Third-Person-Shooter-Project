using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    [Header("Camera Data")]
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask mouseColliderLayer = new LayerMask();
    [SerializeField] private Transform debugTransform;

    [Header("Animation Rigging")]
    [SerializeField] private Rig bodyAimRig;
    [SerializeField] private Rig weaponAimRig;
    [SerializeField] private TwoBoneIKConstraint leftHandRig;

    [Header("Particles")]
    public ParticleSystem WoodParticle;
    public ParticleSystem MetalSpark;
    public ParticleSystem BloodSplash;

    //Animation IDs
    private int _animIDHorizontalMove;
    private int _animIDVerticalMove;
    private int _animIDAim;
    private int _animIDReload;

    //Data
    private bool _isReloading;
    private Transform _hitTransform = null;
    private Vector3 _mouseWorldPosition = Vector3.zero;

    //Sound Data
    private float _footstepDazeTime = 0f;


    private Animator _animator;
    private StarterAssetsInputs _input;
    private PlayerAudioHandler _audioHandler;
    private ThirdPersonController _thirdPersonController;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponent<StarterAssetsInputs>();
        _audioHandler = GetComponent<PlayerAudioHandler>();
        _thirdPersonController = GetComponent<ThirdPersonController>();
        AssignAnimationIDs();

        Application.targetFrameRate = 60;//-----------------------------------------------------------NEED TO BE REMOVED LATER-----||
    }

    private void Update()
    {
        leftHandRig.weight = _animator.GetFloat("LeftHandRigWeight");
    }

    private void FixedUpdate()
    {
        

        HitDetection();
        Aiming();
        FootStepsSound();
    }


    private void AssignAnimationIDs()
    {
        _animIDHorizontalMove = Animator.StringToHash("HorizontalMove");
        _animIDVerticalMove = Animator.StringToHash("VerticalMove");
        _animIDAim = Animator.StringToHash("Aim");
        _animIDReload = Animator.StringToHash("Reload");
    }

    private void HitDetection()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit rayCastHit, 999f, mouseColliderLayer))
        {
            //debugTransform.position = rayCastHit.point;
            _mouseWorldPosition = rayCastHit.point;
            _hitTransform = rayCastHit.transform;
        }

    }

    private void Aiming()
    {
        if (_input.shoot || _input.aim)
        {
            float timeMultiplier = 20f;

            if (_input.shoot)
                timeMultiplier = 40;

            if (_input.aim)
                aimVirtualCamera.gameObject.SetActive(true);
            else
                aimVirtualCamera.gameObject.SetActive(false);

            _thirdPersonController.SetSensitivity(aimSensitivity);
            _thirdPersonController.SetRotateOnMove(false);

            Vector3 worldAimTarget = _mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * timeMultiplier);

            //Animations
            SetAnimMoveDirection();
            _animator.SetBool(_animIDAim, true);
            //_animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * timeMultiplier));
            //_animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 1f, Time.deltaTime * timeMultiplier));

            //Animation Rig
            if (!_isReloading)
            {
                bodyAimRig.weight = Mathf.Lerp(bodyAimRig.weight, 1f, Time.deltaTime * timeMultiplier);
                weaponAimRig.weight = Mathf.Lerp(weaponAimRig.weight, 1f, Time.deltaTime * timeMultiplier);
            }
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            _thirdPersonController.SetSensitivity(normalSensitivity);
            _thirdPersonController.SetRotateOnMove(true);

            //Animations
            _animator.SetBool(_animIDAim, false);
            //_animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            //_animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));

            //Animation Rig
            bodyAimRig.weight = Mathf.Lerp(bodyAimRig.weight, 0f, Time.deltaTime * 20f);
            weaponAimRig.weight = Mathf.Lerp(weaponAimRig.weight, 0f, Time.deltaTime * 20f);

            
        }
    }

    private void SetAnimMoveDirection()
    {
        float moveX = Mathf.Lerp(_animator.GetFloat(_animIDHorizontalMove), _input.move.x, Time.deltaTime * 10f);
        float moveZ = Mathf.Lerp(_animator.GetFloat(_animIDVerticalMove), _input.move.y, Time.deltaTime * 10f);

        _animator.SetFloat(_animIDHorizontalMove, moveX);
        _animator.SetFloat(_animIDVerticalMove, moveZ);
    }

    private void FootStepsSound()
    {
        if(_input.move.magnitude > 0.1)
        {
            _footstepDazeTime += Time.deltaTime;

            if(_footstepDazeTime >= (_animator.GetCurrentAnimatorStateInfo(0).length / 2))
            {
                _footstepDazeTime = 0f;
                _audioHandler.PlayFootstepSound();
            }
        }
        else
        {
            _footstepDazeTime = 0f;
        }
    }

    public void PlayReloadAnimation(float waitTime)
    {
        StartCoroutine(Reload(waitTime));
    }

    IEnumerator Reload(float waitTime)
    {
       _isReloading = true;

        bodyAimRig.weight = 0f;
        weaponAimRig.weight = 0f;

        _animator.SetTrigger(_animIDReload);

        yield return new WaitForSeconds(waitTime);

        _isReloading = false;


    }





















}//class
