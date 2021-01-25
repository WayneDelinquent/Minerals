using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Enemy _enemy;
    private Slider _slider;

    // Start is called before the first frame update
    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_slider == null) return;
        _slider.value = _enemy.GetHealthPercentage();
    }
}
