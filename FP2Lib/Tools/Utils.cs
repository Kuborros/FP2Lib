using System;
using UnityEngine;

namespace FP2Lib.Tools
{
    public class Utils
    {
        public static Sprite[] ExpandSpriteArray(Sprite[] sprites, int newLength, Sprite fill = null)
        {
            int oldLength = sprites.Length;
            Array.Resize(ref sprites, newLength);
            if (fill != null)
            {
                for (int i = oldLength; i < newLength; i++)
                {
                    if (sprites[i] == null)
                        sprites[i] = fill;
                }
            }
            return sprites;
        }

        public static FPStageCollectables[] ExpandStageCollectablesArray(FPStageCollectables[] stageCollectables, int newLength)
        {
            int oldLength = stageCollectables.Length;
            Array.Resize(ref stageCollectables, newLength);
            for (int i = oldLength; i < newLength; i++)
            {
                FPSaveManager.stageCollectables[i].length = 1;
                FPSaveManager.stageCollectables[i].status = new byte[1];
            }
            return stageCollectables;
        }
    }
}
