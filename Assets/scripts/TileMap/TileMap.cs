using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour {
	
	[HideInInspector] [SerializeField] private float m_cellSize = 32.0f;
	[SerializeField] bool m_showGrid = true;
	
	[HideInInspector][SerializeField] TileSet m_tileSet;
	
	bool m_lockEditing = false;

	int m_gridWidth = 40;
	int m_gridHeight = 20;
	
	[HideInInspector][SerializeField]
	private List< RowWrapper > m_grid;
	
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
		InitGrid ();
		SearchTilesInChildren ();

		//Select first tile
		if (m_tileSet && m_tileSet.tilesPrefabs.Length > 0 && m_currentTile == null) {
			m_currentTile = m_tileSet.tilesPrefabs [0];
		}
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
	
	public GameObject AddTile (float x, float y){
		Debug.Log (m_currentTile);
		GameObject go = null;
		if( m_currentTile != null ){			
			//Position in the grid
			float i = Mathf.Floor(x * m_baseUnit / m_cellSize) + Mathf.Floor(m_gridWidth *0.5f);
			float j = Mathf.Floor(y * m_baseUnit / m_cellSize) + Mathf.Floor(m_gridHeight *0.5f);
			
			//outside of grid bounds
			if( i < 0 || j < 0 || i >= m_gridWidth || j >= m_gridHeight)
				return null;
			
			Undo.IncrementCurrentGroup();

			Tile formerTile = GetTile((int)i,(int)j);
			if( formerTile ){
				//if the tile we want to add is already in place
				bool sameTile = formerTile.Compare(m_currentTile.GetComponent<Tile>()) ;
				if(sameTile){
					return null;
				}
				Object.DestroyImmediate(formerTile.gameObject);
			}
						
			//Instantiate tile prefab
			go = (GameObject) PrefabUtility.InstantiatePrefab(m_currentTile.gameObject);

			
			//store the script in the grid
			Tile tileScript = go.GetComponent<Tile>();
			if(tileScript){
				if( m_grid == null ){
					InitGrid();
				}
				RowWrapper row = m_grid[(int)j];
				row[(int)i] = tileScript;
				//set variables
				tileScript.Row = (int) j;
				tileScript.Column =(int) i;
				//Resize
				tileScript.Resize(m_cellSize);

				StoreTileObject(tileScript.gameObject, (int) i, (int) j);
			}

			
			float cellSize = m_cellSize / m_baseUnit;
			//position in the world
			float newX = - m_xRange * 0.5f + i * cellSize + cellSize * 0.5f ;
			float newY = - m_yRange * 0.5f + j * cellSize + cellSize * 0.5f ;
			
			Vector3 newPos = new Vector3(newX  ,newY  ,0.0f);
			go.transform.position = newPos;
			
			Undo.RegisterCreatedObjectUndo(go,"Create"+go.name);
		}
		return go;
	}
	
	void StoreTileObject(GameObject _tile, int _i, int _j){
		GameObject rowObject = null;
		for (int i=0; i < transform.childCount; i++) {
			if( transform.GetChild(i).gameObject.name == "Row"+_j){
				rowObject = transform.GetChild(i).gameObject;
				break;
			}
		}
		if (rowObject == null) {
			rowObject = new GameObject("Row"+_j);
			rowObject.transform.parent = transform;
		}
		_tile.gameObject.name = "Tile" + _i + "_" + _j;
		_tile.transform.parent = rowObject.transform;
	}
	
	void SearchTilesInChildren(){
		for (int c=0; c < transform.childCount; c ++) {
			Transform child = transform.GetChild(c);
			for(int t = 0; t < child.childCount; t++){
				Transform tileT = child.GetChild(t);
				//Store into grid
				Tile tileScript = tileT.gameObject.GetComponent<Tile>();
				m_grid[tileScript.Row][tileScript.Column] = tileScript;
			}
		}
	}

	void InitGrid(){
		m_grid = new List<RowWrapper> ();
		for (int j=0; j < m_gridHeight; j++) {
			RowWrapper tmpList = new RowWrapper();
			for(int i=0; i < m_gridWidth; i ++){
				tmpList.Add(null);
			}
			m_grid.Add( tmpList );
		}
	}

	/// <summary>
	/// Gets the tile.
	/// </summary>
	/// <returns>The tile.</returns>
	/// <param name="i">The index.</param>
	/// <param name="j">J.</param>
	public Tile GetTile(int i, int j){
		RowWrapper row = null;
		try{
			row = m_grid [j];
		}catch(System.Exception e){
			//Debug.LogError("No row "+j);
			return null;
		}

		Tile tileScript;
		try{
			tileScript = row[i];
		}catch(System.Exception e){
			//Debug.LogError("No Column "+i);
			return null;
		}

		return tileScript;
	}

	public void ResizeGridTiles(){
		Tile tile;
		for (int j=0; j < m_gridHeight; j ++) {
			for(int i=0 ; i < m_gridWidth; i++){
				try{
					tile = m_grid [j][i];
				}catch(System.Exception e){
					continue;
				}
				
				if( tile ){
					tile.Resize(m_cellSize);
					float cellSize = m_cellSize / m_baseUnit;
					//position in the world
					float newX = - m_xRange * 0.5f + i * cellSize + cellSize * 0.5f ;
					float newY = - m_yRange * 0.5f + j * cellSize + cellSize * 0.5f ;
					
					Vector3 newPos = new Vector3(newX  ,newY  ,0.0f);
					tile.transform.position = newPos;
				}
			}
		}
	}

	//compare Tiles with new tileset Tiles and remove the wrong ones
	void CleanGrid(){

		Tile tile;
		for (int j=0; j < m_gridHeight; j ++) {
			for(int i=0 ; i < m_gridWidth; i++){
				try{
					tile = m_grid [j][i];
				}catch(System.Exception e){
					continue;
				}

				if( tile != null && (  m_tileSet == null || ! m_tileSet.TileExists(tile) ) ){
					m_grid[j][i] = null;
					Object.DestroyImmediate(tile.gameObject);
				}
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
		set{
			m_cellSize = value;
			m_xRange = ( m_gridWidth * m_cellSize ) / m_baseUnit;
			m_yRange = ( m_gridHeight * m_cellSize ) / m_baseUnit;
			ResizeGridTiles();
		}
	}
	
	public TileSet TileSet {
		get {
			return m_tileSet;
		}
		set{
			m_tileSet = value;
			//Select first tile
			if (m_tileSet && m_tileSet.tilesPrefabs.Length > 0 ) {
				m_currentTile = m_tileSet.tilesPrefabs [0];
			}
			CleanGrid();
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

	public void PrintGrid(){
		Tile tile;
		for (int j=0; j < m_gridHeight; j ++) {
			for(int i=0 ; i < m_gridWidth; i++){
				try{
					tile = m_grid [j][i];
				}catch(System.Exception e){
					continue;
				}

				if( tile ){
					Debug.Log (tile.gameObject.name + " at ["+i+","+j+"]");
				}
			}
		}
	}

	public bool isLocked {
		get {
			return m_lockEditing;
		}
		set {
			m_lockEditing = value;
		}
	}
	#endregion
}
