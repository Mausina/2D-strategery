using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectGenerator : MonoBehaviour
{
    [System.Serializable]
    public class ObjectToGenerate
    {
        public GameObject Prefab;
        public int NumberOfObjects;
        public float MinScale = 0.8f; // Minimum scale of the object
        public float MaxScale = 1.2f; // Maximum scale of the object
        public bool RandomRotation = false; // Should the object have a random rotation?
    }

    public List<ObjectToGenerate> ObjectsToGenerate; // Assign your objects and quantities in the inspector
    public GameObject PointAObject; // Assign your Point A GameObject in the inspector
    public GameObject PointBObject; // Assign your Point B GameObject in the inspector
    public float MinDistanceBetweenObjects = 2f; // Minimum distance between objects
    public GameObject ReferenceObject; // The object on which generated objects should spawn
    public GameObject BushPrefab; // Assign your bush prefab in the inspector
    public float MinDistanceBetweenBushes = 1f; // Minimum distance between bushes
    public float MaxDistanceBetweenBushes = 13f; // Maximum distance between bushes

    private List<GameObject> generatedObjects;

    void Start()
    {
        generatedObjects = new List<GameObject>();
        foreach (var objectToGenerate in ObjectsToGenerate)
        {
            GenerateObjects(objectToGenerate);
        }
        GenerateBushesAlongReferenceObject();
    }
    void GenerateBushesAlongReferenceObject()
    {
        Collider2D referenceCollider = ReferenceObject.GetComponent<Collider2D>();
        if (referenceCollider == null)
        {
            Debug.LogError("ReferenceObject does not have a Collider2D component.");
            return;
        }

        Bounds bounds = referenceCollider.bounds;
        float currentX = bounds.min.x;
        bool firstBush = true; // Flag to track if the bush is the first one being spawned

        while (currentX < bounds.max.x)
        {
            if (firstBush)
            {
                firstBush = false; // Ensure the first bush spawns at the very beginning
            }
            else
            {
                float randomDistance = Random.Range(MinDistanceBetweenBushes, MaxDistanceBetweenBushes);
                currentX += randomDistance;
            }

            if (currentX <= bounds.max.x)
            {
                Vector2 spawnPosition = new Vector2(currentX, bounds.min.y + 1);
                GameObject bush = Instantiate(BushPrefab, spawnPosition, Quaternion.identity);
                generatedObjects.Add(bush);
            }
        }
    }

    void GenerateObjects(ObjectToGenerate objectToGenerate)
    {
        Vector2 center = (PointAObject.transform.position + PointBObject.transform.position) / 2;
        Vector2 size = new Vector2(Mathf.Abs(PointAObject.transform.position.x - PointBObject.transform.position.x),
                                   Mathf.Abs(PointAObject.transform.position.y - PointBObject.transform.position.y));

        float objectHeight = GetPrefabHeight(objectToGenerate.Prefab);

        for (int i = 0; i < objectToGenerate.NumberOfObjects; i++)
        {
            Vector2 position = Vector2.zero;
            bool positionFound = false;

            while (!positionFound)
            {
                position = new Vector2(Random.Range(center.x - size.x / 2, center.x + size.x / 2), center.y);
                position = FindSpawnPoint(position, objectHeight);
                positionFound = true;

                foreach (GameObject otherObject in generatedObjects)
                {
                    if (Vector2.Distance(otherObject.transform.position, position) < MinDistanceBetweenObjects)
                    {
                        positionFound = false;
                        break;
                    }
                }
            }

            GameObject newObj = Instantiate(objectToGenerate.Prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            generatedObjects.Add(newObj);

            float scale = Random.Range(objectToGenerate.MinScale, objectToGenerate.MaxScale);
            newObj.transform.localScale = new Vector3(scale, scale, scale);

            if (objectToGenerate.RandomRotation)
            {
                float rotation = Random.Range(0f, 360f);
                newObj.transform.eulerAngles = new Vector3(0f, 0f, rotation);
            }
        }
    }

    private Vector2 FindSpawnPoint(Vector2 spawnPosition, float objectHeight)
    {

        Collider2D referenceCollider = ReferenceObject.GetComponent<Collider2D>();
        if (referenceCollider != null)
        {
            Bounds bounds = referenceCollider.bounds;
            // If the pivot is at the bottom of the prefab, we add the full objectHeight
            // This will place the bottom of the prefab at the top of the reference object
            float spawnY = bounds.max.y + objectHeight - 5.7f;

            if (spawnPosition.x >= bounds.min.x && spawnPosition.x <= bounds.max.x)
            {
                // Return the x position and a y position that's been adjusted to be on top of the reference object
                return new Vector2(spawnPosition.x, spawnY);
            }
        }
        //Debug.LogError("ReferenceObject does not have a Collider2D component.");
        return spawnPosition; // Fallback to the original position if no collider is found
    }



    private float GetPrefabHeight(GameObject prefab)
    {
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return spriteRenderer.sprite.bounds.size.y;
        }
        else
        {
            Debug.LogError("Prefab does not have a SpriteRenderer component.");
            return 0f;
        }
    }
}
