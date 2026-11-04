using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Compilador
{
    public static class Constants
    {

        
        public static readonly string[] NECESSITA_PARENTESES = new string[] { "while", "for", "if", "Print" };


        public static readonly Dictionary<string, int> TOKENS_RESERVADOS = new Dictionary<string, int>(

            [
            new KeyValuePair<string, int>("EXF", -1),
            new KeyValuePair<string, int>("IOF",0),
            new KeyValuePair<string,int>("+",1),
            new KeyValuePair<string,int>("-",2),
            new KeyValuePair<string,int>("*",3),
            new KeyValuePair<string,int>("/",4),
            new KeyValuePair<string,int>("=",5),
            new KeyValuePair<string,int>("Read",6),
            new KeyValuePair<string,int>("Print",7),
            new KeyValuePair<string,int>("while",8),
            new KeyValuePair<string,int>("for",9),
            new KeyValuePair<string,int>("if",10),
            new KeyValuePair<string,int>("int",11),
            new KeyValuePair<string,int>("string",12),
            new KeyValuePair<string,int>("bool",13),
            new KeyValuePair<string,int>("decimal",14),
            new KeyValuePair<string,int>(";",15),
            ]
            );
        


    }
    
    


    
}
