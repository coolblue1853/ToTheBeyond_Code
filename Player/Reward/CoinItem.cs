using UnityEngine;

public class CoinItem : MonoBehaviour
{
    [SerializeField] private int _coinAmount;



    public void Use(PlayerController player)
    {

        PermanentUpgradeManager.Instance.AddCurrency(_coinAmount);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerController controller))
        {
            Use(controller);
        }
    }


}
