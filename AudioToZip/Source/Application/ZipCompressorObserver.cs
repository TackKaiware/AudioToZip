namespace AudioToZip
{
    /// <summary>
    /// ZIP圧縮オブジェクトの状態を監視するクラス
    /// </summary>
    public class ZipCompressorObserver : FileConverterObserver
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ZipCompressorObserver() : base() { }

        #endregion コンストラクタ

        #region 非公開メソッド

        /// <summary>
        /// 監視対象クラスが待機している場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected override string GetWaitMessage( FileConverter converter )
            => "圧縮対象のファイルがありません。";

        /// <summary>
        /// 監視対象クラスが処理を開始した場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected override string GetStartMessage( FileConverter converter )
            => "\n圧縮処理を開始します。 " +
               $"[{ converter.SourceFileType.GetExtention() }] -> " +
               $"[{ converter.DestinationFileType.GetExtention() }]";

        /// <summary>
        /// 監視対象クラスが処理中の場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected override string GetRunningMessage( FileConverter converter )
            => $"{ converter.TargetFilePath } を圧縮しました。 ({ converter.ProcessedCount }/{ converter.TotalCount })";

        /// <summary>
        /// 監視対象クラスが処理完了した場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected override string GetCompleteMessage( FileConverter converter )
            => $"圧縮ファイルを { converter.OutputPath } に出力しました。";

        /// <summary>
        /// 監視対象クラスが処理失敗した場合のメッセージ
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected override string GetFailedMessage( FileConverter converter )
            => "圧縮処理に失敗しました。";

        #endregion 非公開メソッド
    }
}