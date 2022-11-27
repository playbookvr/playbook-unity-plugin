using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


namespace Playbook.Scripts.Figma
{
    /// <summary>
    /// This class defines functions for syncing a given Figma file. Each FigmaFile object has its own Syncer to track
    /// elements and components for that given file. Currently, this is pretty messy and needs some cleaning.
    /// </summary>
    public class FigmaFileSyncer
    {
        private readonly FigmaFile _parentFigmaFile;

        
        private static Dictionary<string, FigmaComponentType>
            _figmaComponentIDs; // ID lookup for per-file Figma component library

        private static Dictionary<string, GameObject> _syncedFigmaObjects; // all synced objects by Figma Node ID
        
        // Game Object Elements to store for easier access later
        private readonly GameObject _buttonElement;
        private readonly GameObject _imageElement;
        private readonly GameObject _panelElement;
        private readonly GameObject _sliderElement;
        private readonly GameObject _stepperElement;
        private readonly GameObject _textElement;
        private readonly GameObject _toggleElement;
        private readonly Shader _imageShader;

        private float _origAdjustX;
        private float _origAdjustY;

        private int elementCount;

        public FigmaFileSyncer(FigmaFile parent)
        {
            _parentFigmaFile = parent;

            _imageElement = FigmaConstants.Instance.imageElement;
            
            _imageShader = Shader.Find("Universal Render Pipeline/Unlit");
            
            _figmaComponentIDs = new Dictionary<string, FigmaComponentType>();
            _syncedFigmaObjects = new Dictionary<string, GameObject>();
        }
        
        // Function to initiate a complete Figma sync
        public void SyncFigma()
        {
            string figmaFileID = _parentFigmaFile.Url.Split('/')[4];
            Debug.Log($"SyncFigma() - figmaFileID: {figmaFileID}");
            string figmaAPIFileEndpoint = $"/files/{figmaFileID}";
            int figmaPage = _parentFigmaFile.Page;
            int figmaFrame = _parentFigmaFile.Frame;
            JObject figmaFileJSON;
            JObject figmaFrameJSON;
            JObject figmaImageRefJSON;

            // Get the initial document of the Figma page
            FigmaSyncManager.Instance.StartCoroutine(FigmaAPIConnector.Instance.RequestFigmaAPI(figmaAPIFileEndpoint, response =>
            {
                figmaFileJSON = response;
                // if we haven't synced this file before, we need to grab the IDs of all Playbook components to enable
                // matching of Figma Elements -> Playbook native components
                if (_figmaComponentIDs.Count < 1)
                {
                    EvaluateFigmaComponents(figmaFileJSON);
                }

                // TODO: we'll need to hook this NodeID up to the frame dropped in by the Figma Plugin
                // Grab the first page's first frame that contains the rest of the elements

                if (figmaPage > ((JArray)figmaFileJSON["document"]["children"]).Count)
                {
                    Debug.Log("The page entered exceeds number of pages in the file. Resetting to first page.");
                    figmaPage = 0;
                }
;
                if (figmaFrame > ((JArray)figmaFileJSON["document"]["children"][figmaPage]["children"]).Count)
                {
                    Debug.Log("The frame entered exceeds number of frames in the file. Resetting to first frame.");
                    figmaFrame = 0;
                }
                    

                string figmaNodeID = (string)figmaFileJSON["document"]["children"][figmaPage]["children"][figmaFrame]["id"];
                string figmaAPIFrameEndpoint =
                    $"/files/{figmaFileID}/nodes?geometry=paths&ids={figmaNodeID}";
                string figmaAPIImageRefEndpoint = $"/files/{figmaFileID}/images"; // This is a JSON containing image URLs used in a Figma canvas
                FigmaSyncManager.Instance.StartCoroutine(FigmaAPIConnector.Instance.RequestFigmaAPI(figmaAPIImageRefEndpoint, response =>
                {
                    figmaImageRefJSON = (JObject)response["meta"]["images"];
                    FigmaSyncManager.Instance.StartCoroutine(FigmaAPIConnector.Instance.RequestFigmaAPI(figmaAPIFrameEndpoint, response =>
                    {
                        figmaFrameJSON = (JObject)response["nodes"][figmaNodeID]["document"];
                        _origAdjustX = FigmaConstants.Instance.defaultAdjustX - ((float)figmaFrameJSON["absoluteBoundingBox"]["x"] +
                                                        0.5f * (float)figmaFrameJSON["absoluteBoundingBox"]["width"]);
                        _origAdjustY = FigmaConstants.Instance.defaultAdjustY - ((float)figmaFrameJSON["absoluteBoundingBox"]["y"] +
                                                        0.5f * (float)figmaFrameJSON["absoluteBoundingBox"]["height"]);
                        foreach (JObject child in figmaFrameJSON["children"])
                        {
                            EvaluateFigmaObject(child, figmaFileID, figmaImageRefJSON);
                        }

                    }));
                }));
            }));
        }
        
