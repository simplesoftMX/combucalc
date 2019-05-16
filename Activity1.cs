using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Android.Util;

using System.Threading.Tasks;
using System.Threading;
using combucalc;

namespace LocalFiles
{
    [Activity(Label = "Combucalc", MainLauncher = true, Icon = "@drawable/combucalclogo")]
	public class Activity1 : Activity
	{
		double count = 0.0;
        double count2 = 0.0;
		static readonly string Filename = "counz.txt";
        static readonly string Filename2 = "counz2.txt";
		string path;
		string filename;
        string filename2;
        
		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            EditText txt_odometrosalida = FindViewById<EditText>(Resource.Id.editText_odometrosalida);
            EditText txt_odometrollegada = FindViewById<EditText>(Resource.Id.editText_odometrollegada);
            EditText txt_kilometrosrecorridos = FindViewById<EditText>(Resource.Id.editText_kilometrosrecorridos);
            EditText txt_costoporlitro = FindViewById<EditText>(Resource.Id.editText_costoporlitro);
            EditText txt_factordeconsumo = FindViewById<EditText>(Resource.Id.editText_factordeconsumo);
            EditText txt_totaldelitrosdeltanque = FindViewById<EditText>(Resource.Id.editText_totallitrosdetanque);
            EditText txt_totaldelistosrestantes = FindViewById<EditText>(Resource.Id.editText_totallitrosrestante);
            EditText txt_costototal = FindViewById<EditText>(Resource.Id.editText_costototal);
            EditText txt_litrosconsumidos = FindViewById<EditText>(Resource.Id.editText_totallitrosconsumidos);

            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			filename = Path.Combine (path, Filename);
            filename2 = Path.Combine(path, Filename2);
            
			Task<double> loadCount = loadFileAsync(1);
            Task<double> loadCount2 = loadFileAsync(2);

			Button btnSave = FindViewById<Button> (Resource.Id.btn_actualizar);
            Button btnSave2 = FindViewById<Button>(Resource.Id.button_factordeconsumo);

			btnSave.Click += async delegate 
            {
                try
                {
                    await writeFileAsync(txt_costoporlitro.Text);
                    Toast.MakeText(this, "Guardado", ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error: "+ex.Message.ToString(), ToastLength.Short).Show();
                }
			};

            btnSave2.Click += async delegate
            {
                try
                {
                    await writeFileAsync2(txt_factordeconsumo.Text);
                    Toast.MakeText(this, "Guardado", ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error: " + ex.Message.ToString(), ToastLength.Short).Show();
                }
            };



			count = await loadCount;
            count2 = await loadCount2;
            txt_costoporlitro.Text = count.ToString();
            txt_factordeconsumo.Text = count2.ToString();

            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate
            {
                try
                {
                    txt_kilometrosrecorridos.Text = (Convert.ToDouble(txt_odometrollegada.Text) - Convert.ToDouble(txt_odometrosalida.Text)).ToString();
                    txt_costototal.Text = ((Convert.ToDouble(txt_kilometrosrecorridos.Text) / Convert.ToDouble(txt_factordeconsumo.Text)) * Convert.ToDouble(txt_costoporlitro.Text)).ToString();
                    txt_totaldelistosrestantes.Text = (Convert.ToDouble(txt_totaldelitrosdeltanque.Text) - (Convert.ToDouble(txt_kilometrosrecorridos.Text) / Convert.ToDouble(txt_factordeconsumo.Text))).ToString();
                    txt_litrosconsumidos.Text = (Convert.ToDouble(txt_totaldelitrosdeltanque.Text) - Convert.ToDouble(txt_totaldelistosrestantes.Text)).ToString();
                }
                catch (Exception)
                {
                    Toast.MakeText(this, "Debes llenar toda la informaci¨®n", ToastLength.Long).Show();
                }
            };

            Button button2 = FindViewById<Button>(Resource.Id.MyButton_2);
            EditText txt_velocidadenkmh = FindViewById<EditText>(Resource.Id.editText_velocidadactual);
            EditText txt_distanciaarecorrer = FindViewById<EditText>(Resource.Id.editText_distanciaarecorrer);

            button2.Click += delegate
            {
                try
                {
                    double velocidad = Convert.ToDouble(txt_velocidadenkmh.Text);
                    double distancia = Convert.ToDouble(txt_distanciaarecorrer.Text);
                    double resultado = distancia / velocidad;
                    double minutos = resultado - (int)resultado;
                    minutos = minutos * 60;
                    int horas = (int)resultado;
                    Toast.MakeText(this, "se necesitar¨¢n: " + horas.ToString() + " horas y " + (int)minutos + " minutos.", ToastLength.Long).Show();
                }
                catch (Exception)
                {
                    Toast.MakeText(this, "Debes llenar toda la informaci¨®n", ToastLength.Long).Show();
                }
            };

		}

		async Task<double> loadFileAsync(int valor)
		{
                if (valor == 2)
                {
                    if (File.Exists(filename2))
                    {
                        using (var f = new StreamReader(OpenFileInput(Filename2)))
                        {
                            string line;
                            do
                            {
                                line = await f.ReadLineAsync();
                            }
                            while (!f.EndOfStream);
                            try
                            {
                                return double.Parse(line);
                            }
                            catch (Exception)
                            {
                                File.Delete(filename);
                                Toast.MakeText(this, "Error al convertir, vuelva a guardar el factor de consumo", ToastLength.Long).Show();
                            }
                        }
                    }
                    return 0.0;
                }

            if (File.Exists(filename))
            {
                using (var f = new StreamReader(OpenFileInput(Filename)))
                {
                    string line;
                    do
                    {
                        line = await f.ReadLineAsync();
                    }
                    while (!f.EndOfStream);
                    try
                    {
                        return double.Parse(line);
                    }
                    catch (Exception)
                    {
                        File.Delete(filename);
                        Toast.MakeText(this, "Error al convertir, vuelva a guardar el costo", ToastLength.Long).Show();
                    }
                }
            }
            return 0.0;
		}

		async Task writeFileAsync(string data)
		{
			using (var f = new StreamWriter (OpenFileOutput (Filename, FileCreationMode.Append | FileCreationMode.WorldReadable))) 
            {
				await f.WriteLineAsync(data).ConfigureAwait(false);
			}
		}
        async Task writeFileAsync2(string data)
        {
            using (var f = new StreamWriter(OpenFileOutput(Filename2, FileCreationMode.Append | FileCreationMode.WorldReadable)))
            {
                await f.WriteLineAsync(data).ConfigureAwait(false);
            }
        }
	}
}
