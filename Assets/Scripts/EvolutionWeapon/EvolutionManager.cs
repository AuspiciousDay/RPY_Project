using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] private WeaponController[] weapons;

    public List<WeaponController> GetEvolvableWeapons()
    {
        List<WeaponController> candidates =
            new List<WeaponController>();

        foreach (WeaponController weapon in weapons)
        {
            if (weapon != null && weapon.CanEvolve)
            {
                candidates.Add(weapon);
            }
        }

        return candidates;
    }

    public bool TryEvolve()
    {
        List<WeaponController> candidates =
            GetEvolvableWeapons();

        if (candidates.Count == 0)
            return false;

        int index = Random.Range(0, candidates.Count);

        WeaponController selected = candidates[index];

        return selected.Evolve();
    }
}