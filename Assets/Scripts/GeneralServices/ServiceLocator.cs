namespace Services
{
    public static class ServiceLocator<T>
    {
        static T _service;

        public static void Bind(T service)
        {
            _service = service;
        }

        public static T Get()
        {
            if (_service == null)
                throw new System.Exception($"You didn't bind any instance for {typeof(T).Name} service!!!");
            return _service;
        }
    }
}
