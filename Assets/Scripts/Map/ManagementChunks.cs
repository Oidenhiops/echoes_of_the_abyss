using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagementChunks : MonoBehaviour
{
    public AStarPathFinding aStarPathFinding;
    public ManagementOpenCloseScene managementOpenCloseScene;
    public int chunkSize = 17;
    public int chunksX = 5;
    public int chunksZ = 5;
    public List<PositionChunk> positionsChunks = new List<PositionChunk>();
    public List<ManagementMapBlock> allBlocksGenerated = new List<ManagementMapBlock>();
    public List<Vector3> positionsForMakePath = new List<Vector3>();
    public Vector3 spawnPosition;
    public GameObject[] characters;
    public DirectionChunk directionChunk;
    private void Start()
    {
        StartCoroutine(GenerateChunks());
    }
    public IEnumerator GenerateChunks()
    {
        positionsChunks = GenerateMap(chunksX, chunksZ);
        GameObject chunkPrefab = Resources.Load<GameObject>("Prefabs/Map/Chunk");
        float offsetX = chunksX * chunkSize / 2f - (chunkSize / 2f);
        float offsetZ = chunksZ * chunkSize / 2f - (chunkSize / 2f);

        for (int c = 0; c < positionsChunks.Count; c++)
        {
            Vector3 chunkPosition = new Vector3(positionsChunks[c].positionChunk.x * chunkSize - offsetX, 0, positionsChunks[c].positionChunk.z * chunkSize - offsetZ);
            GameObject chunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
            positionsChunks[c].managementChunk = chunk.GetComponent<ManagementChunk>();
            positionsChunks[c].managementChunk.drawerMap = positionsChunks[c].managementChunk.GetAndInstanceRandomRoom();
            chunk.GetComponent<ManagementChunk>().chunkSize = chunkSize;
            chunk.GetComponent<BoxCollider>().size = chunk.GetComponent<ManagementChunk>().chunkSize * Vector3.one;
            chunk.name = $"Chunk_{positionsChunks[c].positionChunk.x}_{positionsChunks[c].positionChunk.z}";
            chunk.transform.parent = this.transform;
        }
        MakeBridges();
        yield return new WaitForSeconds(0.5f);
        foreach (var chunk in positionsChunks)
        {
            chunk.managementChunk.DrawRoom();
        }
        yield return new WaitForSeconds(0.5f);
        spawnPosition = positionsChunks[Random.Range(0, positionsChunks.Count)].managementChunk.transform.position;
        characters = GameObject.FindGameObjectsWithTag("Player");
        foreach (var character in characters)
        {
            character.transform.position = new Vector3(spawnPosition.x, 1, spawnPosition.z);
        }
        //positionsForMakePath = GetPositionsToMakePath();
        yield return new WaitForSeconds(0.5f);
        allBlocksGenerated.Clear();
        //aStarPathFinding.occupiedPositions = positionsForMakePath;
        //aStarPathFinding.GenerateWalkableGrid();
    }
    public void MakeBridges()
    {
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.forward))
            {
                BuildBridge(positionsChunks[i].managementChunk, ManagementDrawerMap.DirectionBridges.Forward);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.back))
            {
                BuildBridge(positionsChunks[i].managementChunk, ManagementDrawerMap.DirectionBridges.Back);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.left))
            {
                BuildBridge(positionsChunks[i].managementChunk, ManagementDrawerMap.DirectionBridges.Left);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.right))
            {
                BuildBridge(positionsChunks[i].managementChunk, ManagementDrawerMap.DirectionBridges.Rigth);
            }
        }
    }
    public List<Vector3> GetPositionsToMakePath()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var block in allBlocksGenerated)
        {
            if (block.isWalkable)
            {
                positions.Add(block.gameObject.transform.position + Vector3.up);
            }
        }
        return positions;
    }
    public void BuildBridge(ManagementChunk chunk, ManagementDrawerMap.DirectionBridges directionBridge)
    {
        chunk.drawerMap.bridges[directionBridge].bridge.SetActive(true);
        for (int i = 0; i < chunk.drawerMap.bridges[directionBridge].blocks.Length; i++)
        {
            chunk.drawerMap.blocksRender.Add(chunk.drawerMap.bridges[directionBridge].blocks[i].meshRenderer);
            chunk.drawerMap.mapBlocks.Add(chunk.drawerMap.bridges[directionBridge].blocks[i]);
        }
    }
    public bool ValidateBridge(Vector3Int pos)
    {
        bool contains = false;
        foreach (var chunk in positionsChunks)
        {
            if (chunk.positionChunk == pos)
            {
                contains = true;
                break;
            }
        }
        return contains;
    }
    List<PositionChunk> GenerateMap(int ancho, int alto)
    {
        Vector3Int inicio = new Vector3Int(Random.Range(0, ancho), 0, Random.Range(0, alto));
        Vector3Int fin = new Vector3Int(Random.Range(0, ancho), 0, Random.Range(0, alto));
        while (inicio == fin)
        {
            fin = new Vector3Int(Random.Range(0, ancho), 0, Random.Range(0, alto));
        }

        List<PositionChunk> camino = new List<PositionChunk>
        {
            new PositionChunk { positionChunk = inicio }
        };
        HashSet<Vector3Int> visitados = new HashSet<Vector3Int> { inicio };
        Vector3Int actual = inicio;

        while (actual != fin)
        {
            List<Vector3Int> movimientos = new List<Vector3Int>
        {
            new Vector3Int(actual.x - 1, 0, actual.z),
            new Vector3Int(actual.x + 1, 0, actual.z),
            new Vector3Int(actual.x, 0, actual.z - 1),
            new Vector3Int(actual.x, 0, actual.z + 1)
        };

            movimientos = movimientos
                .Where(mov => mov.x >= 0 && mov.x < ancho && mov.z >= 0 && mov.z < alto && !visitados.Contains(mov))
                .OrderBy(_ => Random.value)
                .ToList();

            if (movimientos.Count == 0)
            {
                Debug.LogError("No hay movimientos válidos restantes.");
                break;
            }

            movimientos = movimientos.OrderBy(mov => Vector3Int.Distance(mov, fin) + Random.Range(-0.5f, 0.5f)).ToList();
            actual = movimientos[0];

            visitados.Add(actual);
            camino.Add(new PositionChunk { positionChunk = actual });
        }

        int cantidadRamas = Random.Range(3, 6);
        for (int i = 0; i < cantidadRamas; i++)
        {
            int indiceCamino = Random.Range(1, camino.Count - 1);
            Vector3Int puntoRama = camino[indiceCamino].positionChunk;
            int longitudRama = Random.Range(2, 5);

            Vector3Int ramaActual = puntoRama;
            for (int j = 0; j < longitudRama; j++)
            {
                List<Vector3Int> movimientosRama = new List<Vector3Int>
            {
                new Vector3Int(ramaActual.x - 1, 0, ramaActual.z),
                new Vector3Int(ramaActual.x + 1, 0, ramaActual.z),
                new Vector3Int(ramaActual.x, 0, ramaActual.z - 1),
                new Vector3Int(ramaActual.x, 0, ramaActual.z + 1)
            };

                movimientosRama = movimientosRama
                    .Where(mov => mov.x >= 0 && mov.x < ancho && mov.z >= 0 && mov.z < alto && !visitados.Contains(mov))
                    .OrderBy(_ => Random.value)
                    .ToList();

                if (movimientosRama.Count == 0) break;

                ramaActual = movimientosRama[0];
                visitados.Add(ramaActual);
                camino.Add(new PositionChunk { positionChunk = ramaActual });
            }
        }

        return camino;
    }
    [System.Serializable]   public class DirectionChunk
    {
        public int pos = 0;
        public ValidateDirectionChunk validateDirectionChunk;
    }
    [System.Serializable]   public class PositionChunk
    {
        public Vector3Int positionChunk = new Vector3Int();
        public ManagementChunk managementChunk;
    }
    public enum ValidateDirectionChunk
    {
        Forward = 0,
        Back = 1,
        Left = 2,
        Rigth = 3
    }
}