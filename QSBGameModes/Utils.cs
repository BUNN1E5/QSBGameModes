using System;
using System.Collections;

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Mirror;
using OWML.Common;
using QSB;
using QSB.Player;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using UnityEngine;
// ReSharper disable All

namespace QSBGameModes{
    public static class Utils{

        public static IModHelper ModHelper{
            get{ return QSBGameModes.instance.ModHelper; }
        }

        public static bool DebugMode => Utils.ModHelper.OwmlConfig.DebugMode;
        
        public static void WriteLine(string s, MessageType t, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] long callerLineNumber = 0, [CallerMemberName] string callerMember= ""){
            if (Utils.DebugMode)
            {
                int id = (!QSBCore.IsInMultiplayer) ? -1 : (int)QSBPlayerManager.LocalPlayerId;
                string filename = Path.GetFileName(callerFilePath);
                filename = filename.Remove(filename.Length - 3);
                s = $"{id} ({filename}::{callerMember}:{callerLineNumber}) {s}";
            }
            QSBGameModes.instance.ModHelper.Console.WriteLine(s, t);
        }
        
        public static void WriteLine(string s, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] long callerLineNumber = 0, [CallerMemberName] string callerMember= ""){
            if (Utils.DebugMode){
                int id = (!QSBCore.IsInMultiplayer) ? -1 : (int)QSBPlayerManager.LocalPlayerId;
                string filename = Path.GetFileName(callerFilePath);
                filename = filename.Remove(filename.Length - 3);
                s = $"{id} ({filename}::{callerMember}:{callerLineNumber}) {s}";
            }
            QSBGameModes.instance.ModHelper.Console.WriteLine(s, MessageType.Info);
        }

        public static Coroutine RunWhen(Func<bool> predicate, Action action, Coroutine c){
            if(c != null) QSBGameModes.instance.StopCoroutine(c);
            return RunWhen(predicate, action);
        }

        public static Coroutine RunWhen(Func<bool> predicate, Action action) => 
            QSBGameModes.instance.StartCoroutine(WaitUntil(predicate, action));

        public static Coroutine RunWhenState(GameState state, Func<bool> predicate, Action action) =>
            Utils.RunWhen(() => GameManager.state == state, action);

        public static Coroutine RunWhenNotState(GameState state, Func<bool> predicate, Action action) =>
            Utils.RunWhen(() => GameManager.state != state, action);

        public static Coroutine RunWhenState(GameState state, Action action) =>
            RunWhenState(state, () => true, action);
        public static Coroutine RunWhenNotState(GameState state, Action action) =>
            RunWhenNotState(state, () => true, action);

        public static Coroutine WaitFor(float time, Action action) => 
            QSBGameModes.instance.StartCoroutine(WaitUntil(time, action));

        public static Coroutine StartCoroutine(IEnumerator routine){
            return QSBGameModes.instance.StartCoroutine(routine);
        }
        
        public static void StopCoroutine(Coroutine routine){
            QSBGameModes.instance.StopCoroutine(routine);
        }

        private static IEnumerator WaitUntil(float time, Action action){
            yield return new WaitForSeconds(time);
            action();
        }

        private static IEnumerator WaitUntil(Func<bool> predicate, Action action){
            yield return new WaitUntil(predicate);
            action();
        }

        public static List<NotificationDisplay.NotificationDisplayData> getNotificationDisplayData(NotificationData data){
            var datas = new List<NotificationDisplay.NotificationDisplayData>();
            foreach (NotificationDisplay display in NotificationManager.SharedInstance._notifiableElements){
                foreach (var notificationData in display._listDisplayData){
                    if (notificationData.Data == data){
                        datas.Add(notificationData);
                    }
                }
            }
            return datas;
        }
    }
}