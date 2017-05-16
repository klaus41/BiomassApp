using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Services
{
    public class ServiceFacade
    {
        private static ServiceFacade serviceFacade;
        private JobRecLineService jobRecLineService;
        private MaintenanceActivityService maintenanceActivityService;
        private MaintenanceService maintenanceService;
        private PDFService pdfService;
        private ResourcesService resourcesServce;
        private SalesPersonService salesPersonService;
        private TimeRegistrationService timeRegistrationService;
        private ThreadManager threadManager;
        private CustomerService customerService;
        private JobService jobService;
        private JobTaskService jobTaskService;
        public static ServiceFacade GetInstance
        {
            get
            {
                if (serviceFacade == null)
                {
                    serviceFacade = new ServiceFacade();
                }
                return serviceFacade;
            }
        }

        public JobRecLineService JobRecLineService
        {
            get
            {
                if (jobRecLineService == null)
                {
                    jobRecLineService = new JobRecLineService();
                }
                return jobRecLineService;
            }
        }

        public MaintenanceActivityService MaintenanceActivityService
        {
            get
            {
                if (maintenanceActivityService == null)
                {
                    maintenanceActivityService = new MaintenanceActivityService();
                }
                return maintenanceActivityService;
            }
        }

        public MaintenanceService MaintenanceService
        {
            get
            {
                if (maintenanceService == null)
                {
                    maintenanceService = new MaintenanceService();
                }
                return maintenanceService;
            }
        }

        public PDFService PDFService
        {
            get
            {
                if (pdfService == null)
                {
                    pdfService = new PDFService();
                }
                return pdfService;
            }
        }

        public ResourcesService ResourcesService
        {
            get
            {
                if (resourcesServce == null)
                {
                    resourcesServce = new ResourcesService();
                }
                return resourcesServce;
            }
        }

        public SalesPersonService SalesPersonService
        {
            get
            {
                if (salesPersonService == null)
                {
                    salesPersonService = new SalesPersonService();
                }
                return salesPersonService;
            }
        }

        public TimeRegistrationService TimeRegistrationService
        {
            get
            {
                if (timeRegistrationService == null)
                {
                    timeRegistrationService = new TimeRegistrationService();
                }
                return timeRegistrationService;
            }
        }

        public ThreadManager ThreadManager
        {
            get
            {
                if (threadManager == null)
                {
                    threadManager = new ThreadManager();
                }
                return threadManager;
            }
        }

        public CustomerService CustomerService
        {
            get
            {
                if (customerService == null)
                {
                    customerService = new CustomerService();
                }
                return customerService;
            }
        }
        public JobService JobService
        {
            get
            {
                if (jobService == null)
                {
                    jobService = new JobService();
                }
                return jobService;
            }
        }
        public JobTaskService JobTaskService
        {
            get
            {
                if (jobTaskService == null)
                {
                    jobTaskService = new JobTaskService();
                }
                return jobTaskService;
            }
        }
    }
}
