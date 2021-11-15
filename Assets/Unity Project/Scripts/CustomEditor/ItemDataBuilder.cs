using System;
using UnityEditor;
using UnityEngine;

namespace Unity_Project.Scripts.CustomEditor
{
    public class ItemDataBuilder : MonoBehaviour
    {
        const string ITEMDATA_PATH = @"Assets/Unity Project/ScriptableObjects/Items/";
        const string HEALINGWEAPON_PATH = @"Assets/Unity Project/ScriptableObjects/Items/Healing/Healing Weapons/";
        const string HEALINGITEM_PATH = @"Assets/Unity Project/ScriptableObjects/Items/Healing/Healing Items/";
        const string WEAPON_PATH = @"Assets/Unity Project/ScriptableObjects/Items/Weapons/";


        /// <summary>
        /// Called by a custom inspector button, creates ScriptableObjects from a .csv file of Items.
        /// </summary>
        /// <param name="csvString"></param>
        public void BuildItemsFromCSV(string csvString)
        {
            string[] lines = csvString.Split('\n');

            // For each of the item data lines,
            for (int i = 1; i < lines.Length; i++)
            {
                string[] dataValues = lines[i].Split(',');
                var itemSO = GetItemTypedInstance(dataValues);
                CreateItemSOInstance(itemSO, dataValues);
            }
        }

        /// <summary>
        /// Reads from an array of data values for a given item and creates a ScriptableObject for it!
        /// </summary>
        /// <param name="so"></param>
        /// <param name="dataValues"></param>
        private void CreateItemSOInstance(ScriptableObject so, string[] dataValues)
        {
            so.name = dataValues[0];
            if (so is IItem itemData)
            {
                itemData.ItemName = dataValues[0];
                itemData.ID = int.Parse(dataValues[1]);
                itemData.Price = int.Parse(dataValues[2]);
                itemData.UsesLeft = int.Parse(dataValues[3]);
                itemData.UsesTotal = int.Parse(dataValues[4]);

                if (so is IBattleItem battleItemData)
                {
                    battleItemData.BattleItemType = (BattleItemType) Enum.Parse(typeof(BattleItemType), dataValues[6]);
                    battleItemData.Range = int.Parse(dataValues[7]);
                    if (so is IConcreteHealing cbhData)
                    {
                        cbhData.HealingAmount = int.Parse(dataValues[8]);
                        AssetDatabase.CreateAsset(so, HEALINGWEAPON_PATH + so.name + ".asset");
                    }
                    else if (so is IPercentageHealing pbhData)
                    {
                        pbhData.PercentageHealing = float.Parse(dataValues[9]);
                        AssetDatabase.CreateAsset(so, HEALINGWEAPON_PATH + so.name + ".asset");
                    }
                    else if (so is WeaponData weaponData)
                    {
                        weaponData.DamageType = (DamageType)Enum.Parse(typeof(DamageType), dataValues[10]);
                        weaponData.WeaponElement = (WeaponElement)Enum.Parse(typeof(WeaponElement), dataValues[11]);
                        weaponData.BaseDamageAmount = int.Parse(dataValues[12]);
                        weaponData.Accuracy = float.Parse(dataValues[13]);
                        AssetDatabase.CreateAsset(weaponData, WEAPON_PATH + weaponData.name + ".asset");
                    }
                }
                else
                {
                    if (so is ConcreteHealingItemData chiData)
                    {
                        chiData.HealingAmount = int.Parse(dataValues[8]);
                        AssetDatabase.CreateAsset(chiData, HEALINGITEM_PATH + chiData.name + ".asset");
                    }
                    else if (so is PercentageHealingItemData phiData)
                    {
                        phiData.PercentageHealing = float.Parse(dataValues[9]);
                        AssetDatabase.CreateAsset(phiData, HEALINGITEM_PATH + phiData.name + ".asset");
                    }
                    else
                    {
                        // If the IItem isn't an IBattleItem (inventory item),
                        AssetDatabase.CreateAsset(so, ITEMDATA_PATH + so.name + ".asset");
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Used to get a ScriptableObject instance of a type according to the data filled out for an Item.
        /// </summary>
        /// <param name="dataArr">An array of a line of CSV data, split by comma</param>
        /// <returns></returns>
        private ScriptableObject GetItemTypedInstance(string[] dataArr)
        {
            bool hasFilledItemData = true;
            for (int i = 0; i <= 5; i++)
            {
                if (dataArr[i].Equals("") && i != 5)
                {
                    hasFilledItemData = false;
                }
            }

            bool hasBattleItemData = true;
            for (int i = 6; i <= 7; i++)
            {
                if (dataArr[i].Equals(""))
                {
                    hasBattleItemData = false;
                }
            }

            bool hasConcreteHealingData = !dataArr[8].Equals("");
            bool hasPercentageHealingData = !dataArr[9].Equals("");

            bool hasWeaponsData = true;
            for (int i = 10; i <= 13; i++)
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
}
