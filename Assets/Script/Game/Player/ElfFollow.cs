#region using

using System;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class ElfFollow : MonoBehaviour
{
    private float mActionTime;
    public float MaxDistance = 0.8f;
    public float MinDistance = 0.3f;
    private ObjElf mObjElf;
    private float mStopTime;
    private DateTime NextMoveTime = DateTime.Now;
    public ObjCharacter Owner;
    public float Radius = 0.5f;
    public float RadomMoveDuration = 3.0f; // second.
    public float ResetDistance = 10.0f;

    public void CreateObj(int dataId)
    {
        if (mObjElf != null)
        {
            mObjElf.Destroy();
            mObjElf = null;
        }
        if (dataId == -1)
        {
            return;
        }

        var angle = gameObject.transform.rotation.eulerAngles.y;
        angle += 90.0f;
        var temp = Owner.Position;
        temp.x += (float) Math.Cos(angle)*Radius;
        temp.z += (float) Math.Sin(angle)*Radius;
        NavMeshHit hit;
        if (NavMesh.Raycast(Owner.Position, temp, out hit, -1))
        {
            temp = hit.position;
        }

        InitBaseData initData = new InitOtherPlayerData
        {
            ObjId = 0,
            DataId = dataId,
            X = temp.x,
            Y = temp.y,
            Z = temp.z,
            DirX = 1,
            DirZ = 0,
            Name = "",
            RobotId = 0ul
        };

        var go = ComplexObjectPool.NewObjectSync(Resource.PrefabPath.Elf);
        var elf = go.GetComponent<ObjElf>();
        elf.Init(initData, () =>
        {
            mObjElf = elf;
            mObjElf.OnElfMoveOver = OnElfMoveOver;
            mObjElf.DoIdle();
            var character = gameObject.GetComponent<ObjCharacter>();
            if (character)
            {
                mObjElf.SetMoveSpeed(character.GetMoveSpeed() + 0.08f);
            }
        });
        go.SetActive(true);
    }

    public void OnElfMoveOver()
    {
        if (Owner.IsMoving())
        {
            mObjElf.MoveTo(PickStandPosition());
        }
        else
        {
            var ownerPos = Owner.Position.xz();
            var elfPos = mObjElf.Position.xz();
            var dis = (ownerPos - elfPos).magnitude;
            if (dis < MinDistance)
            {
                mObjElf.MoveTo(PickPositionAround());
            }
            else if (dis > MaxDistance)
            {
                mObjElf.MoveTo(PickStandPosition());
            }
            else
            {
                NextMoveTime = DateTime.Now +
                               TimeSpan.FromSeconds(RadomMoveDuration + Random.Range(0, RadomMoveDuration));
                mObjElf.DoIdle();
            }
        }
    }

    private Vector3 PickPositionAround()
    {
        var angle = Random.Range(0, Mathf.PI*2);
        var temp = Owner.Position;

        temp.x += (float) Math.Cos(angle)*Radius;
        temp.z += (float) Math.Sin(angle)*Radius;

        NavMeshHit hit;
        if (NavMesh.Raycast(Owner.Position, temp, out hit, -1))
        {
            temp = hit.position;
        }

        return temp;
    }

    private Vector3 PickStandPosition()
    {
        var temp = Owner.Position - ((Quaternion.Euler(0, 60, 0)*Owner.Direction)*Radius);

        NavMeshHit hit;
        if (NavMesh.Raycast(Owner.Position, temp, out hit, -1))
        {
            temp = hit.position;
        }

        return temp;
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif
        if (null == mObjElf || null == Owner)
        {
            return;
        }

        if (0 != Time.frameCount%5)
        {
            return;
        }

        var ownerPos = Owner.Position.xz();
        var elfPos = mObjElf.Position.xz();
        var dis = (ownerPos - elfPos).magnitude;

        if (Owner.IsMoving())
        {
            mObjElf.TargetPos.Add(PickStandPosition());
            mObjElf.SetMoveSpeed(Owner.GetMoveSpeed()*(1 + (dis/(Radius + 0.5f) - 1)*0.2f));
        }

        if (dis > ResetDistance)
        {
            mObjElf.Position = Owner.Position;
            mObjElf.DoIdle();
            return;
        }

        if (dis > MaxDistance)
        {
            mObjElf.MoveTo(PickStandPosition());
            return;
        }

        if (!mObjElf.IsMoving())
        {
            // Start move
            if (Owner.IsMoving())
            {
                mObjElf.MoveTo(PickPositionAround());
            }
            else
            {
                // so boring, just move around.
                if (DateTime.Now > NextMoveTime)
                {
                    mObjElf.MoveTo(PickPositionAround());
                }
            }
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