using System;

namespace AudioToZip
{
    /// <summary>
    /// FileTypeEnum拡張クラス
    /// </summary>
    public static class FileTypeEnumExtension
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

