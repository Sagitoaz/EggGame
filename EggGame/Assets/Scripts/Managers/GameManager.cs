using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Board _board;
    [SerializeField] private InputController _inputController;
    [SerializeField] private GameLogicController _gameLogicController;
    [SerializeField] private MergeController _mergeController;
    private void Start()
    {
        _gameLogicController.SetBoard(_board);
    } 
}
