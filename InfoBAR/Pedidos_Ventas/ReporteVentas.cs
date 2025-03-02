﻿using InfoBAR.Properties;
using InfoBAR.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoBAR
{
    public partial class ReporteVentas : Form
    {
        private int IdPedidoSeleccionado;
        public ReporteVentas()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Todas las ventas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTodas.Checked)
            {
                MensajesError.BorrarError(lblError);

                dataGridView1.Rows.Clear();
                //Buscar en la based de datos
                try
                {
                    using (InfobarEntities db = new InfobarEntities())
                    {
                        #region LINQ Busqueda en Base de datos

                        //Traer todas las ventas/pedidos con tipo de pago y usuario
                        var pedidosYDetalles = from pedi in db.Pedido
                                               join tipo in db.TipoPago on pedi.Id_TipoPago equals tipo.Id_TipoPago into PedidoPago
                                               from pdp in PedidoPago.DefaultIfEmpty()
                                               join user in db.Usuario on pedi.Id_Usuario equals user.Id
                                               where pedi.Id_TipoPago != null  
                                               select new
                                               {
                                                   Pedido = pedi,
                                                   PagoPedido = pdp,
                                                   Usuario = user,
                                               };
                        var total = (from pedi in db.Pedido
                                     where pedi.Id_TipoPago != null
                                     select pedi.Importe_Total).ToList().Sum();
                        #endregion
                        int count = 0;
                        //Verificar si no se encontraron pedidos
                        if (pedidosYDetalles.Any())
                        {
                            //Añadir al datagrid
                            foreach (var i in pedidosYDetalles)
                            {
                                var tipopago = "";
                                if (i.PagoPedido == null)
                                {
                                    tipopago = "No Pagado";
                                }
                                else
                                {
                                    tipopago = i.PagoPedido.Descripcion;
                                }
                                dataGridView1.Rows.Add(i.Pedido.Id_Pedido, tipopago, i.Pedido.Mesa, i.Pedido.Importe_Total, i.Usuario.Nombre, i.Pedido.Fecha.Value.ToString("dd/MM/yyyy"));
                                count += 1;
                            }
                            //Format agrega comas
                            lblRecaudado.Text = String.Format("{0:n}", total);
                            lblProm.Text = Math.Round((double)total / count,2).ToString();
                        }
                        else
                        {
                            MensajesError.MostrarError(lblError, "No se encontraron pedidos");
                        }

                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo traer los productos de la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ResetearGrid();
            }
        }

        private void ResetearGrid()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            CheckBoxs.DesHabilitarCheckboxs(groupBox1);
        }

        private void chkTipo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTipo.Checked)
            {
                MensajesError.BorrarError(lblError);
                dataGridView1.Rows.Clear();
                //Buscar en la base de datos
                try
                {
                    using (InfobarEntities db = new InfobarEntities())
                    {
                        #region LINQ Busqueda en Base de datos
                        //Traer todas las ventas/pedidos por tipo de pago
                        var pedidosYDetalles = from pedi in db.Pedido
                                               join tipo in db.TipoPago on pedi.Id_TipoPago equals tipo.Id_TipoPago into PedidoPago
                                               from pdp in PedidoPago.DefaultIfEmpty()
                                               join user in db.Usuario on pedi.Id_Usuario equals user.Id
                                               where pdp.Id_TipoPago == cboTipo.SelectedIndex + 1
                                               select new
                                               {
                                                   Pedido = pedi,
                                                   PagoPedido = pdp,
                                                   Total = pdp.Pedido.Sum(pedi => pedi.Importe_Total),
                                                   Usuario = user
                                               };

                        #endregion
                        int count = 0;
                        //Verificar si no se encontraron pedidos
                        if (pedidosYDetalles.Any())
                        {
                            //Añadir al datagrid
                            foreach (var i in pedidosYDetalles)
                            {
                                var tipopago = "";
                                if (i.PagoPedido == null)
                                {
                                    tipopago = "No Pagado";
                                }
                                else
                                {
                                    tipopago = i.PagoPedido.Descripcion;
                                }
                                dataGridView1.Rows.Add(i.Pedido.Id_Pedido, tipopago, i.Pedido.Mesa, i.Pedido.Importe_Total, i.Usuario.Nombre, i.Pedido.Fecha.Value.ToString("dd/MM/yyyy"));
                                count += 1;
                            }
                            //Mostrar total
                            lblRecaudado.Text = String.Format("{0:n}", pedidosYDetalles.FirstOrDefault().Total);
                            lblProm.Text = Math.Round(
                                (double)pedidosYDetalles.FirstOrDefault().Total / count, 2).ToString();
                        }
                        else
                        {
                            MensajesError.MostrarError(lblError, "No se encontraron pedidos");
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo traer las ventas de la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ResetearGrid();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Por Fecha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkFecha_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFecha.Checked)
            {
                MensajesError.BorrarError(lblError);
                dataGridView1.Rows.Clear();
                //Buscar en la base de datos
                try
                {
                    using (InfobarEntities db = new InfobarEntities())
                    {
                        #region LINQ Busqueda en Base de datos
                        //Traer todas las ventas/pedidos por fecha
                        var pedidosYDetalles = from pedi in db.Pedido
                                               join tipo in db.TipoPago on pedi.Id_TipoPago equals tipo.Id_TipoPago into PedidoPago
                                               from pdp in PedidoPago.DefaultIfEmpty()
                                               join user in db.Usuario on pedi.Id_Usuario equals user.Id
                                               where DbFunctions.TruncateTime(pedi.Fecha.Value) == DbFunctions.TruncateTime(dateFecha.Value) && pedi.Id_TipoPago != null
                                               select new
                                               {
                                                   Pedido = pedi,
                                                   PagoPedido = pdp,
                                                   Usuario = user
                                               };

                        var total = from pedi in db.Pedido
                                    where DbFunctions.TruncateTime(pedi.Fecha.Value) == DbFunctions.TruncateTime(dateFecha.Value)
                                    && pedi.Id_TipoPago != null
                                    group pedi by DbFunctions.TruncateTime(pedi.Fecha.Value) into pdf
                                    select new
                                    {
                                        Total = pdf.Sum(pedi => pedi.Importe_Total)
                                    };

                        #endregion
                        int count = 0;
                        //Verificar si no se encontraron pedidos
                        if (pedidosYDetalles.Any())
                        {
                            //Añadir al datagrid
                            foreach (var i in pedidosYDetalles)
                            {
                                var tipopago = "";
                                if (i.PagoPedido == null)
                                {
                                    tipopago = "No Pagado";
                                }
                                else
                                {
                                    tipopago = i.PagoPedido.Descripcion;
                                }
                                dataGridView1.Rows.Add(i.Pedido.Id_Pedido, tipopago, i.Pedido.Mesa, i.Pedido.Importe_Total, i.Usuario.Nombre, i.Pedido.Fecha.Value.ToString("dd/MM/yyyy"));
                                count += 1;
                            }
                            lblRecaudado.Text = String.Format("{0:n}", total.FirstOrDefault().Total);
                            lblProm.Text = Math.Round(
                               (double)total.FirstOrDefault().Total / count, 2).ToString();
                        }
                        else
                        {
                            MensajesError.MostrarError(lblError,"No se encontraron pedidos");
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo traer las ventas de la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                ResetearGrid();
            }
        }

        private void chkPeriodo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPeriodo.Checked)
            {
                MensajesError.BorrarError(lblError);

                dataGridView1.Rows.Clear();
                //Buscar en la base de datos
                try
                {
                    using (InfobarEntities db = new InfobarEntities())
                    {
                        #region LINQ Busqueda en Base de datos
                        //Traer todas las ventas/pedidos por fechas desde hasta
                        var pedidosYDetalles = from pedi in db.Pedido
                                               join tipo in db.TipoPago on pedi.Id_TipoPago equals tipo.Id_TipoPago into PedidoPago
                                               from pdp in PedidoPago.DefaultIfEmpty()
                                               join user in db.Usuario on pedi.Id_Usuario equals user.Id
                                               where (DbFunctions.TruncateTime(dateDesde.Value) <= DbFunctions.TruncateTime(pedi.Fecha.Value) &&
                                               DbFunctions.TruncateTime(dateHasta.Value) >= DbFunctions.TruncateTime(pedi.Fecha.Value)) && pedi.Id_TipoPago != null
                                               select new
                                               {
                                                   Pedido = pedi,
                                                   PagoPedido = pdp,
                                                   Usuario = user
                                               };
                        //Calcular el total del periodo
                        var total = from pedi in db.Pedido
                                    where (DbFunctions.TruncateTime(dateDesde.Value) <= DbFunctions.TruncateTime(pedi.Fecha.Value) &&
                                               DbFunctions.TruncateTime(dateHasta.Value) >= DbFunctions.TruncateTime(pedi.Fecha.Value))
                                    && pedi.Id_TipoPago != null
                                    group pedi by (DbFunctions.TruncateTime(dateDesde.Value) <= DbFunctions.TruncateTime(pedi.Fecha.Value) &&
                                               DbFunctions.TruncateTime(dateHasta.Value) >= DbFunctions.TruncateTime(pedi.Fecha.Value)) into pdf
                                    select new
                                    {
                                        Total = pdf.Sum(pedi => pedi.Importe_Total)
                                    };
                        #endregion
                        int count = 0;
                        //Verificar si no se encontraron pedidos
                        if (pedidosYDetalles.Any())
                        {
                            //Añadir al datagrid
                            foreach (var i in pedidosYDetalles)
                            {
                                var tipopago = "";
                                if (i.PagoPedido == null)
                                {
                                    tipopago = "No Pagado";
                                }
                                else
                                {
                                    tipopago = i.PagoPedido.Descripcion;
                                }
                                dataGridView1.Rows.Add(i.Pedido.Id_Pedido, tipopago, i.Pedido.Mesa, i.Pedido.Importe_Total, i.Usuario.Nombre, i.Pedido.Fecha.Value.ToString("dd/MM/yyyy"));
                                count += 1;
                            }
                            //Mostrar recaudacion
                            lblRecaudado.Text = String.Format("{0:n}", total.FirstOrDefault().Total);
                            lblProm.Text = Math.Round(
                               (double)total.FirstOrDefault().Total / count, 2).ToString();
                        }
                        else
                        {
                            MensajesError.MostrarError(lblError, "No se encontraron pedidos");
                        }

                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo traer los productos de la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ResetearGrid();
            }
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            int selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            //Seleccionar una sola
            if (selectedRowCount > 0 && selectedRowCount <= 1)
            {
                //Añade a la lista
                IdPedidoSeleccionado = int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                string TipoPago = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                int Mesa = int.Parse(dataGridView1.SelectedRows[0].Cells[2].Value.ToString());
                float Total = float.Parse(dataGridView1.SelectedRows[0].Cells[3].Value.ToString());
                string Usuario = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                string Fecha = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
                InfoBAR.openChildForm(
                    new DetallePedido(
                        IdPedidoSeleccionado, TipoPago, Usuario, Mesa, Fecha, Total, this)
                    );
            }
            else
            {
                //TODO Mensaje de que solo se puede seleccionar una sola fila
            }
        }

        private void btnGrafico_Click(object sender, EventArgs e)
        {
            Form graficos = new GraficoPorPeriodo();
            graficos.Show();
        }

        private void btnGraficoPorTipo_Click(object sender, EventArgs e)
        {
            Form graficos = new GraficoPorTipo();
            graficos.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form graficos = new GraficoTodas();
            graficos.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
