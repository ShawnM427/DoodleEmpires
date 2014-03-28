using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace DoodleAnims
{
    /// <summary>
    /// Represents a simple dialog with a yes/no answer
    /// </summary>
    public partial class BoolDialog : Form
    {
        /// <summary>
        /// Creates a new boolean dialog box
        /// </summary>
        /// <param name="text">The message of the box</param>
        public BoolDialog(string text)
        {
            InitializeComponent();

            lbl_text.Text = text;
            lbl_text.SelectAll();
            lbl_text.SelectionAlignment = HorizontalAlignment.Center;
        }
        
        /// <summary>
        /// Called when yes was clicked
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">A blank event args</param>
        private void btn_yes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        /// <summary>
        /// Called when no was clicked
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">A blank event args</param>
        private void btn_no_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}
