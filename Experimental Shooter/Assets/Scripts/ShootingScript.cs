using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScript : MonoBehaviour
{
    //public GameObject dummyObj = new GameObject("dummyObj");
    //create enum for future additions
    public enum Equipment
    {
        None,
        impactGrenade
    }
    public enum Firearm
    {
        None,
        subMachineGun,
        Handgun
    }
    public enum Melee
    {
        None,
        combatKnife
    }

    //Scene Manager
    [Header("Scene Manager")]
    [Tooltip("Current Active SceneManager")]
    public GameObject sceneManager;

    [Header("Trial Manager")]
    [Tooltip("Script that handles Trail rules")]
    public TrialScript trialScript;

    [Header("Player")]
    [Tooltip("Current Active Player")]
    public GameObject player;

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
    [Tooltip("If picking up weapon when all weapon manipulation should be locked")]
    public bool pickupHandling = false;
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
    public float weaponPutDownTime = 0.4f; //Both added 0.1s for transition
    public float weaponPickupTime = 0.6f; //Both added 0.1s for transition
    public float weaponPickupActionTime = 0.8f;
    public float pickupHandlingTime = 0f;
    public float weaponHandlingTime = 0f;
    public float switchCounter = 0f;
    public float pickupDistance = 4f;
    public int currentTrial = -1;
    public bool halfSwitched = false;
    public bool switchTrigger = false;
    public bool pickupLookingat = false;
    public bool interactablesLookingat = false;
    public string pickupName = "";
    public float distanceUnitRatio = 3.3333f;
    public GameObject pickupObj;
    public GameObject interactableObj;
    public PickupHandler pickupHandler;
    public GUIStyle stylePickupBoxGUI = new GUIStyle();

    public GameObject regularDamageDisplayObj;
    public GameObject canvasHUD;
    public GameObject canvas1stCamera;
    public GameObject criticalDamageDisplayObj;


    //TODO: weapon details should come with weapon, consider making enums and dictionary or general weapon class
    /*public float firingRate = 10f;
    public bool autoTrigger;
    public int magSize = 30;
    public float reloadTime = 1.5f;*/

    [Header("Equipment/Skill")]
    //TODO: Create dictionary containing equipments so this becomes "skillObj"s and the current obj are passed in dynamically
    public GameObject[] equipmentObjs;
    /*[Tooltip("Current primary skill/equipment gameObject (WIP)")]
    public GameObject primaryEquipmentObj;
    [Tooltip("Current secondary skill/equipment gameObject (WIP)")]
    public GameObject secondaryEquipmentObj;*/
    //Skill/Equipment (when there are more skills would put data in dictionary for better management)
    public Equipment equipmentPrimary;
    public Equipment equipmentSecondary;
    public float equipmentCoolDownPrimary = 0f;
    public float equipmentCoolDownSecondary = 0f;
    public float equipmentTimePrimary = 0f; 
    public float equipmentTimeSecondary = 0f; 
    public bool equipmentEquippedPrimary = false;
    public bool equipmentEquippedSecondary = false;
    [SerializeField] private bool equipmentPrimaryReady = true;
    [SerializeField] private bool equipmentSecondaryReady = true;

    private float equipmentPrimaryCDCounter = 0f;
    private float equipmentSecondaryCDCounter = 0f;

    [Header("Bool Conditions")]
    [SerializeField] private bool levelEnded = false;
    [SerializeField] private bool levelPaused = false;
    [SerializeField] private bool reloading = false;
    [SerializeField] private bool switching = false;
    [SerializeField] private bool shooting = false;
    [SerializeField] private bool shootingLastRound = false;
    [SerializeField] private bool tacticalReloading = false;
    [SerializeField] private bool emptyReloading = false;
    [SerializeField] private bool equipmentPrimaryUsing = false;
    [SerializeField] private bool equipmentSecondaryUsing = false;
    [SerializeField] private bool interactionCoolDown = false;

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
    public Image imageHUDEquipmentPrimary;
    public GameObject imageHUDEquipmentPrimaryCooldownCover;
    public Image imageHUDEquipmentSecondary;
    public GameObject imageHUDEquipmentSecondaryCooldownCover;
    [Tooltip("If there's weaponset selected")]
    public bool hasWeaponSet;

    [Header("Raycasting LayerMask")]
    public LayerMask layerMask;
    public LayerMask layerMaskPickup;

    [Header("Key Bindings")]
    [SerializeField] private KeyCode equipmentPrimaryKey = KeyCode.G;
    [SerializeField] private KeyCode equipmentSecondaryKey = KeyCode.Q;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode switchKey1 = KeyCode.Alpha1;
    [SerializeField] private KeyCode switchKey2 = KeyCode.Alpha2;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private bool scrollUpGapped = false;
    [SerializeField] private bool scrollDownGapped = false;
    [SerializeField] private float scrollBlocker = 0f;
    [SerializeField] private float scrollBlockTime = 0.1f;

    ///PRIVATE VARIABLES
    //Ray
    private Ray ray;
    private RaycastHit raycastHit;
    private RaycastHit raycastHitPickup;
    /*//Current bullets left in mag
    private int currentMag = 0;*/
    //Reload counter
    private float reloadingTime = 0f;
    //equipment counter
    private float equipmentPrimaryUsingTime = 0f;
    //equipment counter
    private float equipmentSecondaryUsingTime = 0f;
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

        if(weaponEquipped)
            UpdateWeaponInfo();
        UpdateEquipmentInfo();

        stylePickupBoxGUI.alignment = TextAnchor.MiddleLeft;
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
            //reset shooting booleans for animators
            currentWeapon.GetComponent<animController>().animator.SetBool("isShooting", false);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShooting", false);
            currentWeapon.GetComponent<animController>().animator.SetBool("isShootingLastRound", false);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShootingLastRound", false);
        }



        //Shooting logic
        if (!levelPaused && !levelEnded)
        {
            // Get a ray from the camera pointing forwards
            ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

            //Pickup detection using shooting ray
            bool pickUpHit = Physics.Raycast(ray, out raycastHitPickup, pickupDistance, layerMaskPickup);
            if (pickUpHit)
            {
                if(LayerMask.LayerToName(raycastHitPickup.collider.gameObject.layer) == "Pickups")
                {
                    pickupLookingat = true;
                    interactablesLookingat = false;
                    interactableObj = null;
                    pickupObj = raycastHitPickup.collider.gameObject;

                    if (pickupName == "")
                    {
                        if (raycastHitPickup.collider.gameObject.CompareTag("Weapon"))
                            pickupName = raycastHitPickup.collider.gameObject.GetComponent<WeaponInfo>().name;
                        if (raycastHitPickup.collider.gameObject.CompareTag("Equipment"))
                            pickupName = raycastHitPickup.collider.gameObject.GetComponent<EquipmentInfo>().name;
                    }
                }
                else if (raycastHitPickup.collider.gameObject.CompareTag("Interactables"))
                {
                    interactablesLookingat = true;
                    pickupLookingat = false;
                    pickupObj = null;
                    interactableObj = raycastHitPickup.collider.gameObject;
                }
                else
                {
                    pickupLookingat = false;
                    interactablesLookingat = false;
                    pickupObj = null;
                    interactableObj = null;
                    if (pickupName != "")
                    {
                        pickupName = "";
                    }
                }
            }
            else
            {
                pickupLookingat = false;
                interactablesLookingat = false;
                pickupObj = null;
                interactableObj = null;
                if (pickupName != "")
                {
                    pickupName = "";
                }
            }

            if(pickupLookingat && Input.GetKeyDown(pickupKey))
            {
                if (raycastHitPickup.collider.gameObject.CompareTag("Weapon"))
                    pickupHandler.PickupWeapon(pickupObj.GetComponent<WeaponInfo>(), pickupObj);
                if (raycastHitPickup.collider.gameObject.CompareTag("Equipment"))
                    pickupHandler.PickupEquipment(pickupObj.GetComponent<EquipmentInfo>(), pickupObj);

            }

            if (interactablesLookingat && Input.GetKeyDown(interactionKey) && !interactionCoolDown)
            {
                interactionCoolDown = true;
                if (interactableObj.GetComponent<ButtonInfo>().trialButton)
                {
                    trialScript.BeginTrial(interactableObj.GetComponent<ButtonInfo>().typeIdentifier);
                    StartCoroutine(InteractionCoolDown(interactableObj.GetComponent<ButtonInfo>().coolDown));
                    interactableObj.GetComponent<animController>().ButtonPressAnimation();
                }
                else if (interactableObj.GetComponent<ButtonInfo>().ammoBox)
                {
                    //Refill ammo
                    if (weaponEquipped && !interactableObj.GetComponent<SelfRotating>().stopped)
                    {
                        Debug.Log("Refilling Ammo");
                        currentWeapon.GetComponent<WeaponInfo>().backupAmmo = currentWeapon.GetComponent<WeaponInfo>().maxAmmo;
                        if (weaponFull)
                        {
                            if(currentWeaponSlot == 0)
                            {
                                secondaryWeapon.GetComponent<WeaponInfo>().backupAmmo = secondaryWeapon.GetComponent<WeaponInfo>().maxAmmo;
                            }
                            else
                            {
                                primaryWeapon.GetComponent<WeaponInfo>().backupAmmo = primaryWeapon.GetComponent<WeaponInfo>().maxAmmo;
                            }
                        }
                        currentNoAmmo = false;
                        ammoBackupText.text = currentWeapon.GetComponent<WeaponInfo>().backupAmmo.ToString();
                        StartCoroutine(InteractionCoolDown(interactableObj.GetComponent<ButtonInfo>().coolDown));
                        StartCoroutine(AmmoBoxCoolDown(interactableObj));
                    }
                    //interactableObj.GetComponent<animController>().ButtonPressAnimation();
                }
                else if (interactableObj.GetComponent<ButtonInfo>().teleportButton)
                {
                    player.transform.position = interactableObj.GetComponent<ButtonInfo>().TeleportPosition;
                    player.transform.rotation = Quaternion.Euler(Vector3.zero);
                    StartCoroutine(InteractionCoolDown(interactableObj.GetComponent<ButtonInfo>().coolDown));
                    interactableObj.GetComponent<animController>().ButtonPressAnimation();
                }
            }

            if (weaponEquipped)
            {
                if (!currentWeapon.GetComponent<WeaponInfo>().melee)
                {
                    if (reloading)
                    {
                        currentWeapon.GetComponent<animController>().animator.SetBool("isReloading", false);
                        currentWeaponPOV.GetComponent<animController>().animator.SetBool("isReloading", false);
                    }
                    else if (fRatePassed && !equipmentPrimaryUsing && !equipmentSecondaryUsing && !pickupHandling && !weaponHandling)
                    {
                        if( (Input.GetKey(attackKey) && currentWeapon.GetComponent<WeaponInfo>().fireMode == FireSelect.auto) ||
                            (Input.GetKeyDown(attackKey) && currentWeapon.GetComponent<WeaponInfo>().fireMode != FireSelect.auto))
                        {
                            if (!bulletEnough)
                            {
                                if (!currentNoAmmo) //Empty reload //CAUTION: CURRENTNOAMMO WRONG TRIGGERED
                                {
                                    reloading = true;
                                    emptyReloading = true;
                                    //ammoText.text = "Reloading";
                                    currentWeapon.GetComponent<animController>().animator.SetBool("isReloading", true);
                                    currentWeaponPOV.GetComponent<animController>().animator.SetBool("isReloading", true); // This is not working by itself while shooting somehow
                                    //Manual fix for shooting animations
                                    //if (currentWeapon.GetComponent<animController>().animator.GetCurrentAnimatorStateInfo(0).IsName("ShootLastRound"))
                                    if (shootingLastRound)
                                    {
                                        currentWeapon.GetComponent<animController>().animator.Play("ShootLastRound", -1, 1f);
                                        currentWeaponPOV.GetComponent<animController>().animator.Play("ShootLastRound", -1, 1f);
                                        StartCoroutine(EmptyReloadDelayer());
                                    }
                                    else
                                    {
                                        //This is a safety measure to solve unknown cause animation problems
                                        currentWeapon.GetComponent<animController>().EmptyReloadAnimation();
                                        currentWeaponPOV.GetComponent<animController>().EmptyReloadAnimation();
                                    }
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
                       /* else
                        {
                            currentWeapon.GetComponent<animController>().animator.SetBool("isShooting", false);
                            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShooting", false);
                            currentWeapon.GetComponent<animController>().animator.SetBool("isShootingLastRound", false);
                            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShootingLastRound", false);
                        }*/
                    }
                }
                else
                {
                    //TODO: Melee weapon logic
                }
            }
        }

        //Manual reload
        if (Input.GetKeyDown(reloadKey) && weaponEquipped && !currentNoAmmo && !reloading && !equipmentPrimaryUsing && !equipmentSecondaryUsing && !levelPaused && !levelEnded && !pickupHandling && !weaponHandling && currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo < currentWeapon.GetComponent<WeaponInfo>().magSize)
        {
            reloading = true;
            //ammoText.text = "Reloading";
            if(currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo == 0)
            {
                emptyReloading = true;
                currentWeapon.GetComponent<animController>().animator.SetBool("isReloading", true);
                currentWeaponPOV.GetComponent<animController>().animator.SetBool("isReloading", true);
                //Manual fix for shooting animations
                //if (currentWeapon.GetComponent<animController>().animator.GetCurrentAnimatorStateInfo(0).IsName("ShootLastRound"))
                if (shootingLastRound)
                {
                    currentWeapon.GetComponent<animController>().animator.Play("ShootLastRound", -1, 1f);
                    currentWeaponPOV.GetComponent<animController>().animator.Play("ShootLastRound", -1, 1f);
                    StartCoroutine(EmptyReloadDelayer());
                }
                else
                {
                    //This is a safety measure to solve unknown cause animation problems
                    currentWeapon.GetComponent<animController>().EmptyReloadAnimation();
                    currentWeaponPOV.GetComponent<animController>().EmptyReloadAnimation();
                }
                img_reloadRing.GetComponent<ReloadRingAnim>().Play(currentWeapon.GetComponent<WeaponInfo>().emptyReloadTime);
                this.weaponEmptyReloadSound();
            }
            else
            {
                tacticalReloading = true;
                currentWeapon.GetComponent<animController>().animator.SetBool("isReloading", true);
                currentWeaponPOV.GetComponent<animController>().animator.SetBool("isReloading", true);
                //if (currentWeapon.GetComponent<animController>().animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
                if(shooting)
                {
                    currentWeapon.GetComponent<animController>().animator.Play("Shoot", -1, 1f);
                    currentWeaponPOV.GetComponent<animController>().animator.Play("Shoot", -1, 1f);
                    StartCoroutine(TacticalReloadDelayer());
                }
                else
                {
                    //This is a safety measure to solve unknown cause animation problems
                    currentWeapon.GetComponent<animController>().TacticalReloadAnimation();
                    currentWeaponPOV.GetComponent<animController>().TacticalReloadAnimation();
                }
                /*//else if (currentWeapon.GetComponent<animController>().animator.GetCurrentAnimatorStateInfo(0).IsName("ShootLastRound"))
                else if (shootingLastRound)
                {
                    currentWeapon.GetComponent<animController>().animator.Play("ShootLastRound", -1, 1f);
                    currentWeaponPOV.GetComponent<animController>().animator.Play("ShootLastRound", -1, 1f);
                    StartCoroutine(EmptyReloadDelayer());
                }*/
                img_reloadRing.GetComponent<ReloadRingAnim>().Play(currentWeapon.GetComponent<WeaponInfo>().tacticalReloadTime);
                this.weaponTacticalReloadSound();
            }
            img_bulletIcon.enabled = true;
            img_reloadRing.enabled = true;
            img_crossHair.enabled = false;
            Debug.Log("Reloading...");
        }

        //Equipment CoolDown Counter
        if (!equipmentPrimaryReady && equipmentPrimaryCDCounter > 0f)
        {
            equipmentPrimaryCDCounter -= Time.deltaTime;
            Vector3 temp = imageHUDEquipmentPrimaryCooldownCover.GetComponent<RectTransform>().localScale;
            imageHUDEquipmentPrimaryCooldownCover.GetComponent<RectTransform>().localScale =
                new Vector3(temp.x, Mathf.Lerp(0f, -1f, equipmentPrimaryCDCounter/equipmentCoolDownPrimary), temp.z);
        }
        else if (!equipmentPrimaryUsing && !equipmentPrimaryReady)
        {
            equipmentPrimaryCDCounter = 0f;
            equipmentPrimaryReady = true;
            imageHUDEquipmentPrimaryCooldownCover.GetComponent<RectTransform>().localScale = new Vector3(1f, 0f, 1f);
            /*skillText.text = "Skill Ready";
            Debug.Log("Skill Ready");*/
        }

        //Equipment1 Use (TODO: equipment variants for secondary equipment)
        if (Input.GetKeyDown(equipmentPrimaryKey) && equipmentEquippedPrimary && !reloading && !equipmentPrimaryUsing && !equipmentSecondaryUsing && equipmentPrimaryReady && !levelPaused && !levelEnded)
        {
            equipmentPrimaryReady = false;
            equipmentPrimaryUsing = true;
            skillText.text = "Equipment1 Active";
            //currentWeapon.GetComponent<animController>().SkillPutdownAnimation();
            //currentWeaponPOV.GetComponent<animController>().SkillPutdownAnimation();
            if (weaponEquipped)
            {
                currentWeapon.GetComponent<animController>().animator.SetBool("isPuttingdown", true);
                currentWeaponPOV.GetComponent<animController>().animator.SetBool("isPuttingdown", true);
            }
            Vector3 temp = imageHUDEquipmentPrimaryCooldownCover.GetComponent<RectTransform>().localScale;
            imageHUDEquipmentPrimaryCooldownCover.GetComponent<RectTransform>().localScale = new Vector3(temp.x, -1f, temp.z);
            if (equipmentPrimary == Equipment.impactGrenade)
                StartCoroutine(ThrowImpactGrenade(equipmentTimePrimary));
            Debug.Log("Equipment1 Active");
        }

        //Tool: Scroll detect/gapper (future plans to adapt into new input system)
        if (scrollBlocker == 0f && Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0)
                scrollUpGapped = true;
            else
                scrollDownGapped = true;
        }

        //Switch weapon (detect part) (part2 in FixedUpdate with time counter)
        if ( (Input.GetKeyDown(switchKey1) || Input.GetKeyDown(switchKey2) || scrollUpGapped || scrollDownGapped || switchTrigger) && !equipmentPrimaryUsing && !equipmentSecondaryUsing && !levelPaused && !levelEnded)
        {
            if (scrollDownGapped || scrollUpGapped)
            {
                scrollDownGapped = false;
                scrollUpGapped = false;
                scrollBlocker = scrollBlockTime;
            }
            if (switchTrigger)
            {
                switchTrigger = false;
            }

            //If no weapon then do nothing
            if (!noWeapon)
            {
                //Switch happening in first half then reverse
                if (switching)
                {
                    switchCounter = 0f;
                    switching = false;
                    weaponHandlingTime = weaponPickupTime;
                    if (currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo == 0)
                    {
                        currentWeapon.GetComponent<animController>().animator.CrossFadeInFixedTime("PickupEmptyMag", 0.1f);
                        currentWeaponPOV.GetComponent<animController>().animator.CrossFadeInFixedTime("PickupEmptyMag", 0.1f);
                    }
                    else //crossfade seems not working with short period (FIXED)
                    {
                        if (currentWeapon.GetComponent<WeaponInfo>().requireActionPull)
                        {
                            currentWeapon.GetComponent<animController>().animator.CrossFadeInFixedTime("PickupPullAction", 0.1f);
                            currentWeaponPOV.GetComponent<animController>().animator.CrossFadeInFixedTime("PickupPullAction", 0.1f);
                            weaponHandlingTime = weaponPickupActionTime;
                        }
                        else
                        {
                            currentWeapon.GetComponent<animController>().animator.CrossFadeInFixedTime("Pickup", 0.1f);
                            currentWeaponPOV.GetComponent<animController>().animator.CrossFadeInFixedTime("Pickup", 0.1f);
                        }
                    }
                    Debug.Log("Switching reversed");
                }
                //if current is empty
                else if (!weaponEquipped)
                {
                    switchCounter = 0.1f;
                    switching = true;
                    weaponHandlingTime = weaponPutDownTime + weaponPickupActionTime; //Temporary, to be reassigned in part 2 to avoid problem
                    weaponHandling = true;
                    Debug.Log("Switching Weapon from empty handed...");
                }
                //Reload not complete then interrupt 
                else if (reloading)
                {
                    if (emptyReloading)
                    {
                        emptyReloading = false;
                        currentWeapon.GetComponent<animController>().animator.CrossFadeInFixedTime("PutdownEmptyMag", 0.15f);
                        currentWeaponPOV.GetComponent<animController>().animator.CrossFadeInFixedTime("PutdownEmptyMag", 0.15f);
                    }
                    else if (tacticalReloading)
                    {
                        tacticalReloading = false;
                        currentWeapon.GetComponent<animController>().animator.CrossFadeInFixedTime("Putdown", 0.15f);
                        currentWeaponPOV.GetComponent<animController>().animator.CrossFadeInFixedTime("Putdown", 0.15f);
                    }
                    reloadingTime = 0f;
                    reloading = false;
                    img_bulletIcon.enabled = false;
                    img_reloadRing.enabled = false;
                    img_crossHair.enabled = true;
                    img_reloadRing.GetComponent<ReloadRingAnim>().Complete();
                    Debug.Log("Reload interrupted for switching");
                    switchCounter = weaponPutDownTime;
                    switching = true;
                    weaponHandlingTime = weaponPutDownTime + weaponPickupActionTime; //Temporary, to be reassigned in part 2 to avoid problem
                    weaponHandling = true;
                }
                else
                {

                    if (currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo == 0)
                    {
                        currentWeapon.GetComponent<animController>().animator.CrossFadeInFixedTime("PutdownEmptyMag", 0.15f);
                        currentWeaponPOV.GetComponent<animController>().animator.CrossFadeInFixedTime("PutdownEmptyMag", 0.15f);
                        //currentWeapon.GetComponent<animController>().animator.Play("PutdownEmptyMag");
                    }
                    else
                    {
                        currentWeapon.GetComponent<animController>().animator.CrossFadeInFixedTime("Putdown", 0.15f);
                        currentWeaponPOV.GetComponent<animController>().animator.CrossFadeInFixedTime("Putdown", 0.15f);
                        //currentWeapon.GetComponent<animController>().animator.Play("Putdown");
                    }
                    switchCounter = weaponPutDownTime;
                    switching = true;
                    weaponHandlingTime = weaponPutDownTime + weaponPickupActionTime; //Temporary, to be reassigned in part 2 to avoid problem
                    weaponHandling = true;
                    Debug.Log("Switching Weapon...");
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (!levelPaused && !levelEnded)
        {
            if (scrollBlocker > 0f)
                scrollBlocker -= Time.fixedDeltaTime;
            else
                scrollBlocker = 0f;

            if (pickupHandling)
            {
                if (pickupHandlingTime >= 0f)
                    pickupHandlingTime -= Time.fixedDeltaTime;
                else
                {
                    pickupHandling = false;
                    pickupHandlingTime = 0f;
                }
            }

            if (weaponHandling)
            {
                if (weaponHandlingTime >= 0f)
                    weaponHandlingTime -= Time.fixedDeltaTime;
                else
                {
                    weaponHandling = false;
                    weaponHandlingTime = 0f;
                    if (weaponEquipped && currentWeapon.GetComponent<WeaponInfo>().requireActionPull)
                        currentWeapon.GetComponent<WeaponInfo>().requireActionPull = false;
                }
            }

            if (switchCounter > 0f)
                switchCounter -= Time.fixedDeltaTime;
            else
                switchCounter = 0f;

            //Switching logic (part 2)
            if(switching && switchCounter == 0f)
            {
                switching = false;
                weaponHandlingTime = weaponPickupTime;
                weaponHandling = true;

                //Weapon slot differences
                if(currentWeaponSlot == 0)
                {
                    currentWeaponSlot = 1;
                    if (!weaponFull)
                    {
                        //If switching to empty
                        if (weaponEquipped)
                        {
                            primaryWeaponBackDisplay.SetActive(true);
                            primaryWeapon.SetActive(false);
                            primaryWeaponPOV.SetActive(false);
                            currentWeapon = null;
                            currentWeaponPOV = null;
                            currentBullet = null;
                            weaponHandlingTime = 0.1f;
                            weaponEquipped = false;
                        }
                        else //if switching from empty
                        {
                            currentWeapon = secondaryWeapon;
                            currentWeaponPOV = secondaryWeaponPOV;
                            currentBullet = secondaryBullet;
                            secondaryWeaponBackDisplay.SetActive(false);
                            secondaryWeapon.SetActive(true);
                            secondaryWeaponPOV.SetActive(true);
                        }
                    }
                    else
                    {
                        primaryWeaponBackDisplay.SetActive(true);
                        primaryWeapon.SetActive(false);
                        primaryWeaponPOV.SetActive(false);
                        currentWeapon = secondaryWeapon;
                        currentWeaponPOV = secondaryWeaponPOV;
                        currentBullet = secondaryBullet;
                        secondaryWeaponBackDisplay.SetActive(false);
                        secondaryWeapon.SetActive(true);
                        secondaryWeaponPOV.SetActive(true);
                    }
                }
                else if (currentWeaponSlot == 1)
                {
                    currentWeaponSlot = 0;
                    if (!weaponFull)
                    {
                        //If switching to empty
                        if (weaponEquipped)
                        {
                            secondaryWeaponBackDisplay.SetActive(true);
                            secondaryWeapon.SetActive(false);
                            secondaryWeaponPOV.SetActive(false);
                            currentWeapon = null;
                            currentWeaponPOV = null;
                            currentBullet = null;
                            weaponHandlingTime = 0.1f;
                            weaponEquipped = false;
                        }
                        else //if switching from empty
                        {
                            currentWeapon = primaryWeapon;
                            currentWeaponPOV = primaryWeaponPOV;
                            currentBullet = primaryBullet;
                            primaryWeaponBackDisplay.SetActive(false);
                            primaryWeapon.SetActive(true);
                            primaryWeaponPOV.SetActive(true);
                            weaponEquipped = true;
                        }
                    }
                    else
                    {
                        secondaryWeaponBackDisplay.SetActive(true);
                        secondaryWeapon.SetActive(false);
                        secondaryWeaponPOV.SetActive(false);
                        currentWeapon = primaryWeapon;
                        currentWeaponPOV = primaryWeaponPOV;
                        currentBullet = primaryBullet;
                        primaryWeaponBackDisplay.SetActive(false);
                        primaryWeapon.SetActive(true);
                        primaryWeaponPOV.SetActive(true);
                    }
                }

                UpdateWeaponInfo();
                //Animation
                if(currentWeapon != null)
                {
                    if (currentWeapon.GetComponent<WeaponInfo>().currentMagAmmo == 0)
                    {
                        currentWeapon.GetComponent<animController>().PickupEmptyMagAnimation();
                        currentWeaponPOV.GetComponent<animController>().PickupEmptyMagAnimation();
                    }
                    else
                    {
                        if (currentWeapon.GetComponent<WeaponInfo>().requireActionPull)
                        {
                            currentWeapon.GetComponent<animController>().PickupPullActionAnimation();
                            currentWeaponPOV.GetComponent<animController>().PickupPullActionAnimation();
                            weaponHandlingTime = weaponPickupActionTime;
                        }
                        else
                        {
                            currentWeapon.GetComponent<animController>().PickupAnimation();
                            currentWeaponPOV.GetComponent<animController>().PickupAnimation();
                        }
                    }
                }
            }


            //Reload logic //Added Tactical Reload variants
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
            //equipment1 duration logic (for weapon handling)
            if (equipmentPrimaryUsing)
            {

                if (equipmentPrimaryUsingTime < equipmentTimePrimary+weaponPutDownTime+weaponPickupTime+0.3f)
                {
                    equipmentPrimaryUsingTime += Time.fixedDeltaTime;
                }
                else
                {
                    equipmentPrimaryUsingTime = 0f;
                    equipmentPrimaryUsing = false;
                    /*skillCDCounter = equipmentCoolDownPrimary;
                    skillText.text = "Skill Cooling Down";
                    Debug.Log("Skill Used. Cooling Down...");*/
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
            currentWeapon.GetComponent<animController>().ShootLastRoundAnimation(); // NEED FIX on unexpected animation holding problem: This should not be here
            currentWeaponPOV.GetComponent<animController>().ShootLastRoundAnimation();
            shootingLastRound = true;
            StopCoroutine(ReloadShootingLastRoundAnimation());
            StartCoroutine(ReloadShootingLastRoundAnimation());
        }
        else
        {
            currentWeapon.GetComponent<animController>().animator.SetBool("isShooting", true);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isShooting", true);
            currentWeapon.GetComponent<animController>().ShootAnimation(); // NEED FIX on unexpected animation holding problem: This should not be here
            currentWeaponPOV.GetComponent<animController>().ShootAnimation();
            shooting = true;
            StopCoroutine(ReloadShootingAnimation());
            StartCoroutine(ReloadShootingAnimation());
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
            //Trial Purpose conditions
            if (currentTrial == 1)
            {
                trialScript.reactionTrialPenalty += 0.1f;
            }
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

            //Looks like the target is hit
            targetPoint = raycastHit.point;
            var target = raycastHit.collider.gameObject;

            var bulletObject = Instantiate(currentBullet, firePoint.position, firePoint.rotation);
            bulletObject.transform.LookAt(targetPoint);
            bulletObject.GetComponent<BulletMovement>().hit = true;
            bulletObject.GetComponent<BulletMovement>().hitPoint = raycastHit.point;
            //Debug.Log(target.tag);
            //Create bullet hit effects
            if (!target.CompareTag("Targets") /*&& !raycastHit.collider.gameObject.CompareTag("MovingTarget") && !raycastHit.collider.gameObject.CompareTag("RailTarget")*/)
            {
                //Trial Purpose conditions
                if (currentTrial == 1)
                {
                    trialScript.reactionTrialPenalty += 0.1f;
                }
                StartCoroutine(HitOtherBulletEffects(0.1f, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal)));
                var cloneAu = Instantiate(HittingAudioObject, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal));
                cloneAu.GetComponent<HittingAudioManager>().Play(false);
            }
            else
            {
                StartCoroutine(HitTargetBulletEffects(0.1f, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal)));
                var cloneAu = Instantiate(HittingAudioObject, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal));
                cloneAu.GetComponent<HittingAudioManager>().Play(true);
                GameObject damageDisplay;

                if (LayerMask.LayerToName(target.layer) == "HitablesDamageCriticalVariant")
                {
                    //Trial Purpose conditions
                    if (currentTrial == 1 && !target.transform.parent.GetComponent<TargetBehavior>().reactionTrialUse)
                    {
                        trialScript.reactionTrialPenalty += 0.1f;
                    }
                    if (target.transform.parent.GetComponent<TargetBehavior>().damageDisplay)
                    {
                        damageDisplay = Instantiate(criticalDamageDisplayObj, targetPoint, Quaternion.Euler(0f, 0f, 0f));
                        //damageDisplay.transform.SetParent(canvas1stCamera.transform);
                        damageDisplay.transform.SetParent(canvasHUD.transform);
                        damageDisplay.GetComponent<DamageDisplay>().playerCam = playerCam.GetComponent<Camera>();
                        damageDisplay.GetComponent<DamageDisplay>().hitTarget = target;
                        damageDisplay.GetComponent<DamageDisplay>().damageDisplayText.text = (currentWeapon.GetComponent<WeaponInfo>().damage * 2).ToString();
                    }
                    target.transform.parent.GetComponent<TargetBehavior>().DamageBehavior(true, currentWeapon.GetComponent<WeaponInfo>().damage, raycastHit.point, playerCam.transform.forward);
                }
                else if (LayerMask.LayerToName(target.layer) == "HitablesDamageVariant")
                {
                    //Trial Purpose conditions
                    if (currentTrial == 1 && !target.transform.parent.GetComponent<TargetBehavior>().reactionTrialUse)
                    {
                        trialScript.reactionTrialPenalty += 0.1f;
                    }                    
                    if (target.transform.parent.GetComponent<TargetBehavior>().damageDisplay)
                    {
                        damageDisplay = Instantiate(regularDamageDisplayObj, targetPoint, Quaternion.Euler(0f, 0f, 0f));
                        //damageDisplay.transform.SetParent(canvas1stCamera.transform);
                        damageDisplay.transform.SetParent(canvasHUD.transform);
                        damageDisplay.GetComponent<DamageDisplay>().playerCam = playerCam.GetComponent<Camera>();
                        damageDisplay.GetComponent<DamageDisplay>().hitTarget = target;
                        damageDisplay.GetComponent<DamageDisplay>().damageDisplayText.text = currentWeapon.GetComponent<WeaponInfo>().damage.ToString();
                    }
                    target.transform.parent.GetComponent<TargetBehavior>().DamageBehavior(false, currentWeapon.GetComponent<WeaponInfo>().damage, raycastHit.point, playerCam.transform.forward);
                }
                else
                {
                    //Trial Purpose conditions
                    if (currentTrial == 1 && !target.GetComponent<TargetBehavior>().reactionTrialUse)
                    {
                        trialScript.reactionTrialPenalty += 0.1f;
                    }                    
                    if (target.GetComponent<TargetBehavior>().damageDisplay)
                    {
                        damageDisplay = Instantiate(regularDamageDisplayObj, targetPoint, Quaternion.Euler(0f, 0f, 0f));
                        //damageDisplay.transform.SetParent(canvas1stCamera.transform);
                        damageDisplay.transform.SetParent(canvasHUD.transform);
                        damageDisplay.GetComponent<DamageDisplay>().hitTarget = target;
                        damageDisplay.GetComponent<DamageDisplay>().damageDisplayText.text = currentWeapon.GetComponent<WeaponInfo>().damage.ToString();
                    }
                    target.GetComponent<TargetBehavior>().DamageBehavior(false, currentWeapon.GetComponent<WeaponInfo>().damage, raycastHit.point, playerCam.transform.forward);
                }
                //Scoring code here (rework legacy for Trials only)

            }
            //TBC Legacy Scoring/physics code for other targets
            /*
                        //TODO: Integrate scoring to above function
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
                        }*/
        }
        this.weaponRecoil();
        this.weaponShootingSound();
    }

    //TBC Legacy Scoring/physics code for grenade with other targets

    /*    public void hitByProjectile(GameObject target)
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
        }*/

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
        if(currentWeapon != null)
        {
            WeaponInfo currentWeaponInfo = currentWeapon.GetComponent<WeaponInfo>();
            weaponText.text = currentWeaponInfo.name;
            ammoCurrentMagText.text = currentWeaponInfo.currentMagAmmo.ToString();
            ammoBackupText.text = currentWeaponInfo.backupAmmo.ToString();
            //Set firing interval according to input firing rate
            fRateInt = 1f / currentWeaponInfo.firingRate;
            if (currentWeaponInfo.backupAmmo > 0)
            {
                currentNoAmmo = false;
            }
            else if(currentWeaponInfo.backupAmmo == 0)
            {
                currentNoAmmo = true;
            }
            firePoint = currentWeapon.transform.GetChild(0).Find("FirePoint");
            playerCam.GetComponent<CameraController>().UpdateWeaponInfo(currentWeaponInfo.maxHorizontalRecoil, currentWeaponInfo.minHorizontalRecoil, currentWeaponInfo.verticalRecoil);
        }
        else
        {
            weaponText.text = "";
            ammoCurrentMagText.text = "";
            ammoBackupText.text = "";
            fRateInt = 0f;
            firePoint = null;
            playerCam.GetComponent<CameraController>().UpdateWeaponInfo(0f,0f,0f);
        }
        sceneManager.GetComponent<LevelSceneManager>().UpdateWeaponInfo();
    }
    public void UpdateEquipmentInfo()
    {
        if (equipmentEquippedPrimary)
        {
            //Choose and enable equipment primary HUD
            if(equipmentPrimary == Equipment.impactGrenade)
            {
                imageHUDEquipmentPrimary.sprite = Resources.Load<Sprite>("Images/HUDIcon_ImpactGrenade");
            }
            imageHUDEquipmentPrimary.enabled = true;
        }
        else
        {
            equipmentPrimary = Equipment.None;
            //disable equipment primary HUD
            imageHUDEquipmentPrimary.enabled = false;
        }
        if (equipmentEquippedSecondary)
        {
            //enable equipment secondary HUD
            imageHUDEquipmentPrimary.enabled = true;
        }
        else
        {
            equipmentSecondary = Equipment.None;
            //disable equipment secondary HUD
            imageHUDEquipmentSecondary.enabled = false;
        }
    }
    //Future possibly adapt to all projectile function
        IEnumerator ThrowImpactGrenade(float time)
    {
        yield return new WaitForSeconds(weaponPutDownTime);
        if (weaponEquipped)
        {
            currentWeapon.GetComponent<animController>().animator.SetBool("isPuttingdown", false);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isPuttingdown", false);
        }
        var impactGrenadeObject = Instantiate(equipmentObjs[(int)Equipment.impactGrenade], projectileFirePoint.transform.position, Quaternion.Euler(playerCam.transform.forward));
        impactGrenadeObject.transform.LookAt(playerCam.transform.forward * 1000);
        impactGrenadeObject.GetComponent<Rigidbody>().velocity = new Vector3 (player.GetComponent<CharacterController>().velocity.x, player.GetComponent<CharacterController>().velocity.y/2, player.GetComponent<CharacterController>().velocity.z);
        // Modify the throwing angle so the grenade is a bit higher than horizontal
        Vector3 currentRotation = impactGrenadeObject.transform.eulerAngles;
        Vector3 modifiedRotation = currentRotation + new Vector3(-2f, 0f, 0f);
        impactGrenadeObject.transform.localRotation = Quaternion.Euler(modifiedRotation);
        if(currentTrial == 0) //WIP Trial
        {
            if(player.transform.position.x <= 9f)
            {
                trialScript.grenadeObj = impactGrenadeObject;
                trialScript.grenadeInitialPosition = projectileFirePoint.transform.position;
            }
            else
            {   //This will Not Trigger boundary grenades StopTrial in trial site
                trialScript.grenadeBeforeGone = false;
            }
        }
        else
        {
            trialScript.grenadeBeforeGone = false;
        }
        equipmentPrimaryCDCounter = equipmentCoolDownPrimary;
        yield return new WaitForSeconds(time);
        if (weaponEquipped)
        {
            currentWeapon.GetComponent<animController>().animator.SetBool("isPickingup", true);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isPickingup", true);
        }
        yield return new WaitForSeconds(weaponPickupTime);
        if (weaponEquipped)
        {
            currentWeapon.GetComponent<animController>().animator.SetBool("isPickingup", false);
            currentWeaponPOV.GetComponent<animController>().animator.SetBool("isPickingup", false);
        }
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
    //Manual fix shooting animation transition stuck
    IEnumerator EmptyReloadDelayer()
    {
        yield return new WaitForFixedUpdate();
        currentWeapon.GetComponent<animController>().EmptyReloadAnimation();
        currentWeaponPOV.GetComponent<animController>().EmptyReloadAnimation();
    }
    IEnumerator TacticalReloadDelayer()
    {
        yield return new WaitForFixedUpdate();
        currentWeapon.GetComponent<animController>().TacticalReloadAnimation();
        currentWeaponPOV.GetComponent<animController>().TacticalReloadAnimation();
    }
    IEnumerator ReloadShootingAnimation()
    {
        yield return new WaitForSecondsRealtime(0.13f);
        shooting = false;
    }
    IEnumerator ReloadShootingLastRoundAnimation()
    {
        yield return new WaitForSecondsRealtime(0.17f);
        shootingLastRound = false;
    }
    IEnumerator InteractionCoolDown(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        interactionCoolDown = false;
    }

    IEnumerator AmmoBoxCoolDown(GameObject ammoBoxObj)
    {
        //Color _emissionColorValue = ammoBoxObj.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
        ammoBoxObj.GetComponent<SelfRotating>().stopped = true;
        ammoBoxObj.GetComponent<ButtonInfo>().GUIDisplayText[ammoBoxObj.GetComponent<ButtonInfo>().typeIdentifier] = "Cooling Down";
        //ammoBoxObj.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", _emissionColorValue * 0f);
        //ammoBoxObj.GetComponentInChildren<Light>().intensity = 0f;

        //yield return new WaitForSecondsRealtime(ammoBoxObj.GetComponent<ButtonInfo>().coolDown);
        yield return new WaitForSecondsRealtime(ammoBoxObj.GetComponent<ButtonInfo>().coolDown*10f);
        ammoBoxObj.GetComponent<SelfRotating>().stopped = false;
        ammoBoxObj.GetComponent<ButtonInfo>().GUIDisplayText[ammoBoxObj.GetComponent<ButtonInfo>().typeIdentifier] = "[E] Refill Ammo";

        //ammoBoxObj.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", _emissionColorValue);
        //ammoBoxObj.GetComponentInChildren<Light>().intensity = 2f;
    }

    //Temporary GUI for pickup
    private void OnGUI()
    {
        if (pickupLookingat)
        {
            GUI.Box(new Rect(Screen.width / 2 +10f, Screen.height / 2 - 50f, Screen.width/4, Screen.height/6), "[E] Pick up " + pickupName, stylePickupBoxGUI);
        }

        //TODO: Add temp block for GUIText and add "Trial Active" condition
        if (interactablesLookingat)
        {
            GUI.Box(new Rect(Screen.width / 2 + 10f, Screen.height / 2 - 50f, Screen.width / 4, Screen.height / 6), interactableObj.GetComponent<ButtonInfo>().GUIDisplayText[interactableObj.GetComponent<ButtonInfo>().typeIdentifier], stylePickupBoxGUI);
        }
    }
}

