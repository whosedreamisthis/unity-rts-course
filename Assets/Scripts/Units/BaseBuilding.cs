using System.Collections;
using UnityEngine;

namespace GameDevTV.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public void BuildUnit(UnitSO unit)
        {
            StartCoroutine(DoBuildUnitCo(unit));
        }

        private IEnumerator DoBuildUnitCo(UnitSO unit)
        {
            Debug.Log("start building a unit.");
            yield return new WaitForSeconds(unit.BuildTime);
            Debug.Log("building unit completed.");
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
        }
    }
}
