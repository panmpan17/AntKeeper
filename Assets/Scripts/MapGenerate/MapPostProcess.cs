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

        public void Process(MapGeneratorManager manager)
        {
            for (int i = 0; i < Processes.Length; i++)
            {
                Processes[i].Process(manager);
            }
        }


        public enum ProcessType
        {
            WallGenerate,
            EdgeGenerate,
            RemoveEdgeOverlap,
        }

        [System.Serializable]
        public struct ProcessRule
        {
            public ProcessType ProcessType;

            public WallGenerate WallGenerate;
            public EdgeGenerate EdgeGenerate;
            public RemoveEdgeOverlap RemoveEdgeOverlap;


            public void Process(MapGeneratorManager manager)
            {
                switch (ProcessType)
                {
                    case ProcessType.WallGenerate:
                        WallGenerate.Process(manager.Layers);
                        break;
                    case ProcessType.EdgeGenerate:
                        EdgeGenerate.Process(manager.Layers, manager.EdgeWaveTilemap);
                        break;
                    case ProcessType.RemoveEdgeOverlap:
                        RemoveEdgeOverlap.Process(manager);
                        break;
                }
            }
        }
    }
}