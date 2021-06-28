using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{

    /// <summary>
    /// La textura que usaremos como mirilla para el cursor cuando estemos en modo disparo;
    /// </summary>
    public Texture2D PunteroMirilla;

    public int Width = 64;
    public int Height = 64;
    public int MaxRooms = 15;
    public int MinRoomXY = 5;
    public int MaxRoomXY = 10;
    public bool RoomsOverlap = false;
    public int RandomConnections = 1;
    public int RandomSpurs = 3;
    public int Seed = 0;

    public Tilemap WallTilemap;
    public Tilemap BackgroundTilemap;
    public TileBase WallTile;
    public TileBase FloorTile;

    public GameObject EnemyPrefab;
    public int minEnemiesPerRoom = 0;
    public int maxEnemiesPerRoom = 15;

    public HeatlhBar healthBar;
    public Text AmmoCounter;
    public Text EnemyCounterText;
    public GameObject NextLevelPanel;
    public GameObject GameOverPanel;
    public GameObject EndPanel;

    public GameObject LevelText;

    public int NumLevels = 2;

    private MapGenerator mapGenerator;
    private System.Random random;

    private int level = 1;

    private PlayerController playerController;

    void Start()
    {
        level = PlayerPrefs.GetInt("Level", 0); 
        int health = PlayerPrefs.GetInt("Health", 100);
        int gunammo = PlayerPrefs.GetInt("GunAmmo", 10);

        Text[] texts = LevelText.GetComponentsInChildren<Text>();
        foreach (Text t in texts)
        {
            t.text = "Level " + level;
        }

        SetMirilla();

        int mSeed = (Seed != 0) ? Seed: Environment.TickCount;

        //para los niveles en principio vamos a usar seeds ya probadas que nos gustan
        //pero se podrían usar seeds aleatorias para generar un "modo infinito"
        if(level == 1)
        {
            mSeed = -2068246859;
        }
        else if (level == 2)
        {
            mSeed = -2067953171;
        }

        //Debug.Log("Seed:" + mSeed);
        random = new System.Random(mSeed);
        GenerateTilemap(mSeed);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.transform.position = getRoomCenterWorldCoords(mapGenerator.room_list[0]);
        playerController = playerObject.GetComponent<PlayerController>();
        playerController.OnHealthChanged += playerHealthChanged;
        playerController.Health = health;
        healthBar.SetHealth(health);

        playerController.GunAmmo = gunammo;
        AmmoCounter.text = gunammo.ToString();

        SpawnEnemies();
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int count = enemies.Length;
        EnemyCounterText.text = count.ToString();

        float vol = ((float)PlayerPrefs.GetInt("VolumeMusic", 100)) / 100f;
        Camera.main.GetComponent<AudioSource>().volume = vol;

    }

    
    /// <summary>
    /// Método en cargado de cambiar el cursor por el icono de mirilla
    /// </summary>
    private void SetMirilla()
    {
        //tenemos que fijar el offset del cursor en el centro de la imagen
        Vector2 cursorOffset = new Vector2(PunteroMirilla.width / 2, PunteroMirilla.height / 2);

        //Establecemos el cursor con el offset calculado
        Cursor.SetCursor(PunteroMirilla, cursorOffset, CursorMode.Auto);
    }

    /// <summary>
    /// Métedo encargado de generar un mapa con los parámetros fijados y pintarlo en el Tilemap
    /// </summary>
    void GenerateTilemap(int mSeed)
    {
        mapGenerator = new MapGenerator(Width, Height, MaxRooms, MinRoomXY, MaxRoomXY, RoomsOverlap, RandomConnections, RandomSpurs, mSeed);
        mapGenerator.GenerateLevel();
        RenderMap();
    }

    /// <summary>
    /// Método encargado de pintar en el Tilemap el mapa que hemos generado con la clase mapGenerator
    /// </summary>
    void RenderMap()
    {
        BackgroundTilemap.ClearAllTiles();
        WallTilemap.ClearAllTiles();
        for (int x = 0; x < mapGenerator.Level.GetUpperBound(0); x++)
        {
            for (int y = 0; y < mapGenerator.Level.GetUpperBound(1); y++)
            {
                // 0 = no tile, 1 = suelo, 2 = pared
                if (mapGenerator.Level[x, y] == 1 || mapGenerator.Level[x, y] == 2)
                {
                    BackgroundTilemap.SetTile(new Vector3Int(x, y, 0), FloorTile);
                }
                if (mapGenerator.Level[x, y] == 2)
                {
                    WallTilemap.SetTile(new Vector3Int(x, y, 0), WallTile);
                }
            }

        }
    }

    public Vector3 getRoomCenterWorldCoords(Tuple<int, int, int, int> room)
    {
        int x = room.Item1;
        int y = room.Item2;
        int w = room.Item3;
        int h = room.Item4;
        return WallTilemap.CellToWorld(new Vector3Int(y + h / 2, x + w / 2, 0));
    }

    void SpawnEnemies()
    { 
        //nos saltamos la primera habitación porque es en la que empieza el jugador
        for (int i = 1; i < mapGenerator.room_list.Count; i++)
        {
            Tuple<int, int, int, int> room = mapGenerator.room_list[i];

            int x = room.Item1;
            int y = room.Item2;
            int w = room.Item3;
            int h = room.Item4;

            int min = Mathf.Min(minEnemiesPerRoom, w * h);
            int max = Mathf.Min(maxEnemiesPerRoom, w * h);
            int enemy_number = random.Next(min, max);
            for(int j = 0; j < enemy_number; j++)
            {
                int enemyX = j % w;
                int enemyY = j / w;
                Vector3 enemyPos = WallTilemap.CellToWorld(new Vector3Int(y + enemyY, x + enemyX, 0));
                GameObject enemy = Instantiate(EnemyPrefab, enemyPos, Quaternion.Euler(0, 0, 0));
                enemy.GetComponent<EnemyController>().OnEnemyDead += UpdateEnemyCounter;
            }
        }
    }

    void UpdateEnemyCounter()
    {

        int count =  Int32.Parse(EnemyCounterText.text) - 1;
        if (count <= 0) count = 0;
        EnemyCounterText.text = count.ToString();

        if(count <= 0)
        {
            if(level != NumLevels)
            {
                NextLevelPanel.GetComponent<NextLevel>().SetNextLevelParameters(level + 1, playerController.Health);
                NextLevelPanel.SetActive(true);
            }
            else
            {
                EndPanel.SetActive(true);
            }
        }
    }

    void playerHealthChanged()
    {
        healthBar.SetHealth(playerController.Health);
        if(playerController.Health <= 0)
        {
            //Destroy(playerController.gameObject);
            GameOverPanel.SetActive(true);
        }
    }

    void Update()
    {
        AmmoCounter.text = playerController.GunAmmo.ToString();
    }
}
