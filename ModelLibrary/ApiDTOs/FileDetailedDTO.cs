using System.ComponentModel.DataAnnotations;

namespace ModelLib.ApiDTOs
{
    public class FileDetailedDTO : FileListDTO
    {
        public byte[]? Bytes { get; set; }

        public bool ContainsFile()
        {
            return Bytes != null && ContentType != null;
        }
    }
}
