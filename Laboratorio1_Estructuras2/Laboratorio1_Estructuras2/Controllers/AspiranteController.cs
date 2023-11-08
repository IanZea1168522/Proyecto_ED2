using Microsoft.AspNetCore.Mvc;
using Laboratorio1_Estructuras2.Models;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Laboratorio1_Estructuras2.Controllers
{
    public class AspiranteController : Controller
    {
        public static AVL arbol = new AVL();
        public static List<Aspirante> listaAspi = new List<Aspirante>();
        public static ArbolHuffman huffman = new ArbolHuffman();
        public static ArbolIn confidencial = new ArbolIn();
        public static LZW codificador = new LZW();
        public static List<int> permu= new List<int>();
        public static Encriptacion cod = new Encriptacion();
        public static String clave = "Adelante caballero, puede pasar";
        public static RSAA Asimetrico;
        public IActionResult Index()
        {
            huffman = huffman.arbol("234987 81324QWHERTYUIOP,YUEQIUOMQKJADSF-JNVCCHU IEW9\r413278901-234567890 1 2345.ASD,FGHJKLÑHK ADSFHIOEWHE,WFHDSFJBKV-J\nVCUHA678908,793789347897 8954648798ZXCVBNMJAWFJK'ADSFJIODA  SKNLVADN,J79869");
            listaAspi.Clear();
            permu = cod.permutacion("El_ropavejero");
            return View("Index");
        }
        [HttpPost]
        [Route("buscarNom")]
        public IActionResult encontrado(string nombre)
        {
            if (nombre == null)
            {
                return View("ErrorBuscar");
            }
            List<Aspirante> aspirantesEncontrados = new List<Aspirante>();
            aspirantesEncontrados = arbol.busqueda(nombre);
            if (aspirantesEncontrados == null)
            {
                return View("ErrorBuscar");
            }
            confidencial = new ArbolIn();
            foreach (Aspirante para in aspirantesEncontrados)
            {
                confidencial.Insertar(para);
            }
            listaAspi = confidencial.listaOrdenada();
            return View("subirDatos5", listaAspi);
        }
        public string convertidorVec(string[] vector)
        {
            string vectorAString = "";
            if (vector.Count() == 0)
            {
                return vectorAString;
            }
            for (int i = 0; i < vector.Length; i++)
            {
                if (vectorAString == "")
                {
                    vectorAString = vectorAString + vector[i];
                }
                else
                {
                    vectorAString = vectorAString + " " + vector[i];
                }
            }
            return vectorAString;
        }
        public IActionResult SubirDatos4(IFormFile archivo, string ruta)
        {
            if (archivo == null || archivo.Length == 0 || ruta == null)
            {
                ViewBag.Error = "Seleccione un archivo CSV válido. y una ruta de carpeta";
                return View("SubirDatos4", listaAspi);
            }
            using (var reader = new StreamReader(archivo.OpenReadStream()))
            {
                if (listaAspi.Count() > 0)
                {
                    listaAspi.Clear();
                }
                if (confidencial.raiz != null)
                {
                    confidencial.raiz = null;
                }
                while (!reader.EndOfStream)
                {
                    var linea = reader.ReadLine();
                    var partes = linea.Split(';');
                    if (partes.Length != 2)
                    {
                        continue;
                    }
                    var instruccion = partes[0].Trim();
                    var json = partes[1].Trim();
                    try
                    {
                        JObject jsonData = JObject.Parse(json);
                        var idents = new string[3];
                        if (jsonData.TryGetValue("companies", out JToken companiesToken) && companiesToken.Type == JTokenType.Array)
                        {
                            idents = companiesToken.ToObject<string[]>();
                        }
                        string empres;
                        empres = convertidorVec(idents);
                        Aspirante aspirante = new Aspirante
                        {
                            nombre = (string)jsonData["name"],
                            infoPriv = new List<string> { (string)jsonData["dpi"] },
                            nacimiento = (string)jsonData["datebirth"],
                            direccion = (string)jsonData["address"],
                            carta = new List<string>(),
                            Convs = new List<string>(),
                            diccionario = new List<Dictionary<string, int>>(),
                            reclutadores = (string)jsonData["recluiter"]
                        };
                        aspirante.infoPriv.Add(empres);
                        switch (instruccion.ToUpper())
                        {
                            case "INSERT":
                                aspirante.infoPriv[0] = huffman.Codificar(aspirante.infoPriv[0], huffman);
                                aspirante.infoPriv[1] = huffman.Codificar(aspirante.infoPriv[1].ToUpper(), huffman);
                                aspirante.direccion = huffman.Codificar(aspirante.direccion.ToUpper(), huffman);
                                arbol.Insertar(aspirante);
                                confidencial.Insertar(aspirante);
                                break;

                            case "DELETE":
                                aspirante.infoPriv[0] = huffman.Codificar(aspirante.infoPriv[0], huffman);
                                aspirante.infoPriv[1] = huffman.Codificar(aspirante.infoPriv[1].ToUpper(), huffman);
                                aspirante.direccion = huffman.Codificar(aspirante.direccion.ToUpper(), huffman);
                                arbol.BuscaElimina(aspirante);
                                confidencial.Eliminar(aspirante);
                                break;

                            case "PATCH":
                                aspirante.infoPriv[0] = huffman.Codificar(aspirante.infoPriv[0], huffman);
                                aspirante.infoPriv[1] = huffman.Codificar(aspirante.infoPriv[1].ToUpper(), huffman);
                                aspirante.direccion = huffman.Codificar(aspirante.direccion.ToUpper(), huffman);
                                arbol.actual(aspirante);
                                confidencial.actual(aspirante);
                                break;

                            default:
                                return View("ErrorSub5");
                        }
                    }
                    catch (JsonReaderException)
                    {
                        return View("ErrorSub5");
                    }
                }

                try
                {
                    string[] archivos = Directory.GetFiles(ruta, "*.txt");
                    foreach (string archivoEncontrado in archivos)
                    {
                        string contenido = System.IO.File.ReadAllText(archivoEncontrado);
                        string nombreArchivo = Path.GetFileNameWithoutExtension(archivoEncontrado);
                        string[] partesNombre = nombreArchivo.Split('-');
                        if (partesNombre[0] == "REC")
                        {
                            var aspi = confidencial.Buscar(huffman.Codificar(partesNombre[1], huffman));
                            var aspirante = arbol.buscarDPI(aspi.nombre, aspi.infoPriv[0]);
                            var tupla = codificador.comprimir(huffman.Codificar(contenido.ToUpper(), huffman));
                            string codigo = tupla.Item1;
                            Dictionary<string, int> diccionario = tupla.Item2;
                            aspirante.carta.Add(codigo);
                            aspirante.diccionario.Add(diccionario);
                        }
                        else if (partesNombre[0] == "CONV")
                        {
                            var aspi = confidencial.Buscar(huffman.Codificar(partesNombre[1], huffman));
                            var aspirante = arbol.buscarDPI(aspi.nombre, aspi.infoPriv[0]);
                            string codigo = cod.permutar(contenido, permu);
                            codigo = cod.CifrarCesar(codigo);
                            aspirante.Convs.Add(codigo);
                        }
                    }
                }
                catch (Exception e)
                {
                    return View("ErrorSub4");
                }
                listaAspi = arbol.listaOrdenada();
            }
            return RedirectToAction("SubirDatos4");
        }
        static bool EsPrimo(int numero)
        {
            if (numero <= 1)
                return false;
            if (numero <= 3)
                return true;

            if (numero % 2 == 0 || numero % 3 == 0)
                return false;

            for (int i = 5; i * i <= numero; i += 6)
            {
                if (numero % i == 0 || numero % (i + 2) == 0)
                    return false;
            }

            return true;
        }

        static int GenerarNumeroPrimoAleatorio(int min, int max)
        {
            Random random = new Random();
            List<int> primos = new List<int>();

            for (int i = min; i <= max; i++)
            {
                if (EsPrimo(i))
                    primos.Add(i);
            }

            if (primos.Count == 0)
                return -1; // No se encontraron números primos en el rango

            int indiceAleatorio = random.Next(primos.Count);
            return primos[indiceAleatorio];
        }
        [HttpPost]
        [Route("GenerarClave")]
        public IActionResult generar(string Dpi, string reclu, string direc)
        {
            if (Dpi == null || confidencial.raiz == null)
            {
                return View("ErrorSub5");
            }
            var aspirante = confidencial.Buscar(huffman.Codificar(Dpi, huffman));
            if(aspirante.reclutadores != reclu || aspirante.direccion != huffman.Codificar(direc.ToUpper(), huffman))
            {
                return View("ErrorSub5");
            }
            Asimetrico = new RSAA(GenerarNumeroPrimoAleatorio(1, 100), GenerarNumeroPrimoAleatorio(1, 100));
            aspirante.encriptado = Asimetrico.Crypt(clave, Asimetrico.public1, Asimetrico.common);
            return View("clave", ("Identificador: " + Convert.ToString(Asimetrico.private1) + ", Contraseña: " + Convert.ToString(Asimetrico.common)));
        }
        [HttpPost]
        [Route("mostrarInfoClave")]
        public IActionResult buscarCla(string Dpi1, string reclu1, string clave1, string ident)
        {
            if (Dpi1 == null || confidencial.raiz == null)
            {
                return View("ErrorSub5");
            }
            var aspirante = confidencial.Buscar(huffman.Codificar(Dpi1, huffman));
            if (aspirante.reclutadores != reclu1 || aspirante.encriptado == null)
            {
                return View("ErrorSub5");
            }
            if(Asimetrico.Decrypt(aspirante.encriptado, Convert.ToInt64(clave1), Convert.ToInt64(ident)) == clave)
            {
                string cartas = "";
                List<String> lista = new List<string>();
                lista.Add(aspirante.nombre);
                lista.Add(huffman.Decodificar(aspirante.infoPriv[0], huffman));
                lista.Add(huffman.Decodificar(aspirante.infoPriv[1], huffman));
                for (int i = 0; i < aspirante.carta.Count(); i++)
                {
                    if (cartas == "")
                    {
                        cartas = codificador.descomprimir(aspirante.carta[i], aspirante.diccionario[i]);
                    }
                    else
                    {
                        cartas += "\n" + codificador.descomprimir(aspirante.carta[i], aspirante.diccionario[i]);
                    }
                }
                lista.Add(huffman.Decodificar(cartas, huffman));
                string convs = "";
                for(int i = 0; i < aspirante.Convs.Count(); i++)
                {
                    if(convs == "")
                    {
                        convs = cod.desPermutar(cod.DescifrarCesar(aspirante.Convs[i]), permu);
                    }
                    else
                    {
                        convs += "\n" + cod.desPermutar(cod.DescifrarCesar(aspirante.Convs[i]), permu);
                    }
                }
                lista.Add(convs);
                lista.Add(aspirante.nacimiento);
                lista.Add(huffman.Decodificar(aspirante.direccion, huffman));
                lista.Add(aspirante.reclutadores);
                return View("EncontradoClave", lista);
            }
            return View("ErrorSub5");
        }
        [HttpPost]
        [Route("Volver")]
        public IActionResult volver()
        {
            return View("subirDatos5", listaAspi);
        }
    }
}
