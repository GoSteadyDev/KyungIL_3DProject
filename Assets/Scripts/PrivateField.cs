using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateField : MonoBehaviour
{
    public int Publicint;
    private int Privateint;
    
    public int PublicPropertyInt { get; set; }
    private int PrivatePropertyInt { get; set; }
    public int PropertyInt { get; private set; }

    IEnumerator Change()
    {
        while (gameObject.activeInHierarchy)
        {
            Publicint = Random.Range(0, 10);
            
            
            yield return new WaitForSeconds(0.5f);
        }
    }
}
