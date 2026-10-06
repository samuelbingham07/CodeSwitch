using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public float lifetime = 0.6f;
    public List<GameObject> showAfter = new();
    public List<GameObject> hideAfter = new();

    void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            for (int i = 0; i < showAfter.Count; i++)
            {
                showAfter[i].SetActive(true);
            }

            for (int i = 0; i < hideAfter.Count; i++)
            {
                hideAfter[i].SetActive(false);
            }

            Destroy(gameObject);
        }
    }
}
