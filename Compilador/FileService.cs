using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador
{
    public class FileService
    {



        public static async Task<string> LerArquivo(string caminho)
        {
            using (var reader = new StreamReader(caminho))
            {
                return await reader.ReadToEndAsync();
            }
        }


        public static async Task SalvarArquivo(string caminho, string conteudo)
        {
            using (var writer = new StreamWriter(caminho))
            {
                await writer.WriteAsync(conteudo);
            }
        }

    }
}
