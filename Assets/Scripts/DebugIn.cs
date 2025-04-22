using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugIn : MonoBehaviour
{
    public int count = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.green);
        // Debug.DrawLine(transform.position + Vector3.up, transform.position + transform.TransformDirection(Vector3.right), Color.red);

        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Print : {i}");
        }
    }
}

public class Player
{
    public int maxHp;
    public int currentHp;

    public Player()
    {
        maxHp = 100;
        currentHp = 100;
    }
    
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Debug.Log("죽음");
        }
    }

    public void Heal(int heal)
    {
        currentHp += heal;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        
        Debug.Log($"Heal {currentHp}");
    }
}