using System;

namespace MultiLoader.Core.Model
{
    public class ContentMetadata
    {
        public string Name { get; set; }
        public string Request { get; set; }
        public SourceType SourceType { get; set; }
        public Uri Uri { get; set; }

        #region Equals override
        protected bool Equals(ContentMetadata other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Request, other.Request) && SourceType == other.SourceType && Equals(Uri, other.Uri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ContentMetadata)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Request != null ? Request.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)SourceType;
                hashCode = (hashCode * 397) ^ (Uri != null ? Uri.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ContentMetadata left, ContentMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ContentMetadata left, ContentMetadata right)
        {
            return !Equals(left, right);
        } 
        #endregion
    }
}