using XSNotifications.Enum;
using XSNotifications.Exception;

namespace XSNotifications.Helpers
{
    public static class XSGlobals
    {
        public const int DefaultServerPort = 42069;

        // Content height, not title
        public const float MinHeight = 0.0f;
        public const float MaxHeight = 250.0f; // Currently arbitrary. There's some reasonable upper bound to find here.

        public const float MinVolume = 0.0f;
        public const float MaxVolume = 1.0f;

        public const float MinFontSize = 0.0f;
        public const float MaxFontSize = 100.0f; // Currently arbitrary. There's some reasonable upper bound to find here.
        public const float DefaultFontSize = 20.0f; // This is not mutable so that we retain a reference point as in the instance no tag is provided to XSO, it will assume this font size. We don't want to wrap content if we don't have to as it reduces possible message size.

        public const float MinOpacity = 0.0f;
        public const float MaxOpacity = 1.0f;

        public const float MinTimeout = 0.0f;
        public const float MaxTimeout = 60.0f; // Currently arbitrary. There's some reasonable upper bound to find here.

        public const int DefaultIndex = 0; // What else would this be?

        public static XSMessageType DefaultMessageType = XSMessageType.Notification;

        public static string DefaultAudioPath = GetBuiltInAudioSourceString(XSAudioDefault.Default);
        public static string DefaultIcon = GetBuiltInIconTypeString(XSIconDefaults.Default);
        public static string DefaultSourceApp = string.Empty;
        public static string DefaultTitle = string.Empty;

        public static bool DefaultUseBase64Icon = false;

        private static float defaultHeight = 175.0f;
        public static float DefaultHeight
        {
            get => defaultHeight;
            set
            {
                if (value > MaxHeight || value < MinHeight)
                    throw new XSFormatException($"Content height must be {MinHeight}f < value < {MaxHeight}f and cannot fall outside of those bounds.");
                defaultHeight = value;
            }
        }

        private static float defaultOpacity = MaxOpacity;
        public static float DefaultOpacity
        {
            get => defaultOpacity;
            set
            {
                if (value > MaxOpacity || value < MinOpacity)
                    throw new XSFormatException($"Opacity must be {MinOpacity}f < value < {MaxOpacity}f and cannot fall outside of those bounds.");
                defaultOpacity = value;
            }
        }

        private static float defaultTimeout = 3.0f;
        public static float DefaultTimeout
        {
            get => defaultTimeout;
            set
            {
                if (value > MaxTimeout || value < MinTimeout)
                    throw new XSFormatException($"Timeout must be {MinTimeout}f < value < {MaxTimeout}f and cannot fall outside of those bounds.");
                defaultTimeout = value;
            }
        }

        private static float defaultVolume = 0.7f;
        public static float DefaultVolume
        {
            get => defaultVolume;
            set
            {
                if (value > MaxVolume || value < MinVolume)
                    throw new XSFormatException($"Volume must be {MinVolume}f < value < {MaxVolume}f and cannot fall outside of those bounds.");
                defaultVolume = value;
            }
        }

        public static string GetBuiltInIconTypeString(XSIconDefaults iconType)
        {
            switch (iconType)
            {
                case XSIconDefaults.Error:
                    return "error";
                case XSIconDefaults.Warning:
                    return "warning";
                default:
                    return "default";
            }
        }

        public static string GetBuiltInAudioSourceString(XSAudioDefault audioType)
        {
            switch (audioType)
            {
                case XSAudioDefault.Error:
                    return "error";
                case XSAudioDefault.Warning:
                    return "warning";
                default:
                    return "default";
            }
        }
    }
}
