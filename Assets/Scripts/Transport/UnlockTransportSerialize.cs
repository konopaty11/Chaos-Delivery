using System.Collections.Generic;
using NUnit.Framework;
using YG;

public static class UnlockTransportSerialize
{
    public static void SaveDict(Dictionary<TransportType, bool> unlockTypes)
    {
        List<TransportType> types = new();
        List<bool> unlock = new();

        foreach (var (k, v) in unlockTypes)
        {
            types.Add(k);
            unlock.Add(v);
        }

        YG2.saves.types = types;
        YG2.saves.unlock = unlock;
        YG2.SaveProgress();
    }

    public static Dictionary<TransportType, bool> GetDict()
    {
        Dictionary<TransportType, bool> dict = new();

        if (YG2.saves.types != null && YG2.saves.unlock != null)
            for (int i = 0; i < YG2.saves.types.Count; i++)
            {
                dict.Add(YG2.saves.types[i], YG2.saves.unlock[i]);
            }

        return dict;
    }
}
