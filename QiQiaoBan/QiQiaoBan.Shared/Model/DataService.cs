using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace QiQiaoBan.Model
{
    /// <summary>
    /// Use DataService to get some data asynchronously in ViewModel
    /// </summary>
    public class DataService : IDataService
    {
        public async Task<IList<Puzzle>> GetPuzzlesLocal()
        {
            StorageFolder appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string nameFile = @"Assets\puzzles.xml";
            StorageFile file = await appFolder.GetFileAsync(nameFile);
            IRandomAccessStream iRandomAccessStream = await file.OpenAsync(FileAccessMode.Read);
            IInputStream iInputStream = iRandomAccessStream.GetInputStreamAt(0);
            Stream stream = iInputStream.AsStreamForRead();
            StreamReader streamReader = new StreamReader(stream);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Puzzle>));
            return xmlSerializer.Deserialize(streamReader) as List<Puzzle>;
        }
    }
}
