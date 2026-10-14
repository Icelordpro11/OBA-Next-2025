using System.Collections.Generic;
using UnityEngine;
using _Scripts;

public class MaxPlaytime_Manager : MonoBehaviour
{
     private float _timePlaying = 0;
     [SerializeField] private float _maxPlaytime = 300; //300 sec == 5 min
    [SerializeField] private GameObject _playtimeOverScreen;

    void Update()
    {
        if (_timePlaying < _maxPlaytime)
        {
            _timePlaying += Time.deltaTime;

            if (_timePlaying >= _maxPlaytime)
            {
                _playtimeOverScreen.SetActive(true);
            }
        }
    }
}
