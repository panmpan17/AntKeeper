using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager : MonoBehaviour
{
    public static GridManager ins;

    [Header("Reference")]
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Tilemap routeColliderMap;
    [SerializeField]
    private TilemapReference routeColliderMapReference;


    [SerializeField]
    private GridLayer[] layers;
    private int _currentLayerIndex;
    public int CurrentLayer => _currentLayerIndex;

    [Header("Spawn Ants")]
    [SerializeField]
    private int antNestCountCap = 20;
    [SerializeField]
    private SpawnAnimalNest spawnAnimalNest;

    private List<AbstractGroundInteractive> _groundInteractives = new List<AbstractGroundInteractive>();
    private List<AntNestHub> _antNestHubs = new List<AntNestHub>();
    private List<VirtualAnimalSpot> _animals = new List<VirtualAnimalSpot>();

    public List<AntNestHub> AntNestHubs => _antNestHubs;
    public List<VirtualAnimalSpot> Animals => _animals;


    private int _originAnimalCount;
    public int OriginAnimalCount => _originAnimalCount;

    void Awake()
    {
        ins = this;

        routeColliderMapReference.Tilemap = routeColliderMap;
    }

    void Start()
    {
        if (GameManager.ins != null)
            GameManager.ins.OnGameStart += OnGameStart;
    }

    void OnGameStart()
    {
        _originAnimalCount = _animals.Count;

        for (int i = 0; i < _antNestHubs.Count; i++)
        {
            _antNestHubs[i].Unfreeze();
        }
    }


    #region Register and unregister
    public void RegisterGroundInteractive(AbstractGroundInteractive groundInteractive, out Vector3Int gridPosition)
    {
        _groundInteractives.Add(groundInteractive);
        gridPosition = grid.WorldToCell(groundInteractive.transform.position);
    }


    public void RegisterAntNest(AntNestHub antNestHub) => _antNestHubs.Add(antNestHub);
    public void UnregisterAntNest(AntNestHub antNestHub) => _antNestHubs.Remove(antNestHub);

    public void ReigsterAnimal(VirtualAnimalSpot animalSpot, out Vector3Int gridPosition)
    {
        _animals.Add(animalSpot);
        gridPosition = grid.WorldToCell(animalSpot.transform.position);
    }
    #endregion


    #region Find element on ground
    public bool TryFindGroundInteractive(Vector3Int gridPosition)
    {
        for (int i = 0; i < _groundInteractives.Count; i++)
        {
            if (gridPosition == _groundInteractives[i].GridPosition)
                return true;
        }
        return false;
    }
    public bool TryFindGroundInteractive(Vector3Int gridPosition, out AbstractGroundInteractive groundInteractve)
    {
        for (int i = 0; i < _groundInteractives.Count; i++)
        {
            if (gridPosition == _groundInteractives[i].GridPosition)
            {
                groundInteractve = _groundInteractives[i];
                return true;
            }
        }

        groundInteractve = null;
        return false;
    }

    public bool TryFindAntNest(Vector3Int gridPosition, out AntNestHub antNesthub)
    {
        for (int i = 0; i < _antNestHubs.Count; i++)
        {
            if (_antNestHubs[i].RootGridPosition == gridPosition)
            {
                antNesthub = _antNestHubs[i];
                return true;
            }
        }
        antNesthub = null;
        return false;
    }

    public bool TryFindAntNestBranch(Vector3Int gridPosition)
    {
        for (int i = 0; i < _antNestHubs.Count; i++)
        {
            if (_antNestHubs[i].IsGridPositionOverlapBranch(gridPosition))
            {
                return true;
            }
        }
        return false;
    }
    public bool TryFindAntNestBranch(Vector3Int gridPosition, out AntNestHub antNest, out AntRouteBranch branch)
    {
        for (int i = 0; i < _antNestHubs.Count; i++)
        {
            if (_antNestHubs[i].IsGridPositionOverlapBranch(gridPosition, out branch))
            {
                antNest = _antNestHubs[i];
                return true;
            }
        }

        antNest = null;
        branch = null;
        return false;
    }

    public bool TryFindAnimal(Vector3Int gridPosition, out VirtualAnimalSpot animalSpot)
    {
        for (int i = 0; i < _animals.Count; i++)
        {
            if (_animals[i].GridPosition == gridPosition)
            {
                animalSpot = _animals[i];
                return true;
            }
        }

        animalSpot = null;
        return false;
    }
    #endregion


    #region Count map elements
    public int CountAliveAnimal()
    {
        int count = 0;
        for (int i = 0; i < _animals.Count; i++)
        {
            if (_animals[i].IsAlive)
                count++;
        }
        return count;
    }
    #endregion


    #region Layer
    public int GetCurrentLayer(Vector3 worldPosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPosition);
        for (int i = layers.Length - 1; i >= 0; i--)
        {
            if (layers[i].BaseMap.HasTile(gridPosition))
                return i;
        }
        return -1;
    }
    public int GetCurrentLayer(Vector3Int gridPosition)
    {
        for (int i = layers.Length - 1; i >= 0; i--)
        {
            if (layers[i].BaseMap.HasTile(gridPosition))
                return i;
        }
        return -1;
    }

    public void SwitchLayer(int activeLayer)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].WallMap.gameObject.SetActive(i == activeLayer);
        }
        _currentLayerIndex = activeLayer;
    }
    #endregion


    #region Utilities
    public bool RaycastCell(Vector3 position, out Vector3Int cellPosition, out Vector3 centerPosition)
    {
        cellPosition = grid.WorldToCell(position);
        centerPosition = grid.GetCellCenterWorld(cellPosition);

        if (layers[_currentLayerIndex].WallMap.HasTile(cellPosition))
            return false;
        // for (int i = layers.Length - 1; i > _currentLayerIndex; i--)
        // {
        //     if (layers[i].BaseMap.HasTile(cellPosition))
        //         return false;
        // }
        return layers[_currentLayerIndex].BaseMap.HasTile(cellPosition);
    }

    public bool CheckGroundAvalibleForAnt(Vector3Int fromPosition, Vector3Int toPosition)
    {
        if (TryFindGroundInteractive(toPosition))
            return false;

        int fromPositionLayer = GetCurrentLayer(fromPosition);

        if (fromPositionLayer == -1)
            return false;
        if (layers[fromPositionLayer].WallMap.HasTile(toPosition))
            return false;

        return layers[fromPositionLayer].BaseMap.HasTile(toPosition);
    }
    public bool CheckGroundAvalibleForNewAnt(Vector3Int gridPosition)
    {
        if (TryFindGroundInteractive(gridPosition))
            return false;
        if (TryFindAntNestBranch(gridPosition))
            return false;

        int newNestLayer = GetCurrentLayer(gridPosition);

        if (newNestLayer == -1)
            return false;
        if (layers[newNestLayer].WallMap.HasTile(gridPosition))
            return false;
        // for (int i = layers.Length - 1; i > _currentLayerIndex; i--)
        // {
        //     if (layers[i].BaseMap.HasTile(gridPosition))
        //         return false;
        // }
        return layers[newNestLayer].BaseMap.HasTile(gridPosition);
    }
    public bool CheckGroundAvalibleForNewAntAndCap(Vector3Int gridPosition)
    {
        if (_antNestHubs.Count >= antNestCountCap)
            return false;
        return CheckGroundAvalibleForNewAnt(gridPosition);
    }


    public void InstantiateAntNest(Vector3Int gridPosition, bool useFireAnt)
    {
        spawnAnimalNest?.Spawn(useFireAnt, grid.GetCellCenterWorld(gridPosition));
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }
    #endregion

    public void ShowAllColor()
    {
        for (int i = 0; i < _antNestHubs.Count; i++)
        {
            if (!_antNestHubs[i].IsShowTrueColor)
            {
                _antNestHubs[i].StartRevealColor(0.2f);
            }
        }
    }



    [System.Serializable]
    public struct GridLayer
    {
        public Tilemap BaseMap;
        public Tilemap WallMap;
        public Tilemap EdgeMap;
    }
}
