using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPosition;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist;
    float OpDist;
    float optimizerCooldown;
    public float optimizerCooldownDur;


    // Start is called before the first frame update
    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if(!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string directionName = GetDirectionName(moveDir);
        
        CheckAndSpawnChunk(directionName);

        if (directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        if (directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
        if (directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
        if (directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }
    }

    void CheckAndSpawnChunk(string direction)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);
        }
    }

    string GetDirectionName(Vector3 dir)
    {
        dir = dir.normalized;
        // Vertical
        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if(dir.y > 0.5f)
            {
                return dir.x > 0 ? "Right Up" : "Left Up";
            }
            else if(dir.y < -0.5f)
            {
                return dir.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                return dir.x > 0 ? "Right" : "Left";
            }
        }

        // Horizontal
        else
        {
            if(dir.x > 0.5f)
            {
                return dir.y > 0 ? "Right Up" : "Right Down";
            }
            else if(dir.x < -0.5f)
            {
                return dir.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                return dir.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;
        if(optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDur;
            //Optimize();
        }

        foreach(GameObject chunk in spawnedChunks)
        {
            OpDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if(OpDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
