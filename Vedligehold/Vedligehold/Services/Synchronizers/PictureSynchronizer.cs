using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Database;
using Vedligehold.Models;

namespace Vedligehold.Services.Synchronizers
{
    public class PictureSynchronizer
    {
        List<PictureModel> pictureList;

        ServiceFacade facade = ServiceFacade.GetInstance;
        MaintenanceDatabase database = App.Database;

        public async void PutPicturesToNAV()
        {
            try
            {
                pictureList = await database.GetPicturesAsync();
                foreach (PictureModel item in pictureList)
                {
                    await facade.PDFService.PostPicture(item, item.id);
                    await facade.PDFService.PostPictureJobReg(item, item.id);
                    await database.DeletePictureAsync(item);
                }
            }
            catch
            {
                Debug.WriteLine("error no stuff");
            }
        }
    }
}