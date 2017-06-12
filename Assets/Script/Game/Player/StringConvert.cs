#region using

using DataTable;
using UnityEngine;

#endregion

public static class StringConvert
{
    //随机名称
    private static int AdjMax = 9999;
    private static int ManMax = 9999;
    private static int SurMax = 9999;
    private static int WomanMax = 9999;

    private static string GetRandomAdjective()
    {
        return "";
//         while (true)
//         {
//             int rndAdj = UnityEngine.Random.Range(0, AdjMax);
//             var tbRndName = Table.GetRandName(rndAdj);
//             if (tbRndName == null)
//             {
//                 AdjMax = rndAdj;
//             }
//             else
//             {
//                 string adj = tbRndName.Adjective;
//                 if (string.IsNullOrEmpty(adj))
//                 {
//                     AdjMax = rndAdj;
//                 }
//                 else
//                 {
//                     return adj;
//                 }
//             }
//         }
    }

    private static string GetRandomManName()
    {
        while (true)
        {
            var rndMan = Random.Range(0, ManMax);
            var tbRndName = Table.GetRandName(rndMan);
            if (tbRndName == null)
            {
                ManMax = rndMan;
            }
            else
            {
                var Man = tbRndName.Man;
                if (string.IsNullOrEmpty(Man))
                {
                    ManMax = rndMan;
                }
                else
                {
                    return Man;
                }
            }
        }
    }

    public static string GetRandomName(int Sex)
    {
        var xing = GetRandomSurname(Sex);
        var name = "";
        var lenth = 0;
        if (Sex == 0)
        {
            do
            {
                name = GetRandomManName();
                lenth = UIInput.getStringLength(xing) + UIInput.getStringLength(name);
            } while (lenth > 12);

            return string.Format("{0}{1}", xing, name);
        }
        do
        {
            name = GetRandomWomanName();
            lenth = UIInput.getStringLength(xing) + UIInput.getStringLength(name);
        } while (lenth > 12);
        return string.Format("{0}{1}", xing, name);

//         if (Sex == 0)
//         {
//             return string.Format("{0}{1}{2}", GetRandomAdjective(), GetRandomSurname(), GetRandomManName());
//         }
//         return string.Format("{0}{1}{2}", GetRandomAdjective(), GetRandomSurname(), GetRandomWomanName());


        //   UIInput.getStringLength();
    }

    private static string GetRandomSurname(int Sex)
    {
        while (true)
        {
            var rndSur = Random.Range(0, SurMax);
            var tbRndName = Table.GetRandName(rndSur);
            if (tbRndName == null)
            {
                SurMax = rndSur;
            }
            else
            {
                var Sur = string.Empty;
                if (Sex == 0)
                {
                    Sur = tbRndName.BoySurname;
                }
                else
                {
                    Sur = tbRndName.GirlSurname;
                }
                if (string.IsNullOrEmpty(Sur))
                {
                    SurMax = rndSur;
                }
                else
                {
                    return Sur;
                }
            }
        }
    }

    private static string GetRandomWomanName()
    {
        while (true)
        {
            var rndWoman = Random.Range(0, WomanMax);
            var tbRndName = Table.GetRandName(rndWoman);
            if (tbRndName == null)
            {
                WomanMax = rndWoman;
            }
            else
            {
                var Woman = tbRndName.Woman;
                if (string.IsNullOrEmpty(Woman))
                {
                    WomanMax = rndWoman;
                }
                else
                {
                    return Woman;
                }
            }
        }
    }

    //等级修正
    public static int Level_Value_Ref(int nOldValue, int nLevel)
    {
        if (nOldValue < 10000000)
        {
            return nOldValue;
        }
        var tbUpgrade = Table.GetSkillUpgrading(nOldValue%10000000);
        if (tbUpgrade == null)
        {
            return nOldValue;
        }
        switch (tbUpgrade.Type)
        {
            case 0: //枚举
            {
                if (nLevel > tbUpgrade.Values.Count || nLevel < 0)
                {
                    Logger.Fatal("ModifyByLevel=[{0}]  Level=[{1}]  is Out", nOldValue, nLevel);
                    return tbUpgrade.Values[tbUpgrade.Values.Count - 1];
                }
                var result = tbUpgrade.Values[nLevel];
                return result;
            }
            case 1:
            {
                var result = tbUpgrade.Param[0] + tbUpgrade.Param[1]*nLevel;
                return result;
            }
        }
        Logger.Fatal("ModifyByLevel=[{0}]  Level=[{1}]  is not find Type", nOldValue, nLevel);
        return nOldValue;
    }
}