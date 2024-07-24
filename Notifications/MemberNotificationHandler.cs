using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Umbraco13Demo.Notifications
{
    public class MemberNotificationHandler : INotificationAsyncHandler<MemberSavedNotification>,
        INotificationAsyncHandler<MemberSavingNotification>,
        INotificationAsyncHandler<MemberDeletedNotification>
    {
        private const string _isNewMemberKey = "isNewMember";
        private const string _currentEmailKey = "currentEmail";
        private const string _currentUsernameKey = "currentUsername";

        private readonly ILogger<MemberNotificationHandler> _logger;

        public MemberNotificationHandler(ILogger<MemberNotificationHandler> logger)
        {
            _logger = logger;
        }


        public async Task HandleAsync(MemberSavedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var member in notification.SavedEntities)
            {
                if (IsNewMember(member))
                {

                }
                // Write to the logs every time a member is saved.
                _logger.LogInformation("Member {member} has been saved and notification published!", member.Name);
            }
        }

        public async Task HandleAsync(MemberSavingNotification notification, CancellationToken cancellationToken)
        {
            foreach (var member in notification.SavedEntities)
            {
                var isNewMember = member.Id == 0;

                member.AdditionalData[_isNewMemberKey] = isNewMember;

                // Write to the logs every time a member is saving.
                _logger.LogInformation("Member {member} is saving and notification published!", member.Name);
            }
        }

        public async Task HandleAsync(MemberDeletedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var member in notification.DeletedEntities)
            {
                // Write to the logs every time a member is deleted.
                _logger.LogInformation("Member {member} has been deleted and notification published!", member.Name);
            }
        }

        private bool IsNewMember(IMember member) =>
        member.HasAdditionalData && member.AdditionalData.ContainsKey(_isNewMemberKey) && member.AdditionalData[_isNewMemberKey] is bool isNewMember && isNewMember;

        private bool IsUsernameChanged(IMember member, out string oldUsername)
        {
            oldUsername = string.Empty;

            if (!member.HasAdditionalData || !member.AdditionalData.ContainsKey(_currentUsernameKey))
            {
                return false;
            }

            if (member.AdditionalData[_currentUsernameKey].ToString().Equals(member.Username, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            oldUsername = member.AdditionalData[_currentUsernameKey].ToString();

            return true;
        }


        private bool IsEmailChanged(IMember member, out string oldEmail)
        {
            oldEmail = string.Empty;

            if (!member.HasAdditionalData || !member.AdditionalData.ContainsKey(_currentEmailKey))
            {
                return false;
            }

            if (member.AdditionalData[_currentEmailKey].ToString().Equals(member.Email, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            oldEmail = member.AdditionalData[_currentEmailKey].ToString();

            return true;
        }
    }


    public class MemberNotificationComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationAsyncHandler<MemberSavedNotification, MemberNotificationHandler>();
            builder.AddNotificationAsyncHandler<MemberSavingNotification, MemberNotificationHandler>();
            builder.AddNotificationAsyncHandler<MemberDeletedNotification, MemberNotificationHandler>();
        }
    }
}
