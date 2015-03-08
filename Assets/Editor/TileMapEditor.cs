using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor {
	TileMap m_tileMap;
	int m_selectedTileIndex = 0;

	void OnEnable(){
		m_tileMap = (TileMap)target;
	}

	[MenuItem("Assets/Create/Tileset")]
	static void CreateTileset(){
		var asset = ScriptableObject.CreateInstance<TileSet>();
		var path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (string.IsNullOrEmpty (path)) {
			path = "Assets";
		} else if (Path.GetExtension (path) != "") {
			path = path.Replace(Path.GetFileName(path),"");
		} else {
			path += "/";
		}

		var assetPath = AssetDatabase.GenerateUniqueAssetPath (path + "TileSet.asset");
		AssetDatabase.CreateAsset (asset, assetPath);
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
		asset.hideFlags = HideFlags.DontSave;
	}

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();

		//Grid size manipulation
		int newValueWidth = m_tileMap.GridWidth;
		int newValueHeight = m_tileMap.GridHeight;

		//Check Height changes
		EditorGUI.BeginChangeCheck ();
		newValueHeight =  EditorGUILayout.IntField ("grid height", newValueHeight);
		if (EditorGUI.EndChangeCheck ()) {
			m_tileMap.GridHeight = newValueHeight;
		}
		//Check Width Changes
		EditorGUI.BeginChangeCheck ();
		newValueWidth =  EditorGUILayout.IntField ("grid width", newValueWidth);
		if (EditorGUI.EndChangeCheck ()) {
			m_tileMap.GridWidth = newValueWidth;
		}

		//Create arrays for selecting a tile prefab
		string[] names = new string[m_tileMap.TileSet.tilesPrefabs.Length];
		for (int i=0; i < names.Length; i ++) {
			names[i] = m_tileMap.TileSet.tilesPrefabs[i].gameObject.name;
		}
		//check when the value of the dropdown is changed
		EditorGUI.BeginChangeCheck ();
		m_selectedTileIndex = EditorGUILayout.Popup(m_selectedTileIndex, names);
		if (EditorGUI.EndChangeCheck ()) {
			m_tileMap.SetCurrentTileByName(names[m_selectedTileIndex]);
		}
	}

	void OnSceneGUI(){
		int controlID = GUIUtility.GetControlID (FocusType.Passive);
		Event e = Event.current;
		Ray ray = Camera.current.ScreenPointToRay ( 
		                         new Vector2(e.mousePosition.x, 
		            			 -e.mousePosition.y + Camera.current.pixelHeight) 
		                         );
		Vector3 mousePos = ray.origin;

		if (e.isMouse && e.type == EventType.MouseDown) {
			GUIUtility.hotControl = controlID;
			e.Use();

			m_tileMap.AddTile(mousePos.x,mousePos.y);
		}

		//Clean hotControl
		if (e.isMouse && e.type == EventType.MouseUp) {
			GUIUtility.hotControl = 0;
		}
	}
}
