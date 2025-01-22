namespace GameEvents
{
    // 퀘스트 관련 이벤트
    public class QuestUpdatedEvent
    {
        public Quest Quest { get; }
        public QuestUpdatedEvent(Quest quest) => Quest = quest;
    }

    public class QuestCompletedEvent
    {
        public Quest Quest { get; }
        public QuestCompletedEvent(Quest quest) => Quest = quest;
    }
    
    // 업적 관련 이벤트
    public class AchievementsUpdatedEvent
    {
        public Achievement Achievement { get; }
        public AchievementsUpdatedEvent(Achievement achievement) => Achievement = achievement;
    }

    // 게임 일반 이벤트

    // 로그인 이벤트
    public class LoginEvent { }

    // 머리카락 뽑기 이벤트
    public class HairCutEvent
    {
        public int Amount { get; }
        public HairCutEvent(int amount) => Amount = amount;
    }

    // 광고 시청 이벤트
    public class AdWatchedEvent
    {
        public int Amount { get; }
        public AdWatchedEvent(int amount) => Amount = amount;
    }

    // 피버 발동 이벤트
    public class FeverActivatedEvent
    {
        public int Amount { get; }
        public FeverActivatedEvent(int amount) => Amount = amount;
    }

    // 컬렉션 열림 이벤트
    public class CollectionOpenedEvent { }

    // 특별한 문어 조우 이벤트
    public class SpecialOctopusEncounterEvent
    {
        public int EncounterCount { get; }
        public SpecialOctopusEncounterEvent(int count) => EncounterCount = count;
    }

    // 가발 해금 이벤트
    public class WigUnlockedEvent
    {
        public int WigCount { get; }

        public WigUnlockedEvent()
        {
            WigCount = CollectionManager.Instance.currentWigCount;
        }
    }

    // 돈 획득 이벤트
    public class MoneyGetEvent
    {
        public long Amount { get; }
        
        public MoneyGetEvent(long amount) => Amount = amount;
    }

    // 강화 이벤트
    public class EnhancementsEvent
    {
        public int EnhancementCount { get; }

        public EnhancementsEvent()
        {
            EnhancementCount = EnhancementManager.Instance.completedLine1Enhancements.Count + EnhancementManager.Instance.completedLine2Enhancements.Count;
        }
    }
}
