using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFlowController : MonoBehaviour
{
    bool waiting;

    private void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SlowTime(3f);
            }
        }
    }

    public void SlowTime(float duration)
    {
        if (waiting)
        {
            return;
        }

        Time.timeScale = .3f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        waiting = false;
        Time.timeScale = 1f;
    }
}
