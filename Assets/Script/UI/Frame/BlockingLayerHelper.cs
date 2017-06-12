#region using

using System;

#endregion

public struct BlockingLayerHelper : IDisposable
{
    public BlockingLayerHelper(int i)
    {
        Logger.Info("Add blocking layer.");
        UIManager.Instance.ShowBlockLayer(i);
    }

    public void Dispose()
    {
        Logger.Info("Remove blocking layer.");
        UIManager.Instance.RemoveBlockLayer();
    }
}