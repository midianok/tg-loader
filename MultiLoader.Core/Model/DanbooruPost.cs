using Newtonsoft.Json;

namespace MultiLoader.Core.Model
{
    class DanbooruPost
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "uploader_id")]
        public int UploaderId { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "md5")]
        public string Md5 { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "last_comment_bumped_at")]
        public object LastCommentBumpedAt { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "image_width")]
        public int ImageWidth { get; set; }

        [JsonProperty(PropertyName = "image_height")]
        public int ImageHeight { get; set; }

        [JsonProperty(PropertyName = "tag_string")]
        public string TagString { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "is_note_locked")]
        public bool IsNoteLocked { get; set; }

        [JsonProperty(PropertyName = "fav_count")]
        public int FavCount { get; set; }

        [JsonProperty(PropertyName = "file_ext")]
        public string FileExt { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "last_noted_at")]
        public object LastNotedAt { get; set; }

        [JsonProperty(PropertyName = "is_rating_locked")]
        public bool IsRatingLocked { get; set; }

        [JsonProperty(PropertyName = "parent_id")]
        public object ParentId { get; set; }

        [JsonProperty(PropertyName = "has_children")]
        public bool HasChildren { get; set; }

        [JsonProperty(PropertyName = "approver_id")]
        public string ApproverId { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "tag_count_general")]
        public int TagCountGeneral { get; set; }

        [JsonProperty(PropertyName = "tag_count_artist")]
        public int TagCountArtist { get; set; }

        [JsonProperty(PropertyName = "tag_count_character")]
        public int TagCountCharacter { get; set; }

        [JsonProperty(PropertyName = "tag_count_copyright")]
        public int TagCountCopyright { get; set; }

        [JsonProperty(PropertyName = "file_size")]
        public int FileSize { get; set; }

        [JsonProperty(PropertyName = "is_status_locked")]
        public bool IsStatusLocked { get; set; }

        [JsonProperty(PropertyName = "fav_string")]
        public string FavString { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "pool_string")]
        public string PoolString { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "up_score")]
        public int UpScore { get; set; }

        [JsonProperty(PropertyName = "down_score")]
        public int DownScore { get; set; }

        [JsonProperty(PropertyName = "is_pending")]
        public bool IsPending { get; set; }

        [JsonProperty(PropertyName = "is_flagged")]
        public bool IsFlagged { get; set; }

        [JsonProperty(PropertyName = "is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty(PropertyName = "tag_count")]
        public int TagCount { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "is_banned")]
        public bool IsBanned { get; set; }

        [JsonProperty(PropertyName = "pixiv_id")]
        public object PixivId { get; set; }

        [JsonProperty(PropertyName = "last_commented_at")]
        public object LastCommentedAt { get; set; }

        [JsonProperty(PropertyName = "has_active_children")]
        public bool HasActiveChildren { get; set; }

        [JsonProperty(PropertyName = "bit_flags")]
        public int BitFlags { get; set; }

        [JsonProperty(PropertyName = "uploader_name")]
        public string UploaderName { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "has_large")]
        public bool HasLarge { get; set; }

        [JsonProperty(PropertyName = "tag_string_artist")]
        public string TagStringArtist { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "tag_string_character")]
        public string TagStringCharacter { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "tag_string_copyright")]
        public string TagStringCopyright { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "tag_string_general")]
        public string TagStringGeneral { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "has_visible_children")]
        public bool HasVisibleChildren { get; set; }

        [JsonProperty(PropertyName = "children_ids")]
        public object ChildrenIds { get; set; }

        [JsonProperty(PropertyName = "file_url")]
        public string FileUrl { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "large_file_url")]
        public string LargeFileUrl { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "preview_file_url")]
        public string PreviewFileUrl { get; set; } = string.Empty;

    }
}
