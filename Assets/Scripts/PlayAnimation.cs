using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Animation anim;
    public float FastPerSecond = 0.5f;

    void Start()
    {
        anim = GetComponent<Animation>();
        foreach (AnimationState state in anim)
        {
            state.speed = FastPerSecond;
        }
    }
}
