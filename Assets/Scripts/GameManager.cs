using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour, IGameObserver
{
    public IBombStats currentBombStats = new BasicBombStats();
    public static GameManager Instance { get; private set; }
    public GameObject[] players;

    [Header("UI Ayarları")]
    public GameObject gameOverUI; 
    public Text highScoreText;    

    
    private IPlayerRepository _playerRepository;

    private void Awake()
    {
        if (Instance != null) { DestroyImmediate(gameObject); } 
        else { Instance = this; }

        // --- BAĞLANTI BURADA YAPILIYOR ---
        // PlayerPrefs veya DLL kullanmıyoruz. 
        // Yazdığımız 'FileRepository' sistemini başlatıyoruz.
        _playerRepository = new FileRepository();
    }

    private void OnDestroy() { if (Instance == this) { Instance = null; } }

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObj in players)
        {
            MovementController playerController = playerObj.GetComponent<MovementController>();
            if (playerController != null)
            {
                playerController.AddObserver(this);
            }
        }
    }

    public void OnNotify(string eventName, object data)
    {
        if (eventName == "PlayerDied")
        {
            CheckWinState();
        }
    }

   public void CheckWinState()
    {
        int aliveCount = 0;
        GameObject lastAlivePlayer = null;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].activeSelf) {
                aliveCount++;
                lastAlivePlayer = players[i];
            }
        }

        if (aliveCount <= 1) 
        {
            
            
            string winnerName = "Bilinmiyor";

            if (lastAlivePlayer != null)
            {
            
                if (lastAlivePlayer.name == "Player 1") 
                {
                    winnerName = SessionManager.Instance.playerName;
                }
                else
                {
                    winnerName = lastAlivePlayer.name; 
                }
            }
            
            Debug.Log("Kazanan: " + winnerName);

            
            if (_playerRepository != null)
            {
                _playerRepository.SaveWin();
            }

            
            if (highScoreText != null)
            {
                highScoreText.text = winnerName + " KAZANDI!";
            }

            Invoke(nameof(GameOver), 2f);
        }
    }
    private void GameOver()
    {
        if (gameOverUI != null)
        {
            
            if (_playerRepository != null && highScoreText != null)
            {
                int score = _playerRepository.GetHighScore();
                highScoreText.text = "TOPLAM ZAFER: " + score;
            }

            gameOverUI.SetActive(true);
        }
        else
        {
            Invoke(nameof(NewRound), 3f);
        }
    }

    private void NewRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}