        private GameObject EvaluateFigmaObject(JObject figmaObjectJSON, string figmaFileID, JObject figmaImageRefJSON)
        {
            //Debug.Log($"Evaluating object {figmaObjectJSON["name"]}");
            GameObject currObject;

            FigmaComponentType currFigmaComponentType = new FigmaComponentType();

            bool isUnknownTextComponent = ((string)figmaObjectJSON["type"]).Equals("TEXT");
            bool isUnknownComponent = true; // Assume everything is an unknownComponent by default
                                            // Unknown Components are to be instantiated as Playbook Images, including non-Playbook text
                                            // check to see if the object matches a component
            if ((figmaObjectJSON.ContainsKey("componentId") &&
                 _figmaComponentIDs.ContainsKey((string)figmaObjectJSON["componentId"])))
            {
                // set the component type
                currFigmaComponentType = _figmaComponentIDs[(string)figmaObjectJSON["componentId"]];
                isUnknownComponent = false;
            }
            else
            {
                // Default type is imageElement/PlaybookImage for the sake of unknown components
                currFigmaComponentType.GameObjectType = _imageElement;
            }

            GameObject currGameObjectType = currFigmaComponentType.GameObjectType;

            // Calculate the rotation value because Figma REST API does not give rotations to us
            // Instead we have to get it from relativeTransform and flip it due to reversed coords from Figma to Unity
            float rotation = -(float)Mathf.Atan2(-(float)figmaObjectJSON["relativeTransform"][0][1],
                (float)figmaObjectJSON["relativeTransform"][0][0]);

            float width = (float)figmaObjectJSON["size"]["x"];
            float height = (float)figmaObjectJSON["size"]["y"];
            float x = (float)figmaObjectJSON["relativeTransform"][0][2];
            float y = (float)figmaObjectJSON["relativeTransform"][1][2];
            // Use relativeTransform values to make sure that we know where the CENTER of a gameObject should be if it's rotated
            // Because Figma origin is set to top-left while Unity is center, so rotations in Figma affect how it reports its object's origin while Unity doesn't
            Vector2 absolutePos = new Vector2(
                x + (width / 2.0f) * Mathf.Cos(rotation) - (height / 2.0f) * Mathf.Sin(-rotation),
                y + (width / 2.0f) * Mathf.Sin(-rotation) + (height / 2.0f) * Mathf.Cos(rotation)
            );

            Vector3 position;
            position.x = ((absolutePos.x + _origAdjustX) / FigmaConstants.Instance.XScaleFactor) + FigmaConstants.Instance.GameObjectXOffset[currGameObjectType];
            position.y = ((absolutePos.y + _origAdjustY) / FigmaConstants.Instance.YScaleFactor) + FigmaConstants.Instance.GameObjectYOffset[currGameObjectType];
            position.z = FigmaConstants.Instance.GameObjectZPos[currGameObjectType]; // lookup default Z position
            rotation *= 180.0f / Mathf.PI;

            // find current object; otherwise spawn new one
            if (_syncedFigmaObjects.ContainsKey((string)figmaObjectJSON["id"]))
            {
                //Debug.Log(
                //    $"Updating {(string)figmaObjectJSON["name"]}: original pos ({(float)figmaObjectJSON["absoluteBoundingBox"]["x"]}, {(float)figmaObjectJSON["absoluteBoundingBox"]["y"]}, {position.z}) new pos ({position.x}, {position.y}, {position.z})");
                currObject = _syncedFigmaObjects[(string)figmaObjectJSON["id"]];
            }
            else
            {
                //Debug.Log(
                //    $"Spawning {(string)figmaObjectJSON["name"]}: original pos ({(float)figmaObjectJSON["absoluteBoundingBox"]["x"]}, {(float)figmaObjectJSON["absoluteBoundingBox"]["y"]}, {position.z}) new pos ({position.x}, {position.y}, {position.z})");
                if (currGameObjectType == _buttonElement || currGameObjectType == _imageElement ||
                    currGameObjectType == _panelElement || currGameObjectType == _textElement)
                {
                    currObject = SpawnObject(currGameObjectType, FigmaConstants.Instance.GameObjectDefaultPos[currGameObjectType]);
                }
                else
                {
                    currObject = SpawnObject(currGameObjectType, position);
                }

                _syncedFigmaObjects[(string)figmaObjectJSON["id"]] = currObject;
            }

            SetObjectPosition(currFigmaComponentType, currObject, position, width, height);
            SetObjectSize(currFigmaComponentType, currObject, width, height, figmaObjectJSON, rotation);
            SetObjectText(currFigmaComponentType, currObject, figmaObjectJSON, isUnknownComponent);
            SetObjectColor(currFigmaComponentType, currObject, figmaObjectJSON);
            SetImageFill(currFigmaComponentType, currObject, figmaObjectJSON, figmaImageRefJSON,
                width, height, isUnknownComponent, isUnknownTextComponent, figmaFileID);

            currObject.name = "FigmaElement_" + elementCount;
            elementCount++;

            return currObject;
        }

