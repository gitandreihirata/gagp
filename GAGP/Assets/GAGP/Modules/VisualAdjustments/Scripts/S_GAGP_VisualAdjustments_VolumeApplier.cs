using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class S_GAGP_VisualAdjustments_VolumeApplier : MonoBehaviour
{
    public Volume volume;

    public void SetBrightness(float value)
    {
        if (volume.profile.TryGet(out ColorAdjustments color))
            color.postExposure.value = value - 1f;
    }

    public void SetContrast(float value)
    {
        if (volume.profile.TryGet(out ColorAdjustments color))
            color.contrast.value = (value - 1f) * 100f;
    }

    public void SetColorFilter(float amount)
    {
        if (volume.profile.TryGet(out ColorAdjustments color))
            color.colorFilter.value = Color.Lerp(Color.white, Color.gray, amount);
    }
}
