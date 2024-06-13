using System;
using System.Collections.Generic;
using System.Threading;

namespace GestionInventario
{
    internal class Program
    {
        // Semaforo para controlar el orden de las tareas
        static SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);

        // Contador para el control de las tarea
        static int taskCounter = 0;

        // Lista de productos y precio del dolar
        static List<Product> productos = new List<Product>();
        static decimal precioDolar = 1.0m;

        static void Main(string[] args)
        {
            productos.Add(new Product { Nombre = "Producto A", Stock = 50, Precio = 10 });
            productos.Add(new Product { Nombre = "Producto B", Stock = 20, Precio = 15 });
            productos.Add(new Product { Nombre = "Producto C", Stock = 30, Precio = 20 });

            Thread task1 = new Thread(ActualizarStock);
            Thread task2 = new Thread(ActualizarPrecioDolar);
            Thread task3 = new Thread(ActualizarPreciosProductos);
            Thread task4 = new Thread(GenerarInforme);

            task1.Start();
            task2.Start();
            task3.Start();
            task4.Start();

            task1.Join();
            task2.Join();
            task3.Join();
            task4.Join();

            Console.WriteLine("Tareas completadas.");
            Console.ReadLine();
        }

        // Tarea para actualizar el stock
        static void ActualizarStock()
        {
            Console.WriteLine("Actualizando stock...");
            foreach (var producto in productos)
            {
                producto.Stock += 10;
            }
            Console.WriteLine("Stock actualizado.");

            Interlocked.Increment(ref taskCounter);
            semaphore.Release();
        }

        // tarea para actualizar el precio del dolar
        static void ActualizarPrecioDolar()
        {
            semaphore.Wait();

            Console.WriteLine("Actualizando precio del dólar...");
            precioDolar = 1.1m;
            Console.WriteLine("Precio del dólar actualizado.");

            Interlocked.Increment(ref taskCounter);
            semaphore.Release();
        }

        //Tarea para actualizar los precios de los productos
        static void ActualizarPreciosProductos()
        {
            semaphore.Wait();

            if (taskCounter == 2)
            {
                Console.WriteLine("Actualizando precios de los productos...");
                foreach (var producto in productos)
                {
                    producto.Precio *= 1.10m;
                }
                Console.WriteLine("Precios de los productos actualizados.");

                Interlocked.Increment(ref taskCounter);
                semaphore.Release();
            }
        }

        // Tarea para generar el informe
        static void GenerarInforme()
        {
            semaphore.Wait();

            if (taskCounter == 3)
            {
                Console.WriteLine("Generando informe...");
                decimal totalInventario = 0;
                foreach (var producto in productos)
                {
                    totalInventario += producto.Stock * producto.Precio * precioDolar;
                    Console.WriteLine($"Producto: {producto.Nombre}, Stock: {producto.Stock}, Precio: {producto.Precio:C}");
                }
                Console.WriteLine($"Valor total del inventario: {totalInventario:C}");
            }
        }

        // clase para representar un producto.
        class Product
        {
            public string Nombre { get; set; }
            public int Stock { get; set; }
            public decimal Precio { get; set; }
        }
    }
}