using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject enemyObj;
    public GameObject enemyKeeper;
    public GameObject treasure;
    public Text scoreText;
    public MyGrid grid;
    private Player player;
    private List<Enemy> enemies;
    private World world;
    private Vector3 oldTreasurePosition;
    private float enemyTimer = 0.2f;
    private float currentEnemyTimer = 0.0f;
    private float enemySpawnTimer = 0.5f;
    private float currentEnemySpawnTimer = 0.0f;
    private float playerMoveTimer = 0.1f;
    private float currentPlayerMoveTimer = 0.0f;
    private int score;
    private int maxScore = 99999;
    // Start is called before the first frame update
    void Start()
    {
        grid.Initialize();

        playerObj = Instantiate(playerObj, Vector3.zero, Quaternion.identity);
        GameObject newEnemy = Instantiate(enemyObj, Vector3.zero, Quaternion.identity);
        treasure = Instantiate(treasure, Vector3.zero, Quaternion.identity);

        // treasure.transform.position = grid.NodeFromGridPoint(Random.Range(1, grid.columns - 1), Random.Range(1, grid.rows - 1)).worldPosition;
        treasure.transform.position = grid.NodeFromGridPoint(2, grid.rows - 5).worldPosition;
        playerObj.transform.position = grid.NodeFromGridPoint(grid.columns - 1, Random.Range(0, grid.rows)).worldPosition;
        newEnemy.transform.position = grid.NodeFromGridPoint(0, Random.Range(0, grid.rows)).worldPosition;

        player = playerObj.GetComponent<Player>();

        Enemy enemy = newEnemy.GetComponent<Enemy>();
        enemies = new List<Enemy>();
        enemies.Add(enemy);
        newEnemy.transform.parent = enemyKeeper.transform;

        world = new World();
        world.InitializeWorld(grid, newEnemy, treasure);

        player.SetCurrentState(world, grid);

        score = 0;
        scoreText.text = score.ToString();

        player.LoadData();
    }

    private void Reset()
    {
        foreach(Enemy enemy1 in enemies)
        {
            enemy1.gameObject.SetActive(false);
        }

        GameObject newEnemy = Instantiate(enemyObj, Vector3.zero, Quaternion.identity);

        // treasure.transform.position = grid.NodeFromGridPoint(Random.Range(1, grid.columns - 1), Random.Range(1, grid.rows - 1)).worldPosition;
        treasure.transform.position = grid.NodeFromGridPoint(2, grid.rows - 5).worldPosition;
        playerObj.transform.position = grid.NodeFromGridPoint(grid.columns - 1, Random.Range(0, grid.rows)).worldPosition;
        newEnemy.transform.position = grid.NodeFromGridPoint(0, Random.Range(0, grid.rows)).worldPosition;
        
        Enemy enemy = newEnemy.GetComponent<Enemy>();
        enemies = new List<Enemy>();
        enemies.Add(enemy);

        world.InitializeWorld(grid, newEnemy, treasure);

        player.SetCurrentState(world, grid);

        // score = 0;
        // scoreText.text = score.ToString();

        player.SaveData();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        MoveEnemy();
        SpawnEnemy();
    }

    private void MovePlayer()
    {
        currentPlayerMoveTimer += Time.deltaTime;
        if(currentPlayerMoveTimer > playerMoveTimer)
        {
            currentPlayerMoveTimer = 0.0f;
            player.SelectAction();
            player.Move(grid);
            player.SetNextState(world, grid);
            if(player.HasPickedTreasure())
            {
                //Increase score
                player.ResetPickTreasure();
                player.QUpdate(100.0f, false);
                IncreaseScore();
                Reset();
            }
            else if(player.HasHitEnemy())
            {
                player.ResetHitEnemy();
                player.QUpdate(-100.0f, true);
                Reset();
            }
            else if(player.IsStuck())
            {
                player.QUpdate(-1.0f, false);
            }
            else
            {
                player.QUpdate(0.0f, false);
            }
        }
    }

    private void MoveEnemy()
    {
        currentEnemyTimer += Time.deltaTime;
        if(currentEnemyTimer > enemyTimer)
        {
            currentEnemyTimer = 0.0f;
            foreach(Enemy enemy in enemies)
            {
                if(!enemy.HasReachedDestination(grid))
                {
                    enemy.Move(grid);
                    world.UpdateWorldEnemy(enemy.GetPreviousPosition(), enemy.GetPosition());
                }
                else
                {
                    world.RemoveEnemy(enemy.GetPosition());
                    enemy.gameObject.SetActive(false);
                }
            }
            player.SetCurrentState(world, grid);
        }
    }

    private void SpawnEnemy()
    {
        currentEnemySpawnTimer += Time.deltaTime;
        
        if(currentEnemySpawnTimer > enemySpawnTimer)
        {
            currentEnemySpawnTimer = 0.0f;
            GameObject newEnemy = Instantiate(enemyObj, Vector3.zero, Quaternion.identity);
            newEnemy.transform.position = grid.NodeFromGridPoint(0, Random.Range(0, grid.rows)).worldPosition;
            Enemy enemy = newEnemy.GetComponent<Enemy>();
            enemies.Add(enemy);
            newEnemy.transform.parent = enemyKeeper.transform;
            world.AddEnemy(enemy.GetPosition());
            player.SetCurrentState(world, grid);
        }
    }

    private void IncreaseScore()
    {
        score += 100;
        score = Mathf.Min(score, maxScore);
        scoreText.text = score.ToString();
        // ResetTreasure();
    }

    private void ResetTreasure()
    {
        // treasure = Instantiate(treasure, Vector3.zero, Quaternion.identity);
        oldTreasurePosition = treasure.transform.position;
        // treasure.transform.position = grid.NodeFromGridPoint(Random.Range(1, grid.columns - 1), Random.Range(1, grid.rows - 1)).worldPosition;
        treasure.transform.position = grid.NodeFromGridPoint(2, grid.rows - 5).worldPosition;
        world.UpdateWorldTreasure(oldTreasurePosition, treasure.transform.position);
    }
}
