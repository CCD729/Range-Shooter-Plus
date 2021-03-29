using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScript : MonoBehaviour
{
    //Scene management
    public GameObject sceneManmager;
    private bool levelEnded = false;
    private bool levelPaused = false;
    //Bullets per second
    public float firingRate = 10f;
    //Auto? Semi-auto?
    public bool autoTrigger;
    //Magazine size
    public int magSize = 30;
    //References
    public GameObject bullet, gun;
    public Transform firePoint, detectPoint;
    public Text scoreText, ammoText, timeText;
    public GameObject[] BulletTextures;
    public ParticleSystem hitTargetParticle, hitOthersParticle;
    private GameObject character;
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
        Time.timeScale = 1;
        character = this.transform.parent.gameObject;
    }

    void Update()
    {
        if(count == 50)
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
        if (Input.GetKeyDown(KeyCode.R) && currentMag < magSize && !reloading && !levelPaused && !levelEnded)
        {
            reloading = true;
            ammoText.text = "Reloading";
            gun.GetComponent<animController>().ReloadAnimation();
            circleProgressBar.GetComponent<ReloadRingAnim>().Play();
            bulletIcon.enabled = true;
            circleProgressBar.enabled = true;
            crossHair.enabled = false;
            this.ReloadSound();
            Debug.Log("Reloading...");
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
            ray = new Ray(this.transform.position, this.transform.forward);
            if (Input.GetMouseButton(0))
            {
                //Do nothing if reloading 
                if (!reloading)
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


    void Shoot()
    {
        fRatePassed = false;
        currentMag--;
        // Check if we hit anything
        bool hit = Physics.Raycast(ray, out raycastHit, Mathf.Infinity, layerMask);
        // If we did...Shoot to the hitposition
        if (!hit)
        {
            //Modify The angle a bit if not hitting anything
            var straightQ = Quaternion.LookRotation(-this.transform.up, this.transform.forward);
            Vector3 straightV = straightQ.eulerAngles;
            Vector3 modifiedV = straightV + new Vector3(0f,-0.4f,0f);
            //Shoot
            //TODO: Destroy hitted object if target, Score, Recoil, HittingSound, and HittingSparkles
            var clone = Instantiate(bullet, firePoint.position, Quaternion.Euler(modifiedV));
        }
        else
        {
            //Shoot to hit pos
            Vector3 relativePos = raycastHit.point - detectPoint.position;
            Quaternion preQ = Quaternion.LookRotation(relativePos);
            //Modify The angle a bit
            Vector3 preV = preQ.eulerAngles;
            Vector3 modifiedV = preV + new Vector3(90f, 0f, 0f);
            //TODO: Recoil, HittingSound, and HittingSparkles
            var clone = Instantiate(bullet, firePoint.position, Quaternion.Euler(modifiedV));
            clone.GetComponent<BulletMovement>().hit = true;
            clone.GetComponent<BulletMovement>().hitPoint = raycastHit.point;

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
                if (!target.GetComponent<TargetBehavior>().hit)
                {
                    count++;
                    scoreText.text = "Score: " + count.ToString();
                }
                target.GetComponent<TargetBehavior>().Hit(raycastHit.point, this.transform.forward);
            }
            //Moving target hit
            if (target.CompareTag("MovingTarget"))
            {
                if (!target.GetComponent<MovingTargetBehavior>().hit)
                {
                    count += 2;
                    scoreText.text = "Score: " + count.ToString();
                }
                target.GetComponent<MovingTargetBehavior>().Hit(raycastHit.point, this.transform.forward);
            }
            //Another version of moving target (needs optimization)
            if (target.CompareTag("RailTarget"))
            {
                if (!target.GetComponent<RailTargetBehavior>().hit)
                {
                    count += 1;
                    scoreText.text = "Score: " + count.ToString();
                }
                target.GetComponent<RailTargetBehavior>().Hit(raycastHit.point, this.transform.forward);
            }
        }
        this.Recoil();
        this.ShootingSound();
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
        GetComponent<CameraController>().Recoil();
    }
    public void ShootingSound()
    {
        gun.GetComponent<SoundScript>().ShootSound();
    }
    public void ReloadSound()
    {
        gun.GetComponent<SoundScript>().ReloadSound();
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
