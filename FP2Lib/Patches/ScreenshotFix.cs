using HarmonyLib;

using UnityEngine;

namespace FP2Lib.Patches
{
    internal class ScreenshotFix
    {
        //On older game versions a patch is needed in case internal resolution is not the default. v1.2.6 and above handle it themselves.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuPhoto), "Start", MethodType.Normal)]
        private static void PatchMenuPhoto(ref int ___captureHeight, ref int ___captureWidth)
        {
            if (FP2Lib.gameInfo.gameVersion.CompareTo(new System.Version("1.2.6")) <= 0)
            {
                GameObject pixelArtTarget = GameObject.Find("Pixel Art Target");
                if (pixelArtTarget != null)
                {
                    Texture pixelArtBuffer = pixelArtTarget.GetComponent<MeshRenderer>().material.mainTexture;
                    if (pixelArtBuffer != null)
                    {
                        ___captureHeight = pixelArtBuffer.height;
                        ___captureWidth = pixelArtBuffer.width;
                    }
                }
            }
        }

        //Png is bigger than normal screenshots, needs resizing to not break the UI.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuAlbum), "LoadPNG", MethodType.Normal)]
        [HarmonyPatch(typeof(MenuPhoto), "LoadPNG", MethodType.Normal)]
        private static void PatchLoadPNG(ref Texture2D __result)
        {
            if (__result != null)
            {
                if (__result.width != 640 || __result.height != 360)
                Resize(__result, 640, 360);
            }
        }

        //Hacky resize code, there does not seem to be an easier and faster way to do so in Unity 5.6
        private static void Resize(Texture2D texture, int newWidth, int newHeight)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(newWidth, newHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            RenderTexture.active = tmp;
            Graphics.Blit(texture, tmp);
            texture.Resize(newWidth, newHeight, texture.format, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.ReadPixels(new Rect(Vector2.zero, new Vector2(newWidth, newHeight)), 0, 0);
            texture.Apply();
            RenderTexture.ReleaseTemporary(tmp);
        }


    }
}
