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

        private const string defaultCycleString = "Cycle";
        private const string defaultChangeString = "Change";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CyclePassword(object sender, RoutedEventArgs e)
        {
            var uname = username.Text;
            var pwd = password.Password;


            if (cycleOption.IsChecked.Value)
            {
                var nCycles = int.Parse(nCycle.Text);
                Task.Run(() => CyclePasswordAsync(uname, pwd, nCycles));
            }
            else
            {
                var newPass = newPassword.Password;
                Task.Run(() => CyclePasswordAsync(uname, pwd, newPass));
            }
        }

        /// <summary>
        /// Changes the password in another Thread
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void CyclePasswordAsync(string username, string password, int nCycles = 5)
        {
            this.Dispatcher.Invoke(() =>
            {
            //disable button
            submit.IsEnabled = false;
                submit.Content = "Cycling";
            });

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username))
                    {

                        if (user == null)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                submit.Content = "User not found. Try Again.";
                                submit.IsEnabled = true;
                                return;
                            });
                        }

                        var currentPwd = password;

                        var nextPwd = password;

                        for (int i = 0; i < nCycles; i++)
                        {
                            nextPwd = nextPwd + "1";

                            this.Dispatcher.Invoke(() =>
                            {
                                submit.Content += ".";
                            });

                            user.ChangePassword(currentPwd, nextPwd);

                            currentPwd = nextPwd;

                            System.Threading.Thread.Sleep(5000);

                        }


                        user.ChangePassword(currentPwd, password);
                    }
                }

                this.Dispatcher.Invoke(() =>
                {
                    submit.Content = "Cycled!";
                });
            }
            catch (PrincipalServerDownException)
            {
                MessageBox.Show("Active Directory Server not found.", "Alert");
            }
        }


        private void ChangePass(string username, string password, string newPassword)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username))
                    {

                        if (user == null)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                submit.Content = "User not found. Try Again.";
                                submit.IsEnabled = true;
                                return;
                            });
                        }

                        user.ChangePassword(password, new);
                    }
                }

                this.Dispatcher.Invoke(() =>
                {
                    submit.Content = "Changed!";
                });
            }
            catch (PrincipalServerDownException)
            {
                MessageBox.Show("Active Directory Server not found.", "Alert");
            }
        }

        private void SetMode(object sender, RoutedEventArgs e)
        {
            if (!cycleOption.IsChecked.HasValue || newPasswordLabel == null || newPassword == null)
                return;

            if (cycleOption.IsChecked.Value)
            {
                nCycleLabel.Visibility = Visibility.Visible;
                nCycleContents.Visibility = Visibility.Visible;
                newPasswordLabel.Visibility = Visibility.Hidden;
                newPassword.Visibility = Visibility.Hidden;

                submit.Content = defaultCycleString;
            }
            else
            {
                nCycleLabel.Visibility = Visibility.Hidden;
                nCycleContents.Visibility = Visibility.Hidden;
                newPasswordLabel.Visibility = Visibility.Visible;
                newPassword.Visibility = Visibility.Visible;

                submit.Content = defaultChangeString;
            }

        }
    }
}
