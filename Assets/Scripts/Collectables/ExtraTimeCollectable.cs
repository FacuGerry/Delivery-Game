using UnityEngine;

public class ExtraTimeCollectable : Collectable
{
    public override void OnCollected()
    {
        // add time to timer
        Debug.Log("Picked up some extra time!");
        gameObject.SetActive(false);
    }
}