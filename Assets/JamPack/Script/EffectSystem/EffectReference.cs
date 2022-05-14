using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName = "MPack/Effect Reference", order = 0)]
    public class EffectReference : ScriptableObject
    {
        public ParticleSystem Prefab;

        public Stack<ParticleSystem> Pools;
        public Stack<EffectQueue> WaitingList;

        void OnEnable()
        {
            Pools = new Stack<ParticleSystem>();
            WaitingList = new Stack<EffectQueue>();
        }

        public ParticleSystem GetFreshEffect()
        {
            if (Pools.Count > 0)
            {
                return Pools.Pop();
            }

            return GameObject.Instantiate(Prefab);
        }

        public void Put(ParticleSystem effect)
        {
            effect.Stop();
            Pools.Push(effect);
        }

        public void AddWaitingList(EffectQueue queue)
        {
            WaitingList.Push(queue);
        }

        public struct EffectQueue
        {
            public Transform Parent;
            public Vector3 Position;
            public bool UseScaleTime;
        }
    }
}