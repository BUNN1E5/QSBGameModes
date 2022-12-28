using System;
using System.Collections;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace HideAndSeek{
    public static class Utils {
        
        public static IModUnityEvents Unity = HideAndSeek.instance.ModHelper.Events.Unity; 
        public static void WriteLine(string s, MessageType t){
            HideAndSeek.instance.ModHelper.Console.WriteLine(s, t);
        }
        
        public static void WriteLine(string s){
            HideAndSeek.instance.ModHelper.Console.WriteLine(s, MessageType.Info);
        }

        public static Coroutine RunWhen(Func<bool> predicate, Action action, Coroutine c){
            HideAndSeek.instance.StopCoroutine(c);
            return RunWhen(predicate, action);
        }

        public static Coroutine RunWhen(Func<bool> predicate, Action action) => 
            HideAndSeek.instance.StartCoroutine(WaitUntil(predicate, action));

        private static IEnumerator WaitUntil(Func<bool> predicate, Action action){
            yield return new WaitUntil(predicate);
            action();
        }

    }
}