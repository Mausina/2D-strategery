using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebarrel : MonoBehaviour
{
    public GameObject firePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            Instantiate(firePrefab, transform.position, Quaternion.identity);


            Destroy(gameObject);
        }
    }
}
 