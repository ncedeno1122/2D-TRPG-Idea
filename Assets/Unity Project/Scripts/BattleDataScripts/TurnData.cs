using Unity_Project.Scripts.BattleDataScripts;

namespace Unity_Project.Scripts.BattleDataScripts
{
    public class TurnData
    {
        public int TurnNumber;
        public PhaseData PlayerPhaseData;
        public PhaseData EnemyPhaseData;
        public PhaseData AllyPhaseData;

        public TurnData(int turnNum)
        {
            TurnNumber = turnNum;
            PlayerPhaseData = new PhaseData(Allegiance.PLAYER);
            EnemyPhaseData = new PhaseData(Allegiance.ENEMY);
            AllyPhaseData = new PhaseData(Allegiance.ALLY);
        }
    }
}
