using System;
using System.Collections.Generic;
using System.Text;
using Compilador.Lexico;
using Compilador.Semantico;
using Compilador.Sintatico;

namespace Compilador
{
    internal class Compilador
    {

        
        public SemanticoService? Semantico { get; private set; }
        public SintaticoService? Sintatico { get; private set; }




        public void Compilar(string input)
        {
            LexicoService lexico = new LexicoService();
            lexico.SetInput(input);
            this.IniciarCompilador();


            this.Sintatico.Parse(lexico,this.Semantico);

            var final = this.Semantico.ExecutarFim();

            _ = FileService.SalvarArquivo("output.asm", final);
        }

        public void IniciarCompilador()
        {
            this.Semantico = new SemanticoService();
            this.Sintatico = new SintaticoService();
        }
    
    }
}
