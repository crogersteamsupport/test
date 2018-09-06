using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    public partial class TasksViewItem : BaseItem
    {
        public TasksViewItemProxy GetProxy()
        {
            TasksViewItemProxy result = new TasksViewItemProxy();
            result.NeedsIndexing = this.NeedsIndexing;
            result.ModifierID = this.ModifierID;
            result.CreatorID = this.CreatorID;
            result.Creator = this.Creator;
            result.UserName = this.UserName;
            result.TaskParentName = this.TaskParentName;
            result.HasEmailSent = this.HasEmailSent;
            result.IsDismissed = this.IsDismissed;
            result.ParentID = this.ParentID;
            result.CompletionComment = this.CompletionComment;
            result.IsComplete = this.IsComplete;
            result.UserID = this.UserID;
            result.Description = this.Description;
            result.Name = this.Name;
            result.OrganizationID = this.OrganizationID;
            result.TaskID = this.TaskID;

            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
            result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);

            result.ReminderDueDate = this.ReminderDueDateUtc == null ? this.ReminderDueDateUtc : DateTime.SpecifyKind((DateTime)this.ReminderDueDateUtc, DateTimeKind.Utc);
            result.DateCompleted = this.DateCompletedUtc == null ? this.DateCompletedUtc : DateTime.SpecifyKind((DateTime)this.DateCompletedUtc, DateTimeKind.Utc);
            result.DueDate = this.DueDateUtc == null ? this.DueDateUtc : DateTime.SpecifyKind((DateTime)this.DueDateUtc, DateTimeKind.Utc);

            return result;
        }
    }
}
