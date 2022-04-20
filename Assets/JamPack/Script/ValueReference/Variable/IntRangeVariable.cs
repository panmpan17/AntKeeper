using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Variable/Int Range", order=0)]
    public class IntRangeVariable : ScriptableObject
    {
        public int Min;
        public int Max;

        public int PickRandomNumber(bool maxInclusive = false)
        {
            return maxInclusive ? Random.Range(Min, Max + 1) : Random.Range(Min, Max);
        }
    }
}