using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    private int _value;

    public int OnPickUp()
    {
        this.gameObject.SetActive(false);
        return this._value;
    }
}