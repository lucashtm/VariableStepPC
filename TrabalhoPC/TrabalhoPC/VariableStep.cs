﻿using System;
using TrabalhoPC;
using System.Collections;
using System.Collections.Generic;
public class VariableStep

{
    double A, B, Alfa, Sigma, Tol, Hmax, Hmin, Wp, Wc, q;

    double  h;
    double[] saida;
    Boolean Flag, Last, Nflag;
    RungeKutta4 RK4 = new RungeKutta4();
    Ponto[] RespRK = new Ponto[4];
    int i;



    public ArrayList Executa(double extremoA, double extremoB, double alpha, double tolerancia, double Hmax, double Hmin)
    {
        A = extremoA;
        B = extremoB;
        Alfa = alpha;
        this.Hmax = Hmax;
        this.Hmin = Hmin;
        Tol = tolerancia;



        // saida[0] = t, saida[1] = w, saida[2] = h
        double temp1, temp2, temp3, temp4;
        //double w = Alfa;
        ArrayList ultimateResp = new ArrayList();
        List<Double> t = new List<Double>();
        List<Double> w = new List<Double>();

        t.Add(A);
        w.Add(Alfa);
        h = Hmax;
        Flag = true;
        Last = false;
        RespRK = RK4.Executa(h, w[0], t[0]);
        divideRespRK(t, w, RespRK);
        Nflag = true;
        i = 4;
        double Taux = t[3] + h;
        while (Flag)
        {
            Console.WriteLine(h);

            temp1 = RK4.CalculaDiferencial(t[i - 1], w[i - 1]);
            temp2 = RK4.CalculaDiferencial(t[i - 2], w[i - 2]);
            temp3 = RK4.CalculaDiferencial(t[i - 3], w[i - 3]);
            temp4 = RK4.CalculaDiferencial(t[i - 4], w[i - 4]);
            Wp = w[i - 1] + (h / 24d) * ((55d * temp1) - (59d * temp2) + (37d * temp3) - (9d * temp4));
            temp1 = RK4.CalculaDiferencial(Taux, Wp);
            temp2 = RK4.CalculaDiferencial(t[i - 1], w[i - 1]);
            temp3 = RK4.CalculaDiferencial(t[i - 2], w[i - 2]);
            temp4 = RK4.CalculaDiferencial(t[i - 3], w[i - 3]);
            Wc = w[i - 1] + (h / 24d) * ((9d * temp1) + (19d * temp2) - (5d * temp3) + temp4);
            Sigma = 19d * (Math.Abs(Wc - Wp)) / (270d * h);
            Console.WriteLine("Sigma=" + Sigma);

            if (Sigma <= Tol)  //Passo 6
            {
                w.Insert(i, Wc); //Passo 7
                t.Insert(i, Taux);
                if (Nflag)      //Passo 8
                {
                    saida = new double[4];
                    saida[0] = i-4;
                    saida[1] = t[i - 4];
                    saida[2] = w[i - 4];
                    saida[3] = h;
                    //ultimateResp.Insert(i - 4, saida);
                    ultimateResp.Add(saida);

                    for (int j = i - 3; j < i; j++)
                    {
                        saida = new double[4];
                        saida[0] = j;
                        saida[1] = t[j];
                        saida[2] = w[j];
                        saida[3] = h;
                        //ultimateResp.Insert(j, saida);
                        ultimateResp.Add(saida);
                    }
                }
                else
                {
                    saida = new double[4];
                    saida[0] = i-1;
                    saida[1] = t[i-1];
                    saida[2] = w[i-1];
                    saida[3] = h;
                    ultimateResp.Insert(i-1, saida);
                }
                if (Last)       //Passo 9
                {
                    Flag = false;

                }
                else
                {
                    i++;
                    Nflag = false;

                    if ((Sigma <=( 0.1 * Tol)) || ((t[i - 1] + h) > B)) //passo 11
                    {
                        double qAux = Tol / (2d * Sigma);
                        q = Math.Pow(qAux, 0.25);

                        if (q > 4)
                        {
                            h = 4d * h;
                        }
                        else
                        {
                            h = q * h;
                        }

                        if (h > Hmax)
                        {
                            h = Hmax;
                        }


                        if ((t[i - 1] + 4d * h) > B)
                        {
                            h = (B - t[i - 1]) / 4d;
                            Last = true;
                        }


                        RespRK = RK4.Executa(h, w[i - 1], t[i - 1]); //passo 16
                        divideRespRK(t, w, RespRK);

                        Nflag = true;
                        i = i + 3;
                    }


                }
            }
            else  //Passo 17
            {
                double qAux = Tol / (2d * Sigma);
                q = Math.Pow(qAux, 0.25);

                //Console.WriteLine(q);
                if (q < 0.1)        //Passo 18
                {
                    h = 0.1 * h;
                }
                else
                {
                    h = q * h;
                }

                if (h < Hmin)      //passo 19
                {
                    Flag = false;
                    Console.WriteLine("Hmin ultrapassado");
                }
                else
                {
                    if (Nflag)
                    {
                        i = i - 3;

                        //for (int i = 0; i < t.Count; i++)
                        //{
                        //    Console.WriteLine(t[i]);
                        //}

                        //Console.WriteLine();
                        t.RemoveAt(i + 2);
                        t.RemoveAt(i + 1);
                        t.RemoveAt(i);

                        //for (int i = 0; i < t.Count; i++)
                        //{
                        //    Console.WriteLine(t[i]);
                        //}

                        w.RemoveAt(i + 2);
                        w.RemoveAt(i + 1);
                        w.RemoveAt(i);
                    }
                    //Console.WriteLine(h);
                    RespRK = RK4.Executa(h, w[i - 1], t[i - 1]);
                    divideRespRK(t, w, RespRK);

                   // Console.WriteLine();

                    //for (int i = 0; i < t.Count; i++)
                    //{
                    //    Console.WriteLine(t[i]);
                    //}

                    i = i + 3;
                    Nflag = true;
                }
                // Console.WriteLine(i - 1);
               
            }
            Taux = t[i - 1] + h;        //Passo 20
        }







        saida = new double[4];
        saida[0] = i;
        saida[1] = t[i];
        saida[2] = w[i];
        saida[3] = h;
        ultimateResp.Insert(i, saida);
        return ultimateResp;
    }
    public void toString(ArrayList ultimateResp)
    {
        double[] vaux = new double[4];
        for (int i = 0; i < ultimateResp.Count; i++)
        {
            vaux = (double[])ultimateResp[i];
            
            Console.Write(vaux[0] + " "+vaux[1] + " " + vaux[2] + " " + vaux[3]);
                
            
            Console.WriteLine("");
        }
    }
    private void divideRespRK(List<Double> t, List<Double> w, Ponto[] RespRK)
    {
        for (int i = 0; i < RespRK.Length; i++)
        {
            t.Add(RespRK[i].X);
            w.Add(RespRK[i].Y);
        }
    }
}
