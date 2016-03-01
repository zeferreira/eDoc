using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class Messages
    {
        public static readonly string DocumentNotFound = "Nenhum documento foi encontrado.";
        public static readonly string DocumentsFound = "documento(s) encontrado(s).";
        public static readonly string RepositoryDocumentNotImplemented = "Tipo de repositorio de arquivos não implementado.";
        public static readonly string DocumentIndexNotImplemented = "Tipo de indice para documentos não implementado.";
        public static readonly string LexiconTypeNotImplemented = "Tipo de indice para palavras (lexicon) não implementado.";
        public static readonly string RepositoryLogNotImplemented = "Tipo de repositorio de log não implementado.";
        public static readonly string DocParserNotSupportedFile = "Tipo de arquivo não suportado para indexação.";
    }
}
