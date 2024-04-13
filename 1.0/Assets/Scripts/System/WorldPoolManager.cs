using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Archer;
using Farmer;
public class WorldPoolManager : MonoBehaviour
{
    private GameObject safeZone;
    private GameObject searchZone;
    private GameObject CampFireZone;
    public List<ArcherController> archers = new List<ArcherController>();
    public List<BuilderController> builders = new List<BuilderController>();
    public List<FarmerController> farmers = new List<FarmerController>();

    private void OnTriggerEnter2D(Collider2D other) 
    {

        // Trigger detection for SafeZone, SearchZone, and etc
        if (other.CompareTag("SafeZone"))
        {
            Debug.Log("SafeZone find!");
            safeZone = other.gameObject;
            UpdateArcherSafeZones();
        }
        else if (other.CompareTag("SearchZone"))
        {
            Debug.Log("SearchZone find!");
            searchZone = other.gameObject;
            UpdateArcherSearchZones();
        }
        else if (other.CompareTag("CampFire"))
        {
            Debug.Log("Camp Fire find!");
            CampFireZone = other.gameObject;
            UpdateFarmerCampFireZone();

        }
        else if (other.CompareTag("Archer")) 
        {
            Debug.Log("Archer entered");
            ArcherController archer = other.GetComponent<ArcherController>();
            if (archer != null && !archers.Contains(archer))
            {
                RegisterArcher(archer);
            }
        }
        else if (other.CompareTag("Builder"))
        {
            BuilderController builder = other.GetComponent<BuilderController>();
            if (builder != null && !builders.Contains(builder))
            {
                RegisterBuilder(builder);
            }
        }
        else if (other.CompareTag("Farmer"))
        {
            FarmerController farmer = other.GetComponent<FarmerController>();
            if (farmer != null && !farmers.Contains(farmer))
            {
                RegisterFarmer(farmer);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Archer")) 
        {
            Debug.Log("Archer exited");
            ArcherController archer = other.GetComponent<ArcherController>();
            if (archer != null)
            {
                DeregisterArcher(archer);
            }
        }
    }
    private void RegisterBuilder(BuilderController builder) 
    {
        builders.Add(builder);
        builder.SetSafeZone(safeZone); // Assuming SetSafeZone(GameObject safeZone) is implemented in BuilderController
    }
    private void UpdateBuildersSafeZone() // Add this method
    {
        foreach (BuilderController builder in builders)
        {
            builder.SetSafeZone(safeZone);
        }
    }

    private void RegisterFarmer(FarmerController farmer)
    {
        farmers.Add(farmer);
    }

    private void RegisterArcher(ArcherController archer)
    {
        archers.Add(archer);
        archer.AssignPool(this);
    }

    private void DeregisterArcher(Archer.ArcherController archer)
    {
        archers.Remove(archer);
        archer.AssignPool(null);
    }

    private void UpdateArcherSafeZones()
    {
        foreach (ArcherController archer in archers)
        {
            archer.SetSafeZone(safeZone);
        }
    }

    private void UpdateArcherSearchZones()
    {
        foreach (Archer.ArcherController archer in archers)
        {
            archer.SetSearchZone(searchZone);
        }
    }


    private void UpdateFarmerCampFireZone()
    {
        foreach(FarmerController farmer in farmers)
        {
            farmer.SetCampFireZone(CampFireZone);
        }
    }
}
