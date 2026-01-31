using UnityEngine;

public class MaskRandomizer : MonoBehaviour
{
    public static MaskRandomizer Instance { get; private set; }

    private static bool protectedMaskGenerated = false;
    private static MaskConfig ProtectedMask;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // When level is done/changed
    public void ResetRandomizer()
    {
        protectedMaskGenerated = false;
    }

    public MaskConfig GenerateMask(GameObject target)
    {
        // if protected person
        BaseAI ai = target.GetComponent<BaseAI>();
        if (ai != null && ai.isATarget)
        {
            return GenerateSpecialMask();
        }
        return GenerateNormal();
    }

    private MaskConfig GenerateSpecialMask()
    {
        if (!protectedMaskGenerated)
        {
            ProtectedMask = RandomConfig();
            protectedMaskGenerated = true;
        }
        return ProtectedMask;
    }

    private MaskConfig GenerateNormal()
    {
        MaskConfig config;
        do
        {
            config = RandomConfig();
        }
        while (config.Equals(ProtectedMask));

        return config;
    }

    private MaskConfig RandomConfig()
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