        // This function spawns a new GameObject of given type at a given pos
        private GameObject SpawnObject(GameObject type, Vector3 pos)
        {
            GameObject currObject = 
                FigmaSyncManager.Instance.CreateObject(type, pos, Quaternion.identity);
            return currObject;
        }

        // This function enumerates all  Figma Components and Component Sets in a given FJSON
        private void EvaluateFigmaComponents(JObject figmaFileJSON)
        {
            //Debug.Log("Evaluating Figma Components...");

            // create a dictionary to match a set of components to their equivalent Playbook GameObject
            // each Component Set contains a single Playbook GameObject in Figma ("toggle", "stepper", etc)
            // but may contain multiple sizing or other options for that specific GameObject. 
            Dictionary<string, GameObject> componentSetIDToGameObject = new Dictionary<string, GameObject>();
            // first check component sets
            if (figmaFileJSON.ContainsKey("componentSets"))
            {
                foreach (JProperty componentSet in ((JObject)figmaFileJSON["componentSets"]).Properties())
                {
                    if (FigmaConstants.Instance.FigmaComponentNames.ContainsKey(
                            (string)figmaFileJSON["componentSets"][componentSet.Name]["name"]))
                    {
                        //Debug.Log(
                        //    $"Found component set {(string)figmaFileJSON["componentSets"][componentSet.Name]["name"]}");
                        componentSetIDToGameObject[componentSet.Name] =
                            FigmaConstants.Instance.FigmaComponentNames[(string)figmaFileJSON["componentSets"][componentSet.Name]["name"]];
                    }
                }
            }

            foreach (JProperty component in ((JObject)figmaFileJSON["components"]).Properties())
            {
                // Part of a Component Set: each individual component may belong to a component set, but has its own
                // individual component ID that we need to match on; this allows us to have quick sized components
                if (((JObject)figmaFileJSON["components"][component.Name]).ContainsKey("componentSetId") &&
                    componentSetIDToGameObject.ContainsKey(
                        (string)figmaFileJSON["components"][component.Name]["componentSetId"]))
                {
                    // We need a FigmaComponentType for every instance of a given component
                    // Multiple components may belong to a singular component set
                    FigmaComponentType newFigmaComponentType = new FigmaComponentType
                    {
                        FigmaComponentID = component.Name,
                        FigmaComponentSetID = (string)figmaFileJSON["components"][component.Name]["componentSetId"],
                        GameObjectType =
                            componentSetIDToGameObject[
                                (string)figmaFileJSON["components"][component.Name]["componentSetId"]],
                        IsMemberOfComponentSet = true
                    };

                    //Debug.Log(
                    //    $"Found component from set {newFigmaComponentType.FigmaComponentSetID} ({newFigmaComponentType.GameObjectType.name})");

                    // check for size parameter if in a component set
                    if (((JObject)figmaFileJSON["components"][component.Name]).ContainsKey("name") &&
                        ((string)figmaFileJSON["components"][component.Name]["name"]).Length > 5)
                        switch (((string)figmaFileJSON["components"][component.Name]["name"])[5])
                        {
                            case 'L':
                                newFigmaComponentType.Size = QuickSizeChooser.Size.L;
                                break;
                            case 'M':
                                newFigmaComponentType.Size = QuickSizeChooser.Size.M;
                                break;
                            case 'S':
                                newFigmaComponentType.Size = QuickSizeChooser.Size.S;
                                break;
                        }

                    _figmaComponentIDs[component.Name] = newFigmaComponentType;
                }
                // Regular Component – these don't belong to a set and exist by themselves
                else if (FigmaConstants.Instance.FigmaComponentNames.ContainsKey((string)figmaFileJSON["components"][component.Name]["name"]))
                {
                    // Create a FigmaComponentType for these, but they don't have variants/size params
                    FigmaComponentType newFigmaComponentType = new FigmaComponentType
                    {
                        FigmaComponentID = component.Name,
                        FigmaComponentSetID = null,
                        GameObjectType =
                            FigmaConstants.Instance.FigmaComponentNames[(string)figmaFileJSON["components"][component.Name]["name"]],
                        IsMemberOfComponentSet = false,
                        Size = null
                    };
                    //Debug.Log($"Found component {newFigmaComponentType.GameObjectType.name}");
                    _figmaComponentIDs[component.Name] = newFigmaComponentType;
                }
            }
        }

