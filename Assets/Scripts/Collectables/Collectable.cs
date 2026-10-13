using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        OnCollected();
    }

    public virtual void OnCollected() { }
}
