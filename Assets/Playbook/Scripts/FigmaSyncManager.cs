using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playbook.Scripts.Figma
{
    /// <summary>
    /// This script manages Figma Syncing is responsible for containing a list of all syncing/synced files, as well as
    /// managing Coroutines needed for syncing activities.
    /// </summary>
    public class FigmaSyncManager : MonoBehaviour
    {
        // Singleton
        public static FigmaSyncManager Instance;
        
        // Figma API info/auth
        // Need to populate via account system
        [SerializeField] private string figmaFileURL;
        
        // This token has to be an OAuth token from the Figma API
        [SerializeField] private string figmaAPIToken;

        // Figma page number
        [SerializeField] private int page;

        // Figma frame number
        [SerializeField] private int frame;

        private List<FigmaFile> _availableFigmaFiles;
        private int _currentlySelectedFigmaFile;

        private bool _figmaSyncReady;

        private void Awake()
        {
            Instance = this;
        }

        // Set up
        private void Start()
        {
            // create a new API connector for use by FigmaFileSyncer
            var _ = new FigmaAPIConnector(figmaAPIToken);

            var newFile = new FigmaFile(figmaFileURL, page, frame);

            _availableFigmaFiles = new List<FigmaFile> { newFile };
            _currentlySelectedFigmaFile = 0;

            _figmaSyncReady = true;
        }


        // Stub to retrieve from Playbook Web Hub a list of syncable Figma Files
        public List<FigmaFile> RetrieveFigmaFiles()
        {
            if (_figmaSyncReady)
            {
                return _availableFigmaFiles;
            }

            return null;
        }

        public int GetNumAvailableFigmaFiles()
        {
            return _availableFigmaFiles.Count;
        }

        public void SyncFigmaFile(int index)
        {
            if (index == -1)
            {
                _availableFigmaFiles[_currentlySelectedFigmaFile].Sync();
            } else if (index < _availableFigmaFiles.Count && index >= 0)
            {
                _availableFigmaFiles[index].Sync();
            }
        }

        public GameObject CreateObject(GameObject type, Vector3 pos, Quaternion q)
        {
            var currObject = Instantiate(type, pos, q);
            return currObject;
        }
    }
}