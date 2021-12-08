using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeroGenerator : MonoBehaviour
{
    [SerializeField] bool turnOn = false;

    [SerializeField] float spawnTime = 10f;

    [SerializeField] GameObject hero;

    [SerializeField] float xRange = 30f;

    GameObject instance;

    Platform platform;


    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        platform = FindObjectOfType<Platform>();
        StartCoroutine(GenerateCoroutine());
    }

    private IEnumerator GenerateCoroutine()
    {
        while (turnOn)
        {
            float range = Random.Range(0.8f, 1f);
            yield return new WaitForSeconds(range * spawnTime);
            if (instance)
            {
                Destroy(instance);
            }
            instance = Instantiate(hero, SaveRandomPosition(), Quaternion.identity);
        }
    }

    private Vector3 SaveRandomPosition()
    {
        Vector2 platformPos = platform.Position();

        int lrSide = Random.Range(0, 2) * 2 - 1;

        float xfar = Random.Range(0.4f, 1f);

        float yfar = Random.Range(0.8f, 1f);

        return new Vector3(lrSide * xRange * xfar, platformPos.y + 5f * yfar, 0);
    }

}