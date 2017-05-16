using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Services.Synchronizers
{
    public class SynchronizerFacade
    {
        private static SynchronizerFacade synchronizerFacade;

        private JobRecLineSynchronizer jobRecLineSynchronizer;
        private MaintenanceActivitySynchronizer maintenanceActivitySynchronizer;
        private MaintenanceTaskSynchronizer maintenanceTaskSynchronizer;
        private PictureSynchronizer pictureSynchronizer;
        private ResourcesSynchronizer resourcesSynchronizer;
        private TimeRegistrationSynchronizer timeRegistrationSynchronizer;
        private CustomerSynchronizer customerSynchronizer;
        private JobSynchronizer jobSynchronizer;
        private JobTaskSynchronizer jobTaskSynchronizer;
        public static SynchronizerFacade GetInstance
        {
            get
            {
                if (synchronizerFacade == null)
                {
                    synchronizerFacade = new SynchronizerFacade();
                }
                return synchronizerFacade;
            }
        }

        public JobRecLineSynchronizer JobRecLineSynchronizer
        {
            get
            {
                if (jobRecLineSynchronizer == null)
                {
                    jobRecLineSynchronizer = new JobRecLineSynchronizer();
                }
                return jobRecLineSynchronizer;
            }
        }

        public MaintenanceActivitySynchronizer MaintenanceActivitySynchronizer
        {
            get
            {
                if (maintenanceActivitySynchronizer == null)
                {
                    maintenanceActivitySynchronizer = new MaintenanceActivitySynchronizer();
                }
                return maintenanceActivitySynchronizer;
            }
        }
        public MaintenanceTaskSynchronizer MaintenanceTaskSynchronizer
        {
            get
            {
                if (maintenanceTaskSynchronizer == null)
                {
                    maintenanceTaskSynchronizer = new MaintenanceTaskSynchronizer();
                }
                return maintenanceTaskSynchronizer;
            }
        }
        public PictureSynchronizer PictureSynchronizer
        {
            get
            {
                if (pictureSynchronizer == null)
                {
                    pictureSynchronizer = new PictureSynchronizer();
                }
                return pictureSynchronizer;
            }
        }
        public ResourcesSynchronizer ResourcesSynchronizer
        {
            get
            {
                if (resourcesSynchronizer == null)
                {
                    resourcesSynchronizer = new ResourcesSynchronizer();
                }
                return resourcesSynchronizer;
            }
        }
        public TimeRegistrationSynchronizer TimeRegistrationSynchronizer
        {
            get
            {
                if (timeRegistrationSynchronizer == null)
                {
                    timeRegistrationSynchronizer = new TimeRegistrationSynchronizer();
                }
                return timeRegistrationSynchronizer;
            }
        }
        public CustomerSynchronizer CustomerSynchronizer
        {
            get
            {
                if (customerSynchronizer == null)
                {
                    customerSynchronizer = new CustomerSynchronizer();
                }
                return customerSynchronizer;
            }
        }
        public JobSynchronizer JobSynchronizer
        {
            get
            {
                if (jobSynchronizer == null)
                {
                    jobSynchronizer = new JobSynchronizer();
                }
                return jobSynchronizer;
            }
        }
        public JobTaskSynchronizer JobTaskSynchronizer
        {
            get
            {
                if (jobTaskSynchronizer == null)
                {
                    jobTaskSynchronizer = new JobTaskSynchronizer();
                }
                return jobTaskSynchronizer;
            }
        }
    }
}
