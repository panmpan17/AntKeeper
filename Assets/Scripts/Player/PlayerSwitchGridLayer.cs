using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitchGridLayer : MonoBehaviour
{
    private Vector3Int _currentGridPosition;

    void Start()
    {
        _currentGridPosition = GridManager.ins.WorldToCell(transform.position);
        GridManager.ins.SwitchLayer(GridManager.ins.GetCurrentLayer(_currentGridPosition));
    }

    void Update()
    {
        Vector3Int newGridPosition = GridManager.ins.WorldToCell(transform.position);
        if (newGridPosition != _currentGridPosition)
        {
            _currentGridPosition = newGridPosition;

            int newLayer = GridManager.ins.GetCurrentLayer(_currentGridPosition);

            if (newLayer != GridManager.ins.CurrentLayer)
            {
                GridManager.ins.SwitchLayer(newLayer);
            }
        }
    }
}