        private void SetImageFill(FigmaComponentType figmaComponentType, GameObject currObject, JObject figmaObjectJSON,
            JObject figmaImageRefJSON, float width, float height, bool isUnknownComponent, bool isText,
            string figmaFileID)
        {
            if (figmaComponentType.GameObjectType == _imageElement)
            {
                if (isUnknownComponent)
                {
                    // For unknown components, grab their image URLs straight from FigmaFileID and load from there
                    string compId = (string)figmaObjectJSON["id"];
                    string figmaAPIImageEndpoint = $"/images/{figmaFileID}?ids={compId}";
                    // We request the URL of the image based on the endpoint above
                    FigmaSyncManager.Instance.StartCoroutine(FigmaAPIConnector.Instance.RequestFigmaAPI(figmaAPIImageEndpoint, response =>
                    {
                        string imageUrl = (string)response["images"][compId];
                        FigmaSyncManager.Instance.StartCoroutine(GetImage(imageUrl, currObject, 0.0f, 0.0f, isUnknownComponent, isText));
                    }));
                }
                else
                {
                    // For Playbook Images, grab image URLs based on their imageRef code that matches from figmaImageRefJSON
                    string fillType = (string)figmaObjectJSON["children"][0]["fills"][0]["type"];
                    if (fillType.CompareTo("IMAGE") == 0)
                    {
                        //Debug.Log((string)figmaObjectJSON["children"][0]["name"]);
                        string imageRef = (string)figmaObjectJSON["children"][0]["fills"][0]["imageRef"]; // This is the key of the imageRef from the component
                        string imageUrl = (string)figmaImageRefJSON[imageRef]; // This directly grabs the URL based on the imageRef key
                        FigmaSyncManager.Instance.StartCoroutine(GetImage(imageUrl, currObject, width, height, isUnknownComponent, isText));
                    }
                }
            }
        }

