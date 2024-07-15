using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstantiator : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab do inimigo
    public Transform spawnPoint; // Ponto onde os inimigos serão instanciados
    public Transform destinationPoint; // Ponto de destino para onde os inimigos irão se mover
    public float spawnInterval = 2f; // Intervalo entre as instâncias dos inimigos
    public float enemySpeed = 2f; // Velocidade de movimento dos inimigos

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Instancia o inimigo no ponto de spawn
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Inicia a coroutine para mover o inimigo até o ponto de destino
            StartCoroutine(MoveEnemy(enemy));

            // Espera pelo intervalo antes de instanciar o próximo inimigo
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator MoveEnemy(GameObject enemy)
    {
        while (enemy != null && Vector3.Distance(enemy.transform.position, destinationPoint.position) > 0.1f)
        {
            // Calcula a direção e move o inimigo na direção do ponto de destino
            Vector3 direction = (destinationPoint.position - enemy.transform.position).normalized;
            enemy.transform.position += direction * enemySpeed * Time.deltaTime;

            yield return null;
        }

        // Destroi o inimigo quando ele alcançar o ponto de destino
        if (enemy != null)
        {
            Destroy(enemy);
        }
    }
}
