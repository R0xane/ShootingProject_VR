using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class UIManager : MonoBehaviour
{
    private GamePlayManager gamePlayManager;
    public TextMeshProUGUI scoreText;

    private float refreshInterval = 0.2f;
    private Coroutine scoreRoutine;
    
    void Start()
    {
        gamePlayManager = GameObject.Find("GameManager").GetComponent<GamePlayManager>();
        scoreRoutine = StartCoroutine(RefreshScoreRoutine());
    }

    private IEnumerator RefreshScoreRoutine()
    {
        while (true)
        {
            if (gamePlayManager != null && scoreText != null)
            {
                scoreText.text = "Score : " + gamePlayManager.GetScore().ToString();
            }

            yield return new WaitForSeconds(refreshInterval);
        }
    }

    private void OnDisable()
    {
        if (scoreRoutine != null) StopCoroutine(scoreRoutine);
    }
}
