using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerate
{
    public interface IMapProcessor
    {
        void Process(ref int[,] map);
    }
}