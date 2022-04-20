using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Variable/Range", order=0)]
    public class RangeVariable : ScriptableObject
    {
        public float Min;
        public float Max;

        public float PickRandomNumber(bool maxInclusive = false)
        {
            return maxInclusive ? Random.Range(Min, Max + 1) : Random.Range(Min, Max);
        }
    }
}