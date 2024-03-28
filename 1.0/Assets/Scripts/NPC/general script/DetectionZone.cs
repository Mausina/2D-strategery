using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    private Collider2D col;
    public UpgradeBuildingAnimation upgradeBuildingAnimatio;
    private void Awake()
    {
        col = GetComponent<Collider2D>();

        upgradeBuildingAnimatio = GetComponentInParent<UpgradeBuildingAnimation>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
       // if (collision.CompareTag("Builder"))
       // {
       //
       //     // Builder enters the zone, start or resume animation
       //     upgradeBuildingAnimatio.StartUpgradeAnimation(true);
        //}
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
       // if (collision.CompareTag("Builder"))
      //  {
       //     // Builder exits the zone, freeze animation
        //    upgradeBuildingAnimatio.StartUpgradeAnimation(false);
       // }
    }
    
}
