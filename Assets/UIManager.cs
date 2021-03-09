using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Current.onScoreUpdated += OnScoreUpdated;
    }

    private void OnDestroy()
    {
        EventManager.Current.onScoreUpdated -= OnScoreUpdated;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnScoreUpdated()
    {
        scoreText.text = "Score: " + GameManager.Score;
    }
}
