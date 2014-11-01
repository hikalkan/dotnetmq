/*
DotNetMQ - A Complete Message Broker For .NET
Copyright (C) 2011 Halil ibrahim KALKAN

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System.Windows.Forms;

namespace MDS.GUI
{
    /// <summary>
    /// This class is created to make easy common GUI tasks.
    /// </summary>
    public static class MDSGuiHelper
    {
        #region MessageBoxes

        /// <summary>
        /// Show a message box that show an error.
        /// </summary>
        /// <param name="message">Message to show</param>
        public static void ShowErrorMessage(string message)
        {
            ShowErrorMessage(message, "Error!");
        }

        /// <summary>
        /// Show a message box that show an error.
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="caption">Caption of message box</param>
        public static void ShowErrorMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Show a message box that show an warning.
        /// </summary>
        /// <param name="message">Message to show</param>
        public static void ShowWarningMessage(string message)
        {
            ShowWarningMessage(message, "Warning!");
        }

        /// <summary>
        /// Show a message box that show an warning.
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="caption">Caption of message box</param>
        public static void ShowWarningMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Shows a messagebox to ask a question to user.
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="caption">Caption of message box</param>
        /// <returns>User's choice</returns>
        public static DialogResult ShowQuestionDialog(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Shows a messagebox to ask a question to user.
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="caption">Caption of message box</param>
        /// <param name="defaultButton">Default selected button</param>
        /// <returns>User's choice</returns>
        public static DialogResult ShowQuestionDialog(string message, string caption, MessageBoxDefaultButton defaultButton)
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, defaultButton);
        }

        /// <summary>
        /// Shows a messagebox that shows an information.
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="caption">Caption of message box</param>
        public static void ShowInfoDialog(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}
