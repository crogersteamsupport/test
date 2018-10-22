﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.Data;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Linq;

namespace TeamSupport.Permissions
{
    public class UserRights
    {
        //public bool CanOpenAttachment(ConnectionContext context, AttachmentProxy attachment)
        public static bool CanOpenAttachment(LoginUser loginUser, Attachment attachment)
        {
            switch (attachment.RefType)
            {
                case ReferenceType.Actions: 
                case ReferenceType.Tasks:
                case ReferenceType.Organizations:
                    return OrganizationOrParentOrganization(loginUser, attachment);

                case ReferenceType.ProductVersions:
                    return CheckProduct(loginUser, attachment);

                case ReferenceType.UserPhoto:
                case ReferenceType.CustomerHubLogo:
                default:
                    return true;  // no authentication required (HubLogo...)
            }
        }

        static bool TryGetParentID(LoginUser loginUser, out int parentID)
        {
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            using (DataContext db = new DataContext(connection))
            {
                string query = $"SELECT ParentID FROM Organizations WHERE OrganizationID={loginUser.OrganizationID}";
                parentID = db.ExecuteQuery<int>(query).Min();
            }
            return parentID != 1;   // no parent organization?
        }

        static bool OrganizationOrParentOrganization(LoginUser loginUser, Attachment attachment)
        {
            // same organization
            if (attachment.OrganizationID == loginUser.OrganizationID)
                return true;

            // attachment in parent organization
            int parentID;
            if (TryGetParentID(loginUser, out parentID))
                return (attachment.OrganizationID == parentID);

            return false;
        }

        static bool CheckProduct(LoginUser loginUser, Attachment attachment)
        {
            // Member of parent org?
            if (attachment.OrganizationID == loginUser.OrganizationID)
                return true;

            // contact has access to product version?
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            using (DataContext db = new DataContext(connection))
            {
                string query = @"Select a.AttachmentID FROM Users as u 
                    JOIN UserProducts up on up.UserID = u.UserID 
                    JOIN Products p on p.ProductID = up.ProductID 
                    JOIN ProductVersions pv on pv.ProductID = up.ProductID 
                    JOIN Attachments a on a.RefID = pv.ProductVersionID 
                    WHERE " + $"a.AttachmentID={attachment.AttachmentID} AND u.UserID={loginUser.UserID} AND a.RefType={(int)ReferenceType.ProductVersions}";

                bool any = db.ExecuteQuery<int>(query).Any();
                return any;
            }
        }

    }
}
