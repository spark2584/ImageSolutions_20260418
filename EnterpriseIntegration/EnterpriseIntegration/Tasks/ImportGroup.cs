using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;

namespace EnterpriseIntegration.Tasks
{
    public class ImportGroup : NetSuiteBase
    {
        public bool Execute()
        {
            List<NetSuiteLibrary.Entity.EntityGroup> NetSuiteEntityGroups = null;
            NetSuiteLibrary.Entity.EntityGroupFilter NetSuiteEntityGroupFilter = null;

            try
            {
                NetSuiteEntityGroups = new List<NetSuiteLibrary.Entity.EntityGroup>();
                NetSuiteEntityGroupFilter = new NetSuiteLibrary.Entity.EntityGroupFilter();

                //NetSuiteEntityGroupFilter.InternalIDs = new List<string>();
                //NetSuiteEntityGroupFilter.InternalIDs.Add("3259250");
                //NetSuiteEntityGroupFilter.InternalIDs.Add("3259551");
                //NetSuiteEntityGroupFilter.InternalIDs.Add("3259552");

                NetSuiteEntityGroups = NetSuiteLibrary.Entity.EntityGroup.GetEntityGroups(Service, NetSuiteEntityGroupFilter);

                foreach (NetSuiteLibrary.Entity.EntityGroup _EntityGroup in NetSuiteEntityGroups)
                {
                    //ListOrRecordRef[] ListOrRecordRefs = NetSuiteHelper.GetMultiSelectCustomFieldValue(_EntityGroup.NetSuiteEntityGroup, "custentity_img_gr_itemmap");
                    //if(ListOrRecordRefs != null)
                    //{
                    //    foreach (ListOrRecordRef _ListOrRecordRef in ListOrRecordRefs)
                    //    {
                    //        string strTEst = "test";
                    //    }
                    //}

                    ImageSolutions.Entity.EntityGroup EntityGroup = new ImageSolutions.Entity.EntityGroup();
                    ImageSolutions.Entity.EntityGroupFilter EntityGroupFilter = new ImageSolutions.Entity.EntityGroupFilter();
                    EntityGroupFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                    EntityGroupFilter.InternalID.SearchString = Convert.ToString(_EntityGroup.NetSuiteEntityGroup.internalId);
                    EntityGroup = ImageSolutions.Entity.EntityGroup.GetEntityGroup(EntityGroupFilter);

                    if (EntityGroup != null && !string.IsNullOrEmpty(EntityGroup.InternalID))
                    {
                        if (EntityGroup.Name != Convert.ToString(_EntityGroup.NetSuiteEntityGroup.groupName))
                        {
                            EntityGroup.Name = Convert.ToString(_EntityGroup.NetSuiteEntityGroup.groupName);
                            EntityGroup.Update();
                        }
                    }
                    else
                    {
                        EntityGroup = new ImageSolutions.Entity.EntityGroup();
                        EntityGroup.InternalID = Convert.ToString(_EntityGroup.NetSuiteEntityGroup.internalId);
                        EntityGroup.Name = Convert.ToString(_EntityGroup.NetSuiteEntityGroup.groupName);
                        EntityGroup.Create();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            NetSuiteEntityGroups = GetEntityGroups();

            return true;
        }

        public List<NetSuiteLibrary.Entity.EntityGroup> GetEntityGroups()
        {
            return null;
        }

    }
}
