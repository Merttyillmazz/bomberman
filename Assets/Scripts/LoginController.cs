using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    [Header("SAYFALAR (Paneller)")]
    public GameObject mainPanel;      
    public GameObject loginPanel;    
    public GameObject registerPanel; 

    [Header("GİRİŞ KUTULARI")]
    public InputField loginUserName;
    public InputField loginPassword;

    [Header("KAYIT KUTULARI")]
    public InputField registerUserName;
    public InputField registerPassword;

    [Header("DİĞER")]
    public Text feedbackText;

    private FileRepository _repo;

    private void Start()
    {
        _repo = new FileRepository();
        ShowMainPanel();
    }

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        feedbackText.text = "";
    }

    public void ShowLoginPanel()
    {
        mainPanel.SetActive(false);
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        feedbackText.text = "";
    }

    public void ShowRegisterPanel()
    {
        mainPanel.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        feedbackText.text = "";
    }

       public void PerformLogin()
    {
        string u = loginUserName.text;
        string p = loginPassword.text;

        if (_repo.LoginUser(u, p))
        {
            ShowMessage("Giriş Başarılı!", Color.green);
            SessionManager.Instance.playerName = u;
            Invoke("OpenGame", 1f);
        }
        else
        {
            ShowMessage("Hatalı İsim veya Şifre!", Color.red);
        }
    }

    public void PerformRegister()
    {
        string u = registerUserName.text;
        string p = registerPassword.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            ShowMessage("Alanları doldurun!", Color.yellow);
            return;
        }

        if (_repo.RegisterUser(u, p))
        {
            ShowMessage("Kayıt Oldun! Şimdi Giriş Yap.", Color.green);
            Invoke("ShowLoginPanel", 1.5f);
        }
        else
        {
            ShowMessage("Bu isim zaten alınmış.", Color.red);
        }
    }

    void OpenGame() => SceneManager.LoadScene("bomberman");

    void ShowMessage(string msg, Color col)
    {
        feedbackText.text = msg;
        feedbackText.color = col;
    }
}