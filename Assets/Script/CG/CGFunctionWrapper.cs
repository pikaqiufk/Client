using System;
using UnityEngine;
using System.Collections;
using NCalc;


//Auto generate code.Don't edit manually.Use menu [Tools/Export CG Function To Wrapper]


public static class CGFunctionWrapper
{public static object cgAttach(Expression[] args)
{
if (args.Length < 4)
{
Logger.Error("cgAttach() args<[4]");
return 0;
}
PlayCG.Instance.cgAttach(
Convert.ToString(args[0].Evaluate()),
Convert.ToString(args[1].Evaluate()),
Convert.ToString(args[2].Evaluate()),
Convert.ToString(args[3].Evaluate())
);
return 1;
}public static object cgCameraRestore(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgCameraRestore() args<[1]");
return 0;
}
PlayCG.Instance.cgCameraRestore(
Convert.ToSingle(args[0].Evaluate())
);
return 1;
}public static object cgCameraTowardActor(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgCameraTowardActor() args<[3]");
return 0;
}
PlayCG.Instance.cgCameraTowardActor(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate())
);
return 1;
}public static object cgCameraTowardActorWithDistance(Expression[] args)
{
if (args.Length < 4)
{
Logger.Error("cgCameraTowardActorWithDistance() args<[4]");
return 0;
}
PlayCG.Instance.cgCameraTowardActorWithDistance(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate())
);
return 1;
}public static object cgCameraTowardActorWithOffset(Expression[] args)
{
if (args.Length < 6)
{
Logger.Error("cgCameraTowardActorWithOffset() args<[6]");
return 0;
}
PlayCG.Instance.cgCameraTowardActorWithOffset(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate()),
Convert.ToSingle(args[5].Evaluate())
);
return 1;
}public static object cgCloneMyPlayer(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgCloneMyPlayer() args<[1]");
return 0;
}
PlayCG.Instance.cgCloneMyPlayer(
Convert.ToInt32(args[0].Evaluate())
);
return 1;
}public static object cgCreateActor(Expression[] args)
{
if (args.Length < 5)
{
Logger.Error("cgCreateActor() args<[5]");
return 0;
}
PlayCG.Instance.cgCreateActor(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToInt32(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate())
);
return 1;
}public static object cgCreatePrefab(Expression[] args)
{
if (args.Length < 8)
{
Logger.Error("cgCreatePrefab() args<[8]");
return 0;
}
PlayCG.Instance.cgCreatePrefab(
Convert.ToString(args[0].Evaluate()),
Convert.ToString(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate()),
Convert.ToSingle(args[5].Evaluate()),
Convert.ToSingle(args[6].Evaluate()),
Convert.ToSingle(args[7].Evaluate())
);
return 1;
}public static object cgDeletePrefab(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgDeletePrefab() args<[1]");
return 0;
}
PlayCG.Instance.cgDeletePrefab(
Convert.ToString(args[0].Evaluate())
);
return 1;
}public static object cgDestroyActor(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgDestroyActor() args<[1]");
return 0;
}
PlayCG.Instance.cgDestroyActor(
Convert.ToInt32(args[0].Evaluate())
);
return 1;
}public static object cgDestroyEffect(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgDestroyEffect() args<[1]");
return 0;
}
PlayCG.Instance.cgDestroyEffect(
Convert.ToInt32(args[0].Evaluate())
);
return 1;
}public static object cgFaceTo(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgFaceTo() args<[3]");
return 0;
}
PlayCG.Instance.cgFaceTo(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate())
);
return 1;
}public static object cgFaceToActor(Expression[] args)
{
if (args.Length < 2)
{
Logger.Error("cgFaceToActor() args<[2]");
return 0;
}
PlayCG.Instance.cgFaceToActor(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToInt32(args[1].Evaluate())
);
return 1;
}public static object cgFadein(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgFadein() args<[1]");
return 0;
}
PlayCG.Instance.cgFadein(
Convert.ToSingle(args[0].Evaluate())
);
return 1;
}public static object cgFadeout(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgFadeout() args<[1]");
return 0;
}
PlayCG.Instance.cgFadeout(
Convert.ToSingle(args[0].Evaluate())
);
return 1;
}public static object cgFireEvent(Expression[] args)
{
if (args.Length < 2)
{
Logger.Error("cgFireEvent() args<[2]");
return 0;
}
PlayCG.Instance.cgFireEvent(
Convert.ToString(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate())
);
return 1;
}public static object cgITweenClearPos(Expression[] args)
{
if (args.Length < 0)
{
Logger.Error("cgITweenClearPos() args<[0]");
return 0;
}
PlayCG.Instance.cgITweenClearPos(
);
return 1;
}public static object cgITweenMoveTo(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgITweenMoveTo() args<[3]");
return 0;
}
PlayCG.Instance.cgITweenMoveTo(
Convert.ToString(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToBoolean(args[2].Evaluate())
);
return 1;
}public static object cgITweenPushPos(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgITweenPushPos() args<[3]");
return 0;
}
PlayCG.Instance.cgITweenPushPos(
Convert.ToSingle(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate())
);
return 1;
}public static object cgMove(Expression[] args)
{
if (args.Length < 5)
{
Logger.Error("cgMove() args<[5]");
return 0;
}
PlayCG.Instance.cgMove(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToBoolean(args[4].Evaluate())
);
return 1;
}public static object cgMoveTo(Expression[] args)
{
if (args.Length < 5)
{
Logger.Error("cgMoveTo() args<[5]");
return 0;
}
PlayCG.Instance.cgMoveTo(
Convert.ToString(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate())
);
return 1;
}public static object cgPlayAction(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgPlayAction() args<[3]");
return 0;
}
PlayCG.Instance.cgPlayAction(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToInt32(args[1].Evaluate()),
Convert.ToInt32(args[2].Evaluate())
);
return 1;
}public static object cgPlayAnimation(Expression[] args)
{
if (args.Length < 4)
{
Logger.Error("cgPlayAnimation() args<[4]");
return 0;
}
PlayCG.Instance.cgPlayAnimation(
Convert.ToString(args[0].Evaluate()),
Convert.ToString(args[1].Evaluate()),
Convert.ToString(args[2].Evaluate()),
Convert.ToString(args[3].Evaluate())
);
return 1;
}public static object cgPlayBGM(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgPlayBGM() args<[3]");
return 0;
}
PlayCG.Instance.cgPlayBGM(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate())
);
return 1;
}public static object cgPlayEffect(Expression[] args)
{
if (args.Length < 9)
{
Logger.Error("cgPlayEffect() args<[9]");
return 0;
}
PlayCG.Instance.cgPlayEffect(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToInt32(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate()),
Convert.ToSingle(args[5].Evaluate()),
Convert.ToSingle(args[6].Evaluate()),
Convert.ToSingle(args[7].Evaluate()),
Convert.ToSingle(args[8].Evaluate())
);
return 1;
}public static object cgPlaySound(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgPlaySound() args<[1]");
return 0;
}
PlayCG.Instance.cgPlaySound(
Convert.ToInt32(args[0].Evaluate())
);
return 1;
}public static object cgPlaySoundByRole(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgPlaySoundByRole() args<[3]");
return 0;
}
PlayCG.Instance.cgPlaySoundByRole(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToInt32(args[1].Evaluate()),
Convert.ToInt32(args[2].Evaluate())
);
return 1;
}public static object cgPopTalk(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgPopTalk() args<[3]");
return 0;
}
PlayCG.Instance.cgPopTalk(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToInt32(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate())
);
return 1;
}public static object cgRotate(Expression[] args)
{
if (args.Length < 5)
{
Logger.Error("cgRotate() args<[5]");
return 0;
}
PlayCG.Instance.cgRotate(
Convert.ToString(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate())
);
return 1;
}public static object cgScale(Expression[] args)
{
if (args.Length < 3)
{
Logger.Error("cgScale() args<[3]");
return 0;
}
PlayCG.Instance.cgScale(
Convert.ToString(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate())
);
return 1;
}public static object cgSetCamera(Expression[] args)
{
if (args.Length < 4)
{
Logger.Error("cgSetCamera() args<[4]");
return 0;
}
PlayCG.Instance.cgSetCamera(
Convert.ToSingle(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate())
);
return 1;
}public static object cgSetCameraEx(Expression[] args)
{
if (args.Length < 7)
{
Logger.Error("cgSetCameraEx() args<[7]");
return 0;
}
PlayCG.Instance.cgSetCameraEx(
Convert.ToSingle(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate()),
Convert.ToSingle(args[5].Evaluate()),
Convert.ToSingle(args[6].Evaluate())
);
return 1;
}public static object cgSetPosDir(Expression[] args)
{
if (args.Length < 4)
{
Logger.Error("cgSetPosDir() args<[4]");
return 0;
}
PlayCG.Instance.cgSetPosDir(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate())
);
return 1;
}public static object cgSetSyncActorDirection(Expression[] args)
{
if (args.Length < 2)
{
Logger.Error("cgSetSyncActorDirection() args<[2]");
return 0;
}
PlayCG.Instance.cgSetSyncActorDirection(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToBoolean(args[1].Evaluate())
);
return 1;
}public static object cgShowDialog(Expression[] args)
{
if (args.Length < 2)
{
Logger.Error("cgShowDialog() args<[2]");
return 0;
}
PlayCG.Instance.cgShowDialog(
Convert.ToInt32(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate())
);
return 1;
}public static object cgStopAnimation(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgStopAnimation() args<[1]");
return 0;
}
PlayCG.Instance.cgStopAnimation(
Convert.ToString(args[0].Evaluate())
);
return 1;
}public static object cgWait(Expression[] args)
{
if (args.Length < 1)
{
Logger.Error("cgWait() args<[1]");
return 0;
}
PlayCG.Instance.cgWait(
Convert.ToSingle(args[0].Evaluate())
);
return 1;
}public static object cgCameraBezeir(Expression[] args)
{
if (args.Length < 8)
{
Logger.Error("cgCameraBezeir() args<[8]");
return 0;
}
PlayCG.Instance.cgCameraBezeir(
Convert.ToSingle(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate()),
Convert.ToSingle(args[5].Evaluate()),
Convert.ToSingle(args[6].Evaluate()),
Convert.ToBoolean(args[7].Evaluate())
);
return 1;
}public static object cgCameraBezeirLookAt(Expression[] args)
{
if (args.Length < 10)
{
Logger.Error("cgCameraBezeirLookAt() args<[10]");
return 0;
}
PlayCG.Instance.cgCameraBezeirLookAt(
Convert.ToSingle(args[0].Evaluate()),
Convert.ToSingle(args[1].Evaluate()),
Convert.ToSingle(args[2].Evaluate()),
Convert.ToSingle(args[3].Evaluate()),
Convert.ToSingle(args[4].Evaluate()),
Convert.ToSingle(args[5].Evaluate()),
Convert.ToSingle(args[6].Evaluate()),
Convert.ToSingle(args[7].Evaluate()),
Convert.ToSingle(args[8].Evaluate()),
Convert.ToSingle(args[9].Evaluate())
);
return 1;
}
}
