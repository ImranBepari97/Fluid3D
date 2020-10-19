using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPresets {
    public enum HueColorNames {
        Black,
        Brown,
        Green,
        Purple,
        Red,
        Yellow,
        Lime,
        Aqua,
        Blue,
        Pink,
        Orange,
        White,
        Olive,
        Teal
    }

    private static Hashtable hueColourValues = new Hashtable{
        { HueColorNames.Black, new Color32( 0, 0, 0, 1 ) },
        { HueColorNames.Brown, new Color32( 139, 69, 19, 1 ) },
        { HueColorNames.Green, new Color32( 0, 255, 0, 1 ) },
        { HueColorNames.Purple, new Color32( 75, 0, 130, 1 ) },
        { HueColorNames.Red, new Color32( 245, 0, 0, 1 ) },
        { HueColorNames.Yellow, new Color32( 254, 224, 0, 1 ) },
        { HueColorNames.Lime, new Color32( 166, 254, 0, 1 ) },
        { HueColorNames.Aqua, new Color32( 0, 201, 255, 1 ) },
        { HueColorNames.Blue, new Color32( 0, 0, 255, 1 ) },
        { HueColorNames.Pink, new Color32( 255, 105, 180, 1 ) },
        { HueColorNames.Orange, new Color32( 235, 168, 0, 1 ) },
        { HueColorNames.White, new Color32( 255, 255, 255, 1 ) },
        { HueColorNames.Olive, new Color32( 128, 128, 0, 1 ) },
        { HueColorNames.Teal, new Color32( 0, 128, 128, 1 ) }
    };

    public static Color32 HueColourValue(HueColorNames color) {
        return (Color32)hueColourValues[color];
    }
}
