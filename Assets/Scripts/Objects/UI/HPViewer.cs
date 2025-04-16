using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPViewer : MonoBehaviour
{
    private EnemyHP enemyHP;
    private UnitHP unitHP;
    
    private Slider hpSlider;

    public void EnemyHPSetup(EnemyHP enemyHP)
    {
        this.enemyHP = enemyHP;
        hpSlider = GetComponent<Slider>();
    }

    public void UnitHPSetup(UnitHP unitHP)
    {
        
    }
    
    
    
    private void Update()
    {
    hpSlider.value = enemyHP.CurrentHP / enemyHP.MaxHP;
    }
}