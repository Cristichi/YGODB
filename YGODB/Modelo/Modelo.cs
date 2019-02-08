using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace YGODB.Modelo
{
    class Modelo
    {
        private static StorageFolder CarpetaPadre;
        private static StorageFile ArchivoJson;
        private static bool iniciado = false;

        private async static Task Iniciar()
        {
            CarpetaPadre = await KnownFolders.DocumentsLibrary.CreateFolderAsync("Base de Datos YGO", CreationCollisionOption.OpenIfExists);
            ArchivoJson = await CarpetaPadre.CreateFileAsync("", CreationCollisionOption.OpenIfExists);
        }

        public async static Task<List<Carta>> getTodas(bool forceUpdate)
        {
            if (!iniciado)
            {
                await iniciar();
            }

            String result;
            if (forceUpdate)
            {
                var http = new HttpClient();
                var response = await http.GetAsync("https://db.ygoprodeck.com/api/v2/cardinfo.php");
                result = await response.Content.ReadAsStringAsync();
                await FileIO.WriteTextAsync(ArchivoJson, result);
            }
            else
            {
                result = await FileIO.ReadTextAsync(ArchivoJson);
            }
            var serializer = new DataContractJsonSerializer(typeof(List<Carta>));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (List<Carta>)serializer.ReadObject(ms);

            return data;
        }
    }

    [DataContract]
    public class Carta
    {
        [DataMember]
        public string id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string atk { get; set; }
        public string def { get; set; }
        public string type { get; set; }
        public string level { get; set; }
        public string race { get; set; }
        public string attribute { get; set; }
        public object scale { get; set; }
        public object linkval { get; set; }
        public object linkmarkers { get; set; }
        public string archetype { get; set; }
        public string set_tag { get; set; }
        public string setcode { get; set; }
        public object ban_tcg { get; set; }
        public object ban_ocg { get; set; }
        public object ban_goat { get; set; }
        public string image_url { get; set; }
        public string image_url_small { get; set; }
    }
}
