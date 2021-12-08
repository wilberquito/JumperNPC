using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeroGenerator : MonoBehaviour
{
    [SerializeField] bool turnOn = false;

    [SerializeField] float spawnTime = 10f;

    [SerializeField] GameObject hero;

    GameObject instance;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateCoroutine());
    }

    private IEnumerator GenerateCoroutine()
    {
        while (turnOn)
        {
            float range = Random.Range(0.5f, 1f);
            yield return new WaitForSeconds(range * spawnTime);
            if (instance)
            {
                Destroy(instance);
            }
            instance = Instantiate(hero, transform);
        }
    }

}