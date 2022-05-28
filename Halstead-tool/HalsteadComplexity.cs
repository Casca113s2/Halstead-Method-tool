using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace HalsteadComplexity
{
    class HalsteadComplexity
    {
        public static List<string> reorderVariables(List<string> variables)
        {
            int[] lengths = new int[variables.Count];

            for (int i = 0; i < variables.Count; i++)
            {
                lengths[i] = variables[i].Length;
            }

            for (int i = 0; i < lengths.Length; i++)
            {
                for (int j = i + 1; j < lengths.Length; j++)
                {
                    if (lengths[i] < lengths[j])
                    {
                        var temp = lengths[i];
                        lengths[i] = lengths[j];
                        lengths[j] = temp;
                        var tmp_var = variables[i];
                        variables[i] = variables[j];
                        variables[j] = tmp_var;
                    }
                }
            }

            return variables;
        }

        public static List<string> extractConstants(string line) // extract constants from string
        {
            bool continueFlag = false;
            List<string> extracted = new List<string>();
            string temp = "";

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] >= '0' && line[i] <= '9')
                {
                    if (!continueFlag)
                    {
                        continueFlag = !continueFlag;
                    }
                    temp = temp + line[i].ToString() + "";
                }
                else
                {
                    if (continueFlag)
                    {
                        extracted.Add(temp);
                        temp = "";
                        continueFlag = !continueFlag;
                    }
                }
            }

            return extracted;
        }
        public static Dictionary<string, int> getUniqueCount(List<string> list)
        {
            Dictionary<string, int> uniqueList = new Dictionary<string, int>();

            for (int i = 0; i < list.Count; i++)
            {
                var s = list[i];
                if (!uniqueList.ContainsKey(s))
                {
                    var count = 0;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].Equals(s))
                        {
                            count++;
                        }
                    }
                    uniqueList[s] = count;
                }
            }

            return uniqueList;
        }

        public static void displayMetrics(int N1, int N2, int n1, int n2)
        {
            int N;
            int n;
            float V;
            float D;
            float L;
            float E;
            float T;
            float B;
            N = N1 + N2;
            n = n1 + n2;
            V = N * (float)(Math.Log(n) / Math.Log(2));
            D = ((int)(n1 / 2)) * ((int)(N2 / n2));
            L = 1 / D;
            E = V * D;
            T = E / 18;
            B = (float)(Math.Pow(E, (int)(2 / 3)) / 3000);
            Console.WriteLine("\t[N] Program Length      : " + N.ToString());
            Console.WriteLine("\t[n] Vocabulary Size     : " + n.ToString());
            Console.WriteLine("\t[V] Program Volume      : " + V.ToString());
            Console.WriteLine("\t[D] Difficulty          : " + D.ToString());
            Console.WriteLine("\t[K] Program Level       : " + L.ToString());
            Console.WriteLine("\t[E] Effort to implement : " + E.ToString());
            Console.Write("\t[T] Time to implement   : ");
            Console.WriteLine(T);
            Console.Write("\t[B] # of delivered bugs : ");
            Console.WriteLine(B);
        }

        public static void Main(string[] args)
        {
            try
            {
                string[] keywords = { "scanf", "printf", "main", "static" };
                // datatypes shouldnt be added to keywords 
                string[] datatypes = { "int", "float", "double", "char" };
                List<string> operators = new List<String>();
                List<string> operands = new List<String>();
                List<string> variables = new List<String>();

                int operatorCount = 0;
                int operandCount = 0;
                bool skipFlag = false;
                StreamReader reader = new StreamReader("C:\\Users\\Casca\\Desktop\\Halstead\\Halstead-tool\\Halstead-tool\\Halstead-tool\\code.c");
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    foreach (string keyword in keywords)
                    {
                        if (line.StartsWith(keyword))
                        {
                            line = line.Substring(0 + keyword.Length);
                            operators.Add(keyword);
                            operatorCount++;
                        }
                    }

                    foreach (string datatype in datatypes)
                    {
                        if (line.StartsWith(datatype))
                        {
                            operators.Add(datatype);
                            operatorCount++;
                            int index = line.IndexOf(datatype);
                            int length = (line.Length - 1) - (index + datatype.Length);
                            line = line.Substring(index+datatype.Length, length); // -1 to ignore the semicolon

                            string[] vars = line.Split(',');
                            for(int i=0; i<vars.Length; i++)
                            {
                                vars[i] = vars[i].Trim();
                                variables.Add(vars[i]);
                            }
                        }
                    }

                    variables = reorderVariables(variables); // very important !
                    skipFlag = false;
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] >= 'A' && line[i] <= 'Z' || line[i] >= 'a' && line[i] <= 'z' || line[i] >= '0' && line[i] <= '9' || line[i] == ' ' || line[i] == ',' || line[i] == ';' || line[i] == '(' || line[i] == '{')
                        {
                        }
                        else if (line[i] == ')')
                        {
                            if (skipFlag == false)
                            {
                                operatorCount++;
                                operators.Add("()");
                            }
                        }
                        else if (line[i] == '}')
                        {
                            if (skipFlag == false)
                            {
                                operatorCount++;
                                operators.Add("{}");
                            }
                        }
                        else if (line[i] == '\"')
                        {
                            // for detecting double quotes
                            skipFlag = !skipFlag;
                            if (skipFlag)
                            {
                                operandCount++;
                            }
                            else
                            {
                                int startIndex = line.IndexOf("\"");
                                int endIndex = line.LastIndexOf("\"");
                                int length = (endIndex + 1) - startIndex;
                                operands.Add(line.Substring(startIndex, length));
                            }
                        }
                        else
                        {
                            if (!skipFlag)
                            {
                                operators.Add(line[i].ToString() + "");
                                operatorCount++;
                            }
                        }
                    }
                    // removing string literals from line if any
                    if (line.Contains("\""))
                    {
                        int startIndex = line.IndexOf("\"");
                        int endIndex = line.LastIndexOf("\"");
                        line = line.Substring(0, startIndex) + line.Substring(endIndex + 1);
                    }

                    foreach (string variable in variables)
                    {
                        while (line.Contains(variable))
                        {
                            int index = line.IndexOf(variable);
                            int length = line.Length - (index + variable.Length);
                            line = line.Substring(0, index) + line.Substring(index + variable.Length, length);
                            operands.Add(variable);
                            operandCount++;
                        }
                    }

                    // checking for constants 
                    operands.AddRange(extractConstants(line));
                }

                Console.WriteLine("Operators identified : ");
                foreach (string o in operators)
                {
                    Console.WriteLine("\t" + o);
                }

                Console.WriteLine("Operands identified : ");
                foreach (string o in operands)
                {
                    Console.WriteLine("\t" + o);
                }

                Console.WriteLine("Variables identified : ");
                foreach (string v in variables)
                {
                    Console.WriteLine("\t" + v);
                }

                Console.WriteLine("Number of operators (N1) " + operators.Count.ToString());
                Console.WriteLine("Number of operands  (N2) " + operands.Count.ToString());

                Dictionary<string, int> uniqueOperators = getUniqueCount(operators);
                Dictionary<string, int> uniqueOperands = getUniqueCount(operands);
                Console.WriteLine("Number of unique operators (n1) " + uniqueOperators.Count.ToString());
                Console.WriteLine("Number of unique operands (n2) " + uniqueOperands.Count.ToString());

                displayMetrics(operators.Count, operands.Count, uniqueOperators.Count, uniqueOperands.Count);
                reader.Close();
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe);
            }

            Console.ReadLine();
        }
    }
}
