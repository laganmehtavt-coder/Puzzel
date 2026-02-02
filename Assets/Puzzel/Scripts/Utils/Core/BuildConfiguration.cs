using TMPro;
using UnityEngine;
using static GameEnums;

public class BuildConfiguration : MonoBehaviour
{
    public static BuildConfiguration Instance { get; private set; }
    [SerializeField] private TMP_Text tmp_Version_BootStrap;

    [SerializeField] private BuildType buildType = BuildType.Dev;
    [SerializeField] private PlayerRole playerRoleTargetBuild = PlayerRole.Host;

    public BuildType BuildType => buildType;
    public PlayerRole PlayerRoleTargetBuild => playerRoleTargetBuild;

    private void Awake()
    {
        BuildConfiguration[] instances = FindObjectsByType<BuildConfiguration>(FindObjectsSortMode.InstanceID);

        foreach (var instance in instances)
        {
            if (instance != this)
            {
                gameObject.SetActive(false);
                Destroy(gameObject); 
                return;
            }
        }
        Instance = this; 
    
        DontDestroyOnLoad(this);
        UpdateVersionText();
    }
    
    public void UpdateVersionText()
    {
        if (tmp_Version_BootStrap != null)
        {
            string vString = (buildType == BuildType.Dev) 
                ? $"{Application.version} ({buildType})" 
                : $"{Application.version}";
            tmp_Version_BootStrap.text = vString;
        }
    }
}