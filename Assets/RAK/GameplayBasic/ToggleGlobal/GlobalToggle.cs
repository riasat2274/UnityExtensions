using RAK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalToggle : MonoBehaviour
{
    static Dictionary<GlobalToggleType, HardData<bool>> toggleData;
    static void EnsureHardInit()
    {
        if (toggleData == null)
        {
            toggleData = new Dictionary<GlobalToggleType, HardData<bool>>();
            foreach (GlobalToggleType item in System.Enum.GetValues(typeof(GlobalToggleType)))
            {
                bool startEnabled = true;// ((int)item > 0);
                toggleData.Add(item, new HardData<bool>($"Global_Toggle_{item}", startEnabled));
            }
        }
    }

    public static bool IsEnabled(GlobalToggleType type)
    {
        EnsureHardInit();
        return toggleData[type].value;
    }
    public static void SetToggle(GlobalToggleType type, bool enabled)
    {
        EnsureHardInit();
        toggleData[type].value = enabled;
    }

    public GlobalToggleType type;
    public Image image;
    public Button button;
    public Sprite enabledSprite;
    public Sprite disabledSprite;
    public bool IsToggleEnabled => toggleData[type].value;
    bool initialized;
    void EnsureInit()
    {
        if (initialized) return;
        EnsureHardInit();
        button.onClick.AddListener(OnClick);
        initialized = true;
    }
    private void OnEnable()
    {
        EnsureInit();
        Sync();
    }

    void OnClick()
    {
        SetToggle(type,!IsToggleEnabled);
        Sync();
    }
    protected virtual void Sync()
    {
        image.sprite = IsToggleEnabled?enabledSprite: disabledSprite;
    }


}
public enum GlobalToggleType
{
    Vibration = -1,
    Sound =2,
    LegacyControl = 3,
}