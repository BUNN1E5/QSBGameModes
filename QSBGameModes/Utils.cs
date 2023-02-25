using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using OWML.Common;
using QSB.Player;
using UnityEngine;

namespace QSBGameModes{
    public static class Utils{

        public static IModHelper ModHelper{
            get{ return QSBGameModes.instance.ModHelper; }
        }
        public static void WriteLine(string s, MessageType t){
            QSBGameModes.instance.ModHelper.Console.WriteLine(s, t);
        }
        
        public static void WriteLine(string s){
            QSBGameModes.instance.ModHelper.Console.WriteLine(s, MessageType.Info);
        }

        public static Coroutine RunWhen(Func<bool> predicate, Action action, Coroutine c){
            QSBGameModes.instance.StopCoroutine(c);
            return RunWhen(predicate, action);
        }

        public static Coroutine RunWhen(Func<bool> predicate, Action action) => 
            QSBGameModes.instance.StartCoroutine(WaitUntil(predicate, action));

        private static IEnumerator WaitUntil(Func<bool> predicate, Action action){
            yield return new WaitUntil(predicate);
            action();
        }
    }
}