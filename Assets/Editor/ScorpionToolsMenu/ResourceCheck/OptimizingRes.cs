using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ScorpionLib.Tools.Editor
{
    internal class OptimizingRes
    {
		[MenuItem("Tools/Optimize Resource/Optimizing Scene Res")]
		//优化资源
		public static void OptimizingScenePrefab()
		{
            const string ASSET_PATH = "Assets/Res/Scene";


            RemoveSameMeshColliderComponent.RemoveSameMeshColliderInEnumAssetAtPath(EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH));
            ShadowsCloseTools.CloseShadows(EnumAssets.EnumAllComponentDependenciesRecursive<Renderer>(EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH)));
            //AnimationClipTool.ChangeAnimatorToAnimationInPath(ASSET_PATH);
            T4MComponentDisable.T4MComponentDisablemethod(EnumAssets.EnumAllComponentDependenciesRecursive<T4MObjSC>(EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH)));
            DrawcallOptimize.DrawcallOptimizemethod(ASSET_PATH);
		}


		[MenuItem("Tools/Optimize Resource/Optimizing Res")]
		//优化资源
		public static void OptimizingResDirectory()
		{
			const string ASSET_PATH = "Assets/Res";

			ShadowsCloseTools.CloseShadows(EnumAssets.EnumComponentRecursiveAtPath<Renderer>(ASSET_PATH+"/Model"));
			OptimizingPropertiesOfModelResources.ResetObjectModelProperties(EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH + "/Model"));
			OptimizingPropertiesOfModelResources.ResetObjectModelProperties(EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH + "/Scene"));
			OptimizeTextureProperty.ResetTextureProperty(EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH + "/Model"));
			OptimizeTextureProperty.ResetTextureProperty(EnumAssets.EnumAssetAtPath<UnityEngine.Object>(ASSET_PATH + "/Effect"));

			//AnimationClipTool.ChangeAnimatorToAnimation();
			//T4MComponentDisable.T4MComponentDisablemethod();
		}
    }
}
