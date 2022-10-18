using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    
    public void SetHealthFill(float healthFillAmount)
    {
        healthBarImage.fillAmount = healthFillAmount;
    }

    private void Update()
    {
        
    }
}
