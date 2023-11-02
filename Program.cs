using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Data.Common;

namespace Jogo_da_velha
{
    internal class Program
    {

        private struct MatchResult
        {
            public string Player1;
            public string Player2;
            public string Winner;
        }

        static List<MatchResult> matchResults = new List<MatchResult>();

        static void Main(string[] args)
        {
            Menu();
            Console.ReadLine();
        }

        public static void Menu()
        {
            int opcao = 0;
            int opcao2 = 0;
            string player1;
            string player2 = "Robo";
            int tamanho = 0;

            while (opcao != 1 && opcao != 2)
            {
                Console.Write("MENU\n1.Iniciar jogo\n2.Histórico\nDigite a opção desejada: ");
                opcao = int.Parse(Console.ReadLine());

                if (opcao == 1)
                {
                    while (opcao2 != 1 && opcao2 != 2)
                    {
                        Console.Write("1.Jogar vs player\n2.Jogar vs robô\nDigite a opção desejada: ");
                        opcao2 = int.Parse(Console.ReadLine());
                        Console.Write("Digite o nome do player 1: ");
                        player1 = Console.ReadLine();
                        if (opcao2 == 1)
                        {
                            Console.Write("Digite o nome do player 2: ");
                            player2 = Console.ReadLine();
                        }
                        Console.Write("Digite o tamanho do mapa 3-10: ");
                        tamanho = int.Parse(Console.ReadLine());

                        if (opcao2 == 1)
                        {
                            JogoVsPlayer(Mapa(tamanho), player1, player2, tamanho);
                        }
                        else if (opcao2 == 2)
                        {
                            JogoVsRobo(Mapa(tamanho), player1, tamanho);
                        }
                        else
                        {
                            Console.WriteLine("Opção Invalida!");
                        }
                    }
                }
                else if (opcao == 2)
                {
                    MostrarHistorico();
                }
                else
                {
                    Console.WriteLine("Opção Invalida!");
                }
            }
        }

