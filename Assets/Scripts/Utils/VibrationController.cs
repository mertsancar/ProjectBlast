using Lofelt.NiceVibrations;

public static class VibrationController
{
    private static bool _isActive = true;
    
    public static void SetActive(bool active)
    {
        _isActive = active;
    }

    public static bool ActiveSelf() => _isActive;

    public static void TapBubbleVibration()
    {
        if (_isActive) HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);  
    }
    
    public static void LevelSuccessVibration()
    {
        if (_isActive) HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    
    public static void LevelFailVibration()
    {
        if (_isActive) HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
    }
}
