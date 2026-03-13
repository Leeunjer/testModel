using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image gauge;

    public void setHpGauge(float Hp)
    {
        gauge.fillAmount = Hp;
    }
}
