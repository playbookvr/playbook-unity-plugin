using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Playbook.Scripts.Figma
{
    /// <summary>
    /// This struct allows us to manage Figma Component sets/types easier and more natively by containing size and type
    /// members.
    /// </summary>
    public struct FigmaComponentType
    {
        public string FigmaComponentID;
        [CanBeNull] public string FigmaComponentSetID;
        public bool IsMemberOfComponentSet;
        public GameObject GameObjectType;
        public QuickSizeChooser.Size? Size;
    }
}