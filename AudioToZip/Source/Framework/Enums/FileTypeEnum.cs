namespace AudioToZip
{
    /// <summary>
    /// ファイルの種類
    /// </summary>
    public enum FileTypeEnum
    {
        /// <summary>
        /// 全てのファイル(*.*)
        /// </summary>
        All,

        /// <summary>
        /// WAVEファイル(*.wav)
        /// </summary>
        Wave,

        /// <summary>
        /// MP3ファイル(*.mp3)
        /// </summary>
        Mp3,

        /// <summary>
        /// ZIPファイル(*.zip)
        /// </summary>
        Zip,
    }
}