using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectExplodeEffecct : MonoBehaviour
{
    public GameObject[] bodyParts;
    public float explosionForce = 500f;
    public float partLifetime = 3.5f;
    public void Break()
    {
        Destroy(this.gameObject);
        foreach (var part in bodyParts)
        {
            if (part == null)
            {
                continue;
            }
              
            var go = Instantiate(part, transform.position, Quaternion.identity, transform.parent);
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = go.AddComponent<Rigidbody2D>();
            }
            float angle = Random.Range(-90f, 90f); 
            Vector2 upwardDir = Quaternion.Euler(0, 0, angle) * Vector2.up;
             
            rb.AddForce(upwardDir * explosionForce);
              
            float randomTorque = Random.Range(-200f, 200f);
            rb.AddTorque(randomTorque);
              
            Destroy(go, partLifetime);
        }
    }
}
