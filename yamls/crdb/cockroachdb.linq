<Query Kind="Program">
  <NuGetReference>Npgsql</NuGetReference>
  <Namespace>Npgsql</Namespace>
  <Namespace>System.Net.Security</Namespace>
  <Namespace>System.Security.Cryptography.X509Certificates</Namespace>
</Query>

class MainClass
{
	static void Main(string[] args)
	{
		var connStringBuilder = new NpgsqlConnectionStringBuilder();
		connStringBuilder.Host = "172.42.42.100";
		// !!! Update the port exposed by CockroachDB for the internal port of 26257"
		// "kubectl get svc | grep 26257"
		connStringBuilder.Port = 31225;
		connStringBuilder.SslMode = SslMode.Disable;
		connStringBuilder.Username = "root";
		connStringBuilder.Database = "test";
		Simple(connStringBuilder.ConnectionString);
	}

	static void Simple(string connString)
	{
		// Prepare table and changefeed
		using (var conn = new NpgsqlConnection(connString))
		{
			conn.ProvideClientCertificatesCallback += ProvideClientCertificatesCallback;
			conn.UserCertificateValidationCallback += UserCertificateValidationCallback;
			conn.Open();

			// Drop table if exists
			new NpgsqlCommand("DROP TABLE IF EXISTS counter", conn).ExecuteNonQuery();
			// Create the counter table.
			new NpgsqlCommand("CREATE TABLE IF NOT EXISTS counter (id INT PRIMARY KEY, value INT)", conn).ExecuteNonQuery();
			// Create changesfeed
			//new NpgsqlCommand("CREATE CHANGEFEED FOR TABLE counter INTO 'kafka://stultified-snail-cp-kafka:9092'", conn).ExecuteNonQuery();
		}


		int counter = 0;
		while (true)
		{

			try
			{
				using (var conn = new NpgsqlConnection(connString))
				{
					conn.ProvideClientCertificatesCallback += ProvideClientCertificatesCallback;
					conn.UserCertificateValidationCallback += UserCertificateValidationCallback;
					conn.Open();
					using (var cmd = new NpgsqlCommand())
					{
						cmd.Connection = conn;
						cmd.CommandText = "UPSERT INTO counter(id, value) VALUES(@id1, @val1)";
						cmd.Parameters.AddWithValue("id1", ++counter);
						cmd.Parameters.AddWithValue("val1", 1000);
						cmd.ExecuteNonQuery();
						System.Console.WriteLine("Upserted counter: {0}", counter);

					}
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine("Ups, an exception was thrown: {0}", e.ToString() );
				System.Console.WriteLine("Retrying ...");
			}
			Thread.Sleep(1000);
		}

		// Print out the balances.
		//System.Console.WriteLine("Initial balances:");
		//using (var cmd = new NpgsqlCommand("SELECT id, balance FROM accounts", conn))
		//using (var cmd = new NpgsqlCommand("EXPERIMENTAL CHANGEFEED FOR bank.accounts", conn))
		//using (var reader = cmd.ExecuteReader())
		//	while (reader.Read())
		//		Console.Write("\taccount {0}: {1}\n", reader.GetValue(0), reader.GetValue(1));
	}
}

	static void ProvideClientCertificatesCallback(X509CertificateCollection clientCerts)
	{
		// To be able to add a certificate with a private key included, we must convert it to
		// a PKCS #12 format. The following openssl command does this:
		// openssl pkcs12 -password pass: -inkey client.maxroach.key -in client.maxroach.crt -export -out client.maxroach.pfx
		// As of 2018-12-10, you need to provide a password for this to work on macOS.
		// See https://github.com/dotnet/corefx/issues/24225

		// Note that the password used during X509 cert creation below
		// must match the password used in the openssl command above.
		clientCerts.Add(new X509Certificate2("client.maxroach.pfx", "pass"));
	}

	// By default, .Net does all of its certificate verification using the system certificate store.
	// This callback is necessary to validate the server certificate against a CA certificate file.
	static bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain defaultChain, SslPolicyErrors defaultErrors)
	{
		X509Certificate2 caCert = new X509Certificate2("ca.crt");
		X509Chain caCertChain = new X509Chain();
		caCertChain.ChainPolicy = new X509ChainPolicy()
		{
			RevocationMode = X509RevocationMode.NoCheck,
			RevocationFlag = X509RevocationFlag.EntireChain
		};
		caCertChain.ChainPolicy.ExtraStore.Add(caCert);

		X509Certificate2 serverCert = new X509Certificate2(certificate);

		caCertChain.Build(serverCert);
		if (caCertChain.ChainStatus.Length == 0)
		{
			// No errors
			return true;
		}

		foreach (X509ChainStatus status in caCertChain.ChainStatus)
		{
			// Check if we got any errors other than UntrustedRoot (which we will always get if we don't install the CA cert to the system store)
			if (status.Status != X509ChainStatusFlags.UntrustedRoot)
			{
				return false;
			}
		}
		return true;
	}