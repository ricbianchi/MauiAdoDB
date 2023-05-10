﻿using System.Diagnostics;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Text;

namespace ADO_SQLITE;

public partial class DetailsPage : ContentPage
{
    DTO_Details b = new DTO_Details();
    List<DTO_Details> bs = new List<DTO_Details>();
    

    public DetailsPage(Int64 id)
	{
		InitializeComponent();
        GetQuery(id);  
    }

	async void GetQuery(Int64 idLibro)
	{
        try
        {
            string mainDir = FileSystem.Current.AppDataDirectory;
            string fileName = "DB_Libri_App.db";
            //Genero il path completo con anche il nome del file al nuovo file
            string targetFile = System.IO.Path.Combine(mainDir, fileName);
            string connectionTarget = $"Data Source={targetFile}";
            using (var connection = new SqliteConnection(connectionTarget))
            {
                connection.Open();

                //Ricerca autori
                var getAutori = connection.CreateCommand();
                getAutori.CommandText =
                    @"
                        SELECT A.Nome, A.Cognome
                        From Book as B
                        INNER JOIN Write as W on B.ISBN = W.Libro
                        INNER JOIN Author as A on A.Id = W.Autore
                        Where B.ISBN = @idLibro
                    ";
                getAutori.Parameters.AddWithValue("@idLibro", idLibro);

                using (var readerAutori = getAutori.ExecuteReader())
                {
                    if (readerAutori.HasRows)
                    {
                        StringBuilder elencoAutori = new StringBuilder();
                        while (readerAutori.Read())
                        {
                            elencoAutori.Append(readerAutori.GetString(0));
                            elencoAutori.Append(" ");
                            elencoAutori.Append(readerAutori.GetString(1));
                            elencoAutori.Append(", ");
                        }
                        lbl_Autori.Text = elencoAutori.ToString().Substring(0, (elencoAutori.Length) - 2);
                    }
                    //Ricerca libro
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                        SELECT B.ISBN, B.Titolo, B.Anno, B.prezzo, A.Cognome as Autori, P.Nome as Editore, G.Descrizione as GenereNarrativo
                        FROM Book as B
                        INNER JOIN Write as W ON B.ISBN = W.Libro
                        INNER JOIN Author as A ON A.Id = W.Autore
                        INNER JOIN Publisher as P ON B.Editore = P.Id
                        INNER JOIN Genre as G ON B.Genere = G.Id
                        WHERE B.ISBN = @idLibro
                        ";
                    command.Parameters.AddWithValue("@idLibro", idLibro);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                b.ISBN = reader.GetInt64(0);
                                b.Titolo = reader.GetString(1);
                                b.Anno = reader.GetInt32(2);
                                b.Prezzo = reader.GetFloat(3);
                                //b.Autori = reader.GetString(4);
                                b.Editore = reader.GetString(5);
                                b.GenereNarrativo = reader.GetString(6);
                            }
                        }
                    }

                    lbl_ISBN.Text = b.ISBN.ToString();
                    lbl_Titolo.Text = b.Titolo.ToString();
                    lbl_Anno.Text = b.Anno.ToString();
                    lbl_Prezzo.Text = b.Prezzo.ToString();
                    //lbl_Autori.Text = b.Autori.ToString();
                    lbl_Editore.Text = b.Editore.ToString();
                    lbl_Genere.Text = b.GenereNarrativo.ToString();
                }
            }
           
        }
        catch (Exception ex)
        {
            await DisplayAlert(ex.Message, "Errore2", "OK");
        }

    }

}
