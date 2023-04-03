using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OkapiKit;
using TMPro;

public class PlayerHealthDisplay : MonoBehaviour
{
    [SerializeField] private Hypertag   playerTag;
    [SerializeField] private Color      activeColor;
    [SerializeField] private Color      inactiveColor;

    private HealthSystem            hs;
    private List<TextMeshProUGUI>   elements;

    private void Start()
    {
        elements = new List<TextMeshProUGUI>();

        foreach (Transform t in transform)
        {
            elements.Add(t.gameObject.GetComponent<TextMeshProUGUI>());
        }

    }
    void Update()
    {
        if (hs == null)
        {
            hs = gameObject.FindObjectOfTypeWithHypertag<HealthSystem>(playerTag);
            if (hs == null) return;

        }
        
        int v = Mathf.FloorToInt(hs.GetCurrentHealth());

        int i = 0;
        foreach (var elem in elements)
        {
            elem.color = (i < v)?(activeColor):(inactiveColor);
            i++;
        }
    }
}