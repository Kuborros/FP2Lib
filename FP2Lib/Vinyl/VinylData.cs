using UnityEngine;

namespace FP2Lib.Vinyl
{
    enum VAddToShop{

        None,
        ClassicOnly,
        Naomi, //Battlesphere
        Digo, //Adventure Square
        Fawnstar //Paradise Prime
    }

    internal class VinylData
    {
        int id;
        string name;
        AudioClip audioClip;
        VAddToShop shopLocation;

    }
}
