using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScript : MonoBehaviour
{
    //public GameObject dummyObj = new GameObject("dummyObj");
    //create enum for future additions
    enum Skill
    {
        impactGrenade
    }
    enum Firearm
    {
        subMachineGun,
        pistol
    }
    enum Melee
    {
        combatKnife
    }

    //Scene Manager
    [Header("Scene Manager")]
    [Tooltip("Current Active SceneManager")]
    public GameObject sceneManager;

    [Header("Camera")]
    [Tooltip("Current player's camera")]
    public GameObject playerCam;
    [Tooltip("Current player's weapon POV camera")]
    public GameObject weaponCam;
    [Tooltip("Current player's weapon container")]
    public GameObject weaponContainer;
    [Tooltip("Current player's weapon container for 1st person camera")]
    public GameObject weaponContainerPOV;
    [Tooltip("Current player's weapon container for unequipped weapon on back")]
    public GameObject weaponBackDisplayContainer;

    [Header("Images")]
    public Image img_crossHair;
    public Image img_bulletIcon;
    public Image img_reloadRing;

    [Header("Textures")]
    [Tooltip("Current bullethole textures")]
    public GameObject[] BulletTextures;

    [Header("Partical System")]
    [SerializeField] private ParticleSystem hitTargetParticle;
    [SerializeField] private ParticleSystem hitOthersParticle;

    [Header("Audio Objects")]
    public GameObject HittingAudioObject;

    [Header("Game Rules")]
    [Tooltip("Current score (if the gamemode is scored)")]
    public int score;
    [Tooltip("Current gamemode is timed or not")]
    public bool mode_Timed;
    [Tooltip("Current gamemode is scored or not")]
    public bool mode_Scored;
    [Tooltip("Time limit")]
    [SerializeField] private float timeLimit = 45.00f;
    private float timeLeft = 45.00f;

    [Header("Weapon")]
    //WIP weapon system rework
    /*[Tooltip("Current primary weapon instance")]
    public Weapon primaryWeapon;
    [Tooltip("Current primary weapon instance for 1st person camera")]
    public Weapon primaryWeaponPOV;
    [Tooltip("Current secondary weapon instance")]
    public Weapon secondaryWeapon;
    [Tooltip("Current secondary weapon instance for 1st person camera")]
    public Weapon secondaryWeaponPOV;*/
    [Tooltip("If there's no weapon in inventory")]
    public bool noWeapon =true;
    [Tooltip("If there's no weapon equipped")]
    public bool weaponEquipped = false;
    [Tooltip("If both weapon slots equipped")]
    public bool weaponFull = false;
    [Tooltip("If handling weapon (like switching) to freeze weapon firing and other functions")]
    public bool weaponHandling = false;
    [Tooltip("If run out of ammunation")]
    public bool currentNoAmmo = false;
    [Tooltip("Weapon slot active")] // CAUTION: Cannot switch slot if empty handed or switch to unequipped/fist
    public int currentWeaponSlot = 0;
    [Tooltip("Current weapon gameObject")]
    public GameObject currentWeapon;
    [Tooltip("Current weapon gameObject for 1st person camera")]
    public GameObject currentWeaponPOV;
    /*[Tooltip("Current weapon current ammo in magazine")]
    public int currentWeaponCurrentMagAmmo = 0;
    [Tooltip("Current weapon total ammo left")]
    public int currentWeaponTotalAmmo = 0;
    [Tooltip("Current weapon maximum ammo storage")]
    public int currentWeaponMaxAmmo = 0;*/
    [Tooltip("Primary weapon gameObject")]
    public GameObject primaryWeapon;
    [Tooltip("Primary weapon gameObject for 1st person camera")]
    public GameObject primaryWeaponPOV;
    [Tooltip("Primary weapon gameObject for back display")]
    public GameObject primaryWeaponBackDisplay;
    /*[Tooltip("Primary weapon current ammo in magazine")]
    public int primaryWeaponCurrentMagAmmo = 0;
    [Tooltip("Primary weapon total ammo left")]
    public int primaryWeaponTotalAmmo = 0;
    [Tooltip("Primary weapon maximum ammo storage")]
    public int primaryWeaponMaxAmmo = 0;*/
    [Tooltip("Secondary weapon gameObject")]
    public GameObject secondaryWeapon;
    [Tooltip("Secondary weapon gameObject for 1st person camera")]
    public GameObject secondaryWeaponPOV;
    [Tooltip("Secondary weapon gameObject for back display")]
    public GameObject secondaryWeaponBackDisplay;
/*    [Tooltip("Secondary weapon current ammo in magazine")]
    public int secondaryWeaponCurrentMagAmmo = 0;
    [Tooltip("Secondary weapon total ammo left")]
    public int secondaryWeaponTotalAmmo = 0;
    [Tooltip("Secondary weapon maximum ammo storage")]
    public int secondaryWeaponMaxAmmo = 0;*/
    [Tooltip("Bullet gameObject for equiped weapon (if it's a gun)")]
    public GameObject currentBullet;
    public GameObject primaryBullet;
    public GameObject secondaryBullet;

    [Header("WIP Content")]
    public float weaponPutDownTime = 0.3f;
    public float weaponPickupTime = 0.5f;

    //TODO: weapon details should come with weapon, consider making enums and dictionary or general weapon class
    /*public float firingRate = 10f;
    public bool autoTrigger;
    public int magSize = 30;
    public float reloadTime = 1.5f;*/

    [Header("Equipment/Skill")]
    //TODO: Create dictionary containing equipments so this becomes "skillObj"s and the current obj are passed in dynamically
    public GameObject[] skillObjs;
    [Tooltip("Current primary skill/equipment gameObject (WIP)")]
    public GameObject primarySkillObj;
    [Tooltip("Current secondary skill/equipment gameObject (WIP)")]
    public GameObject secondarySkillObj;

    //Skill/Equipment (when there are more skills would put data in dictionary for better management)
    [SerializeField] private Skill skill;
    [SerializeField] private float skillCoolDown = 5f;
    [SerializeField] private bool skillReady = true;
    [SerializeField] private float skillTime = 1f; //Skill Interval
    private float skillCDCounter = 0f;

    [Header("Bool Conditions")]
    [SerializeField] private bool levelEnded = false;
    [SerializeField] private bool levelPaused = false;
    [SerializeField] private bool reloading = false;
    [SerializeField] private bool tacticalReloading = false;
    [SerializeField] private bool emptyReloading = false;
    [SerializeField] private bool skillUsing = false;

    [Header("References")]
    public Transform firePoint;
    //public Transform detectPoint;
    public Transform projectileFirePoint;
    public Text scoreText;
    public Text ammoBackupText;
    public Text ammoCurrentMagText;
    public Text weaponText;
    public Text timeText;
    public Text skillText;
    [Tooltip("If there's weaponset selected")]
    public bool hasWeaponSet;

    [Header("Raycasting LayerMask")]
    public LayerMask layerMask;

    [Header("Key Bindings")]
    [SerializeField] private KeyCode skillKey = KeyCode.G;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;

    ///PRIVATE VARIABLES
    //Ray
    private Ray ray;
    private RaycastHit raycastHit;
    /*//Current bullets left in mag
    private int currentMag = 0;*/
    //Reload counter
    private float reloadingTime = 0f;
    //Skill counter
    private float skillUsingTime = 0f;
    //Check if bullets are enough to shoot
    private bool bulletEnough = true;
    //Firing rate interval
    private float fRateInt;
    //Firing gap counter
    private float fGapCount = 0f;
    //able to shoot status
    private bool fRatePassed = true;
    //IMPORTANT: UPDATING PRESSINFO
    private bool EscPressed = false;


    void Start()
    {
        score = 0;
        timeLeft = timeLimit;
        scoreText.text = "Score: " + score.ToString();
        timeText.text = "Time: " + timeLeft.ToString();
        if (!mode_Scored)
            scoreText.enabled = false;
        if (!mode_Timed)
            timeText.enabled = false;
        if(!hasWeaponSet)
            weaponEquipped = false;
        Time.timeScale = 1;
        //player = this.transform.parent.gameObject;

        if (skill == Skill.impactGrenade)
        {
            skillTime = 0.5f;
            skillCoolDown = 5f;
        }
        if(weaponEquipped)
            UpdateWeaponInfo();
    }

    void Update()
    {
        if(mode_Scored && score == 50)
        {
            this.Perfect();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            EscPressed = false;
        }
        if (EscPressed && !levelPaused && !levelEnded)
        {
            EscPressed = false;
            this.Pause();
        }
        else if (EscPressed && levelPaused && !levelEnded)
        {
            EscPressed = false;
            this.Resume();
        }

        if (weaponEquipped)
        {
            if (!reloading)
                ammoCurrentMagText.text = currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo.ToString();
            bulletEnough = currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo > 0;
        }

        //Switching weapon logic

        //Shooting logic
        if (!levelPaused && !levelEnded)
        {
            // Get a ray from the camera pointing forwards
            ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            if (weaponEquipped)
            {
                if (!currentWeapon.GetComponent<WeaponInfo>().melee)
                {
                    if (reloading)
                    {
                        currentWeapon.GetComponent<animController>().animator.SetBool("isReloading", false);
                        currentWeaponPOV.GetComponent<animController>().animator.SetBool("isReloading", false);
                    }
                    else if (fRatePassed && !skillUsing)
                    {
                        if( (Input.GetKey(attackKey) && currentWeapon.GetComponent<WeaponInfo>().fireMode == FireSelect.auto) ||
                            (Input.GetKeyDown(attackKey) && currentWeapon.GetComponent<WeaponInfo>().fireMode != FireSelect.auto))
                        {
                            if (!bulletEnough)
                            {
                                if (!currentNoAmmo) //Empty reload
                                {
                                    reloading = true;
                                    emptyReloading = true;
                                    //ammoText.text = "Reloading";
                                    //currentWeapon.GetComponent<animController>().ReloadAnimation();
                                    //currentWeaponPOV.GetComponent<animController>().ReloadAnimation();
                                    currentWeapon.GetComponent<animController>().animator.SetBool("isReloading", true);
                                    currentWeaponPOV.GetComponent<animController>().animator.SetBool("isReloading", true);
                                    img_reloadRing.GetComponent<ReloadRingAnim>().Play(currentWeapon.GetComponent<WeaponInfo>().emptyReloadTime);
                                    this.weaponEmptyReloadSound();
                                    img_crossHair.enabled = false;
                                    img_bulletIcon.enabled = true;
                                    img_reloadRing.enabled = true; //WIP: rework reload ring to fit different reload times
                                    Debug.Log("Reloading...");
                                }
                                else //No backup ammo
                                {
                                    //TODO: dry ammo sound/display
                                    fRatePassed = false;
                                    this.weaponDryShootingSound();
                                }
                            }
                            else
                            {
                                //Shoot depend on fireselect
                                if (currentWeapon.GetComponent<WeaponInfo>().fireMode == FireSelect.burst) { }
                                    //TODO: Coroutine for different number of Shoot() and recalculate fRatepassed with burstRateInt (TBD)
                                else
                                    Shoot();
                            }
                        }
                        else
                        {
                            currentWeapon.GetComponent<animController>().animator.SetBool("isShooting", false);
                            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShooting", false);
                            currentWeapon.GetComponent<animController>().animator.SetBool("isShootingLastRound", false);
                            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShootingLastRound", false);
                        }
                    }
                }
                else
                {
                    //TODO: Melee weapon logic
                }
            }
        }

        //Manual reload
        if (Input.GetKeyDown(reloadKey) && weaponEquipped && !currentNoAmmo && !reloading && !skillUsing && !levelPaused && !levelEnded && currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo < currentWeapon.GetComponent<WeaponInfo>().magSize)
        {
            reloading = true;
            //ammoText.text = "Reloading";
            if(currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo == 0)
            {
                emptyReloading = true;
                //currentWeapon.GetComponent<animController>().ReloadAnimation();
                //currentWeaponPOV.GetComponent<animController>().ReloadAnimation();
                currentWeapon.GetComponent<animController>().animator.SetBool("isReloading", true);
                currentWeaponPOV.GetComponent<animController>().animator.SetBool("isReloading", true);
                img_reloadRing.GetComponent<ReloadRingAnim>().Play(currentWeapon.GetComponent<WeaponInfo>().emptyReloadTime);
                this.weaponEmptyReloadSound();
            }
            else
            {
                tacticalReloading = true;
                //currentWeapon.GetComponent<animController>().ReloadAnimation();
                //currentWeaponPOV.GetComponent<animController>().ReloadAnimation();
                currentWeapon.GetComponent<animController>().animator.SetBool("isReloading", true);
                currentWeaponPOV.GetComponent<animController>().animator.SetBool("isReloading", true);
                img_reloadRing.GetComponent<ReloadRingAnim>().Play(currentWeapon.GetComponent<WeaponInfo>().tacticalReloadTime);
                this.weaponTacticalReloadSound();
            }
            img_bulletIcon.enabled = true;
            img_reloadRing.enabled = true;
            img_crossHair.enabled = false;
            Debug.Log("Reloading...");
        }

        //Skill CoolDown Counter
        if (!skillUsing && skillCDCounter > 0f)
        {
            skillCDCounter -= Time.deltaTime;
        }
        else if (!skillUsing && !skillReady)
        {
            skillCDCounter = -1f;
            skillReady = true;
            skillText.text = "Skill Ready";
            Debug.Log("Skill Ready");
        }

        //Skill Use
        if (Input.GetKeyDown(skillKey) && weaponEquipped && !reloading && !skillUsing && skillReady && !levelPaused && !levelEnded)
        {
            skillReady = false;
            skillUsing = true;
            skillText.text = "Skill Active";
            //TODO: In future split animation by putdown/pickup for other skill durations
            //currentWeapon.GetComponent<animController>().SkillPutdownAnimation();
            //currentWeaponPOV.GetComponent<animController>().SkillPutdownAnimation();
            currentWeapon.GetComponent<animController>().animator.SetBool("isPuttingdown", true);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isPuttingdown", true);
            StartCoroutine(ThrowImpactGrenade(skillTime));
            Debug.Log("Skill Active...");
        }
    }
    void FixedUpdate()
    {
        if (!levelPaused && !levelEnded)
        {
            //Reload logic //TODO: Add Tactical Reload variants
            if (reloading)
            {
                if (reloadingTime < currentWeapon.GetComponent<WeaponInfo>().ammoFillTime)
                {
                    reloadingTime += Time.fixedDeltaTime;
                }
                else
                {
                    if(currentWeapon.GetComponent<WeaponInfo>().backupAmmo > currentWeapon.GetComponent<WeaponInfo>().magSize - currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo)
                    {
                        currentWeapon.GetComponent<WeaponInfo>().backupAmmo -= currentWeapon.GetComponent<WeaponInfo>().magSize - currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo;
                        currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo = currentWeapon.GetComponent<WeaponInfo>().magSize;
                    }
                    else
                    {
                        currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo += currentWeapon.GetComponent<WeaponInfo>().backupAmmo;
                        currentWeapon.GetComponent<WeaponInfo>().backupAmmo = 0;
                        currentNoAmmo = true; //TODO: set false with all ammo refill methods
                    }
                    ammoCurrentMagText.text = currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo.ToString();
                    ammoBackupText.text = currentWeapon.GetComponent<WeaponInfo>().backupAmmo.ToString();
                    if (emptyReloading)
                        currentWeapon.GetComponent<WeaponInfo>().requireActionPull = true;
                    if(emptyReloading &&  (reloadingTime < currentWeapon.GetComponent<WeaponInfo>().emptyReloadTime) ||
                       tacticalReloading && (reloadingTime < currentWeapon.GetComponent<WeaponInfo>().tacticalReloadTime))
                    {
                        reloadingTime += Time.fixedDeltaTime;
                    }
                    else
                    {
                        reloadingTime = 0f;
                        reloading = false;
                        emptyReloading = false;
                        tacticalReloading = false;
                        img_bulletIcon.enabled = false;
                        img_reloadRing.enabled = false;
                        img_crossHair.enabled = true;
                        img_reloadRing.GetComponent<ReloadRingAnim>().Complete();
                        currentWeapon.GetComponent<WeaponInfo>().requireActionPull = false;
                        Debug.Log("Reloaded");
                    }
                }
            }
            //Skill duration logic
            if (skillUsing)
            {

                if (skillUsingTime < skillTime+weaponPutDownTime+weaponPickupTime)
                {
                    skillUsingTime += Time.fixedDeltaTime;
                }
                else
                {
                    skillUsingTime = 0f;
                    skillUsing = false;
                    skillCDCounter = skillCoolDown;
                    skillText.text = "Skill Cooling Down";
                    Debug.Log("Skill Used. Cooling Down...");
                }
            }
            //Firing rate recover logic
            if (!fRatePassed)
            {
                if (fGapCount < fRateInt)
                {
                    fGapCount += Time.fixedDeltaTime;
                }
                else
                {
                    fGapCount = 0f;
                    fRatePassed = true;
                }
            }
            if (mode_Timed)
            {
                if (timeLeft > 0f)
                {
                    timeLeft -= Time.fixedDeltaTime;
                    timeText.text = "Time: " + timeLeft.ToString("F2");
                }
                else
                {
                    timeLeft = 0.00f;
                    timeText.text = "Time: 0.00";
                    this.End();
                }
            }
        }
    }


    void Shoot()
    {
        if (currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo == 1)
        {
            currentWeapon.GetComponent<animController>().animator.SetBool("isShootingLastRound", true);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShootingLastRound", true);
        }
        else
        {
            currentWeapon.GetComponent<animController>().animator.SetBool("isShooting", true);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShooting", true);
            currentWeapon.GetComponent<animController>().ShootAnimation(); // NEED FIX on unexpected animation holding problem: This should not be here
            currentWeaponPOV.GetComponent<animController>().ShootAnimation();
        }
        Vector3 targetPoint;
        fRatePassed = false;
        currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo--;
        // Check if we hit anything
        bool hit = Physics.Raycast(ray, out raycastHit, Mathf.Infinity, layerMask);
        // If we did...Shoot to the hitposition
        if (!hit)
        {
            /*
            //Modify The angle a bit if not hitting anything
            var straightQ = Quaternion.LookRotation(-this.transform.up, this.transform.forward);
            Vector3 straightV = straightQ.eulerAngles;
            Vector3 modifiedV = straightV + new Vector3(0f,-0.4f,0f);
            //Shoot
            //TODO: Destroy hitted object if target, Score, Recoil, HittingSound, and HittingSparkles
            var clone = Instantiate(bullet, firePoint.position, Quaternion.Euler(modifiedV));
            */
            targetPoint = playerCam.transform.forward * 1000;
            var bulletObject = Instantiate(currentBullet, firePoint.position, firePoint.rotation);
            bulletObject.transform.LookAt(targetPoint);
        }
        else
        {
            /* 
            //Shoot to hit pos
            Vector3 relativePos = raycastHit.point - detectPoint.position;
            Quaternion preQ = Quaternion.LookRotation(relativePos);
            //Modify The angle a bit
            Vector3 preV = preQ.eulerAngles;
            Vector3 modifiedV = preV + new Vector3(90f, 0f, 0f);
            //TODO: Recoil, HittingSound, and HittingSparkles
            var clone = Instantiate(bullet, firePoint.position, Quaternion.Euler(modifiedV));
            */
            targetPoint = raycastHit.point;

            var bulletObject = Instantiate(currentBullet, firePoint.position, firePoint.rotation);
            bulletObject.transform.LookAt(targetPoint);
            bulletObject.GetComponent<BulletMovement>().hit = true;
            bulletObject.GetComponent<BulletMovement>().hitPoint = raycastHit.point;

            //Create bullet hit effects
            if (!raycastHit.collider.gameObject.CompareTag("Target") && !raycastHit.collider.gameObject.CompareTag("MovingTarget") && !raycastHit.collider.gameObject.CompareTag("RailTarget"))
            {
                StartCoroutine(HitOtherBulletEffects(0.1f, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal)));
                var cloneAu = Instantiate(HittingAudioObject, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal));
                cloneAu.GetComponent<HittingAudioManager>().Play(false);
            }
            else
            {
                StartCoroutine(HitTargetBulletEffects(0.1f, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal)));
                var cloneAu = Instantiate(HittingAudioObject, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal));
                cloneAu.GetComponent<HittingAudioManager>().Play(true);
            }
            //Looks like the target is hit
            var target = raycastHit.collider.gameObject;
            if (target.CompareTag("Target"))
            {
                if (mode_Scored && !target.GetComponent<TargetBehavior>().hit)
                {
                    score++;
                    scoreText.text = "Score: " + score.ToString();
                }
                target.GetComponent<TargetBehavior>().Hit(raycastHit.point, playerCam.transform.forward);
            }
            //Moving target hit
            if (target.CompareTag("MovingTarget"))
            {
                if (mode_Scored && !target.GetComponent<MovingTargetBehavior>().hit)
                {
                    score += 2;
                    scoreText.text = "Score: " + score.ToString();
                }
                target.GetComponent<MovingTargetBehavior>().Hit(raycastHit.point, playerCam.transform.forward);
            }
            //Another version of moving target (needs optimization)
            if (target.CompareTag("RailTarget"))
            {
                if (mode_Scored && !target.GetComponent<RailTargetBehavior>().hit)
                {
                    score += 1;
                    scoreText.text = "Score: " + score.ToString();
                }
                target.GetComponent<RailTargetBehavior>().Hit(raycastHit.point, playerCam.transform.forward);
            }
        }
        this.weaponRecoil();
        this.weaponShootingSound();
    }

    public void hitByProjectile(GameObject target)
    {
        if (target.CompareTag("Target"))
        {
            if (mode_Scored && !target.GetComponent<TargetBehavior>().hit)
            {
                score++;
                scoreText.text = "Score: " + score.ToString();
            }
            target.GetComponent<TargetBehavior>().HitByProjectile();
        }
        //Moving target hit
        if (target.CompareTag("MovingTarget"))
        {
            if (mode_Scored && !target.GetComponent<MovingTargetBehavior>().hit)
            {
                score += 2;
                scoreText.text = "Score: " + score.ToString();
            }
            target.GetComponent<TargetBehavior>().HitByProjectile();
        }
        //Another version of moving target (needs optimization)
        if (target.CompareTag("RailTarget"))
        {
            if (mode_Scored && !target.GetComponent<RailTargetBehavior>().hit)
            {
                score += 1;
                scoreText.text = "Score: " + score.ToString();
            }
            target.GetComponent<TargetBehavior>().HitByProjectile();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        sceneManager.GetComponent<LevelSceneManager>().Pause(reloading);
        levelPaused = true;
    }
    public void Resume()
    {
        sceneManager.GetComponent<LevelSceneManager>().Resume();
        levelPaused = false;
        Time.timeScale = 1;
    }
    public void End()
    {
        Time.timeScale = 0;
        levelEnded = true;
        sceneManager.GetComponent<LevelSceneManager>().End(score);
    }
    public void Perfect()
    {
        Time.timeScale = 0;
        levelEnded = true;
        sceneManager.GetComponent<LevelSceneManager>().Perfect();
    }
    public void weaponRecoil()
    {
        playerCam.GetComponent<CameraController>().Recoil();
    }
    public void weaponShootingSound()
    {
        currentWeapon.GetComponent<SoundScript>().ShootSound();
    }
    public void weaponDryShootingSound()
    {
        currentWeapon.GetComponent<SoundScript>().dryShootSound();
    }
    public void weaponTacticalReloadSound()
    {
       currentWeapon.GetComponent<SoundScript>().tacticalReloadSound();
    }
    public void weaponEmptyReloadSound()
    {
        currentWeapon.GetComponent<SoundScript>().emptyReloadSound();
    }
    public void weaponPickupSound()
    {
        currentWeapon.GetComponent<SoundScript>().emptyReloadSound();
    }
    public void weaponPutdownSound()
    {
        currentWeapon.GetComponent<SoundScript>().emptyReloadSound();
    }
    public void UpdateWeaponInfo()
    {
        WeaponInfo currentWeaponInfo = currentWeapon.GetComponent<WeaponInfo>();
        weaponText.text = currentWeaponInfo.name;
        ammoCurrentMagText.text = currentWeaponInfo.currentMagAmmo.ToString();
        ammoBackupText.text = currentWeaponInfo.backupAmmo.ToString();
        //Set firing interval according to input firing rate
        fRateInt = 1f / currentWeaponInfo.firingRate;
        playerCam.GetComponent<CameraController>().UpdateWeaponInfo(currentWeaponInfo.maxHorizontalRecoil, currentWeaponInfo.minHorizontalRecoil, currentWeaponInfo.verticalRecoil);
    }
    IEnumerator ThrowImpactGrenade(float time)
    {
        yield return new WaitForSeconds(weaponPutDownTime);
        currentWeapon.GetComponent<animController>().animator.SetBool("isPuttingdown", false);
        currentWeaponPOV.GetComponent<animController>().animator.SetBool("isPuttingdown", false);
        var impactGrenadeObject = Instantiate(skillObjs[0], projectileFirePoint.transform.position, Quaternion.Euler(playerCam.transform.forward));
        impactGrenadeObject.transform.LookAt(playerCam.transform.forward * 1000);
        // TODO: Modify the throwing angle so the grenade is a bit higher than horizontal
        Vector3 currentRotation = impactGrenadeObject.transform.eulerAngles;
        Vector3 modifiedRotation = currentRotation + new Vector3(0f, 0f, 0f);
        impactGrenadeObject.transform.localRotation = Quaternion.Euler(modifiedRotation);
        yield return new WaitForSeconds(time);
        currentWeapon.GetComponent<animController>().animator.SetBool("isPickingup", true);
        currentWeaponPOV.GetComponent<animController>().animator.SetBool("isPickingup", true);
        yield return new WaitForSeconds(weaponPickupTime);
        currentWeapon.GetComponent<animController>().animator.SetBool("isPickingup", false);
        currentWeaponPOV.GetComponent<animController>().animator.SetBool("isPickingup", false);
    }
    IEnumerator HitOtherBulletEffects(float time, Vector3 location, Quaternion facingDir)
    {
        yield return new WaitForSeconds(time);
        Instantiate(BulletTextures[Random.Range(0, 3)], location, facingDir);
        Instantiate(hitOthersParticle, location, facingDir);
    }
    IEnumerator HitTargetBulletEffects(float time, Vector3 location, Quaternion facingDir)
    {
        yield return new WaitForSeconds(time);
        Instantiate(hitTargetParticle, location, facingDir);
    }
}
