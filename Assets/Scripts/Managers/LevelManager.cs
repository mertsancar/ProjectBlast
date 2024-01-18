using Managers;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelInfo CurrentLevelInfo;
        private Level level;
        private int totalLevelCount = 3;
        
        private void Start()
        {
            totalLevelCount = GetTotalLevelCount();
            
            EventManager.Instance.AddListener(EventNames.PrepareGame, GenerateLevel);
            EventManager.Instance.AddListener(EventNames.ResetLevel, ResetLevel);
        }

        private void GenerateLevel()
        {
            var currentLevelIndex = PersistenceManager.GetCurrentLevelIndex();
            CurrentLevelInfo = GetLevelInfoByIndex(currentLevelIndex);
            var levelPrefab = CurrentLevelInfo.levelPrefab;
            
            level = Instantiate(levelPrefab, transform).GetComponent<Level>();
            
            EventManager.Instance.TriggerEvent(EventNames.UpdateMoveCount, CurrentLevelInfo.moveCount);
            EventManager.Instance.TriggerEvent(EventNames.SetCamera);
            EventManager.Instance.TriggerEvent(EventNames.UpdateTargetCards);
        }

        private LevelInfo GetLevelInfoByIndex(int index)
        {
            return Resources.Load<LevelData>("Levels/Levels").levels.Find(level => level.levelIndex == index%totalLevelCount);
        }
        
        private int GetTotalLevelCount()
        {
            return Resources.Load<LevelData>("Levels/Levels").levels.Count;
        }

        private void ResetLevel()
        {
            Destroy(level.gameObject);
            
            EventManager.Instance.TriggerEvent(EventNames.PrepareGame);
        }
    }
    
}