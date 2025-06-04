namespace FriChat.Infrastructure.Constants
{
    public class ModelConstants
    {
        /// <summary>
        /// Student Name Minimum Length
        /// </summary>
        public const int UserNameMinLength = 2;
        /// <summary>
        /// Student Name Maximum Length
        /// </summary>
        public const int UserNameMaxLength = 100;

        /// <summary>
        /// Profile Picture Minimum Length
        /// </summary>
        public const int ProfilePictureMinLength = 20;
        /// <summary>
        /// Profile Picture Maximum Length
        /// </summary>
        public const int ProfilePictureMaxLength = 255;

        /// <summary>
        /// Message Content Minimum Length
        /// </summary>
        public const int MessageContentMinLength = 1;

        /// <summary>
        /// Message Content Maximum Length
        /// </summary>
        public const int MessageContentMaxLength = 1000;

        /// <summary>
        /// Message Attachment URL Minimum Length
        /// </summary>
        public const int MessageTypeMinLength = 1;

        /// <summary>
        /// Message Attachment URL Maximum Length
        /// </summary>
        public const int AttachmentUrlMaxLength = 500;
    }
}
