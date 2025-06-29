namespace MovieAppWeb.Web.Helpers
{
    public class UploadFile
    {
        public static string Upload(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var newFileName = Guid.NewGuid() + extension;
            var location = "";

            location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/", newFileName);


            var stream = new FileStream(location, FileMode.Create);
            file.CopyTo(stream);
            return newFileName;
        }
    }
}
