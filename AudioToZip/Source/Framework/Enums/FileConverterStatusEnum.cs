namespace AudioToZip
{
    /// <summary>
    /// FileConverterオブジェクトの状態
    /// </summary>
    public enum FileConverterStatusEnum
    {
        /// <summary>
        /// 待機中
        /// </summary>
        Wait,

        /// <summary>
        /// 処理開始
        /// </summary>
        Start,

        /// <summary>
        /// 処理中
        /// </summary>
        Running,

        /// <summary>
        /// 処理完了
        /// </summary>
        Complete,

        /// <summary>
        /// 処理失敗
        /// </summary>
        Failed
    }
}