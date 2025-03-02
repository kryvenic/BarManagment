﻿using InfoBAR.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoBAR
{
    public partial class AgregarPedido : Form
    {
        
        private FiltroSeleccionado filtro;

        public AgregarPedido()
        {
            InitializeComponent();
            ((DataGridViewImageColumn)gridProductos.Columns[4]).ImageLayout = DataGridViewImageCellLayout.Stretch;
            filtro = FiltroSeleccionado.Ninguno;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void AgregarPedido_Load(object sender, EventArgs e)
        {
        }

        private void btnEnlistar_Click(object sender, EventArgs e)
        {
            int selectedRowCount = gridProductos.Rows.GetRowCount(DataGridViewElementStates.Selected);
            //Permite agregar varias filas a la vez con la misma cantidad del producto seleccionado
            if (selectedRowCount > 0)
            {
                //Recorre cada fila
                for (int i = 0; i < selectedRowCount; i++)
                {
                    //Guarda los valores por fila
                    List<string> valoresPorFila = new List<string>();
                    //Recorre cada valor de celda
                    for (int j = 0; j < gridProductos.ColumnCount; j++)
                    {
                        //Añade
                        valoresPorFila.Add(gridProductos.SelectedRows[i].Cells[j].Value.ToString());
                    }
                    //Si el mismo producto ya esta en la lista, modificar solo su cantidad
                    int IndexFila = IdEnDatagridPedido(int.Parse(valoresPorFila[0]));
                    //El producto esta en la lista
                    if (IndexFila != -1)
                    {
                        ActualizarDatagridPedido(IndexFila, int.Parse(txtCantidad.Text));
                    }
                    else //No esta, agregar nuevo
                    {
                        AgregarAlDatagridPedido(valoresPorFila);

                    }
                    //Limpia la lista temporal
                    valoresPorFila.Clear();
                }
            }
        }



        private void AgregarAlDatagridPedido(List<string> valoresPorFila)
        {
            if (txtCantidad.Text.Equals("")) return;
            gridPedido.Rows.Add(valoresPorFila[0], valoresPorFila[1], txtCantidad.Text, float.Parse(valoresPorFila[3]),
                float.Parse(valoresPorFila[3]) * float.Parse(txtCantidad.Text));
            ActualizarTotal();
        }

        /// <summary>
        /// Verifica si el pedido ya ha sido agregado al datagrid pedido y devuelve
        /// el index para modificar luego la fila.
        /// </summary>
        /// <param name="compararId"></param>
        /// <returns></returns>
        private int IdEnDatagridPedido(int compararId)
        {
            foreach (DataGridViewRow row in gridPedido.Rows)
            {
                if (int.Parse(row.Cells[0].Value.ToString()) == compararId)
                {
                    return row.Index;
                }
            }
            return -1;
        }

        private void ActualizarDatagridPedido(int filaIndex, int cantidadNueva)
        {
            if (txtCantidad.Text.Equals("")) return;
            gridPedido.Rows[filaIndex].Cells["Cantidad"].Value = cantidadNueva;
            gridPedido.Rows[filaIndex].Cells["Importe"].Value =
                Convert.ToDecimal(gridPedido.Rows[filaIndex].Cells[3].Value) * cantidadNueva;
            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            lblTotal.Text = String.Format("{0:n}", CalcularImporteTotal());
        }


        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (gridPedido.Rows.Count > 0)
            {
                RegistrarPedidoEnBase();
            }
            else
            {
                MessageBox.Show("No hay comidas/bebidas agregadas al pedido", "Error: No hay productos agregados", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void RegistrarPedidoEnBase()
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show("¿Quiere registrar el pedido? ", "Confirmar alta", buttons, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                //Agregar a la base de datos
                try
                {
                    using (InfobarEntities db = new InfobarEntities())
                    {
                        Pedido oPedido = new Pedido();
                        int IdUsuario = (from user in db.Usuario
                                         where user.Nombre.Contains(Global.Usuario)
                                         select user.Id).FirstOrDefault();
                        oPedido.Id_Usuario = IdUsuario;
                        oPedido.Fecha = DateTime.Now;
                        oPedido.Mesa = int.Parse(txtMesa.Text);
                        oPedido.Importe_Total = decimal.Parse(lblTotal.Text);
                        db.Pedido.Add(oPedido);

                        db.SaveChanges();

                    }
                    using (InfobarEntities db = new InfobarEntities())
                    {
                        int ultimoId = (from pedido in db.Pedido
                                        select pedido.Id_Pedido).Max();
                        foreach (DataGridViewRow row in gridPedido.Rows)
                        {
                            Detalle_Pedido oDetalle = new Detalle_Pedido();
                            int IDProducto = int.Parse(row.Cells[0].Value.ToString());
                            int Cantidad = int.Parse(row.Cells[2].Value.ToString());
                            decimal Precio = decimal.Parse(row.Cells[3].Value.ToString());
                            decimal PrecioTotal = (decimal.Parse(row.Cells[3].Value.ToString()) * Cantidad);

                            oDetalle.Id_Pedido = ultimoId;
                            // Celda del Id
                            oDetalle.Id_Prod = IDProducto;
                            // Cantidad
                            oDetalle.Cantidad = Cantidad;
                            // Importe
                            oDetalle.Precio = Precio;
                            oDetalle.PrecioTotal = PrecioTotal;
                            db.Detalle_Pedido.Add(oDetalle);
                        }
                        db.SaveChanges();
                    }

                    gridPedido.Rows.Clear();
                    MessageBox.Show("Pedido registrado ", "Agregar pedido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("No se agrego correctamente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BuscarTodos()
        {
            //Buscar en la based de datos
            try
            {
                using (InfobarEntities db = new InfobarEntities())
                {
                    Producto oProducto = new Producto();

                    //Traer todos los productos con categoria
                    var list = from prod in db.Producto
                               join tipo in db.TipoProducto on prod.Id_TipoProd equals tipo.Id_TipoProd
                               where prod.Activado == 1
                               select new
                               {
                                   //Necesario para hacer el foreach
                                   Prod = prod,
                                   Tipo = tipo
                               };

                    //Añadir al datagrid
                    foreach (var i in list)
                    {
                        Image imagen = null;
                        if (i.Prod.Imagen != null)
                        {
                            imagen = Image.FromFile(i.Prod.Imagen);
                        }
                        gridProductos.Rows.Add(i.Prod.Id_Producto, i.Prod.Descripcion, i.Tipo.Descripcion, i.Prod.Precio, imagen);
                    }
                    filtro = FiltroSeleccionado.Todos;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo traer los productos de la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Data grid productos por Tipo
        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                gridProductos.Rows.Clear();
                BuscarTodos();
            }
            else
            {
                ResetearGridProductos();
            }
        }

        private void chkTipo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTipo.Checked)
            {
                gridProductos.Rows.Clear();
                BuscarPorTipo();
            }
            else
            {
                ResetearGridProductos();
            }
        }

        private void BuscarPorTipo()
        {
            //Buscar en la based de datos
            try
            {
                using (InfobarEntities db = new InfobarEntities())
                {
                    Producto oProducto = new Producto();

                    //Traer todos los productos con categoria
                    var list = from prod in db.Producto
                               join tipo in db.TipoProducto on prod.Id_TipoProd equals tipo.Id_TipoProd
                               where tipo.Id_TipoProd == (cboTipo.SelectedIndex + 1) && prod.Activado == 1
                               select new
                               {
                                   //Necesario para hacer el foreach
                                   Prod = prod,
                                   Tipo = tipo
                               };

                    //Añadir al datagrid
                    foreach (var i in list)
                    {
                        Image imagen = null;
                        if (i.Prod.Imagen != null)
                        {
                            imagen = Image.FromFile(i.Prod.Imagen);
                        }
                        gridProductos.Rows.Add(i.Prod.Id_Producto, i.Prod.Descripcion, i.Tipo.Descripcion, i.Prod.Precio, imagen);
                    }
                    //Filtro anterior para refrescar
                    if (cboTipo.SelectedIndex != -1)
                    {
                        filtro = FiltroSeleccionado.Tipo;
                    }
                    else
                    {
                        filtro = FiltroSeleccionado.Ninguno;
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo traer los productos de la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkNombre_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ResetearGridProductos()
        {
            gridProductos.Rows.Clear();
            gridProductos.Refresh();
            CheckBoxs.DesHabilitarCheckboxs(groupBox1);
        }

        private void gridPedido_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            ActualizarTotal();
        }
        /// <summary>
        /// Sumatoria de todos los importes
        /// </summary>
        /// <returns></returns>
        private float CalcularImporteTotal()
        {
            float importeTotal = 0f;
            foreach (DataGridViewRow row in gridPedido.Rows)
            {
                importeTotal += float.Parse(row.Cells["Importe"].Value.ToString());
            }
            return importeTotal;
        }

        private void gridPedido_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ActualizarTotal();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            gridProductos.Rows.Clear();
            if (txtNombre.TextLength != 0)
            {
                BuscarPorNombre();
            }
        }
        private void BuscarPorNombre()
        {
            //Buscar en la based de datos
            try
            {
                using (InfobarEntities db = new InfobarEntities())
                {
                    Producto oProducto = new Producto();
                    //Traer el producto con el nombre
                    var prodBuscado = (from prod in db.Producto
                                       join tipo in db.TipoProducto on prod.Id_TipoProd equals tipo.Id_TipoProd
                                       where prod.Descripcion.Contains(txtNombre.Text)
                                       select new
                                       {
                                           Prod = prod,
                                           Tipo = tipo
                                       }).FirstOrDefault();
                    if (prodBuscado == null)
                    {
                        MessageBox.Show("No se encontro el producto", "Informativo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    //Añadir al datagrid
                    Image imagen = null;
                    if (prodBuscado.Prod.Imagen != null)
                    {
                        imagen = Image.FromFile(prodBuscado.Prod.Imagen);
                    }
                    gridProductos.Rows.Add(prodBuscado.Prod.Id_Producto, prodBuscado.Prod.Descripcion,
                        prodBuscado.Tipo.Descripcion, prodBuscado.Prod.Precio, imagen);
                    filtro = FiltroSeleccionado.Nombre;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo traer los productos de la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnRefr_Click(object sender, EventArgs e)
        {
            ResetearGridProductos();
            switch (filtro)
            {
                case FiltroSeleccionado.Nombre:
                    BuscarPorNombre();
                    break;
                case FiltroSeleccionado.Tipo:
                    BuscarPorTipo();
                    break;
                case FiltroSeleccionado.Todos:
                    BuscarTodos();
                    break;
                case FiltroSeleccionado.Ninguno:
                    break;
            }
        }

        private void lblTotal_Click(object sender, EventArgs e)
        {

        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            VerificarCampos.SoloNumeros(e);
        }

        private void txtMesa_KeyPress(object sender, KeyPressEventArgs e)
        {
            VerificarCampos.SoloNumeros(e);
        }
    }
}
