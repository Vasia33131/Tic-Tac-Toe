using System.Collections.Generic;
using UnityEngine;

public class EasyAI : AIPlayer
{
    public override void MakeMove(string[] board)
    {
        List<int> emptyCells = GetEmptyCells(board);
        if (emptyCells.Count > 0)
        {
            int move = emptyCells[Random.Range(0, emptyCells.Count)];
            gameController.ProcessAIMove(move);
        }
    }
}