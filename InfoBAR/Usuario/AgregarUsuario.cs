﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfoBAR.Utilidades;

namespace InfoBAR
{
    public partial class AgregarUsuario : Form
    {
        public AgregarUsuario()
        {
            InitializeComponent();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (VerificarCampos.VerificarCamposVacios(this))
            {
                MessageBox.Show("Debe rellenar/completar todos los campos" , "Error: Campos Vacios", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show("¿Quiere agregar este Usuario?: " + TNombre.Text, "Confirmar alta", buttons, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    //Agregar a la base de datos
                    try
                    {
                        using (InfobarEntities db = new InfobarEntities())
                        {
                            Usuario oUsuario = new Usuario();
                            oUsuario.Nombre = TNombre.Text;
                            oUsuario.Id_Tipo = CTipo.SelectedIndex+1;
                            oUsuario.Clave = TClave.Text;
                            oUsuario.Activado = 1;
                            db.Usuario.Add(oUsuario);
                            db.SaveChanges();
                        }

                        MessageBox.Show("Usuario: " + TNombre.Text + " agregado correctamente ", "Agregar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        VaciarCampos();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("No se agrego correctamente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                    
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Cancelar operacion? ", "Cancelar", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void VaciarCampos()
        {
            TNombre.Text = "";
            CTipo.SelectedIndex = -1;
            TClave.Text = "";
        }
    }
}
