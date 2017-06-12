using UnityEngine;
using System.Collections;

public class ChangeRenderQueue : MonoBehaviour {

    public UIWidget BackgroundWidget;
    public int CustomRenderQueue = 3025;

    void Awake()
    {
        if (BackgroundWidget)
        {
            StartCoroutine(UpdateRenderQueue());
        }
        else
        {
            gameObject.SetRenderQueue(CustomRenderQueue);
        }
    }

    IEnumerator UpdateRenderQueue()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        var customRenderQueue = 0;
        if (null != BackgroundWidget)
        {
            if (null != BackgroundWidget.drawCall)
            {
                if (BackgroundWidget.SkippedRenderQueue == 0)
                {
                    //Logger.Warn("将 {0} 的SkippedRenderQueue 设置一下吧，否则有些特效或者模型没有办法在UI中正常显示，一般设50就可以了。", BackgroundWidget.gameObject.name);
                }

                customRenderQueue = BackgroundWidget.drawCall.renderQueue + +1;
                customRenderQueue += BackgroundWidget.SkippedRenderQueue / 2;
            }
        }
        else
        {
            customRenderQueue = CustomRenderQueue;
        }

        gameObject.SetRenderQueue(customRenderQueue);
    }

    public void RefleshRenderQueue()
    {
        StartCoroutine(UpdateRenderQueue());
    }
}
