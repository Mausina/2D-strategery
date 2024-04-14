using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Archer;
using Farmer;
using SwordsMan;
public class WorldPoolManager : MonoBehaviour
{
    private GameObject safeZone;
    private GameObject searchZone;
    private GameObject CampFireZone;
    private GameObject field;
    public List<ArcherController> archers = new List<ArcherController>();
    public List<BuilderController> builders = new List<BuilderController>();
    public List<FarmerController> farmers = new List<FarmerController>();
    public List<SwordsmanController> swordsmans = new List<SwordsmanController>();
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Use switch statement to handle different collider tags
        switch (other.tag)
        {
            case "Field":
                Debug.Log("Field found!");
                field = other.gameObject;
                UpdateFieldLocation();
                break;
            case "SafeZone":
                Debug.Log("SafeZone find!");
                safeZone = other.gameObject;
                UpdateSafeZones();
                break;

            case "SearchZone":
                Debug.Log("SearchZone find!");
                searchZone = other.gameObject;
                UpdateArcherSearchZones();
                break;

            case "CampFire":
                Debug.Log("Camp Fire find!");
                CampFireZone = other.gameObject;
                UpdateFarmerCampFireZone();
                break;

            case "Archer":
                Debug.Log("Archer entered");
                ArcherController archer = other.GetComponent<ArcherController>();
                if (archer != null && !archers.Contains(archer))
                {
                    RegisterArcher(archer);
                }
                break;

            case "Builder":
                BuilderController builder = other.GetComponent<BuilderController>();
                if (builder != null && !builders.Contains(builder))
                {
                    RegisterBuilder(builder);
                }
                break;

            case "Farmer":
                FarmerController farmer = other.GetComponent<FarmerController>();
                if (farmer != null && !farmers.Contains(farmer))
                {
                    RegisterFarmer(farmer);
                }
                break;

            case "Swordsman":
                SwordsmanController swordsman = other.GetComponent<SwordsmanController>();
                if (swordsman != null && !swordsmans.Contains(swordsman))
                {
                    RegisterSwordsman(swordsman);
                }
                break;

            default:
                // Optionally handle the case where no known tag is matched
                //Debug.Log("Unknown tag detected: " + other.tag);
                break;
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
    private void RegisterSwordsman(SwordsmanController swardman)
    {
        swordsmans.Add(swardman);
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

    private void UpdateSafeZones()
    {
        foreach (ArcherController archer in archers)
        {
            archer.SetSafeZone(safeZone);
        }
        foreach (SwordsmanController swordsman in swordsmans)
        {
            swordsman.SetSafeZone(safeZone);
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
        foreach (FarmerController farmer in farmers)
        {
            farmer.SetCampFireZone(CampFireZone);
        }
    }

    private void UpdateFieldLocation()
    {
        if (farmers.Count > 0 && field != null)
        {
            // Sending field coordinates to the first farmer in the list
            farmers[0].SetField(field);
            Debug.Log($"Field coordinates sent to Farmer: {field.transform.position}");
        }
    }
}
