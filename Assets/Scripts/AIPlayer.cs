using UnityEngine;
using System.Collections.Generic;

// Базовый класс для всех ИИ-игроков
public abstract class AIPlayer : MonoBehaviour
{
    // Публичные свойства только для чтения
    public string AISymbol { get; private set; }  // Символ, которым играет ИИ (X/O)
    public string PlayerSymbol { get; private set; } // Символ противника
    protected TicTacToe gameController;  // Ссылка на контроллер игры

    // Метод инициализации ИИ
    public void Init(string symbol, TicTacToe controller)
    {
        AISymbol = symbol;
        PlayerSymbol = (symbol == "X") ? "O" : "X"; // Автоматически определяем символ противника
        gameController = controller;

        Debug.Log($"AI initialized as {AISymbol} (vs {PlayerSymbol})");
    }

    // Абстрактный метод для реализации в конкретных ИИ
    public abstract void MakeMove(string[] board);

    // Получение списка пустых клеток
    protected List<int> GetEmptyCells(string[] board)
    {
        List<int> emptyCells = new List<int>();
        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                emptyCells.Add(i);
            }
        }
        return emptyCells;
    }

    // Поиск выигрышного хода для указанного символа
    protected int? FindWinningMove(string[] board, string symbol)
    {
        // Все возможные выигрышные комбинации
        int[,] winCombinations = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Горизонтали
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Вертикали
            {0, 4, 8}, {2, 4, 6}             // Диагонали
        };

        for (int i = 0; i < 8; i++)
        {
            int a = winCombinations[i, 0];
            int b = winCombinations[i, 1];
            int c = winCombinations[i, 2];

            // Проверяем все возможные выигрышные комбинации
            if (board[a] == symbol && board[b] == symbol && string.IsNullOrEmpty(board[c]))
                return c;
            if (board[a] == symbol && board[c] == symbol && string.IsNullOrEmpty(board[b]))
                return b;
            if (board[b] == symbol && board[c] == symbol && string.IsNullOrEmpty(board[a]))
                return a;
        }

        return null; // Выигрышный ход не найден
    }
}