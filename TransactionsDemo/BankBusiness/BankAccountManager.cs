﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDAL
{
    public class BankAccountManager
    {
        public void TransfererArgent(string ibanOrigine, string ibanDestination, double montantATransferer)
        {
            using (SqlConnection cn = GetDatabaseConnection())
            {
                cn.Open();
                try
                {
                    DebiterDe(250, ibanOrigine, cn);
                    CrediterDe(250, ibanDestination, cn);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private void CrediterDe(int montantAAjouter, string iban, SqlConnection cn)
        {
            var command = new SqlCommand("UPDATE [CompteEnBanque] SET [Solde]=[Solde]+@montantAAjouter WHERE [IBAN]=@iban", cn);
            command.Parameters.AddWithValue("@montantAAjouter", montantAAjouter);
            command.Parameters.AddWithValue("@iban", iban);
            ExecuterRequeteEtVerifierImpact(command);
        }

        private void DebiterDe(int montantARetirer, string iban, SqlConnection cn)
        {
            var command = new SqlCommand("UPDATE [CompteEnBanque] SET [Solde]=[Solde]-@montantARetirer WHERE [IBAN]=@iban", cn);
            command.Parameters.AddWithValue("@montantARetirer", montantARetirer);
            command.Parameters.AddWithValue("@iban", iban);
            ExecuterRequeteEtVerifierImpact(command);
        }

        private static void ExecuterRequeteEtVerifierImpact(SqlCommand command)
        {
            var nombreDeComptesEnBanqueAffectes = command.ExecuteNonQuery();
            if (nombreDeComptesEnBanqueAffectes != 1)
                throw new BankAccountNotFoundException();
        }

        public SqlConnection GetDatabaseConnection()
        {
            return new SqlConnection(@"Data Source=(LocalDb)\MSSQLLOCALDB; Initial Catalog=TransactionsDemo;Integrated Security=True;");
        }

        public double ObtenirSolde(string iban)
        {
            using (SqlConnection connection = GetDatabaseConnection())
            {
                connection.Open();
                var command = new SqlCommand("SELECT [Solde] FROM [CompteEnBanque] WHERE [IBAN]=@iban", connection);
                command.Parameters.AddWithValue("@iban", iban);
                var solde = (decimal)command.ExecuteScalar();
                connection.Close();
                return Convert.ToDouble(solde);
            }
        }
    }
}
