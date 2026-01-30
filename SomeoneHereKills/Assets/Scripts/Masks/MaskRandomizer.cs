using UnityEngine;

public static class MaskRandomizer
{
    public static MaskConfig Generate()
    {
        MaskConfig config = new MaskConfig
        {
            maskBase = RandomEnum<MaskBase>(),
            cheeks = RandomEnum<Cheeks>(),
            horns = RandomEnum<Horns>(),
            eyes = RandomEnum<Eyes>()
        };

        return config;
    }

    public static T RandomEnum<T>() where T : System.Enum
    {
        var values = System.Enum.GetValues(typeof(T));
        int index = Random.Range(0, values.Length);
        return (T)values.GetValue(index);
    }

}