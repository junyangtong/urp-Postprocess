using System; 
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.Rendering; 
using UnityEngine.Rendering.Universal; 
 
[Serializable, VolumeComponentMenu("Post-processing/TiltShiftBlur")] 
public class TiltShiftBlur : VolumeComponent, IPostProcessComponent 
{ 
    [Range(0.0f, 1.0f)] 
    public FloatParameter Offset = new ClampedFloatParameter(0f, -2f, 2f);
 
    [Range(0.0f, 5.0f)] 
    public FloatParameter Area = new ClampedFloatParameter(0f, 0f, 5f);
 
    [Range(0.0f, 1.0f)] 
    public FloatParameter Spread = new ClampedFloatParameter(0f, 0f, 2f);
 
    [Range(0.0f, 50.0f)] 
    public FloatParameter BlurInt = new ClampedFloatParameter(0f, 0f, .3f);


 
    public bool IsActive() => BlurInt.value > 0f; 
 
    public bool IsTileCompatible() => false; 
} 