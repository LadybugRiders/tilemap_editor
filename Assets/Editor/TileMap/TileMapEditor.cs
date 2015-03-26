using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor {
	TileMap m_tileMap;
	int m_selectedTileIndex = 0;

	int m_newGridHeight, m_newGridWidth;

	void OnEnable(){
		m_tileMap = (TileMap)target;
		m_newGridWidth = m_tileMap.GridWidth;
		m_newGridHeight = m_tileMap.GridHeight;
	}

	[MenuItem("Assets/Create/TileMap/Tileset")]
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

	[MenuItem("Assets/Create/TileMap/Tile")]
	static void CreateTilePrefab(){
		var path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (string.IsNullOrEmpty (path)) {
			path = "Assets";
		} else if (Path.GetExtension (path) != "") {
			path = path.Replace(Path.GetFileName(path),"");
		} else {
			path += "/";
		}
		
		var assetPath = AssetDatabase.GenerateUniqueAssetPath (path + "Tile.prefab");
		Object prefab = PrefabUtility.CreateEmptyPrefab (assetPath);
		GameObject go = new GameObject ("Tile");
		go.AddComponent<Tile> ();
		go.AddComponent<SpriteRenderer> ();
		PrefabUtility.ReplacePrefab (go, prefab);
		Object.DestroyImmediate (go);
		/*
		AssetDatabase.CreateAsset (asset, assetPath);
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
		asset.hideFlags = HideFlags.DontSave; */
	}

	public override void OnInspectorGUI(){
		
		base.OnInspectorGUI ();

		//Check cell size changes
		EditorGUI.BeginChangeCheck ();
		float newValueCellSize = m_tileMap.CellSize;
		newValueCellSize =  EditorGUILayout.FloatField ("Cell Size", newValueCellSize);
		if (EditorGUI.EndChangeCheck ()) {
			m_tileMap.CellSize = newValueCellSize;
		}

		//==============GRID SIZE=====================
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		EditorGUILayout.BeginHorizontal ();
		m_newGridWidth =  EditorGUILayout.IntField ("Grid Size", m_newGridWidth);
		m_newGridHeight =  EditorGUILayout.IntField (m_newGridHeight);
		EditorGUILayout.EndHorizontal ();
		if (GUILayout.Button ("Apply")) {
			m_tileMap.SetGridSize(m_newGridWidth,m_newGridHeight);
			m_newGridWidth = m_tileMap.GridWidth;
			m_newGridHeight = m_tileMap.GridHeight;
		}
		//=======================================================

		//Draw Bar
		EditorGUILayout.Space ();
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		EditorGUILayout.Space ();

		GUILayout.Label ("TileSet");
		//TileSet Selection
		TileSet ts = m_tileMap.TileSet;
		EditorGUI.BeginChangeCheck ();
		ts = (TileSet) EditorGUILayout.ObjectField (ts, typeof(TileSet), false);
		if (EditorGUI.EndChangeCheck ()) {
			m_tileMap.TileSet = ts;
		}

		EditorGUILayout.Space ();
		GUILayout.Label ("Select Tile");
		//Create arrays for selecting a tile prefab
		if (m_tileMap.TileSet != null) {
			string[] names = new string[m_tileMap.TileSet.tilesPrefabs.Length];
			for (int i=0; i < names.Length; i ++) {
				if(m_tileMap.TileSet.tilesPrefabs[i] == null)
					continue;
				names [i] = m_tileMap.TileSet.tilesPrefabs [i].gameObject.name;
				if( m_tileMap.GetCurrentTile() && m_tileMap.GetCurrentTile().gameObject.name == names[i] )
					m_selectedTileIndex = i;
			}
			//check when the value of the dropdown is changed
			EditorGUI.BeginChangeCheck ();
			m_selectedTileIndex = EditorGUILayout.Popup (m_selectedTileIndex, names);
			if (EditorGUI.EndChangeCheck ()) {
				m_tileMap.SetCurrentTileByName (names [m_selectedTileIndex]);
			}
		}

		EditorGUILayout.Space ();
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		EditorGUILayout.Space ();

		//Edition Lock
		m_tileMap.isLocked = EditorGUILayout.Toggle ("Lock", m_tileMap.isLocked);

		//clean button
		if (GUILayout.Button ("Clean")) {
			m_tileMap.CleanGrid();
		}
	}

	void OnSceneGUI(){

		if (m_tileMap && m_tileMap.isLocked)
			return;

		int controlID = GUIUtility.GetControlID (FocusType.Passive);
		Event e = Event.current;
		Ray ray = Camera.current.ScreenPointToRay ( 
		                         new Vector2(e.mousePosition.x, 
		            			 -e.mousePosition.y + Camera.current.pixelHeight) 
		                         );
		Vector3 mousePos = ray.origin;

		if (e.isMouse && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) ) {
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
