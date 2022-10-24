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
        
        // private string _figmaClientId = "aXD8E7MiO6RQYwcj0Rp3zw";
        // private string _figmaClientSecret = "izLFCcfd5hvO7Q1BfzeP9rZxTiM1GG";
        // private string _figmaCallbackURL = "https://playbookxr.com";
        private string _figmaApiBaseUrl = "https://api.figma.com/v1";

        private string
            _figmaAccessToken = "figu_PoBANpi5y70_Y69h1oOx4v8DimLPZ6wa_yPKhxBN";

        public FigmaAPIConnector([CanBeNull] string figmaAccessToken)
        {
            Instance = this;
            if (figmaAccessToken != null)
            {
                _figmaAccessToken = figmaAccessToken;
            }
        }
        public IEnumerator RequestFigmaAPI(string figmaAPIEndpoint, Action<JObject> callback)
        {
            UnityWebRequest figmaRequest = UnityWebRequest.Get(_figmaApiBaseUrl + figmaAPIEndpoint);
            figmaRequest.SetRequestHeader("Authorization", $"Bearer {_figmaAccessToken}");
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