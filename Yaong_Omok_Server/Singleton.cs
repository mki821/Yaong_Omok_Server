namespace Yaong_Omok_Server {
    public class Singleton<T> where T : Singleton<T>, new() {
        private static Lazy<T>? _instance = null;

        public static T Instance {
            get {
                if(!Exist()) {
                    T instance = new T();
                    _instance = new Lazy<T>(instance);
                }

                return _instance.Value;
            }
        }

        private static bool Exist() {
            return _instance != null && _instance.IsValueCreated;
        }
    }
}
