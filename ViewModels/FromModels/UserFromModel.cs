using MindScribe.Models;
using MindScribe.ViewModels.EditViewModel;

namespace MindScribe.ViewModels.FromModel
{
    public static class UserFromModel
    {
        public static User Convert(this User user, UserEditViewModel usereditvm)
        {
            user.Image = usereditvm.Image;
            user.LastName = usereditvm.LastName;
            user.FirstName = usereditvm.FirstName;
            user.Email = usereditvm.Email;
            user.About = usereditvm.About;

            return user;
        }
    }
}
