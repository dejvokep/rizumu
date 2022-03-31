using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideParticlesOnBack : MonoBehaviour
{
    public GameObject particleHolder;
    public GameObject[] buttonActivators;

    private bool? lastUpdate = null;

    private void checkIfActive()
    {
        bool isActive = transform.parent.GetComponent<navigationManager>().getCurrentMenu() == gameObject;
        if (lastUpdate == null || lastUpdate != isActive)
        {
            foreach (Transform child in particleHolder.transform)
                enableParticles(child.GetComponent<ParticleSystem>(), isActive);
            
            buttonActivators[0].GetComponent<Activate>().Button();
            buttonActivators[1].GetComponent<ActivateNote>().Button();
            buttonActivators[1].GetComponent<ActivateNote>().Button();

            lastUpdate = isActive;
        }
    }

    private void enableParticles(ParticleSystem particleSystem, bool enable)
    {
        if (enable)
            particleSystem.Play(true);
        else
            particleSystem.Stop(true);
    }

    void Start()
    {
        checkIfActive();
    }

    void OnEnable()
    {
        MenuEventManager.changedMenuEvent += checkIfActive;
    }
    void OnDisable()
    {
        MenuEventManager.changedMenuEvent -= checkIfActive;
    }
}
