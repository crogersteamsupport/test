using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TeamSupport.Data
{
	public partial class UserSetting
	{
	}

	public partial class UserSettings
	{
		public virtual void LoadByUserSettingKey(int userID, string settingKey)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SET NOCOUNT OFF; SELECT [UserSettingID], [UserID], [SettingKey], [SettingValue], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[UserSettings] WHERE ([UserID] = @UserID and [SettingKey] = @SettingKey);";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("SettingKey", settingKey);
				command.Parameters.AddWithValue("UserID", userID);
				Fill(command);
			}
		}

		public static string ReadString(LoginUser loginUser, string key, string defaultValue)
		{
			return ReadString(loginUser, loginUser.UserID, key, defaultValue);
		}

		public static string ReadString(LoginUser loginUser, int userID, string key, string defaultValue = "")
		{
			string result = defaultValue;
			using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
			{
				connection.Open();

				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "SELECT SettingValue FROM UserSettings WHERE (UserID=@UserID) AND (SettingKey=@SettingKey)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@UserID", userID);
				command.Parameters.AddWithValue("@SettingKey", key);

				SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
				if (reader.Read())
				{
					result = (string)reader[0];
				}
				reader.Close();
				connection.Close();
			}

			return result;
		}

		public static void DeleteOrganizationSettings(LoginUser loginUser, int organizationID)
		{
			UserSettings settings = new UserSettings(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "DELETE FROM UserSettings WHERE UserID IN (SELECT UserID FROM Users WHERE OrganizationID = @OrganizationID)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				settings.ExecuteNonQuery(command, "UserSettings");
			}

		}

		public static void WriteString(LoginUser loginUser, string key, string value)
		{
			WriteString(loginUser, loginUser.UserID, key, value);
		}

		public static void WriteString(LoginUser loginUser, int userID, string key, string value)
		{


			using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
			{
				connection.Open();

				using (SqlCommand command = new SqlCommand())
				{
					command.Connection = connection;
					command.CommandText = @"
IF EXISTS(SELECT * FROM UserSettings WHERE (UserID=@UserID) AND (SettingKey=@SettingKey))
  BEGIN
    UPDATE [dbo].[UserSettings]
  	SET SettingValue = @SettingValue
    WHERE (UserID = @UserID)
	  AND (SettingKey = @SettingKey)
  END
  ELSE
  BEGIN
	  INSERT INTO [dbo].[UserSettings]
	  (
		[UserID],
		[SettingKey],
		[SettingValue])
	  VALUES (
		@UserID,
		@SettingKey,
		@SettingValue)
  END";
					command.Parameters.AddWithValue("@UserID", userID);
					command.Parameters.AddWithValue("@SettingKey", key);
					command.Parameters.AddWithValue("@SettingValue", value);
					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}
	}
}
