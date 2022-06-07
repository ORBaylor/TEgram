using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAO
{
     public interface IPhotoDao
     {

        
          //Photo CreatePhoto(Photo photo);

          int SaveFile(string fileData, string fileName);

          Photo GetPhotoById(int Photoid);

        //Photo DeletePhotoById(int id);

        Photo GetLikesFromPost(int postId);




     }
}
