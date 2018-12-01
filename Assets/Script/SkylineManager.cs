using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SkylineManager : MonoBehaviour
{/*This class manages skyline blocks: they are created, moved and pooled here. 
   In order to fetch a pooled block of the same type of the one we are looking for, 
   I test the prefab name. Object pooling will not work correctly if blocks of the same
   type have a different name. */

    public GameObject[] skylineBlocks; //prefab list of blocks
    public float BLOCKS_VELOCITY = 2;
    public float SPACE_BETWEEN_BLOCKS = 4.9f;
    public int MAX_BLOCKS = 5;
    public Transform BlocksStartPosition;
    public Transform BlocksEndPosition;

    public List<GameObject> blocksInScene;
    public List<GameObject> pooledBlocks;

    public bool running;

    void Start ()
    {
        blocksInScene = new List<GameObject>();
        pooledBlocks = new List<GameObject>();

        for (int i = 0; i < MAX_BLOCKS; i++)
        {//I instantiate the initial blocks
            NewBlock();
        }
	}

    void Update()
    {
        if (running)
        {
            MoveBlocks();
            if (blocksInScene.Count < MAX_BLOCKS)
                NewBlock();
        }
    }
   
    private void MoveBlocks()
    {//Move blocks and remove them if they crossed the end position
        int i = 0;
        while (i < blocksInScene.Count)
        {
            GameObject block = blocksInScene[i];

            block.transform.Translate(-BLOCKS_VELOCITY * Time.deltaTime, 0, 0);

            if (block.transform.position.x < BlocksEndPosition.position.x)
            {
                RemoveBlock(block);
            }
            else
                i++;
        }
    }
    private void RemoveBlock(GameObject block)
    {
        blocksInScene.Remove(block);
        pooledBlocks.Add(block);
        block.SetActive(false);
    }
    public  void ClearBlocks()
    {//Remove all blocks
        while (blocksInScene.Count > 0)
        {
            RemoveBlock(blocksInScene[0]);
        }
    }
    private void NewBlock()
    {//Instantiates a new block and puts it into the list

        //Get spawn position
        Vector3 spawnPosition;
        if (blocksInScene.Count > 0)
            spawnPosition = blocksInScene[blocksInScene.Count - 1].transform.position + new Vector3(SPACE_BETWEEN_BLOCKS, 0);
        else
            spawnPosition = BlocksStartPosition.position;

        //Get block type
        GameObject nextBlockPrefab = skylineBlocks[UnityEngine.Random.Range(0, skylineBlocks.Length)];

        GameObject newBlock = null;
        //Look for a pooled block
        foreach (GameObject pooledBlock in pooledBlocks)
        {
            if (pooledBlock.name == nextBlockPrefab.name)
            {//We found an already used block of the same type of the one we are looking for
                newBlock = pooledBlock;
                break;
            }
        }

        if (newBlock != null)
        {//We found a pooled block, let's use it again
            pooledBlocks.Remove(newBlock);
            newBlock.SetActive(true);
            newBlock.transform.position = spawnPosition;
        }
        else
        {//We didn't find a new objects, let's create a new one 
            newBlock = Instantiate(nextBlockPrefab, spawnPosition, Quaternion.identity) as GameObject;
            newBlock.name = nextBlockPrefab.name;//I make sure that the names are ok for pooling
        }

        blocksInScene.Add(newBlock);
    }
}
