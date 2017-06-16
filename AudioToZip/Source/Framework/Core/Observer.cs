namespace AudioToZip
{
    /// <summary>
    /// 監視クラスの最上位クラス
    /// </summary>
    public abstract class Observer
    {
        /// <summary>
        /// 監視対象の状態変化時に呼び出される
        /// </summary>
        /// <param name="subject"></param>
        public abstract void Update( IObservable subject );
    }
}