        public static void JogoVsRobo(string[,] mapa, string player1, int tamanho)
        {
            //Player 1 = X
            //Player Robô = O

            int x = 0;
            int y = 0;
            int opcao = 0;

            bool isInvalida = false;

            while (MapaisCheio(mapa) == false || FimDeJogo(mapa) == false)
            {

                Console.WriteLine("Jogador atual: " + player1);
                MostrarMapa(mapa);

                do
                {
                    Console.Write("Jogador " + player1 + " entre com as coordenadas da próxima jogada: ");
                    string input = Console.ReadLine();

                    string[] coordenadas = input.Split(' ');

                    if (coordenadas.Length == 2)
                    {
                        x = int.Parse(coordenadas[0]);
                        y = int.Parse(coordenadas[1]);

                        Console.WriteLine("Coordenadas digitadas: x = " + x + ", y = " + y);


                        if (mapa[x, y] != "O" && x <= tamanho - 1 && y <= tamanho - 1 && mapa[x, y] == null)
                        {
                            mapa[x, y] = "X";
                            isInvalida = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Opção invalida. Digite novamente!");
                            isInvalida = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Entrada inválida. Digite duas coordenadas validas e separadas por um espaço.");
                        isInvalida = true;
                    }
                } while (isInvalida || x >= tamanho || y >= tamanho);


                if (FimDeJogo(mapa) == true)
                {
                    Console.WriteLine("Jogador " + player1 + " vencedor");
                    MostrarMapa(mapa);
                    EscreverHistorico(player1 + " vs Robô\nVencedor: " + player1);
                    break;
                }
                else if (MapaisCheio(mapa) == true)
                {
                    Console.WriteLine("Velha!");
                    MostrarMapa(mapa);
                    break;
                }

                do
                {
                    bool fezJogada = false;

                    // Verifica se pode vencer na próxima jogada
                    for (int i = 0; i < tamanho; i++)
                    {
                        for (int j = 0; j < tamanho; j++)
                        {
                            if (mapa[i, j] == null)
                            {
                                mapa[i, j] = "O";
                                if (FimDeJogo(mapa))
                                {
                                    fezJogada = true;
                                    break;
                                }
                                mapa[i, j] = null; // Desfaz a jogada
                            }
                        }
                        if (fezJogada) break;
                    }

                    if (!fezJogada)
                    {
                        // Verifica se o jogador pode vencer na próxima jogada e bloqueia
                        for (int i = 0; i < tamanho; i++)
                        {
                            for (int j = 0; j < tamanho; j++)
                            {
                                if (mapa[i, j] == null)
                                {
                                    mapa[i, j] = "X";
                                    if (FimDeJogo(mapa))
                                    {
                                        mapa[i, j] = "O";
                                        fezJogada = true;
                                        break;
                                    }
                                    mapa[i, j] = null; // Desfaz a jogada
                                }
                            }
                            if (fezJogada) break;
                        }
                    }

                    if (!fezJogada)
                    {
                        for (int i = 0; i < tamanho; i++)
                        {
                            for (int j = 0; j < tamanho; j++)
                            {
                                if (mapa[i, j] == null)
                                {
                                    mapa[i, j] = "X";
                                    if (FimDeJogo(mapa))
                                    {
                                        mapa[i, j] = "O";
                                        isInvalida = false;
                                        break;
                                    }
                                    mapa[i, j] = null; // Desfaz a jogada
                                }
                            }
                            if (!isInvalida) break;
                        }
                    }

                    if (!fezJogada)
                    {
                        // Escolha uma jogada inteligente
                        for (int i = 0; i < tamanho; i++)
                        {
                            for (int j = 0; j < tamanho; j++)
                            {
                                if (mapa[i, j] == null)
                                {
                                    mapa[i, j] = "O";
                                    fezJogada = true;
                                    break;
                                }
                            }
                            if (fezJogada) break;
                        }
                    }

                } while (isInvalida == true || x >= tamanho || y >= tamanho);
            }

            while (opcao != 1 && opcao != 2)
            {
                Console.WriteLine("Deseja jogar novamente?\n1.Sim\n2.Voltar ao menu\nDigita a opção: ");
                opcao = int.Parse(Console.ReadLine());
            }
            if (opcao == 1)
            {
                JogoVsRobo(Mapa(tamanho), player1, tamanho);
            }
            else
            {
                Menu();
            }
        }

        public static void JogoVsPlayer(string[,] mapa, string player1, string player2, int tamanho)
        {
            //Player 1 = X
            //Player 2 = O

            int x = 0;
            int y = 0;
            int opcao = 0;

            bool isInvalida = false;

            while (MapaisCheio(mapa) == false || FimDeJogo(mapa) == false)
            {

                Console.WriteLine("Jogador atual: " + player1);
                MostrarMapa(mapa);

                do
                {
                    Console.Write("Jogador " + player1 + " entre com as coordenadas da próxima jogada: ");
                    string input = Console.ReadLine();

                    string[] coordenadas = input.Split(' ');

                    if (coordenadas.Length == 2)
                    {
                        x = int.Parse(coordenadas[0]);
                        y = int.Parse(coordenadas[1]);

                        Console.WriteLine("Coordenadas digitadas: x = " + x + ", y = " + y);


                        if (mapa[x, y] != "O" && x <= tamanho - 1 && y <= tamanho - 1 && mapa[x, y] == null)
                        {
                            mapa[x, y] = "X";
                            isInvalida = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Opção invalida. Digite novamente!");
                            isInvalida = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Entrada inválida. Digite duas coordenadas validas e separadas por um espaço.");
                        isInvalida = true;
                    }
                } while (isInvalida == true || x >= tamanho - 1 || y >= tamanho - 1);


                if (FimDeJogo(mapa) == true)
                {
                    Console.WriteLine("\nJogador " + player1 + " vencedor");
                    MostrarMapa(mapa);
                    EscreverHistorico(player1 + " vs " + player2 + "\nVencedor: " + player1);
                    break;
                }
                else if (MapaisCheio(mapa) == true)
                {
                    Console.WriteLine("Velha!");
                    MostrarMapa(mapa);
                    break;
                }

                Console.WriteLine("Jogador atual: " + player2);
                MostrarMapa(mapa);
                do
                {
                    Console.Write("Jogador " + player2 + " entre com as coordenadas da próxima jogada: ");
                    string input = Console.ReadLine();

                    string[] coordenadas = input.Split(' ');

                    if (coordenadas.Length == 2)
                    {
                        x = int.Parse(coordenadas[0]);
                        y = int.Parse(coordenadas[1]);

                        Console.WriteLine("Coordenadas digitadas: x = " + x + ", y = " + y);


                        if (mapa[x, y] != "X" && x <= tamanho - 1 && y <= tamanho - 1 && mapa[x, y] == null)
                        {
                            mapa[x, y] = "O";
                            isInvalida = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Opção invalida ja selecionada");
                            isInvalida = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Entrada inválida. Digite duas coordenadas separadas por um espaço.");
                        isInvalida = true;
                    }
                } while (isInvalida == true || x >= tamanho - 1 || y >= tamanho - 1);


                if (FimDeJogo(mapa) == true)
                {
                    Console.WriteLine("Jogador " + player2 + " vencedor");
                    MostrarMapa(mapa);
                    EscreverHistorico(player1 + " vs " + player2 + "\nVencedor: " + player2);

                    break;
                }
                else if (MapaisCheio(mapa) == true)
                {
                    Console.WriteLine("Velha!");
                    MostrarMapa(mapa);
                    break;
                }

            }

            while (opcao != 1 && opcao != 2)
            {
                Console.WriteLine("Deseja jogar novamente?\n1.Sim\n2.Voltar ao menu\nDigita a opção: ");
                opcao = int.Parse(Console.ReadLine());
            }
            if (opcao == 1)
            {
                JogoVsPlayer(Mapa(tamanho), player1, player2, tamanho);
            }
            else
            {
                Menu();
            }

        }

        public static void EscreverHistorico(string x)
        {


            MatchResult result = new MatchResult();
            string[] lines = x.Split('\n');
            if (lines.Length == 2)
            {
                result.Player1 = lines[0].ToLower();
                result.Player2 = lines[1].ToLower();
                result.Winner = lines[1].ToLower();
                matchResults.Add(result);
            }

            try
            { //Cria ou abre um arquivo
                StreamWriter arq = new StreamWriter("Historico.txt", true, Encoding.UTF8);

                //Escreve no arquivo
                arq.WriteLine(x);

                //Fecha o arquivo
                arq.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public static void EscreverRanking()
        {
            try
            {
                using (StreamWriter arquivo = new StreamWriter("Ranking.txt", false, Encoding.UTF8))
                {
                    Dictionary<string, int> playerWins = new Dictionary<string, int>();

                    foreach (var result in matchResults)
                    {
                        if (!playerWins.ContainsKey(result.Player1))
                        {
                            playerWins[result.Player1] = 0;
                        }
                        if (!playerWins.ContainsKey(result.Player2))
                        {
                            playerWins[result.Player2] = 0;
                        }

                        if (result.Winner == result.Player1)
                        {
                            playerWins[result.Player1]++;
                        }
                        else if (result.Winner == result.Player2)
                        {
                            playerWins[result.Player2]++;
                        }
                    }

                    arquivo.WriteLine("Ranking de Vitórias:");

                    foreach (var kvp in playerWins.OrderByDescending(p => p.Value))
                    {
                        arquivo.WriteLine($"{kvp.Key} - {kvp.Value} vitória(s)");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public static void MostrarHistorico()
        {
            string linha;
            try
            {

                Dictionary<string, int> playerWins = new Dictionary<string, int>();

                foreach (var result in matchResults)
                {
                    if (!playerWins.ContainsKey(result.Winner))
                    {
                        playerWins[result.Winner] = 1;
                    }
                    else
                    {
                        playerWins[result.Winner]++;
                    }
                }

                Console.WriteLine("Ranking de Vitórias:");

                foreach (var tmp in playerWins.OrderByDescending(p => p.Value))
                {
                    Console.WriteLine($"{tmp.Key} - {tmp.Value} vitória(s)");
                }

                Console.WriteLine("Histórico: ");
                //Abre arquivo para leitura
                StreamReader arq = new StreamReader("Historico.txt", Encoding.UTF8);
                linha = arq.ReadLine(); //Lê a primeira linha do arquivo
                                        //Continue lendo até atingir o final do arquivo
                while (linha != null)
                {
                    Console.WriteLine(linha);
                    linha = arq.ReadLine(); //Lê a próxima linha
                }
                arq.Close(); //Fecha o arquivo

                EscreverRanking();
                Menu();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public static string[,] Mapa(int tamanho)
        {
            string[,] mapa = new string[tamanho, tamanho];
            return mapa;

        }

        public static void MostrarMapa(string[,] mapa)
        {
            for (int i = 0; i < mapa.GetLength(0); i++)
            {
                for (int j = 0; j < mapa.GetLength(1); j++)
                {
                    Console.Write(mapa[i, j]);
                    if (mapa[i, j] == null)
                    {
                        Console.Write("-");
                    }
                    if (j != mapa.GetLength(0) - 1)
                    {
                        Console.Write("|");
                    }
                }
                Console.WriteLine("\n");
            }
        }

        public static bool MapaisCheio(string[,] mapa)
        {
            for (int i = 0; i < mapa.GetLength(0); i++)
            {
                for (int j = 0; j < mapa.GetLength(1); j++)
                {
                    if (mapa[i, j] == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool FimDeJogo(string[,] mapa)
        {
            int tamanho = mapa.GetLength(0);

            for (int i = 0; i < tamanho; i++)
            {
                int contLinhaX = 0;
                int contLinhaO = 0;
                int contColunaX = 0;
                int contColunaO = 0;

                for (int j = 0; j < tamanho; j++)
                {
                    // Verifica as linhas e colunas
                    if (mapa[i, j] == "X")
                        contLinhaX++;
                    else if (mapa[i, j] == "O")
                        contLinhaO++;

                    if (mapa[j, i] == "X")
                        contColunaX++;
                    else if (mapa[j, i] == "O")
                        contColunaO++;
                }

                if (contLinhaX == tamanho || contColunaX == tamanho)
                    return true;
                if (contLinhaO == tamanho || contColunaO == tamanho)
                    return true;
            }

            int contDiagonalPrincipalX = 0;
            int contDiagonalPrincipalO = 0;
            int contDiagonalSecundariaX = 0;
            int contDiagonalSecundariaO = 0;

            for (int i = 0; i < tamanho; i++)
            {
                // Verifica a diagonal principal
                if (mapa[i, i] == "X")
                    contDiagonalPrincipalX++;
                else if (mapa[i, i] == "O")
                    contDiagonalPrincipalO++;

                // Verifica a diagonal secundária
                if (mapa[i, tamanho - 1 - i] == "X")
                    contDiagonalSecundariaX++;
                else if (mapa[i, tamanho - 1 - i] == "O")
                    contDiagonalSecundariaO++;
            }

            if (contDiagonalPrincipalX == tamanho || contDiagonalSecundariaX == tamanho)
                return true;
            if (contDiagonalPrincipalO == tamanho || contDiagonalSecundariaO == tamanho)
                return true;

            return false;
        }

    }
}