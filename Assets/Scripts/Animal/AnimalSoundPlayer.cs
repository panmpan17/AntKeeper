using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class AnimalSoundPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioListener listener;
    [SerializeField]
    private float radius;
    [SerializeField]
    private AudioPreset sound;
    [SerializeField]
    private RangeReference interval;
    private Timer intervalTimer;
    
    void Awake()
    {
        intervalTimer = new Timer(interval.PickRandomNumber());
    }

    void Update()
    {
        if (intervalTimer.UpdateEnd)
        {
            int index = Random.Range(0, GridManager.ins.Animals.Count);
            VirtualAnimalSpot animalSpot = GridManager.ins.Animals[index];

            float sqrMagnitude = (animalSpot.transform.position - listener.transform.position).sqrMagnitude;
            if (sqrMagnitude > radius * radius)
                return;
            
            if (animalSpot.IsAlive)
                VirtualAudioManager.ins.PlayOneShotAtPosition(sound, animalSpot.transform.position);

            intervalTimer.Reset();
            intervalTimer.TargetTime = interval.PickRandomNumber();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(listener.transform.position, radius);
        // Debug.Log(listener);
        // if (listener != null)
        // {
        // }
    }
}
