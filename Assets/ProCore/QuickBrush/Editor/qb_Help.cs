using UnityEngine;
using UnityEditor;

public class qb_Help : qb_Window
{
	[MenuItem ("Tools/QuickBrush/������")]
	public static void ShowWindow()
	{
		window = GetWindow<qb_Help>(false,"QB ������");
		window.maxSize = new Vector2(400,500);
		window.minSize = new Vector2(400,300);
    }
	
	static Vector2 sliderVal;
	
	void OnGUI()
	{
		BuildStyles();
		
		EditorGUILayout.Space();
		
		sliderVal = EditorGUILayout.BeginScrollView(sliderVal,false,false);
			
			MenuListItem(false,true,"������ ������������ � �������� (EN) �������� ��:");
            MenuListItem(false,true,"http://www.proCore3d.com/quickBrush/");

		EditorGUILayout.LabelField("�����������:",labelStyle_bold);
		EditorGUILayout.BeginVertical(menuBlockStyle);
        	MenuListItem(true, "�� ������ ��������� ������� �� ����� ����������� � ���-�� ��������� ��� ����� ������� ������");
            MenuListItem(true, "����������� ��� ���������� ���������� � ������� ������� ��������� ������� ������");
			MenuListItem(true, "������ ���������� �� ������ ������ ������ ������ � ������ �������� ��� �� ����� �����������");
            MenuListItem(true, "����������� �������� ��� ��������� �������� ����������");
            MenuListItem(true, "�������������� ���������� ����������� ���������� �������� ��� ���������");
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("������ ������ ��������:", labelStyle_bold);
        EditorGUILayout.BeginVertical(menuBlockStyle);
			MenuListItem(true, "������ ������� � ������ ������ ����� �������, ��� ������ � ��������");
			MenuListItem(true, "��������� � ������� �������� ��������� ��������� ��������� ���������� ��������");
			MenuListItem(true, "��� ������� �� ������ � ���� �������� X, ������ ��������� �� ������ ");
			MenuListItem(true, "��� ������� �� ������ � ���� ������� ������� ������ ������ ���������� ��� ������������ ��� ����������.");
        EditorGUILayout.EndVertical();
	
		EditorGUILayout.EndScrollView();
	}


	
}