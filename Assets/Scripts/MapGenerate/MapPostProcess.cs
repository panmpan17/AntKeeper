using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace MapGenerate
{
    [CreateAssetMenu]
    public class MapPostProcess : ScriptableObject
    {
        public ProcessRule[] Processes;

        public void Process(GridManager.GridLayer[] layers, Tilemap edgeWaveTilemap)
        {
            for (int i = 0; i < Processes.Length; i++)
            {
                Processes[i].Process(layers, edgeWaveTilemap);
            }
        }


        public enum ProcessType
        {
            WallGenerate,
            EdgeGenerate,
        }

        [System.Serializable]
        public struct ProcessRule
        {
            public ProcessType ProcessType;

            public WallGenerate WallGenerate;
            public EdgeGenerate EdgeGenerate;


            public void Process(GridManager.GridLayer[] layers, Tilemap edgeWaveTilemap)
            {
                switch (ProcessType)
                {
                    case ProcessType.WallGenerate:
                        WallGenerate.Process(layers);
                        break;
                    case ProcessType.EdgeGenerate:
                        EdgeGenerate.Process(layers, edgeWaveTilemap);
                        break;
                }
            }
        }
    }
}