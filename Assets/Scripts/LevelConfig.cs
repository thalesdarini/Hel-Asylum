using UnityEngine;

public class LevelConfig : MonoBehaviour
{
    [SerializeField] GameObject initialPos = null;
    [SerializeField] GameObject finalPos = null;
    [SerializeField] GameObject soundManagerPrefab = null;
    int enemyCount = 0;

    private void Awake()
    {
        // para q seja possível iniciar direto de um level ao invés do "Loader"
        int soundManagerCount = FindObjectsOfType<SoundManager>().Length;
        if (soundManagerCount < 1)
        {
            Instantiate(soundManagerPrefab);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        finalPos.SetActive(false);
    }

    public Vector3 GetInitialPos()
    {
        return initialPos.transform.position;
    }

    public void DecreaseEnemyCount()
    {
        enemyCount--;
        Debug.Log("enemyCount: " + enemyCount);
        if (enemyCount == 0)
        {
            finalPos.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            FindObjectOfType<SceneLoader>().LoadNextScene();
        }
    }
}
