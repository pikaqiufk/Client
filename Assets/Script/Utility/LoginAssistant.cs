using UnityEngine;
using System.Collections;

public class LoginAssistant : MonoBehaviour
{
    private int lCounterForward = 0;
    private int rCounterForward = 0;

    private int lCounterReverse = 0;
    private int rCounterReverse = 0;
    // Use this for initialization
    public static void CreateAssistant(Transform parent)
    {
        ResourceManager.PrepareResource<GameObject>("UI/LoginAssistant.prefab", (res) =>
        {
            var bd = Instantiate(res) as GameObject;
            if (bd != null) bd.transform.parent = parent;
            bd.transform.localScale = Vector3.one;
            bd.transform.localPosition = Vector3.zero;
        });
    }

    public void OnLeftClick()
    {
        lCounterForward++;
        lCounterReverse++;

        if (rCounterReverse != 3)
        {
            lCounterReverse = 0;
            rCounterReverse = 0;
        }
        CheckCondition();
    }

    public void OnRightClick()
    {
        rCounterForward++;
        rCounterReverse++;

        if (lCounterForward != 3)
        {
            rCounterForward = 0;
            lCounterForward = 0;
        }

        CheckCondition();
    }

    private void CheckCondition()
    {
        if (lCounterForward == 3 && rCounterForward == 7)
        {
            DoForward();
        }

        if (rCounterReverse == 3 && lCounterReverse == 7)
        {
            DoReverse();
        }
    }

    private void DoForward()
    {
        UIManager.Instance.ShowMessage(MessageBoxType.Ok, "debug mode enable!", "", () =>
        {
            PlayerPrefs.SetInt(GameSetting.LoginAssistantKey, 1);
            PlayerPrefs.Save();
            Game.Instance.ExitToLogin();
        });
    }

    private void DoReverse()
    {
        UIManager.Instance.ShowMessage(MessageBoxType.Ok, "debug mode cancel!", "", () =>
        {
            PlayerPrefs.SetInt(GameSetting.LoginAssistantKey, 0);
            PlayerPrefs.Save();
            Game.Instance.ExitToLogin();
        });
    }

}
