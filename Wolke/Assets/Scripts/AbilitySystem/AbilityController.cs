using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public readonly List<Ability> Abilities = new List<Ability>();

    protected virtual void Start()
    {
        foreach (Ability abilityComponent in GetComponents<Ability>())
        {
            Abilities.Add(abilityComponent);
        }
    }

    protected virtual void Update()
    {
        HandleAbilities();
    }

    /// <summary>
    /// Executes all the availible abilities
    /// </summary>
    protected virtual void HandleAbilities()
    {
        foreach (Ability ability in Abilities.Where(a => a.AbilityActive()))
        {
            ability.AbilityUpdate();
        }

        foreach (Ability ability in Abilities.Where(a => a.AbilityOnCoolDown()))
        {
            ability.CoolDownUpdate();
        }

        foreach (Ability ability in Abilities)
        {
            ability.PermanentUpdate();
        }
    }
}
