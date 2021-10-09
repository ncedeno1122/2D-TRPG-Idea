using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemDataBuilder : MonoBehaviour
{
    private const int rumRows = 8;

    public void BuildItemsFromCSV(string csvString)
    {
        string[] lines = csvString.Split('\n');
        StringBuilder sb = new StringBuilder();

        // For each of the item data lines,
        for (int i = 1; i < lines.Length - 1; i++)
        {
            string[] dataValues = lines[i].Split(',');
            for (int j = 0; j < dataValues.Length - 1; j++)
            {
                switch(j)
                {
                    // ItemData
                    case 0: // Name
                        sb.Append($"Name: {dataValues[j]}, ");
                        break;
                    case 1: // ID
                        sb.Append($"ID: {dataValues[j]}, ");
                        break;
                    case 2: // Price
                        sb.Append($"Price: {dataValues[j]}, ");
                        break;
                    case 3: // UsesLeft
                        sb.Append($"UsesLeft: {dataValues[j]}, ");
                        break;
                    case 4: // UsesTotal
                        sb.Append($"UsesTotal: {dataValues[j]}, ");
                        break;
                    case 5: // Icon
                        sb.Append($"Icon: {dataValues[j]}, ");
                        break;
                    // BattleItem
                    case 6: // BattleItemType
                        sb.Append($"BattleItemType: {dataValues[j]}, ");
                        break;
                    case 7: // Range
                        sb.Append($"Range: {dataValues[j]}, ");
                        break;
                    // ConcreteHealingItem
                    case 8: // ConcreteHealing Amount
                        sb.Append($"ConcreteHealing: {dataValues[j]}, ");
                        break;
                    case 9: // PercentageHealing Amount
                        sb.Append($"PercentageHealing: {dataValues[j]}, ");
                        break;
                    // Weapon
                    case 10: // DamageType
                        sb.Append($"DamageType: {dataValues[j]}, ");
                        break;
                    case 11: // WeaponElement
                        sb.Append($"WeaponElement: {dataValues[j]}, ");
                        break;
                    case 12: // BaseDamageAmount
                        sb.Append($"BaseDamageAmount: {dataValues[j]}, ");
                        break;
                    case 13: // Accuracy
                        sb.Append($"Accuracy: {dataValues[j]}, ");
                        break;
                }
            }

            Debug.Log(sb.ToString());
            sb.Clear();
        }
    }

    /// <summary>
    /// Used to get a ScriptableObject instance of a type according to the data filled out.
    /// </summary>
    /// <param name="dataArr">An array of a line of CSV data, split by comma</param>
    /// <returns></returns>
    private ScriptableObject GetTypedInstance(string[] dataArr)
    {
        bool hasFilledItemData = true;
        for (int i = 0; i <= 5; i++)
        {
            if (dataArr[i].Equals(""))
            {
                hasFilledItemData = false;
            }
        }

        bool hasBattleItemData = true;
        for (int i = 6; i <= 8; i++)
        {
            if (dataArr[i].Equals(""))
            {
                hasBattleItemData = false;
            }
        }

        bool hasConcreteHealingData = !dataArr[9].Equals("");
        bool hasPercentageHealingData = !dataArr[10].Equals("");

        bool hasWeaponsData = true;
        for (int i = 11; i <= 14; i++)
        {
            if (dataArr[i].Equals(""))
            {
                hasWeaponsData = false;
            }
        }

        // Now, to determine the type
        if (hasFilledItemData)
        {
            if (hasBattleItemData)
            {
                if (hasConcreteHealingData)
                {
                    return ScriptableObject.CreateInstance<ConcreteBattleHealingData>();
                }
                else if (hasPercentageHealingData)
                {
                    return ScriptableObject.CreateInstance<PercentageBattleHealingData>();
                }
                else if (hasWeaponsData)
                {
                    return ScriptableObject.CreateInstance<WeaponData>();
                }
            }
            else if (hasConcreteHealingData)
            {
                return ScriptableObject.CreateInstance<ConcreteHealingItemData>();
            }
            else if (hasPercentageHealingData)
            {
                return ScriptableObject.CreateInstance<PercentageHealingItemData>();
            }
            else
            {
                return ScriptableObject.CreateInstance<ItemData>();
            }
        }
        // If it has incomplete data, return NOTHING.
        return null;
    }
}
