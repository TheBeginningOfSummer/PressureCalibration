namespace Module
{
    public class Service
    {
        #region 单例模式
        private static Service? _instance;
        private static readonly object _instanceLock = new();
        public static Service Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                        _instance = new Service();
                }
                return _instance;
            }
        }
        #endregion

        public readonly StatisticTime Statistic = new();

        public Service()
        {

        }

    }
}
