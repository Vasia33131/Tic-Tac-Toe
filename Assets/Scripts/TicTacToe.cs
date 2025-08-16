using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TicTacToe : MonoBehaviour
{
    [Header("Game Elements")]
    [SerializeField] private Button[] buttons = new Button[9];
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text[] buttonTexts = new TMP_Text[9];
    [SerializeField] private TMP_Text timerText;

    [Header("Animation Settings")]
    [SerializeField] private float symbolAppearDuration = 0.5f;
    [SerializeField] private float lineDrawDuration = 1f;
    [SerializeField] private AnimationCurve scaleCurve;

    [Header("Game Settings")]
    [SerializeField] private Color xColor = Color.red;
    [SerializeField] private Color oColor = Color.blue;
    [SerializeField] private Color winLineColor = Color.green;
    [SerializeField] private float winLineWidth = 10f;
    [SerializeField] private AIDifficulty difficulty = AIDifficulty.Medium;
    [SerializeField] private bool playAgainstAI = true;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip xSound;
    [SerializeField] private AudioClip oSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip drawSound;

    public enum AIDifficulty { Easy, Medium, Hard, Expert }

    private string currentPlayer;
    private string[] board = new string[9];
    private bool gameOver;
    private bool isProcessingMove;
    private LineRenderer winLine;
    private int[] winCombination;
    private float gameTime;
    private bool isTimerRunning;
    private AIPlayer aiPlayer;

    void Start()
    {
        ValidateReferences();
        CreateWinLineRenderer();
        InitializeGame();
    }

    private void ValidateReferences()
    {
        if (buttonTexts.Length != 9 || buttons.Length != 9)
        {
            Debug.LogError("Need exactly 9 buttons and 9 text elements!");
            enabled = false;
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            gameTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void CreateWinLineRenderer()
    {
        GameObject lineObj = new GameObject("WinLine");
        winLine = lineObj.AddComponent<LineRenderer>();

        winLine.material = new Material(Shader.Find("Sprites/Default"));
        winLine.startColor = winLineColor;
        winLine.endColor = winLineColor;
        winLine.startWidth = winLineWidth;
        winLine.endWidth = winLineWidth;
        winLine.positionCount = 2;
        winLine.useWorldSpace = true;
        winLine.sortingLayerName = "UI";
        winLine.sortingOrder = 10;

        lineObj.transform.SetParent(transform);
        winLine.enabled = false;
    }

    private void InitializeGame()
    {
        currentPlayer = "X";
        board = new string[9];
        gameOver = false;
        isProcessingMove = false;
        winCombination = null;
        gameTime = 0f;
        isTimerRunning = true;

        if (winLine != null)
            winLine.enabled = false;

        for (int i = 0; i < 9; i++)
        {
            board[i] = "";
            buttonTexts[i].text = "";
            buttonTexts[i].color = currentPlayer == "X" ? xColor : oColor;
            buttonTexts[i].transform.localScale = Vector3.zero;
            buttons[i].interactable = true;
        }

        // Удаляем старый ИИ, если есть
        AIPlayer[] existingAIs = GetComponents<AIPlayer>();
        foreach (AIPlayer ai in existingAIs)
        {
            Destroy(ai);
        }

        // Инициализируем новый ИИ
        if (playAgainstAI)
        {
            InitializeAI();
        }

        UpdateStatusText();
    }

    private void InitializeAI()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                aiPlayer = gameObject.AddComponent<EasyAI>();
                break;
            case AIDifficulty.Medium:
                aiPlayer = gameObject.AddComponent<MediumAI>();
                break;
            case AIDifficulty.Hard:
                aiPlayer = gameObject.AddComponent<HardAI>();
                break;
            case AIDifficulty.Expert:
                aiPlayer = gameObject.AddComponent<ExpertAI>();
                break;
        }

        aiPlayer.Init(currentPlayer == "X" ? "O" : "X", this);
    }

    public void OnButtonClick(int index)
    {
        if (gameOver || !string.IsNullOrEmpty(board[index]) || isProcessingMove)
            return;

        StartCoroutine(ProcessMove(index));
    }

    private IEnumerator ProcessMove(int index)
    {
        isProcessingMove = true;
        SetAllButtonsInteractable(false);

        yield return StartCoroutine(PlayMoveAnimation(index));

        if (!gameOver)
        {
            // Если игра против ИИ и сейчас его ход
            if (playAgainstAI && aiPlayer != null && currentPlayer == aiPlayer.AISymbol)
            {
                yield return new WaitForSeconds(0.5f); // Небольшая задержка перед ходом ИИ
                aiPlayer.MakeMove(board);
            }
            else
            {
                SetAllButtonsInteractable(true);
            }
        }

        isProcessingMove = false;
    }

    // Добавим этот метод в класс TicTacToe
    public void ProcessAIMove(int index)
    {
        if (!gameOver && string.IsNullOrEmpty(board[index]))
        {
            StartCoroutine(ProcessMove(index));
        }
    }

    private IEnumerator PlayMoveAnimation(int index)
    {
        buttons[index].interactable = false;
        board[index] = currentPlayer;
        buttonTexts[index].text = currentPlayer;
        buttonTexts[index].color = currentPlayer == "X" ? xColor : oColor;

        // Play sound
        AudioManager.Instance.PlaySound(currentPlayer == "X" ? xSound : oSound);

        // Animate symbol appearance
        float elapsedTime = 0f;
        while (elapsedTime < symbolAppearDuration)
        {
            float progress = elapsedTime / symbolAppearDuration;
            float scale = scaleCurve.Evaluate(progress);
            buttonTexts[index].transform.localScale = Vector3.one * scale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        buttonTexts[index].transform.localScale = Vector3.one;

        if (CheckWin())
        {
            gameOver = true;
            isTimerRunning = false;
            statusText.text = $"{currentPlayer} Победа! Время: {FormatTime(gameTime)}";
            AudioManager.Instance.PlaySound(winSound);
            yield return StartCoroutine(DrawWinLine());
            yield break;
        }

        if (CheckDraw())
        {
            gameOver = true;
            isTimerRunning = false;
            statusText.text = $"Ничья! Время: {FormatTime(gameTime)}";
            AudioManager.Instance.PlaySound(drawSound);
            yield break;
        }

        currentPlayer = currentPlayer == "X" ? "O" : "X";
        UpdateStatusText();
    }

    private IEnumerator DrawWinLine()
    {
        if (winCombination == null || winCombination.Length != 3) yield break;

        Vector3 startPos = buttons[winCombination[0]].transform.position;
        Vector3 endPos = buttons[winCombination[2]].transform.position;

        winLine.SetPosition(0, startPos);
        winLine.SetPosition(1, startPos);
        winLine.enabled = true;

        float elapsedTime = 0f;
        while (elapsedTime < lineDrawDuration)
        {
            float progress = elapsedTime / lineDrawDuration;
            Vector3 currentEndPos = Vector3.Lerp(startPos, endPos, progress);
            winLine.SetPosition(1, currentEndPos);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        winLine.SetPosition(1, endPos);
    }

    private bool CheckWin()
    {
        int[,] winCombinations = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Rows
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Columns
            {0, 4, 8}, {2, 4, 6}             // Diagonals
        };

        for (int i = 0; i < 8; i++)
        {
            int a = winCombinations[i, 0];
            int b = winCombinations[i, 1];
            int c = winCombinations[i, 2];

            if (!string.IsNullOrEmpty(board[a]) &&
                board[a] == board[b] &&
                board[b] == board[c])
            {
                winCombination = new[] { a, b, c };
                return true;
            }
        }

        return false;
    }

    private bool CheckDraw()
    {
        foreach (string cell in board)
        {
            if (string.IsNullOrEmpty(cell))
                return false;
        }
        return true;
    }

    private void UpdateStatusText()
    {
        Color playerColor = currentPlayer == "X" ? xColor : oColor;
        statusText.text = $"Игрок <color=#{ColorUtility.ToHtmlStringRGB(playerColor)}>{currentPlayer}</color> Очередь";
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = FormatTime(gameTime);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    private void SetAllButtonsInteractable(bool state)
    {
        foreach (Button button in buttons)
        {
            button.interactable = state;
        }
    }

    public void RestartGame()
    {
        StartCoroutine(ResetGameAnimation());
    }

    private IEnumerator ResetGameAnimation()
    {
        if (winLine.enabled)
        {
            Vector3 startPos = winLine.GetPosition(0);
            Vector3 endPos = winLine.GetPosition(1);

            float elapsedTime = 0f;
            while (elapsedTime < symbolAppearDuration / 2)
            {
                float progress = elapsedTime / (symbolAppearDuration / 2);
                Vector3 currentEndPos = Vector3.Lerp(endPos, startPos, progress);
                winLine.SetPosition(1, currentEndPos);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            winLine.enabled = false;
        }

        float symbolElapsedTime = 0f;
        while (symbolElapsedTime < symbolAppearDuration)
        {
            float progress = symbolElapsedTime / symbolAppearDuration;
            float scale = 1f - scaleCurve.Evaluate(progress);

            for (int i = 0; i < 9; i++)
            {
                if (!string.IsNullOrEmpty(board[i]))
                {
                    buttonTexts[i].transform.localScale = Vector3.one * scale;
                }
            }

            symbolElapsedTime += Time.deltaTime;
            yield return null;
        }

        InitializeGame();
    }
}