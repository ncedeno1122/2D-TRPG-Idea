using System.Collections.Generic;

namespace Unity_Project.Scripts.BattleDataScripts
{
    public class PhaseData
    {
        private Allegiance m_PhaseAllegiance; // Could be redundant to type with field and this as well.
        private readonly List<TurnActionCommand> m_TurnActions;

        public PhaseData(Allegiance allegiance)
        {
            m_PhaseAllegiance = allegiance;
            m_TurnActions = new List<TurnActionCommand>();
        }

        public void AddTurn(TurnActionCommand cmd)
        {
            m_TurnActions.Add(cmd);
        }

        public void RemoveTurn(TurnActionCommand cmd)
        {
            m_TurnActions.Remove(cmd);
        }

        /// <summary>
        /// Searches the m_TurnActions list for an existing TAC from a specified Unit
        /// </summary>
        /// <returns></returns>
        public TurnActionCommand GetTurn(TurnActionCommand cmd)
        {
            var existingTurn = m_TurnActions.Find(existingCmd=> existingCmd.Equals(cmd));
            return existingTurn;
        }

        /// <summary>
        /// Tries to find and destroy an existing TAC from the same User, and adds the specified TAC.
        /// </summary>
        /// <param name="cmd"></param>
        public void SetTurn(TurnActionCommand cmd)
        {
            // Remove the old Command
            var existingTurn = m_TurnActions.Find(existingCmd=> existingCmd.User == cmd.User);
            if (existingTurn != null)
            {
                RemoveTurn(existingTurn);
            }
            
            // Then add the new Command
            AddTurn(cmd);
        }

        /// <summary>
        /// Ensures that all TACs in m_TurnActions are valid.
        /// </summary>
        /// <returns></returns>
        private bool ValidateAllTurns()
        {
            foreach (var cmd in m_TurnActions)
            {
                if (!cmd.IsActionValid()) return false;
            }

            return true;
        }
    }
}
