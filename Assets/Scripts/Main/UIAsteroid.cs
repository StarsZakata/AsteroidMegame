using UnityEngine;
using UnityEngine.UI;

public class UIAsteroid : MonoBehaviour
{
    [Space(10)]
    // MainPanels
    public GameObject MainPanel;
    public GameObject MainPlayerScorePanel;
    public GameObject MainPlayerSetting;

    [Space(10)]
    // UI Panel
    public GameObject UIPause;
    public GameObject UIPanel;
    // PlayerData
    public Text PlayerLifeText;
    public Text PlayerScoreText;

    [Space(10)]
    // Buttons
    public Button PauseButton;
    public Button NewGameButton;
    public Button ContinueButton;
    public Button SettingButton;
    public Button ChangeControllerPlayer;

    [Space(10)]
    // MainPlayerData
    public Text MainPlayerLifeText;
    public Text MainPlayerScoreText;
    public Text MainPlayerSettingController;

    // True - keyboard
    // False - keyboard + mouse
    private bool _playerController = false;
    private bool _isSettingPanel = false;
    private bool _newGameInit = false;

    private Asteroid _asteroid;

    private string _playerLife = "PlayerLife: ";
    private string _playerScore = "PlayerScore: ";

    private void Awake()
    {
        InitPanels();
        InitEventsButtons();
    }

    private void InitPanels()
    {
        MainPanel.SetActive(false);
        MainPlayerScorePanel.SetActive(false);
        MainPlayerSetting.SetActive(false);
        UIPause.SetActive(false);
        UIPanel.SetActive(false);
    }

    private void InitEventsButtons()
    {
        PauseButton.onClick.AddListener(OpenMainPanel);
        NewGameButton.onClick.AddListener(NewGameInit);
        ContinueButton.onClick.AddListener(ContinueGame);
        SettingButton.onClick.AddListener(OpenSettingButton);
        ChangeControllerPlayer.onClick.AddListener(ChangePlayerController);
    }

    public void Init(Asteroid asteroid)
    {
        _asteroid = asteroid;
        gameObject.SetActive(true);
        MainPanel.SetActive(true);
        MainPlayerScorePanel.SetActive(true);
        ContinueButton.gameObject.SetActive(false);
        _isSettingPanel = false;
        _newGameInit = false;
    }

    public void ChangeGameData(int life, int score)
    {
        PlayerLifeText.text = _playerLife + life.ToString();
        PlayerScoreText.text = _playerScore + score.ToString();
        MainPlayerLifeText.text = _playerLife + life.ToString();
        MainPlayerScoreText.text = _playerScore + score.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OpenMainPanel();
    }

    private void NewGameInit()
    {
        _asteroid.StartNewGame();
        MainPanel.SetActive(false);
        UIPause.SetActive(true);
        UIPanel.SetActive(true);
        _newGameInit = true;
    }

    private void ContinueGame()
    {
        _asteroid.ContinueThisGame();
        MainPanel.SetActive(false);
        UIPause.SetActive(true);
        UIPanel.SetActive(true);
    }

    private void OpenMainPanel()
    {
        _asteroid.PauseGame();
        MainPanel.SetActive(true);
        UIPause.SetActive(false);
        UIPanel.SetActive(false);
        if (_newGameInit)
            ContinueButton.gameObject.SetActive(true);
    }

    private void OpenSettingButton()
    {
        if (!_isSettingPanel)
        {
            MainPlayerScorePanel.SetActive(false);
            MainPlayerSetting.SetActive(true);
            _isSettingPanel = true;
        }
        else
        {
            MainPlayerScorePanel.SetActive(true);
            MainPlayerSetting.SetActive(false);
            _isSettingPanel = false;
        }
    }

    private void ChangePlayerController()
    {
        if (_playerController)
            MainPlayerSettingController.text = Asteroid.PlayerKeyboard;
        else
            MainPlayerSettingController.text = Asteroid.PlayerKeyboardMouse;
        _playerController = !_playerController;
    }

    public int GetController()
    {
        if (_playerController)
            return 2;
        else
            return 1;
    }

}
