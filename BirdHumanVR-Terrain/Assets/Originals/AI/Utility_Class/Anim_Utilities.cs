using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Animator))]

public abstract class Anim_Utilities : Utilities
{
    protected abstract Animator anim {get; set;}

    // Animation Clipの再生時間を取得
    protected float GetClipLength(string clip_name)
    {
        float clip_length = 0;
        var clips = anim.runtimeAnimatorController.animationClips.Where(c => c.name == clip_name);
        foreach(AnimationClip clip in clips)
        {
            clip_length = clip.length;
        }
        return clip_length;
    }
}
