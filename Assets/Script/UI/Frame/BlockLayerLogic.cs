using System;
#region using

using UnityEngine;

#endregion

public class BlockLayerLogic : MonoBehaviour
{
    public BindDataRoot BindRoot;
    //public UISprite BlockSprite;
    public BoxCollider BlockCollider;
    public static BlockLayerLogic Instance { get; private set; }

    public void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        BindRoot.RemoveBinding();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        var data = UIManager.Instance.GetController(UIConfig.BlockLayer).GetDataModel("");
        BindRoot.SetBindDataSource(data);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif
        if (BlockCollider == null)
        {
            return;
        }
        //var collider = gameObject.AddComponent<BoxCollider>();
        BlockCollider.center = Vector3.zero;
        BlockCollider.isTrigger = true;
        var root = gameObject.transform.root.GetComponent<UIRoot>();
        if (root != null)
        {
            var s = (float) root.activeHeight/Screen.height;
            BlockCollider.size = new Vector3(Screen.width*s, Screen.height*s, 0f);
        }
        else
        {
            Logger.Error("root not find");
        }
        Instance = this;
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}