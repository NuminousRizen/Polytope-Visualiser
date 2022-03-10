using System;
using System.Collections.Generic;
using Polytope2D.UI;
using Polytope2D.Util.Other;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.GUI
{
    public class UIManager : MonoBehaviour
    {
        // Some prefabs
        public GameObject EquationInput2D;
        public GameObject EquationInput3D;
        public GameObject PointInput2D;
        public GameObject PointInput3D;

        public TMP_Dropdown Mode;
        public TMP_Dropdown InputMode;

        public GameObject InputPanel;

        public GameObject AlertBox;

        public PolytopeUI PolytopeDisplay;

        private List<GameObject> Inputs = new List<GameObject>();

        public void AddInput()
        {
            // In 2D mode
            if (Mode.value == 0)
            {
                // Building with points
                if (InputMode.value == 0)
                {
                    GameObject newInput = Instantiate(PointInput2D, InputPanel.transform, false);
                    Transform removeButton = newInput.transform.GetChild(1);
                    removeButton.GetComponent<Button>().onClick.AddListener(delegate { RemoveInput(newInput); });
                    Inputs.Add(newInput);
                }
                // Building with equations
                else if (InputMode.value == 1)
                {
                    GameObject newInput = Instantiate(EquationInput2D, InputPanel.transform, false);
                    Transform removeButton = newInput.transform.GetChild(1);
                    removeButton.GetComponent<Button>().onClick.AddListener(delegate { RemoveInput(newInput); });
                    Inputs.Add(newInput);
                }
            }
            // In 3D mode
            else if (Mode.value == 1)
            {
                // Building with points
                if (InputMode.value == 0)
                {
                    GameObject newInput = Instantiate(PointInput3D, InputPanel.transform, false);
                    Transform removeButton = newInput.transform.GetChild(1);
                    removeButton.GetComponent<Button>().onClick.AddListener(delegate { RemoveInput(newInput); });
                    Inputs.Add(newInput);
                }
                // Building with equations
                else if (InputMode.value == 1)
                {
                    GameObject newInput = Instantiate(EquationInput3D, InputPanel.transform, false);
                    Transform removeButton = newInput.transform.GetChild(1);
                    removeButton.GetComponent<Button>().onClick.AddListener(delegate { RemoveInput(newInput); });
                    Inputs.Add(newInput);
                }
            }
        }

        public void RemoveInput(GameObject toRemove)
        {
            Inputs.Remove(toRemove);
            Destroy(toRemove);
        }

        public void Clear()
        {
            foreach (GameObject input in Inputs)
            {
                Destroy(input);
            }
            
            Inputs.Clear();
        }

        public void Build()
        {
            PolytopeDisplay.Clear();
            // Building with points
            try
            {
                if (InputMode.value == 0)
                {
                    List<VectorD3D> points = new List<VectorD3D>();

                    foreach (GameObject input in Inputs)
                    {
                        List<double> values = new List<double>();
                        foreach (Transform inputField in input.transform.GetChild(0).GetChild(0))
                        {
                            string userInput = inputField.GetComponent<TMP_InputField>().text;
                            values.Add(Convert.ToDouble(userInput));
                        }

                        // In 2D mode
                        if (Mode.value == 0)
                        {
                            values.Add(0);
                        }
                        
                        points.Add(new VectorD3D(values[0], values[1], values[2]));
                    }

                    if (Mode.value == 0)
                    { 
                        PolytopeDisplay.BuildFromPoints2D(points);
                    }
                    else if (Mode.value == 1)
                    { 
                        PolytopeDisplay.BuildFromPoints3D(points);
                    }
                }
            
                // Building with inequalities
                else if (InputMode.value == 1)
                {
                    // In 2D mode
                    if (Mode.value == 0)
                    {
                        List<Inequality> inequalities = new List<Inequality>();

                        foreach (GameObject input in Inputs)
                        {
                            List<double> values = new List<double>();
                            foreach (Transform inputField in input.transform.GetChild(0).GetChild(0))
                            {
                                string userInput = inputField.GetComponent<TMP_InputField>().text;
                                values.Add(Convert.ToDouble(userInput));
                            }

                            inequalities.Add(new Inequality(values[0], values[1], values[2]));
                        }
                        
                        PolytopeDisplay.BuildFromInequalities2D(inequalities);
                    }
                    
                    else if (Mode.value == 1)
                    {
                        List<PlaneInequality> inequalities = new List<PlaneInequality>();
                        foreach (GameObject input in Inputs)
                        {
                            List<double> values = new List<double>();
                            foreach (Transform inputField in input.transform.GetChild(0).GetChild(0))
                            {
                                string userInput = inputField.GetComponent<TMP_InputField>().text;
                                values.Add(Convert.ToDouble(userInput));
                            }

                            inequalities.Add(new PlaneInequality(values[0], values[1], values[2], values[3]));
                        }
                        
                        PolytopeDisplay.BuildFromInequalities3D(inequalities);
                    }
                }
            }
            catch
            {
                AlertBox.GetComponent<AlertBox>().DisplayMessage("Could not build polytope with given inputs");
            }
        }
    }
}
