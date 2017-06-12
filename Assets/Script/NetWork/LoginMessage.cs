#region using

using System.Collections;
using ClientService;
using ScorpionNetLib;

#endregion

public partial class NetManager : ClientAgentBase, ILogin9xServiceInterface, ILogic9xServiceInterface,
                                  IScene9xServiceInterface, IRank9xServiceInterface, IActivity9xServiceInterface,
                                  IChat9xServiceInterface, ITeam9xServiceInterface
{
    public IEnumerator PrepareEnterScene(ulong clientId)
    {
        Logger.Debug("---------------PrepareEnterScene.");
        return null;
        //throw new NotImplementedException();
    }
}