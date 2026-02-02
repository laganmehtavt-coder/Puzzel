using UnityEngine;
using static GameEnums;

public static class Utils
{
    public static Platform GetPlatform()
    {
        if(Application.platform == RuntimePlatform.Android) return Platform.Android;
        else if(Application.platform == RuntimePlatform.IPhonePlayer) return Platform.Ios;
        else if(Application.platform == RuntimePlatform.WindowsEditor) return Platform.Windows;
        else if (Application.platform == RuntimePlatform.OSXPlayer) return Platform.MacOS;
        else if(Application.platform == RuntimePlatform.tvOS) return Platform.TvOS;
        
        return Platform.Unknown;
    }

    public static PlayerRole GetPlayerRole()
    {
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return PlayerRole.Client;
        }
        return PlayerRole.Host;
    }
    
}
