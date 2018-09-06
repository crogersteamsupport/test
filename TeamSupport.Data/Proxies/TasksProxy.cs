using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    public partial class Task : BaseItem
    {
        public TaskProxy GetProxy()
        {
            TaskProxy result = new TaskProxy();
            result.CompletionComment = this.CompletionComment;
            result.NeedsIndexing = this.NeedsIndexing;
            result.IsDismissed = this.IsDismissed;
            result.ReminderDueDate = this.ReminderDueDate;
            result.ReminderID = this.ReminderID;
            result.ModifierID = this.ModifierID;
            result.CreatorID = this.CreatorID;
            result.ParentID = this.ParentID;
            result.IsComplete = this.IsComplete;
            result.UserID = this.UserID;
            result.Description = this.Description;
            result.Name = this.Name;
            result.OrganizationID = this.OrganizationID;
            result.TaskID = this.TaskID;

            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
            result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);

            result.DateCompleted = this.DateCompletedUtc == null ? this.DateCompletedUtc : DateTime.SpecifyKind((DateTime)this.DateCompletedUtc, DateTimeKind.Utc);
            result.DueDate = this.DueDateUtc == null ? this.DueDateUtc : DateTime.SpecifyKind((DateTime)this.DueDateUtc, DateTimeKind.Utc);

            return result;
        }
    }
}
