using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    public string playerName = "Player 1"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}