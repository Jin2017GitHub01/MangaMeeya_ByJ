using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace MangaMeeya_by_Jin.Services
{
    /// <summary>
    /// ZIP 파일에서 이미지 파일을 메모리로 직접 로드하여 관리하는 클래스입니다.
    /// 임시 폴더에 추출하지 않고 ZIP 파일에서 직접 이미지 스트림을 읽어옵니다.
    /// </summary>
    public class ZipFileLoader : IDisposable
    {
        private ZipFile _zipFile;
        private List<ZipEntry> _imageEntries = new List<ZipEntry>();
        private string _currentZipPath = "";
        private bool _disposed = false;

        /// <summary>ZIP 내 이미지 항목의 인덱스 리스트 (표시용)</summary>
        public IReadOnlyList<int> ImageIndices => _imageEntries.Select((e, i) => i).ToList().AsReadOnly();
        public string CurrentZipPath => _currentZipPath;
        public int ImageCount => _imageEntries.Count;

        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };

        /// <summary>
        /// 주어진 파일명이 이미지 파일인지 확인합니다.
        /// </summary>
        public static bool IsImageFile(string fileName)
        {
            return ImageExtensions.Contains(Path.GetExtension(fileName).ToLower());
        }

        /// <summary>
        /// ZIP 파일을 열고 이미지 항목들의 인덱스를 메모리에 저장합니다.
        /// 실제 이미지 데이터는 GetImageStream() 호출 시에만 읽습니다.
        /// </summary>
        /// <param name="zipPath">열 ZIP 파일의 전체 경로</param>
        public void LoadZipFile(string zipPath)
        {
            // 기존 ZIP 파일 정리
            Cleanup();

            _currentZipPath = zipPath;

            // ZIP 파일 열기 (스트림 유지)
            _zipFile = new ZipFile(zipPath);

            // 이미지 항목만 필터링하여 저장 (실제 데이터는 읽지 않음)
            _imageEntries = _zipFile
                .Cast<ZipEntry>()
                .Where(entry => !entry.IsDirectory && IsImageFile(entry.Name))
                .OrderBy(entry => entry.Name) // ZIP 내 파일명 순서로 정렬
                .ToList();
        }

        /// <summary>
        /// 지정된 인덱스의 이미지 데이터를 ZIP 파일에서 직접 읽어 MemoryStream으로 반환합니다.
        /// </summary>
        /// <param name="index">이미지 인덱스 (0부터 시작)</param>
        /// <returns>이미지 데이터를 담은 MemoryStream</returns>
        public MemoryStream GetImageStream(int index)
        {
            if (_zipFile == null || index < 0 || index >= _imageEntries.Count)
                return null;

            ZipEntry entry = _imageEntries[index];

            using (Stream zipStream = _zipFile.GetInputStream(entry))
            {
                var memoryStream = new MemoryStream();
                zipStream.CopyTo(memoryStream);
                memoryStream.Position = 0; // 읽기 위치를 처음으로
                return memoryStream;
            }
        }

        /// <summary>
        /// 리소스를 정리합니다. 열려있는 ZIP 파일을 닫습니다.
        /// </summary>
        public void Cleanup()
        {
            if (_zipFile != null)
            {
                _zipFile.Close();
                _zipFile = null;
            }
            _imageEntries.Clear();
            _currentZipPath = "";
        }

        /// <summary>
        /// IDisposable 구현. ZIP 파일 핸들을 해제합니다.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Cleanup();
                _disposed = true;
            }
        }
    }
}