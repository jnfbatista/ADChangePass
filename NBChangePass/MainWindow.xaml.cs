using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NBChangePass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CyclePassword(object sender, RoutedEventArgs e)
        {
            //disable button
            cycleButton.IsEnabled = false;
            cycleButton.Content = "Cycling";

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username.Text))
                    {

                        if (user == null)
                        {
                            cycleButton.Content = "User not found. Try Again.";
                            cycleButton.IsEnabled = true;
                            return;
                        }

                        var currentPwd = password.Password;

                        var nextPwd = password.Password;

                        for (int i = 0; i < 5; i++)
                        {
                            nextPwd = nextPwd + "1";

                            cycleButton.Content += ".";

                            user.ChangePassword(currentPwd, nextPwd);

                            currentPwd = nextPwd;

                            System.Threading.Thread.Sleep(5000);

                        }


                        user.ChangePassword(currentPwd, password.Password);
                    }
                }

                cycleButton.Content = "Cycled!";

            }
            catch (PrincipalServerDownException)
            {

                MessageBox.Show("Active Directory Server not found.", "Alert");
                throw;
            }

        }
    }
}
