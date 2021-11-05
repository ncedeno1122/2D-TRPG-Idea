using System.Collections.Generic;
using UnityEngine;

namespace Unity_Project.Scripts.BattleDataScripts
{
    public class BattleDataManager : MonoBehaviour
    {
        private int m_CurrentTurn = 0;
    
        [SerializeField]
        private List<TurnData> m_TurnData;

        private void Start()
        {
            m_TurnData = new List<TurnData>();
        }
    
        // + + + + | Functions | + + + + 

        /// <summary>
        /// Sets a given TAC where it belongs in the current turn's TurnData
        /// </summary>
        /// <param name="cmd"></param>
        public void AddTurnActionCommand(TurnActionCommand cmd)
        {
            // Get or Create TurnData
            var td = m_TurnData[m_CurrentTurn - 1]; // TODO: Might this throw an out-of-bounds?
            if (td == null)
            {
                td = new TurnData(m_CurrentTurn);
                m_TurnData.Insert(m_CurrentTurn - 1, td);
            }
        
            // Add TAC to allegiance-specific PhaseData
            switch (cmd.User.UnitData.Allegiance)
            {
                case Allegiance.PLAYER:
                    td.PlayerPhaseData.SetTurn(cmd);
                    break;
                case Allegiance.ENEMY:
                    td.EnemyPhaseData.SetTurn(cmd);
                    break;
                case Allegiance.ALLY:
                    td.AllyPhaseData.SetTurn(cmd);
                    break;
            }
        }
    }
}
