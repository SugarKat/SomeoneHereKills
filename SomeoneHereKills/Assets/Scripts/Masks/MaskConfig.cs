[System.Serializable]
public class MaskConfig
{
    public MaskBase maskBase;
    public Cheeks cheeks;
    public Horns horns;
    public Eyes eyes;
}
public enum MaskBase { Gold, Silver, Diamond }
public enum Cheeks { Color1, Color2, Color3, None }
public enum Horns { Pointy, Wide, Split, None }
public enum Eyes { Wide, Small, Lined }