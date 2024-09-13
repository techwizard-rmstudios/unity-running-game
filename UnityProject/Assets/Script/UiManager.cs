using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Canvas _gameOverScreen;
    private int _score;

    public static UiManager Instance { get; private set; }

    private void Awake()
    {
        UiManager.Instance = this;
        this._score = 0;
        this._scoreText.text = string.Format("Score: {0}", (object)this._score);
        this._gameOverScreen.enabled = false;
    }

    public void UpdateScore(int value)
    {
        this._score += value;
        this._scoreText.text = string.Format("Score: {0}", (object)this._score);
    }

    public void ShowGameOverScreen() => this._gameOverScreen.enabled = true;
}