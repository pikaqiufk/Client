#region using

using System.Collections;

#endregion

public interface IManager
{
    void Destroy();
    IEnumerator Init();
    void Reset();
    void Tick(float delta);
}