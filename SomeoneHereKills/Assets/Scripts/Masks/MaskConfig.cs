[System.Serializable]
public class MaskConfig
{
    public MaskBase maskBase;
    public Cheeks cheeks;
    public Horns horns;
    public Eyes eyes;
}
public enum MaskBase { Gold, Silver, Diamond }
public enum Cheeks { Green, Mint, Purple, None }
public enum Horns { Cake, Cat, Curvy, None }
public enum Eyes { Cat, Circled, Scary }