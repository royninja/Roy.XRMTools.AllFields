using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Roy.XRMTools.AllFields
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;

        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbSample_Click(object sender, EventArgs e)
        {
            // The ExecuteMethod method handles connecting to an
            // organization if XrmToolBox is not yet connected
            ExecuteMethod(GetAccounts);
        }

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetEntities_Click(object sender, EventArgs e)
        {
            ExecuteMethod(getAllEntities);
        }
        /// <summary>
        /// 
        /// </summary>
        private void getAllEntities()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Entities from D365 CRM",
                Work = (worker, args) =>
                {
                    RetrieveAllEntitiesRequest retrieveAllEntitiesRequest = new RetrieveAllEntitiesRequest();
                    retrieveAllEntitiesRequest.EntityFilters = EntityFilters.Attributes;
                    retrieveAllEntitiesRequest.RetrieveAsIfPublished = true;

                    RetrieveAllEntitiesResponse allEntitiesResponse = (RetrieveAllEntitiesResponse)Service.Execute(retrieveAllEntitiesRequest);

                    List<string> listEntities = new List<string>();
                    if (allEntitiesResponse.EntityMetadata.Length > 0)
                    {
                        foreach (EntityMetadata entity in allEntitiesResponse.EntityMetadata)
                        {
                            listEntities.Add(entity.LogicalName.ToString());
                        }
                    }
                    args.Result = listEntities;
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        cmbEntity.DataSource = args.Result;
                    }
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetRecord_Click(object sender, EventArgs e)
        {
            ExecuteMethod(getRecordsData);
        }
        /// <summary>
        /// 
        /// </summary>
        private void getRecordsData()
        {
            string logicalname = cmbEntity.SelectedItem.ToString();
            string recordId = txtGuid.Text;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Entities from D365 CRM",
                Work = (worker, args) =>
                {
                    if(recordId != null & recordId != string.Empty && logicalname != null && logicalname != string.Empty)
                    {

                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        cmbEntity.DataSource = args.Result;
                    }
                }
            });
        }
    }
}