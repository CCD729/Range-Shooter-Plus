using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    //New recoil references
    public Vector3 hipFireRecoilRotation;
    public Vector3 adsRecoilRotation;
    public Vector3 targetRotation;
    public Vector3 modifiedRotation;
    public float recoilSpeed;
    public float recoilRecoverSpeed;

    void FixedUpdate()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, recoilRecoverSpeed * Time.fixedDeltaTime);
        modifiedRotation = Vector3.Slerp(modifiedRotation, targetRotation, recoilSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(modifiedRotation);
    }
    public void Recoil(bool ads)
    {
        //recoil = true;
        if (ads)
        {
            targetRotation += new Vector3(-adsRecoilRotation.x, Random.Range(-adsRecoilRotation.y, adsRecoilRotation.y), Random.Range(-adsRecoilRotation.z, adsRecoilRotation.z));
        }
        else
        {
            targetRotation += new Vector3(-hipFireRecoilRotation.x, Random.Range(-hipFireRecoilRotation.y, hipFireRecoilRotation.y), Random.Range(-hipFireRecoilRotation.z, hipFireRecoilRotation.z));
        }
    }
}
