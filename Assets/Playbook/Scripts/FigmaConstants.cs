using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playbook.Scripts.Figma
{
    /// <summary>
    /// This class contains all constants that are used by Figma Sync operations.
    /// </summary>
    public class FigmaConstants: MonoBehaviour
    {
        public static FigmaConstants Instance;
        
        // Game Object Elements
        [SerializeField]
        public GameObject imageElement;
        [SerializeField]
        public GameObject textElement;

        // Default positions for elements
        public Dictionary<GameObject, float> GameObjectZPos; // default Z Position for each element

        public Dictionary<GameObject, float>
            GameObjectXOffset; // offset for X so that each element "looks like" it's at the same position

        public  Dictionary<GameObject, float>
            GameObjectYOffset; // offset for Y so that each element "looks like" it's at the same position

        public Dictionary<GameObject, Vector3>
            GameObjectDefaultPos; // default position overall for base elements with sub-child positioning

        // Figma names for each playbook component
        public Dictionary<string, GameObject>
            FigmaComponentNames; // name lookup for the Playbook Figma component library

        List<GameObject> gameObjects;

        
        // Figma Positioning Scalars
        [SerializeField]
        public float scaleFactor = 700.0f;

        [SerializeField]
        public float defaultZPosition = 1.3f;

        // Offset for all the elements of playbook components if they're off screen
        [SerializeField]
        public float defaultAdjustX;
        [SerializeField]
        public float defaultAdjustY;
        
        // Scale factor for each element
        [NonSerialized]
        public float XScaleFactor;
        [NonSerialized]
        public float YScaleFactor;

        private void Awake()
        {
            Instance = this;

            gameObjects = new List<GameObject> { imageElement, textElement };
            GameObjectZPos = new Dictionary<GameObject, float>() { { imageElement, 0.0f }, { textElement, 0.0f } };
            GameObjectXOffset = new Dictionary<GameObject, float>() { { imageElement, 0.0f }, { textElement, -0.2f } };
            GameObjectYOffset = new Dictionary<GameObject, float>() { { imageElement, 0.0f }, { textElement, 0.0f } };
            GameObjectDefaultPos = new Dictionary<GameObject, Vector3>() { { imageElement, new Vector3(0.0f, 0.0f, 0.0f) }, { textElement, new Vector3(0.0f, 0.0f, 0.0f) } };

            FigmaComponentNames = new Dictionary<string, GameObject>() { { "Playbook Image", imageElement },  { "Playbook Text", textElement } };
            

            XScaleFactor = scaleFactor;
            YScaleFactor = XScaleFactor * -1;
        }

    }


}