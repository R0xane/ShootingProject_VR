using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
   public int currentScore;

   [Header("Target Settings")]
   public int maxTargets = 6;
   public int currentTargets;

    public void RegisterTargetSpawned()
    {
        currentTargets++;
    }

    public void AddScore(int Amount)
    {
        currentScore += Amount;
    }

    public int GetScore(){
        return currentScore;
    }

    public void TargetDespawned()
    {
        currentTargets = Mathf.Max(0, currentTargets - 1);
    }
}
