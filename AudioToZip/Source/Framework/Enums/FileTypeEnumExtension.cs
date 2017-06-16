using System;

namespace AudioToZip
{
    /// <summary>
    /// FileTypeEnum�g���N���X
    /// </summary>
    public static class FileTypeEnumExtension
    {
        /// <summary>
        /// �I�[�f�B�I�t�H�[�}�b�g�ɑΉ������t�@�C���g���q���擾����
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

