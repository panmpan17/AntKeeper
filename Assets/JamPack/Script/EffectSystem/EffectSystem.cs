using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack
{
    public class EffectSystem : MonoBehaviour
    {
        [SerializeField]
        private EffectReference[] listenEffects;

        void LateUpdate()
        {
            for (int i = 0; i < listenEffects.Length; i++)
            {
                EffectReference effectReference = listenEffects[i];

                while (effectReference.WaitingList.Count > 0)
                {
                    EffectReference.EffectQueue effectQueue = effectReference.WaitingList.Pop();

                    ParticleSystem newEffect = effectReference.GetFreshEffect();
                    newEffect.transform.SetParent(effectQueue.Parent);
                    newEffect.transform.position = effectQueue.Position;
                    ParticleSystem.MainModule main = newEffect.main;
                    main.useUnscaledTime = !effectQueue.UseScaleTime;
                    newEffect.Play();

                    StartCoroutine(WaitToCollectEffect(effectReference, newEffect, effectQueue.UseScaleTime, newEffect.main.duration));
                }
            }
        }

        IEnumerator WaitToCollectEffect(EffectReference effectReference, ParticleSystem effect, bool useScaleTime, float duration)
        {
            if (useScaleTime)
                yield return new WaitForSeconds(duration);
            else
                yield return new WaitForSecondsRealtime(duration);

            effectReference.Put(effect);
        }
    }
}