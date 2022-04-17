using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager : MonoBehaviour
{
    public static GridManager ins;

    [SerializeField]
    private Grid grid;
    public Grid Grid => grid;

    [SerializeField]
    private Tilemap baseMap;

    [SerializeField]
    private Tilemap routeColliderMap;
    [SerializeField]
    private TilemapReference routeColliderMapReference;

    [SerializeField]
    private Transform antNestCollection;
    [SerializeField]
    private int antNestCountCap = 20;
    [SerializeField]
    private GameObject fireAntPrefab;
    [SerializeField]
    private GameObject navtiveAntPrefab;

    private List<AbstractGroundInteractive> _groundInteractives;
    private List<AntNest> _antNests;
    private List<VirtualAnimalSpot> _animals;

    void Awake()
    {
        ins = this;
        _groundInteractives = new List<AbstractGroundInteractive>();
        _antNests = new List<AntNest>();
        _animals = new List<VirtualAnimalSpot>();

        routeColliderMapReference.Tilemap = routeColliderMap;
    }

    void Start()
    {
        GameManager.ins.OnGameReady += delegate { antNestCollection.gameObject.SetActive(true); };
    }

    #region Register and unregister
    public void RegisterGroundInteractive(AbstractGroundInteractive groundInteractive, out Vector3Int gridPosition)
    {
        _groundInteractives.Add(groundInteractive);
        gridPosition = grid.WorldToCell(groundInteractive.transform.position);
    }

    public void RegisterAntNest(AntNest antNest)
    {
        _antNests.Add(antNest);
        HUDManager.ins.UpdateFireAntCount();
    }
    public void UnregisterAntNest(AntNest antNest)
    {
        _antNests.Remove(antNest);
        HUDManager.ins.UpdateFireAntCount();
    }

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

    public bool TryFindAntNestBranch(Vector3Int gridPosition)
    {
        for (int i = 0; i < _antNests.Count; i++)
        {
            if (_antNests[i].IsGridPositionOverlapBranch(gridPosition))
            {
                return true;
            }
        }
        return false;
    }
    public bool TryFindAntNestBranch(Vector3Int gridPosition, out AntNest antNest, out AntRouteBranch branch)
    {
        for (int i = 0; i < _antNests.Count; i++)
        {
            if (_antNests[i].IsGridPositionOverlapBranch(gridPosition, out branch))
            {
                antNest = _antNests[i];
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

    public int CountFireAnt()
    {
        int count = 0;
        for (int i = 0; i < _antNests.Count; i++)
        {
            if (_antNests[i].IsFireAnt)
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

    public bool InstantiateAntNestOnGrid(Vector3Int position, bool useFireAnt)
    {
        if (TryFindGroundInteractive(position))
            return false;
        if (!baseMap.HasTile(position))
            return false;
        if (TryFindAntNestBranch(position))
            return false;
        if (_antNests.Count >= antNestCountCap)
            return false;

        GameObject newAntNest = Instantiate(useFireAnt ? fireAntPrefab : navtiveAntPrefab, antNestCollection);

        newAntNest.transform.position = grid.GetCellCenterWorld(position);

        return true;
    }
    #endregion
}
