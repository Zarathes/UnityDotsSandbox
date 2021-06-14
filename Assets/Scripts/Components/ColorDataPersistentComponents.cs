// Author: Bart Schut
using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("Brightness", MaterialPropertyFormat.Float)]
public struct ColorBrightness : IComponentData
{    
    public float Value;
    public static ColorBrightness Default => new ColorBrightness()
    {
        Value = 0.1f
    };
}
