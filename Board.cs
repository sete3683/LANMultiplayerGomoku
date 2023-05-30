using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    (int r, int c)[] _direction = { (0, 1), (1, 1), (1, 0), (1, -1) };

    public GameObject block;
    public Stone[,] board = new Stone[19, 19];

    void Awake()
    {
        Stone[] stones = GetComponentsInChildren<Stone>();
        int i = 0;

        for (int r = 0; r < 19; r++)
        {
            for (int c = 0; c < 19; c++)
            {
                board[r, c] = stones[i++];
                board[r, c].gameObject.name = $"{r},{c}";
            }
        }
    }

    void Start()
    {
        GameManager.Instance.Board = this;
        GameManager.Instance.StartGame();
    }

    public void Block()
    {
        block.SetActive(true);
    }

    public void Unblock()
    {
        block.SetActive(false);
    }

    public void DestroyBoard()
    {
        foreach (Stone stone in board)
        {
            if (stone.HasStone())
                stone.DestroyStone();
        }
    }

    public bool CheckBoard()
    {
        for (int r = 0; r < 19; r++)
        {
            for (int c = 0; c < 19; c++)
            {
                if (board[r, c].HasStone())
                {
                    for (int dir = 0; dir < 4; dir++)
                    {
                        if (CheckLine((r, c), dir, 1))
                            return true;
                    }
                }
            }
        }

        return false;
    }

    bool CheckLine((int r, int c) pos, int dir, int depth)
    {
        if (depth == 5)
            return true;

        (int r, int c) nextPos = (pos.r + _direction[dir].r, pos.c + _direction[dir].c);

        if (!IsOutOfBoard(nextPos) && board[nextPos.r, nextPos.c].HasStone())
        {
            if ((board[pos.r, pos.c].HasWhite() && board[nextPos.r, nextPos.c].HasWhite()) ||
                (board[pos.r, pos.c].HasBlack() && board[nextPos.r, nextPos.c].HasBlack()))
                return CheckLine(nextPos, dir, depth + 1);
        }

        return false;
    }

    bool IsOutOfBoard((int r, int c) pos)
    {
        return pos.r < 0 || pos.r >= 19 || pos.c < 0 || pos.c >= 19;
    }
}
