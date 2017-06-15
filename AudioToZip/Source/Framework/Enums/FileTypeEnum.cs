using System;

namespace AudioToZip
{
    /// <summary>
    /// ファイルの種類
    /// </summary>
    internal enum FileTypeEnum
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

    /// <summary>
    /// FileTypeEnum拡張クラス
    /// </summary>
    internal static class FileTypeEnumExtension
    {
        /// <summary>
        /// オーディオフォーマットに対応したファイル拡張子を取得する
        /// </summary>
        /// <param name="format"></param>
        /// <param name="withPeriod">true = ".txt", false = "txt"</param>
        /// <returns></returns>
        public static string GetExtention( this FileTypeEnum format, bool withPeriod = false )
        {
            var extension = string.Empty;
            switch ( format )
            {
                case FileTypeEnum.Wave:
                    extension = "wav";
                    break;

                case FileTypeEnum.Mp3:
                    extension = "mp3";
                    break;

                case FileTypeEnum.Zip:
                    extension = "zip";
                    break;

                default:
                    throw new NotImplementedException();
            }
            return withPeriod ? "." + extension : extension;
        }
    }
}