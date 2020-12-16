using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    [SerializeField] protected string[] TagsToInteractWith;
    [SerializeField] protected UnityEvent OnTriggerEnterEvent = new UnityEvent();
    [SerializeField] protected int TimesEntered = 0;
    [SerializeField] protected UnityEvent OnTriggerExitEvent = new UnityEvent();
    [SerializeField] protected int TimesExited = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (TagsToInteractWith.Contains(other.tag))
        {
            OnTriggerEnterEvent.Invoke();
            TimesEntered++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (TagsToInteractWith.Contains(other.tag))
        {
            OnTriggerExitEvent.Invoke();
            TimesExited++;
        }
    }
}
