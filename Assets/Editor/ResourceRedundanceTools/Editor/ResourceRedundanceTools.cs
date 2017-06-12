using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ResourceRedundanceTools : EditorWindow
{
    RedundantResourceCheckOption mRedundantResourceCheckOption = new RedundantResourceCheckOption();
    MeshAnalyzerOption mMeshAnalyzerWindowOption = new MeshAnalyzerOption();
    EffectAnalyzeOption mEffectAnalyzeOption = new EffectAnalyzeOption();

    AnalyzeTexturesOption mAnalyzeTexturesOption = new AnalyzeTexturesOption( );

    private string[] mToolbarStrings = { "Redundant Resource Check", "Mesh Analyze", "Texture Analyze", "Effect Analyze" };

    [MenuItem( "BY Engine/ResourceManagementTools" )]

    static void Init()
    {
        ResourceRedundanceTools window = (ResourceRedundanceTools)EditorWindow.GetWindow( typeof( ResourceRedundanceTools ) );
        window.minSize = new Vector2(400, 300);
    }

    static int currentSelected = 0;
    void OnGUI()
    {
        currentSelected = GUILayout.Toolbar( currentSelected, mToolbarStrings );

        switch ( currentSelected )
        {
            case 0:
                mRedundantResourceCheckOption.OnGUI( );
                break;
            case 1:
                mMeshAnalyzerWindowOption.OnGUI();
                break;
            case 2:
                mAnalyzeTexturesOption.OnGUI( );
                break;
            case 3:
                mEffectAnalyzeOption.OnGUI();
                break;
        }
    }
}