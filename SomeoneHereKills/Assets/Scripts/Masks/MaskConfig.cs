[System.Serializable]
public class MaskConfig
{
    public MaskBase maskBase;
    public Cheeks cheeks;
    public Horns horns;
    public Eyes eyes;
    public override bool Equals(object obj)
    {
        if (!(obj is MaskConfig other)) return false;

        return maskBase == other.maskBase &&
               cheeks == other.cheeks &&
               horns == other.horns &&
               eyes == other.eyes;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + maskBase.GetHashCode();
        hash = hash * 23 + cheeks.GetHashCode();
        hash = hash * 23 + horns.GetHashCode();
        hash = hash * 23 + eyes.GetHashCode();
        return hash;
    }
}
public enum MaskBase { Gold, Silver, Diamond }
public enum Cheeks { Green, Mint, Purple, None }
public enum Horns { Cake, Cat, Curvy, None }
public enum Eyes { Cat, Circled, Scary }

