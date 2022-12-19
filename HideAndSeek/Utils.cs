using OWML.Common;

namespace HideAndSeek{
    public static class Utils {
        
        public static IModUnityEvents Unity = HideAndSeek.instance.ModHelper.Events.Unity; 
        public static void WriteLine(string s, MessageType t){
            HideAndSeek.instance.ModHelper.Console.WriteLine(s, t);
        }
        
    }
}