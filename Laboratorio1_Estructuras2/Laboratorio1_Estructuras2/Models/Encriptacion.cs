namespace Laboratorio1_Estructuras2.Models
{
    public class Encriptacion
    {
        public List<int> permutacion(string clave)
        {
            int longitudDeseada = 8;

            // Paso 1: Dividir la frase en subcadenas de 8 caracteres (completando con espacios)
            List<string> subcadenas = new List<string>();
            for (int i = 0; i < clave.Length; i += longitudDeseada)
            {
                string subcadena = clave.Substring(i, Math.Min(longitudDeseada, clave.Length - i));
                if (subcadena.Length < longitudDeseada)
                {
                    subcadena += clave.Substring(0, longitudDeseada - subcadena.Length);
                }
                subcadenas.Add(subcadena);
            }

            // Paso 2: Crear una tupla con el índice y el valor ASCII de cada subcadena
            List<(int, int)> tuplaSubcadenas = subcadenas
            .Select((subcadena, index) => (index, GetASCIISum(subcadena)))
            .ToList();
            tuplaSubcadenas = tuplaSubcadenas.OrderBy(t => t.Item2).ToList();
            List<int> permu = tuplaSubcadenas
            .Select(tupla => tupla.Item1)
            .ToList();
            return permu;
        }
        public string permutar(string cadena, List<int> perm)
        {
            List<string> lista = new List<string>();
            int cantidad = perm.Count();
            for (int i = 0; i < cadena.Length; i += cantidad)
            {
                if (i + cantidad <= cadena.Length)
                {
                    string sub = cadena.Substring(i, cantidad);
                    char[] permutado = new char[cantidad];
                    for (int j = 0; j < cantidad; j++)
                    {
                        int newIndex = perm.IndexOf(j);
                        permutado[newIndex] = sub[j];
                    }
                    lista.Add(new string(permutado));
                }
                else
                {
                    lista.Add(cadena.Substring(i));
                    break;
                }
            }
            string desordenado = string.Join("", lista);
            return desordenado;
        }
        public string desPermutar(string cadena, List<int> perm)
        {
            int cantidad = perm.Count;
            int longitud = cadena.Length;
            char[] resultado = new char[longitud];

            for (int i = 0; i < longitud; i += cantidad)
            {
                int tamaño = Math.Min(cantidad, longitud - i);
                string segmento = cadena.Substring(i, tamaño);

                if (tamaño == cantidad)
                {
                    char[] reordered = new char[tamaño];

                    for (int j = 0; j < tamaño; j++)
                    {
                        int originalIndex = perm[j];
                        reordered[originalIndex] = segmento[j];
                    }

                    for (int j = 0; j < tamaño; j++)
                    {
                        resultado[i + j] = reordered[j];
                    }
                }
                else
                {
                    for (int j = 0; j < tamaño; j++)
                    {
                        resultado[i + j] = segmento[j];
                    }
                }
            }
            return new string(resultado);
        }
        public string CifrarCesar(string mensaje)
        {
            char[] caracteres = mensaje.ToCharArray();

            for (int i = 0; i < caracteres.Length; i++)
            {
                char caracter = caracteres[i];

                if (char.IsLetter(caracter) && caracter != 'á' && caracter != 'é' && caracter != 'í' && caracter != 'ó' && caracter != 'ú' && caracter != 'Á' && caracter != 'É' && caracter != 'Í' && caracter != 'Ó' && caracter != 'Ú')
                {
                    char limite = char.IsUpper(caracter) ? 'A' : 'a';
                    caracteres[i] = (char)(limite + (caracter - limite + 8) % 26);
                }
            }

            return new string(caracteres);
        }
        public string DescifrarCesar(string mensajeCifrado)
        {
            char[] caracteres = mensajeCifrado.ToCharArray();

            for (int i = 0; i < caracteres.Length; i++)
            {
                char caracter = caracteres[i];

                if (char.IsLetter(caracter) && caracter != 'á' && caracter != 'é' && caracter != 'í' && caracter != 'ó' && caracter != 'ú' && caracter != 'Á' && caracter != 'É' && caracter != 'Í' && caracter != 'Ó' && caracter != 'Ú')
                {
                    char limite = char.IsUpper(caracter) ? 'A' : 'a';
                    int offset = (caracter - limite - 8 + 26) % 26;
                    caracteres[i] = (char)(limite + offset);
                }
            }

            return new string(caracteres);
        }
        private int GetASCIISum(string input)
        {
            int suma = 0;
            foreach (char c in input)
            {
                suma += (int)c;
            }
            return suma;
        }
    }
}
