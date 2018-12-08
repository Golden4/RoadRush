using UnityEngine;
using UnityEditor;

public class qb_Help : qb_Window
{
	[MenuItem ("Tools/QuickBrush/Помощь")]
	public static void ShowWindow()
	{
		window = GetWindow<qb_Help>(false,"QB Помощь");
		window.maxSize = new Vector2(400,500);
		window.minSize = new Vector2(400,300);
    }
	
	static Vector2 sliderVal;
	
	void OnGUI()
	{
		BuildStyles();
		
		EditorGUILayout.Space();
		
		sliderVal = EditorGUILayout.BeginScrollView(sliderVal,false,false);
			
			MenuListItem(false,true,"Полная документация и описание (EN) доступны на:");
            MenuListItem(false,true,"http://www.proCore3d.com/quickBrush/");

		EditorGUILayout.LabelField("Особенности:",labelStyle_bold);
		EditorGUILayout.BeginVertical(menuBlockStyle);
        	MenuListItem(true, "Вы можете размещать объекты на любой поверхности с тое-же легкостью как будто рисуете кистью");
            MenuListItem(true, "Используйте три снтрумента размещения с помощью простых сочетаний горячих клавиш");
			MenuListItem(true, "Просто перетащите на панель списка нужный префаб и можете наносить его на любую поверхность");
            MenuListItem(true, "Используйте слайдеры для изменения значений параметров");
            MenuListItem(true, "Самостоятельно назначайте ограничения диапазонов значений для слайдеров");
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Панель списка объектов:", labelStyle_bold);
        EditorGUILayout.BeginVertical(menuBlockStyle);
			MenuListItem(true, "Каждый элемент в панели списка имеет слайдер, две кнопки и картинку");
			MenuListItem(true, "Задавайте с помошью слайдера необходые параметры плотности размещения объектов");
			MenuListItem(true, "При нажатии на кнопку в виде красного X, объект удаляется из списка ");
			MenuListItem(true, "При нажатии на кнопку в виде зеленой галочки объект объект помечается как единственный для размещения.");
        EditorGUILayout.EndVertical();
	
		EditorGUILayout.EndScrollView();
	}


	
}