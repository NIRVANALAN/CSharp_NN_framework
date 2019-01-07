﻿using System;

namespace cs_nn_fm
{
    public class Model
    {
        public Layer[] Layers { get; }
        public int LayerNum;
        public double[][] LayerSum; // should start from 1
        public double[][] Nodes { get; }
        public double[] XValues;

        public Model(Layer[] layers)
        {
            Layers = layers;
            LayerNum = 0;
            foreach (var t in Layers) // init the nodes
            {
                if (t.GetType() != typeof(PropogationLayer)) continue;
                var cLayer = (PropogationLayer) t;
                if (Nodes[LayerNum] == null)
                {
                    Nodes[LayerNum] = new double[t.DIn + 1]; // add biases
                    Nodes[LayerNum][t.DIn] = 1; // for bias
                }

                if (Nodes[LayerNum].Length == t.DIn) // check the LastLayer.DOut==ThisLayer.Dint
                {
                    LayerNum++;
//                    cLayer.Weights = Helper.MakeMatrix(cLayer.DIn + 1, cLayer.DOut);
//                    Helper.InitializeWeights(ref cLayer.Weights); // init weights
                    Nodes[LayerNum] = new double[t.DOut + 1];
                    Nodes[LayerNum][t.DOut] = 1; // for bias
                    LayerSum[LayerNum] = new double[t.DOut + 1];
                    LayerSum[LayerNum][t.DOut] = 1;//for bias
                }
                else
                {
                    throw new Exception("layer not compatible in layerNum: " + LayerNum.ToString());
                }
            }
        }

        public double[] Forward()
        {
            //check input dimension
            if (XValues.Length + 1 != Nodes[0].Length)
            {
                throw new Exception("Input x_value not compatible");
            }

            // copy x_input to nodes[0]
            for (int i = 0; i < XValues.Length; i++)
            {
                Nodes[0][i] = XValues[i]; // nn input
            }

//            nodes[0][x_values.Length] = 1; // have done above
            // forward
            var currentLayer = 0;
//            var activationNextFlag = false;
            foreach (var t in Layers)
            {
                if (t.GetType() == typeof(PropogationLayer))
                {
                    var cLayer = (PropogationLayer) t; // this is a Propogation Layer
                    for (int j = 0; j < Nodes[currentLayer+1].Length-1; j++)//input-next-layer -1 to remove bias-node
                    {
                        for (int i = 0; i < Nodes[currentLayer].Length; i++)//input-layer
                        {
                            LayerSum[currentLayer + 1][j] += Nodes[currentLayer][i] * cLayer.Weights[i, j];
                        }
                    }

                    currentLayer++; // point to next input-layer
//                    activationNextFlag = !activationNextFlag; // should be true in next circulation, proving that ActivationFunc is needed next
                    Nodes[currentLayer] = LayerSum[currentLayer]; // avoid no activation after
                    Nodes[currentLayer][cLayer.DOut] = 1; //should be 1,for bias
                    continue;
                }

                // activation
                if ((t.GetType() != typeof(Activation))) // check at last if
                    throw new Exception("Don't accept this type of Class in Forward: "+t.GetType());
                {
                    var cLayer = (ActivationLayer) t;
                    // polymorphism calculate activation
                    Nodes[currentLayer] = cLayer.Calculate(ref LayerSum[currentLayer]);
                    Nodes[currentLayer][cLayer.DOut] = 1; //should be 1,for bias
                }
            } // forward finish

            return Nodes[currentLayer];//result
        }
    }
}