using System;
using System.Collections;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Playbook.Scripts.Figma
{
    /// <summary>
    /// This script maintains the Figma API Request Coroutine
    /// </summary>
    public class FigmaAPIConnector
    {
        public static FigmaAPIConnector Instance;
        private string _figmaApiBaseUrl = "https://api.figma.com/v1";

        private readonly string
            _figmaAccessToken;

        public FigmaAPIConnector(string figmaAccessToken)
        {
            Instance = this;
            _figmaAccessToken = figmaAccessToken;
        }
        public IEnumerator RequestFigmaAPI(string figmaAPIEndpoint, Action<JObject> callback)
        {
            UnityWebRequest figmaRequest = UnityWebRequest.Get(_figmaApiBaseUrl + figmaAPIEndpoint);
            // figmaRequest.SetRequestHeader("Authorization", $"Bearer {_figmaAccessToken}"); // for OAuth
            figmaRequest.SetRequestHeader("X-FIGMA-TOKEN", $"{_figmaAccessToken}"); // personal access token
            yield return figmaRequest.SendWebRequest();

            if (figmaRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(figmaRequest.error);
            }
            else
            {
                JObject figmaFile = JObject.Parse(figmaRequest.downloadHandler.text);
                callback(figmaFile);
            }
        }
    }
}