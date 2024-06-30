using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterControl : MonoBehaviour
{
    public int mobHP;
    int mobDMG;
    // Start is called before the first frame update
    void Start()
    {
        mobHP = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (mobHP <= 0)
        {
            Destroy(gameObject);
        }
            
    }
}
