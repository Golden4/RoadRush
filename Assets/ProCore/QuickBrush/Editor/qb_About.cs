using UnityEngine;
using UnityEditor;
using System.Collections;

//	QuickBrush: Prefab Placement Tool
//	by PlayTangent
//	all rights reserved
//	www.ProCore3d.com

public class qb_About : qb_Window
{
	[MenuItem ("Tools/QuickBrush/О программе")]
	public static void ShowWindow()
	{
		window = EditorWindow.GetWindow<qb_About>(false,"QB О программе");
		
		//window.position = new Rect(100,100,290,600);
		window.maxSize = new Vector2(400,180);
		window.minSize = new Vector2(400,180);
    }
	
	public const string RELEASE_VERSION = "1.0.3";
	const string BUILD_DATE = "02-24-2014";
	
	void OnGUI()
	{
		BuildStyles();
		
		EditorGUILayout.Space();

		EditorGUILayout.BeginVertical();
			
			MenuListItem(false,true,"QuickBrush" + RELEASE_VERSION);
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Номер версии:");
			EditorGUILayout.LabelField(RELEASE_VERSION);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Дата сборки:");
			EditorGUILayout.LabelField(BUILD_DATE);
		EditorGUILayout.EndHorizontal();
			
		EditorGUILayout.EndVertical();
	}
}