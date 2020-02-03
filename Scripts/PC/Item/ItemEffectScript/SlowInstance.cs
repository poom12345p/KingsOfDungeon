using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowInstance : MonoBehaviour
{

    public float speedSlowPercent = 0.5f;
    public float slowTime = 4f;
    unitHitbox targetSlowed;


    public void SetSlowTarget(unitHitbox target)
    {
        if (target != null && target.statContol != null)
        {
            targetSlowed = target;
            targetSlowed.statContol.ChangeSpeedPercent(-speedSlowPercent);
            Destroy(gameObject, slowTime);
        }
    }

    private void OnDestroy()
    {
        if (targetSlowed != null && targetSlowed.statContol != null)
            targetSlowed.statContol.ChangeSpeedPercent(speedSlowPercent);
    }

}
