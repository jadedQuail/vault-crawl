using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    public CircleCollider2D characterCollider;
    public CircleCollider2D blockerCollider;

    // Start is called before the first frame update
    void Awake()
    {
        if (characterCollider != null && blockerCollider != null)
        {
            Physics2D.IgnoreCollision(characterCollider, blockerCollider, true);
        }
    }
}
