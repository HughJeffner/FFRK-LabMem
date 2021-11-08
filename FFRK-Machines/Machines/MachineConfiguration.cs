using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FFRK_Machines.Machines
{
    /// <summary>
    /// Base machine configuration class
    /// </summary>
    public class MachineConfiguration
    {

        public bool Debug { get; set; } = true;

        public MachineConfiguration() {}

        public async Task Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
            await Task.CompletedTask;
        }

        public static async Task<T> Load<T>(string path) where T:MachineConfiguration
        {
            return await Task.FromResult(JsonConvert.DeserializeObject<T>(File.ReadAllText(path), new JsonSerializerSettings() { Error = HandleDeserializationError }));
        }

        protected static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            ColorConsole.WriteLine(ConsoleColor.Red, "Error reading configuraiton: {0}", e.ErrorContext.Error);
            e.ErrorContext.Handled = true;
        }

    }
}
