using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatlhBar : MonoBehaviour
{
    /// <summary>
    /// Referencia a la máscara que usaremos para mostrar la vida 
    /// </summary>
    private Mask healthMask;

    /// <summary>
    /// El porcentaje de vida del jugador
    /// </summary>
    private int health = 100;

    private float maxWidth;

    void Awake()
    {
        healthMask = GetComponentInChildren<Mask>();
        maxWidth = healthMask.rectTransform.sizeDelta.x;
    }

    public void SetHealth(int h)
    {

        health = h - 1;
        if (health > 100) health = 100;
        if (health < 0) health = 0;
        UpdateMask();
    }

    public void UpdateMask() 
    {
        healthMask.rectTransform.sizeDelta = new Vector2(health * maxWidth / 100, healthMask.rectTransform.sizeDelta.y);
    }
}
