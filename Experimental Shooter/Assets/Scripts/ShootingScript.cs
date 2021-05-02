using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScript : MonoBehaviour
{
    //create enum for future additions
    enum Skill
    {
        impactGrenade
    }
    //Scene management
    public GameObject sceneManmager;
    private bool levelEnded = false;
    private bool levelPaused = false;
    //Bullets per second
    public float firingRate = 10f;
    //Auto? Semi-auto? is this gamemode Timed?
    public bool autoTrigger, mode_Timed, mode_Scored;
    //Skill/Equipment (when therea re more skills would put data in dictionary for better management)
    [SerializeField]
    private Skill skill;
    [SerializeField]
    private KeyCode skillKey = KeyCode.G;
    [SerializeField]
    private float skillCoolDown = 5f;
    private float skillCDCounter = 0f;
    [SerializeField]
    private bool skillReady = true;

    //Magazine size
    public int magSize = 30;
    //References
    public GameObject bullet, gun, gunPOV, impactGrenadeObj;
    public Transform firePoint, detectPoint, projectileFirePoint;
    public Text scoreText, ammoText, timeText, skillText;
    public GameObject[] BulletTextures;
    public ParticleSystem hitTargetParticle, hitOthersParticle;
    public GameObject playerCam;
    private int count;

    //Reload Assets
    public Image crossHair, bulletIcon, circleProgressBar;

    [SerializeField]
    private float timeLimit = 45.00f;
    private float timeLeft = 45.00f;

    private Ray ray;
    private RaycastHit raycastHit;
    public LayerMask layerMask;

    //Hitting Sound
    public GameObject HittingAudioObject;

    //Current bullets left in mag
    private int currentMag = 30;
    //Reload interval
    [SerializeField]
    private float reloadTime = 1.5f;
    //Reload counter
    private float reloadingTime = 0f;
    //Reload status
    private bool reloading = false;
    //Skill interval
    [SerializeField]
    private float skillTime = 1f;
    //Skill counter
    private float skillUsingTime = 0f;
    //Skill status
    [SerializeField]
    private bool skillUsing = false;
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
        //Set firing interval according to input firing rate
        fRateInt = 1f / firingRate;
        count = 0;
        timeLeft = timeLimit;
        scoreText.text = "Score: " + count.ToString();
        timeText.text = "Time: " + timeLeft.ToString();
        if (!mode_Scored)
            scoreText.enabled = false;
        if (!mode_Timed)
            timeText.enabled = false;

        Time.timeScale = 1;
        //player = this.transform.parent.gameObject;

        if (skill == Skill.impactGrenade)
        {
            skillTime = 1f;
            skillCoolDown = 5f;
        }
    }

    void Update()
    {
        if(mode_Scored && count == 50)
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
        if (EscPressed && levelPaused == false && !levelEnded)
        {
            EscPressed = false;
            this.Pause();
        }
        else if (EscPressed && levelPaused == true && !levelEnded)
        {
            EscPressed = false;
            this.Resume();
        }
        //Manual reload
        if (Input.GetKeyDown(KeyCode.R) && currentMag < magSize && !reloading && !skillUsing && !levelPaused && !levelEnded)
        {
            reloading = true;
            ammoText.text = "Reloading";
            gun.GetComponent<animController>().ReloadAnimation();
            gunPOV.GetComponent<animController>().ReloadAnimation();
            circleProgressBar.GetComponent<ReloadRingAnim>().Play();
            bulletIcon.enabled = true;
            circleProgressBar.enabled = true;
            crossHair.enabled = false;
            this.ReloadSound();
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
        if (Input.GetKeyDown(skillKey) && !reloading && !skillUsing && skillReady && !levelPaused && !levelEnded)
        {
            skillReady = false;
            skillUsing = true;
            skillText.text = "Skill Active";
            gun.GetComponent<animController>().SkillAnimation();
            gunPOV.GetComponent<animController>().SkillAnimation();
            StartCoroutine(ThrowImpactGrenade(0.5f));
            Debug.Log("Skill Active...");
        }
    }
    void FixedUpdate()
    {
        if (!levelPaused && !levelEnded)
        {
            if (!reloading)
                ammoText.text = currentMag.ToString() + "/30";
            bulletEnough = currentMag > 0;
            // Get a ray from the camera pointing forwards
            ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            if (Input.GetMouseButton(0))
            {
                //Do nothing if reloading or using a skill
                if (!reloading && !skillUsing)
                {
                    //Start shooting if mag has bullets
                    if (bulletEnough)
                    {
                        if (fRatePassed)
                            Shoot();
                    }
                    else
                    {
                        reloading = true;
                        ammoText.text = "Reloading";
                        gun.GetComponent<animController>().ReloadAnimation();
                        gunPOV.GetComponent<animController>().ReloadAnimation();
                        circleProgressBar.GetComponent<ReloadRingAnim>().Play();
                        crossHair.enabled = false;
                        bulletIcon.enabled = true;
                        circleProgressBar.enabled = true;
                        this.ReloadSound();
                        Debug.Log("Reloading...");
                    }
                }
            }
            //Reload logic
            if (reloading)
            {
                if (reloadingTime < reloadTime)
                {
                    reloadingTime += Time.fixedDeltaTime;
                }
                else
                {
                    reloadingTime = 0f;
                    currentMag = magSize;
                    reloading = false;
                    ammoText.text = currentMag.ToString() + "/30";
                    bulletIcon.enabled = false;
                    circleProgressBar.enabled = false;
                    crossHair.enabled = true;
                    circleProgressBar.GetComponent<ReloadRingAnim>().Complete();
                    Debug.Log("Reloaded");
                }
            }
            //Skill duration logic
            if (skillUsing)
            {

                if (skillUsingTime < skillTime)
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
        Vector3 targetPoint;
        fRatePassed = false;
        currentMag--;
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
            var bulletObject = Instantiate(bullet, firePoint.position, firePoint.rotation);
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

            var bulletObject = Instantiate(bullet, firePoint.position, firePoint.rotation);
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
                    count++;
                    scoreText.text = "Score: " + count.ToString();
                }
                target.GetComponent<TargetBehavior>().Hit(raycastHit.point, playerCam.transform.forward);
            }
            //Moving target hit
            if (target.CompareTag("MovingTarget"))
            {
                if (mode_Scored && !target.GetComponent<MovingTargetBehavior>().hit)
                {
                    count += 2;
                    scoreText.text = "Score: " + count.ToString();
                }
                target.GetComponent<MovingTargetBehavior>().Hit(raycastHit.point, playerCam.transform.forward);
            }
            //Another version of moving target (needs optimization)
            if (target.CompareTag("RailTarget"))
            {
                if (mode_Scored && !target.GetComponent<RailTargetBehavior>().hit)
                {
                    count += 1;
                    scoreText.text = "Score: " + count.ToString();
                }
                target.GetComponent<RailTargetBehavior>().Hit(raycastHit.point, playerCam.transform.forward);
            }
        }
        this.Recoil();
        this.ShootingSound();
    }

    public void hitByProjectile(GameObject target)
    {
        if (target.CompareTag("Target"))
        {
            if (mode_Scored && !target.GetComponent<TargetBehavior>().hit)
            {
                count++;
                scoreText.text = "Score: " + count.ToString();
            }
            target.GetComponent<TargetBehavior>().HitByProjectile();
        }
        //Moving target hit
        if (target.CompareTag("MovingTarget"))
        {
            if (mode_Scored && !target.GetComponent<MovingTargetBehavior>().hit)
            {
                count += 2;
                scoreText.text = "Score: " + count.ToString();
            }
            target.GetComponent<TargetBehavior>().HitByProjectile();
        }
        //Another version of moving target (needs optimization)
        if (target.CompareTag("RailTarget"))
        {
            if (mode_Scored && !target.GetComponent<RailTargetBehavior>().hit)
            {
                count += 1;
                scoreText.text = "Score: " + count.ToString();
            }
            target.GetComponent<TargetBehavior>().HitByProjectile();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        sceneManmager.GetComponent<LevelSceneManager>().Pause(reloading);
        levelPaused = true;
    }
    public void Resume()
    {
        sceneManmager.GetComponent<LevelSceneManager>().Resume();
        levelPaused = false;
        Time.timeScale = 1;
    }
    public void End()
    {
        Time.timeScale = 0;
        levelEnded = true;
        sceneManmager.GetComponent<LevelSceneManager>().End(count);
    }
    public void Perfect()
    {
        Time.timeScale = 0;
        levelEnded = true;
        sceneManmager.GetComponent<LevelSceneManager>().Perfect();
    }
    public void Recoil()
    {
        playerCam.GetComponent<CameraController>().Recoil();
    }
    public void ShootingSound()
    {
        gun.GetComponent<SoundScript>().ShootSound();
    }
    public void ReloadSound()
    {
        gun.GetComponent<SoundScript>().ReloadSound();
    }
    IEnumerator ThrowImpactGrenade(float time)
    {
        yield return new WaitForSeconds(time);
        var impactGrenadeObject = Instantiate(impactGrenadeObj, projectileFirePoint.transform.position, Quaternion.Euler(playerCam.transform.forward));
        impactGrenadeObject.transform.LookAt(playerCam.transform.forward * 1000);
        //Modify the throwing angle so the grenade is a bit higher than horizontal
        Vector3 currentRotation = impactGrenadeObject.transform.eulerAngles;
        Vector3 modifiedRotation = currentRotation + new Vector3(0f, 0f, 0f);
        impactGrenadeObject.transform.localRotation = Quaternion.Euler(modifiedRotation);

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
