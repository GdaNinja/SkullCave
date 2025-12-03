using UnityEngine;

public class ConfigModel : MonoBehaviour
{
    public Quality quality;
    public Resolution resolution;
    public float sensibility;
    public float music;
    public float soundEffects;
}
public enum Quality
{
    Low,
    Medium,
    High,
    Ultra
}
public class Resolution
{
    public int width;
    public int height;
}
public class Sensibility
{
    
}
public class Music
{
    
}
public class soundEffects
{
    
}

