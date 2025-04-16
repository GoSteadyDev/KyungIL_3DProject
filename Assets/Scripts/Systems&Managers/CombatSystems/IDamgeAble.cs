using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
   float CurrentHP { get; }
   float MaxHP { get; }
   
   public void TakeDamage(float amount);
}