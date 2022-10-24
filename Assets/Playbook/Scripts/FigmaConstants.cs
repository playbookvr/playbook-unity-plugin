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
        public GameObject buttonElement;
        [SerializeField]
        public GameObject imageElement;
        [SerializeField]
        public GameObject panelElement;
        [SerializeField]
        public GameObject sliderElement;
        [SerializeField]
        public GameObject stepperElement;
        [SerializeField]
        public GameObject textElement;
        [SerializeField]
        public GameObject toggleElement;
        
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

            gameObjects = new List<GameObject> { buttonElement, imageElement, panelElement, sliderElement, stepperElement, textElement, toggleElement };
            GameObjectZPos = new Dictionary<GameObject, float>() { { buttonElement, 0.0f }, { imageElement, 0.0f }, { panelElement, 0.0f }, { sliderElement, 0.0f }, { stepperElement, 0.0f }, { textElement, 0.0f }, { toggleElement, 0.0f } };
            GameObjectXOffset = new Dictionary<GameObject, float>() { { buttonElement, 0.0f }, { imageElement, 0.0f }, { panelElement, 0.0f }, { sliderElement, 0.007f }, { stepperElement, 0.0f }, { textElement, -0.2f }, { toggleElement, -0.01f } };
            GameObjectYOffset = new Dictionary<GameObject, float>() { { buttonElement, 0.0f }, { imageElement, 0.0f }, { panelElement, 0.0f }, { sliderElement, 0.0f }, { stepperElement, 0.0f }, { textElement, 0.0f }, { toggleElement, 0.0f } };
            // GameObjectDefaultPos = new Dictionary<GameObject, Vector3>() { { buttonElement, new Vector3(-0.388f, -0.0665f, 1.3f) }, { imageElement, new Vector3(-0.393812001f, 0.0877403021f, 1.3f) }, { panelElement, new Vector3(-0.512246609f, 0.0878968537f, 1.3f) }, { textElement, new Vector3(-0.512122393f, 0.0923901498f, 1.3f) } };
            GameObjectDefaultPos = new Dictionary<GameObject, Vector3>() { { buttonElement, new Vector3(0.0f, 0.0f, 0.0f) }, { imageElement, new Vector3(0.0f, 0.0f, 0.0f) }, { panelElement, new Vector3(0.0f, 0.0f, 0.0f) }, { textElement, new Vector3(0.0f, 0.0f, 0.0f) } };

            FigmaComponentNames = new Dictionary<string, GameObject>() { { "Playbook Button", buttonElement }, { "Playbook Image", imageElement }, { "Playbook Panel", panelElement }, { "Playbook Slider", sliderElement }, { "Playbook Stepper", stepperElement }, { "Playbook Text", textElement }, { "Playbook Toggle", toggleElement } };
            

            XScaleFactor = scaleFactor;
            YScaleFactor = XScaleFactor * -1;
        }

    }


}