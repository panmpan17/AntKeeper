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
    public Grid Grid => grid;
    [SerializeField]
    private Tilemap baseMap;
    public Tilemap BaseMap => baseMap;
    [SerializeField]
    private Tilemap routeColliderMap;
    [SerializeField]
    private TilemapReference routeColliderMapReference;

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


    #region Utilities
    public bool RaycastCell(Vector3 position, out Vector3Int cellPosition, out Vector3 centerPosition)
    {
        cellPosition = grid.WorldToCell(position);
        centerPosition = grid.GetCellCenterWorld(cellPosition);

        return baseMap.HasTile(cellPosition);
    }

    public bool CheckGroundAvalibleForAnt(Vector3Int position)
    {
        if (TryFindGroundInteractive(position))
            return false;

        return baseMap.HasTile(position);
    }
    public bool CheckGroundAvalibleForNewAnt(Vector3Int position)
    {
        if (TryFindGroundInteractive(position))
            return false;
        if (TryFindAntNestBranch(position))
            return false;

        return baseMap.HasTile(position);
    }

    public bool InstantiateAntNestOnGrid(Vector3 position, bool useFireAnt)
    {
        return InstantiateAntNestOnGrid(grid.WorldToCell(position), useFireAnt);
    }
    public bool InstantiateAntNestOnGrid(Vector3Int position, bool useFireAnt)
    {
        if (TryFindGroundInteractive(position))
            return false;
        if (!baseMap.HasTile(position))
            return false;
        if (TryFindAntNestBranch(position))
            return false;
        if (_antNestHubs.Count >= antNestCountCap)
            return false;

        spawnAnimalNest?.Spawn(useFireAnt, grid.GetCellCenterWorld(position));
        return true;
    }

    public void InstantiateAntNestOnGridWithoutChecking(Vector3Int position, bool useFireAnt)
    {
        spawnAnimalNest?.Spawn(useFireAnt, grid.GetCellCenterWorld(position));
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

    public struct AntCountInfo
    {
        public int NativeAnt;
        public int NativeAntDisabled;
        public int FireAnt;
        public int FireAntDisabled;
    }
}
