
namespace Compilador.Constantes
{
    public static class Constants
    {


        public static readonly string[] NECESSITA_PARENTESES = new string[] { "while", "for", "if", "Print","Read" };

        public static readonly int[] CODIGOS_TIPOS = new[] { (int)CodigosEnum.CODIGO_TIPO_INT,
                                                               (int)CodigosEnum.CODIGO_TIPO_STRING,
                                                               (int)CodigosEnum.CODIGO_TIPO_DECIMAL,
                                                               (int)CodigosEnum.CODIGO_TIPO_BOOL };


        public static readonly int[] CODIGOS_LITERALS = new[] { (int)CodigosEnum.CODIGO_LITERAL_INT,
                                                                   (int)CodigosEnum.CODIGO_LITERAL_STRING,
                                                                   (int)CodigosEnum.CODIGO_LITERAL_DECIMAL,
                                                                   (int)CodigosEnum.CODIGO_LITERAL_BOOL };

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
                        new KeyValuePair<string,int>("string",((int)CodigosEnum.CODIGO_TIPO_STRING)),

            new KeyValuePair<string,int>("int",((int)CodigosEnum.CODIGO_TIPO_INT)),
            new KeyValuePair<string,int>("decimal",((int)CodigosEnum.CODIGO_TIPO_DECIMAL)),
                        new KeyValuePair<string,int>("bool",((int)CodigosEnum.CODIGO_TIPO_BOOL)),

            new KeyValuePair<string,int>(";",15),
            new KeyValuePair<string,int>("(",(int)CodigosEnum.CODIGO_ABRE_PARENTESES),
            new KeyValuePair<string,int>(")",(int)CodigosEnum.CODIGO_FECHA_PARENTESES),
            new KeyValuePair<string,int>("%",28)
            ]
            );



    }





}
