namespace DAI
{
    /// <summary>
    /// Summary description for DAIFileDetails
    /// </summary>
    public class DAIFileDetails
    {
        private byte[] fileContents;
        private long fileSize;
        private int bytesSent;
        private string fileName;
        private string errorMessage;

        public byte[] FileContents
        {
            get { return fileContents; }
            set { fileContents = value; }
        }

        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public int BytesSent
        {
            get { return bytesSent; }
            set { bytesSent = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public DAIFileDetails()
        {
        }

        public DAIFileDetails(byte[] fileContents, string fileName, long fileSize, int bytesSent, string errorMessage)
        {
            this.fileContents = fileContents;
            this.fileName = fileName;
            this.fileSize = fileSize;
            this.bytesSent = bytesSent;
            this.errorMessage = errorMessage;
        }
    }
}