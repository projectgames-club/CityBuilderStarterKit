// Credit: Bunny83 http://answers.unity3d.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html

using UnityEngine;
using System.Collections;
namespace CBSK
{
    public class BitMaskAttribute : PropertyAttribute
    {
        public System.Type propType;
        public BitMaskAttribute(System.Type aType)
        {
            propType = aType;
        }
    }
}