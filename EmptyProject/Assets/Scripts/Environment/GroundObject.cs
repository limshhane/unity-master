using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LIM_TRAN_HOUACINE_NGUYEN;
using SDD.Events;

public class GroundObject : SimpleGameStateObserver
{
    // Start is called before the first frame update
    [SerializeField] Transform chunkPos;
    [SerializeField] GameObject chunkPrefab;
    [SerializeField] GameObject Obstacle;
    [SerializeField] GameObject piecePrefab;
    private static int ObstacleSpawnFrequency = Constants.ObstacleSpawnFrequency;


    void Start()
    {
        ObstacleSpawnFrequency -= 1;
        if (ObstacleSpawnFrequency == 0)
        {
            setObstaclePosition(Obstacle);
            GameObject obs = Instantiate(Obstacle, Obstacle.transform.position, Quaternion.identity);
            float hauteur_obs = piecePrefab.GetComponent<Renderer>().bounds.size.y + obs.GetComponent<Renderer>().bounds.size.y / 2;
            GameObject piecette = Instantiate(piecePrefab, Obstacle.transform.position + new Vector3(0,hauteur_obs+2,0) , Quaternion.identity);
            obs.transform.SetParent(chunkPos);
            ObstacleSpawnFrequency = Constants.ObstacleSpawnFrequency;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    /***
     * Permet de définir aléatoirement la position d'un obstacle dans son chunk respectif
     */
    private void setObstaclePosition(GameObject Obstacle)
    {
        Vector3 chunkPosVector = chunkPos.position;
        float randomX = Random.Range(chunkPosVector.x-chunkPrefab.GetComponent<Renderer>().bounds.size.x/2, chunkPosVector.x + chunkPrefab.GetComponent<Renderer>().bounds.size.x/2);
        float randomZ = Random.Range(chunkPosVector.z, chunkPosVector.z+ chunkPrefab.GetComponent<Renderer>().bounds.size.x/2);
        
        Vector3 v = new Vector3(randomX, 0, randomZ);
        Obstacle.transform.position = v;
    }

    public override string ToString()
    {
        return chunkPrefab.name;
    }
}
