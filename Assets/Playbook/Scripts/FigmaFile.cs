using System;
using UnityEngine;

namespace Playbook.Scripts.Figma
{
    /// <summary>
    /// This class defines a Figma File, allowing us to sync multiple files at once
    /// </summary>
    public class FigmaFile
    {
        public string Name;
        public string Url;
        public int Page;
        public int Frame;
        
        // Status of sync
        public bool IsSyncing;
        public readonly bool ReadyToSync;
        public DateTime? LastSyncTime;

        private readonly FigmaFileSyncer _figmaFileSyncer;

        // track whether the file is synced to the current scene
        public bool IsCurrentlySynced;
        

        public FigmaFile(string url, int page, int frame)
        {
            ReadyToSync = false;
            Url = url;
            Page = page;
            Frame = frame;
            //FigmaMenuDropdownOption = new DropdownOption
            //{
            //    optionName = "Loading..."
            //};
            IsCurrentlySynced = false;
            IsSyncing = false;
            LastSyncTime = null;
            _figmaFileSyncer = new FigmaFileSyncer(this);
            ReadyToSync = true;
        }

        public void Sync()
        {
            // if the file is already syncing, or unable to sync, don't do anything
            if (IsSyncing || !ReadyToSync) return;
            IsSyncing = true; // lock for sync
            Debug.Log("Beginning Figma Sync");
            if (!IsCurrentlySynced)
            {
                IsCurrentlySynced = true;
            }
            _figmaFileSyncer.SyncFigma();
            LastSyncTime = DateTime.Now;
            IsSyncing = false;
            Debug.Log("Figma Sync Completed");
        }
    }
}