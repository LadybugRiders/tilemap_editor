using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour {

	[SerializeField] private float m_cellSize = 32.0f;
	[SerializeField] bool m_showGrid = true;

	[SerializeField] TileSet m_tileSet;

	int m_gridWidth = 40;
	int m_gridHeight = 20;

	private List< List<Tile> > m_grid;

	private float m_xRange = 800.0f;
	private float m_yRange = 800.0f;

	private float m_baseUnit = 100.0f;
	
	Transform m_currentTile;

	// Use this for initialization
	void Start () {
		//compute range of the grid ( in world units )
		m_xRange = ( m_gridWidth * m_cellSize ) / m_baseUnit;
		m_yRange = ( m_gridHeight * m_cellSize ) / m_baseUnit;

		//create the grid data structure
		m_grid = new List<List<Tile>> (m_gridHeight);
		for (int i=0; i < m_gridHeight; i++) {
			m_grid.Add( new List<Tile>(m_gridWidth) );
		}

		Debug.Log (m_xRange);
		if (m_tileSet && m_tileSet.prefabs.Length > 0) {
			m_currentTile = m_tileSet.prefabs [0];
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject AddTile (float x, float y){
		GameObject go = null;
		if( m_currentTile != null ){

			//Position in the grid
			float i = Mathf.Floor(x * m_baseUnit / m_cellSize) + Mathf.Floor(m_gridWidth *0.5f);
			float j = Mathf.Floor(y * m_baseUnit / m_cellSize) + Mathf.Floor(m_gridHeight *0.5f);

			if( i < 0 || j < 0 || i >= m_gridWidth || j >= m_gridHeight)
				return null;
			
			Undo.IncrementCurrentGroup();
			//Instantiate tile prefab
			go = (GameObject) PrefabUtility.InstantiatePrefab(m_currentTile.gameObject);

			float cellSize = m_cellSize / m_baseUnit;
			//position in the world
			float newX = - m_xRange * 0.5f + i * cellSize + cellSize * 0.5f ;
			float newY = - m_yRange * 0.5f + j * cellSize + cellSize * 0.5f ;

			Vector3 aligned = new Vector3(newX  ,newY  ,0.0f);
			go.transform.position = aligned;
			go.transform.parent = this.transform;
			
			Undo.RegisterCreatedObjectUndo(go,"Create"+go.name);
		}
		return go;
	}

	void OnDrawGizmos(){
		if (m_showGrid) {
			Vector3 pos = Camera.current.transform.position;
			Gizmos.color = new Color (1.0f, 0.5f, 1.0f,0.5f);
			for (float y = pos.y -m_yRange *0.5f; y < pos.y + m_yRange * 0.5f; y+= this.m_cellSize / m_baseUnit) {
				Gizmos.DrawLine (new Vector3 (-m_xRange*0.5f, y, 0),
				                 new Vector3 (m_xRange*0.5f, y, 0));
			}
			for (float x = pos.x -m_xRange*0.5f; x < pos.x + m_xRange * 0.5f; x+= this.m_cellSize / m_baseUnit) {
				Gizmos.DrawLine (new Vector3 (x,-m_yRange*0.5f, 0),
				                 new Vector3 (x, m_yRange*0.5f, 0));
			}
		}
	}

	#region GETTERS
	/**
	 * GETTERS
	 * */

	public void SetCurrentTile(Transform _tile){
		m_currentTile = _tile;
	}

	public void SetCurrentTileByName(string _tileName){
		if (m_tileSet == null)
			return;
		Transform t = m_tileSet.GetTileByName (_tileName);
		if( t != null )
			m_currentTile = t;
	}

	public Transform GetCurrentTile(){
		return m_currentTile;
	}


	public float CellSize {
		get {
			return m_cellSize;
		}
	}

	public TileSet TileSet {
		get {
			return m_tileSet;
		}
		set{
			m_tileSet = value;
		}
	}

	public int GridWidth {
		get {
			return m_gridWidth;
		}
		set {
			if(value % 2 != 0){
				value += 1;
			}
			m_gridWidth = value;
			//compute range of the grid ( in world units )
			m_xRange = ( m_gridWidth * m_cellSize ) / m_baseUnit;
		}
	}

	public int GridHeight {
		get {
			return m_gridHeight;
		}
		set {
			if(value % 2 != 0){
				value += 1;
			}
			m_gridHeight = value;
			//compute range of the grid ( in world units )
			m_yRange = ( m_gridHeight * m_cellSize ) / m_baseUnit;
		}
	}
	#endregion
}
