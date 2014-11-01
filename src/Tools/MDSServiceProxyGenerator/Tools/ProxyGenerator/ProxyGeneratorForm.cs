using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MDS.Client.MDSServices;
using MDS.GUI;

namespace MDS.Tools.ProxyGenerator
{
    public partial class ProxyGeneratorForm : Form
    {
        private Assembly _selectedAssembly;

        public ProxyGeneratorForm()
        {
            InitializeComponent();
        }

        private void btnBrowseAssembly_Click(object sender, EventArgs e)
        {
            var dialogResult = AssemblyBrowseDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            txtAssemblyPath.Text = AssemblyBrowseDialog.FileName;

            try
            {
                FillClasses();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillClasses()
        {
            cmbClasses.Items.Clear();
            cmbClasses.Items.Add("All MDSService classes");
            var assembly = Assembly.LoadFile(txtAssemblyPath.Text);
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes(typeof (MDSServiceAttribute), true);
                if (attributes.Length <= 0)
                {
                    continue;
                }

                cmbClasses.Items.Add(type.FullName);
            }

            _selectedAssembly = assembly;
            cmbClasses.SelectedIndex = 0;
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            btnGenerateCode.Enabled = false;
            Application.DoEvents();

            try
            {
                GenerateCode();
                MDSGuiHelper.ShowInfoDialog("Proxy classes are generated.", "Success.");
            }
            catch (Exception ex)
            {
                MDSGuiHelper.ShowErrorMessage(ex.Message);                
            }
            finally
            {
                btnGenerateCode.Enabled = true;
            }
        }

        private void GenerateCode()
        {
            if(cmbClasses.Items.Count < 2)
            {
                MDSGuiHelper.ShowWarningMessage("There is no class to generate.");
                return;
            }

            var namespaceName = txtNamespace.Text;
            if(string.IsNullOrEmpty(namespaceName))
            {
                MDSGuiHelper.ShowWarningMessage("Please enter a namespace.");
                return;
            }

            var targetFolder = txtTargetFolder.Text;
            if (string.IsNullOrEmpty(targetFolder))
            {
                MDSGuiHelper.ShowWarningMessage("Please enter a target folder to generate code files.");
                return;
            }

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            if(cmbClasses.SelectedIndex == 0)
            {
                for(var i=1;i<cmbClasses.Items.Count;i++)
                {
                    GenerateProxyClass(_selectedAssembly,namespaceName, cmbClasses.Items[i].ToString(), targetFolder);
                }
            }
            else
            {
                GenerateProxyClass(_selectedAssembly, namespaceName, cmbClasses.Items[cmbClasses.SelectedIndex].ToString(), targetFolder);
            }
        }

        private static void GenerateProxyClass(Assembly assembly, string namespaceName, string className,  string targetFolder)
        {
            var type = assembly.GetType(className);
            var generateMethod = type.GetMethod("GenerateProxyClass");
            var obj = Activator.CreateInstance(type);
            var classCode = (string) generateMethod.Invoke(obj, new object[] { namespaceName });
            var proxyClassName = type.Name + "Proxy";

            using (var writer = new StreamWriter(Path.Combine(targetFolder, proxyClassName + ".cs"), false, Encoding.UTF8))
            {
                writer.WriteLine(classCode);
            }
        }

        private void btnTargetFolderBrowse_Click(object sender, EventArgs e)
        {
            var dialogResult = TargetFolderBrowseDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            txtTargetFolder.Text = TargetFolderBrowseDialog.SelectedPath;
        }
    }
}