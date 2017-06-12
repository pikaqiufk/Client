using System;
#region using

using System.Collections;
using UnityEngine;

#endregion

public class LoginEffect : MonoBehaviour
{
    public static int playTimes;
    public GameObject Camera;
    public float Delay = 1.0f;
    public Vector3 DesDir;
    public Vector3 DesPos;
    public float MoveTime = 2.0f;
    public GameObject Title;
    public LoginWindow Window;

    private IEnumerator CameraCoroutine(float time, Vector3 pos, Vector3 angle)
    {
        var forward = Quaternion.Euler(angle.x, angle.y, angle.z)*Vector3.forward;

        while (time > 0)
        {
            time -= Time.deltaTime;
            Camera.transform.position =
                Vector3.Lerp(Camera.transform.position,
                    pos,
                    Time.deltaTime/time);

            Camera.transform.forward =
                Vector3.Slerp(Camera.transform.forward,
                    forward,
                    Time.deltaTime/time);

            yield return null;
        }
    }

    private IEnumerator DoEffect()
    {
        yield return StartCoroutine(CameraCoroutine(MoveTime, DesPos, DesDir));
        Title.SetActive(true);
        yield return new WaitForSeconds(Delay);
        Window.gameObject.SetActive(true);
        while (true)
        {
            const float range = 0.5f;
            const float speed = 0.08f;
            var p = DesPos + new Vector3(UnityEngine.Random.Range(-range, range),
                UnityEngine.Random.Range(-range, range),
                0);
            var time = (p - Camera.transform.position).magnitude/speed;
            yield return StartCoroutine(CameraCoroutine(time, p, DesDir));
        }
    }

    private IEnumerator DoEffectQuickly()
    {
        var forward = Quaternion.Euler(DesDir.x, DesDir.y, DesDir.z)*Vector3.forward;
        Camera.transform.position = DesPos;
        Camera.transform.forward = forward;
        Title.SetActive(true);
        Window.gameObject.SetActive(true);
        yield return new WaitForSeconds(Delay);
        while (true)
        {
            const float range = 0.5f;
            const float speed = 0.08f;
            var p = DesPos + new Vector3(UnityEngine.Random.Range(-range, range),
                UnityEngine.Random.Range(-range, range),
                0);
            var time = (p - Camera.transform.position).magnitude/speed;
            yield return StartCoroutine(CameraCoroutine(time, p, DesDir));
        }
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif
	    MoveTime = 0.1f;
		Delay = 0.1f;
        if (playTimes == 0)
        {
            playTimes++;
            StartCoroutine(DoEffect());
        }
        else
        {
            StartCoroutine(DoEffectQuickly());
        }
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
}