        private IEnumerator GetImage(string imageUrl, GameObject currObject, float trueWidth, float trueHeight,
            bool unknownComponent, bool isText)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
            yield return www.SendWebRequest();

            // Check to see if we successfully get an image from the given URL
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (tex != null)
                {
                    Texture2D image;
                    if (!unknownComponent)
                    {
                        // Set up for cropping texture to retain aspect ratio when applied on component
                        // This only assumes that images are to be centered when cropped
                        int cropWidth = tex.width, cropHeight = tex.height;
                        float divFactor;

                        // If there is a greater difference between the actual component width vs. texture width compared
                        // to the actual height vs. texture height
                        if ((tex.width / trueWidth) > (tex.height / trueHeight))
                        {
                            divFactor = tex.height /
                                        trueHeight; // Get the factor at which the texture will shrink in height to fit component
                            cropWidth = (int)(trueWidth *
                                              divFactor); // Multiply the true width of the component with the divFactor
                                                        // to cut off excess width in texture
                        }
                        else // If the difference between the actual height and texture height is greater instead
                        {
                            divFactor = tex.width / trueWidth;
                            cropHeight =
                                (int)(trueHeight *
                                      divFactor); // Cut off the excess height, because we WILL be using the full width,
                                                // but cutting off excess height to avoid stretching/compressing
                        }

                        // Debug.Log($"x: {(tex.width - cropWidth) / 2}, y: {(tex.height - cropHeight) / 2}, blockWidth: {cropWidth}, blockHeight: {cropHeight}");
                        Color[] c = tex.GetPixels((tex.width - cropWidth) / 2, (tex.height - cropHeight) / 2, cropWidth,
                            cropHeight, 0);
                        image = new Texture2D(cropWidth, cropHeight);
                        image.SetPixels(c);
                        image.Apply();
                        //File.WriteAllBytes(Application.dataPath + "/JSONData/Image" + counts[1] + ".png", comp.imageData);
                    }
                    else
                    {
                        image = tex;
                        // If the image is text, we don't want to change the scaling at all because text should not be cropped or stretched
                        // Instead, make the gameObject localScale match the text's based on the given texture/image width & height
                        if (isText)
                        {
                            Vector3 scalePanel = new Vector3(image.width / 695.0f, image.height / 695.0f,
                                currObject.transform.GetChild(0).localScale.z);
                            currObject.transform.GetChild(0).localScale = scalePanel;
                        }
                    }

                    Renderer compRenderer = currObject.transform.GetChild(0).GetComponent<Renderer>();
                    compRenderer.material.shader = _imageShader;
                    compRenderer.material.SetFloat("_Surface", 1.0f);
                    compRenderer.material.renderQueue = 2999; // We change the renderQueue to always have images render on top of panels
                    compRenderer.material.mainTexture = image;
                    currObject.transform.GetChild(0).transform.localPosition = new Vector3(
                        currObject.transform.GetChild(0).transform.localPosition.x,
                        currObject.transform.GetChild(0).transform.localPosition.y,
                        currObject.transform.GetChild(0).transform.localPosition.z - 0.001f /* slight change to local Z to make sure images are always above panels */
                    );
                }
                else
                {
                    Debug.Log("ERROR: NO IMAGE");
                }
            }
        }

        private void SetObjectPosition(FigmaComponentType figmaComponentType, GameObject currObject, Vector3 position,
            float width, float height)
        {
            GameObject type = figmaComponentType.GameObjectType;
            position.z = FigmaConstants.Instance.defaultZPosition;
            if (type == _textElement)
            {
                RectTransform rt = currObject.transform.GetChild(0).GetComponent<RectTransform>();

                // rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    // width / FigmaConstants.Instance.XScaleFactor * 140.0f); // multiply by text field scale

                Transform childGameObject = currObject.transform.GetChild(0).transform;
                currObject.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);

                currObject.transform.localPosition = new Vector3(position.x, position.y, FigmaConstants.Instance.defaultZPosition);
            }
            else
            {
                currObject.transform.position = position;
                // currObject.transform.GetChild(0).transform.localPosition = new Vector3(
                //     currObject.transform.GetChild(0).transform.localPosition.x,
                //     currObject.transform.GetChild(0).transform.localPosition.y, FigmaConstants.Instance.defaultZPosition);
            }
        }

        private void SetObjectSize(FigmaComponentType figmaComponentType, GameObject currObject, float width,
            float height, JObject figmaObjectJSON, float rotation)
        {
            GameObject type = figmaComponentType.GameObjectType;
            if (type == _panelElement)
            {
                Vector3 scalePanel = new Vector3(width / 695.0f, height / 695.0f,
                    currObject.transform.GetChild(0).localScale.z);
                currObject.transform.GetChild(0).localScale = scalePanel;
            }
            else if (type == _imageElement) // Unknown components are also set here
            {
                Vector3 scalePanel = new Vector3(width / 695.0f, height / 695.0f,
                    currObject.transform.GetChild(0).localScale.z);
                currObject.transform.GetChild(0).localScale = scalePanel;
            }
        }

        private void SetObjectText(FigmaComponentType figmaComponentType, GameObject currObject,
            JObject figmaObjectJSON, bool unknownComponent)
        {
            GameObject type = figmaComponentType.GameObjectType;
        }

        private void SetObjectColor(FigmaComponentType figmaComponentType, GameObject currObject,
            JObject figmaObjectJSON)
        {
            GameObject type = figmaComponentType.GameObjectType;
            if (type.GetComponent<ColorModifier>() != null)
            {
                JObject colorJSON = null;
                if ((type == _textElement || type == _toggleElement) && figmaObjectJSON.ContainsKey("children") &&
                    figmaObjectJSON["children"].HasValues &&
                    ((JObject)figmaObjectJSON["children"][0]).ContainsKey("fills") &&
                    figmaObjectJSON["children"][0]["fills"].HasValues)
                {
                    colorJSON = (JObject)figmaObjectJSON["children"][0]["fills"][0]["color"];
                }
                else if ((type == _buttonElement || type == _stepperElement || type == _panelElement) &&
                         figmaObjectJSON.ContainsKey("fills") && figmaObjectJSON["fills"].HasValues)
                {
                    colorJSON = (JObject)figmaObjectJSON["fills"][0]["color"];
                    
                }
                else if ((type == _sliderElement) && figmaObjectJSON.ContainsKey("children") &&
                         figmaObjectJSON["children"].HasValues &&
                         ((JObject)figmaObjectJSON["children"][0]).ContainsKey("children") &&
                         figmaObjectJSON["children"][0]["children"].HasValues &&
                         ((JObject)figmaObjectJSON["children"][0]["children"][1]).ContainsKey("fills") &&
                         figmaObjectJSON["children"][0]["children"][1]["fills"].HasValues)
                {
                    colorJSON = (JObject)figmaObjectJSON["children"][0]["children"][1]["fills"][0]["color"];
                }


                if (colorJSON != null)
                {
                    ColorModifier objCM = currObject.GetComponent<ColorModifier>();
                    var newColor = new Color((float)colorJSON["r"], (float)colorJSON["g"], (float)colorJSON["b"],
                        (float)colorJSON["a"]);
                    objCM.currentColor = newColor;
                }
            }
        }
    }
}