namespace AudioToZip
{
    /// <summary>
    /// 監視可能インターフェース
    /// </summary>
    internal interface IObservable
    {
        /// <summary>
        /// 監視クラスを追加する
        /// </summary>
        /// <param name="observer"></param>
        void AddObserver( FileConverterObserver observer );

        /// <summary>
        /// 監視クラスを削除する
        /// </summary>
        /// <param name="observer"></param>
        void RemoveObserver( FileConverterObserver observer );

        /// <summary>
        /// 監視クラスに状態変化を通知する
        /// </summary>
        /// <param name="status"></param>
        void NotifyObservers();
    }
}