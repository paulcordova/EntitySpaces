using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdView : View
	{
		public FirebirdView()
		{

		}

		override public IViews SubViews 
		{ 
			get
			{
				if(!_subViewInfoLoaded)
				{
					LoadSubViewInfo();
				}
				return _views;				
			}
		}

		override public ITables SubTables
		{ 
			get
			{
				if(!_subViewInfoLoaded)
				{
					LoadSubViewInfo();
				}
				return _tables;
			}
		}

		private void LoadSubViewInfo()
		{
			_views  = (Views)this.dbRoot.ClassFactory.CreateViews();
			_tables = (Tables)this.dbRoot.ClassFactory.CreateTables();

			try
			{
				string select = @"SELECT TRIM(r.RDB$RELATION_NAME) AS view_name
								FROM RDB$VIEW_RELATIONS r
								WHERE r.RDB$VIEW_NAME = '" + this.Name + @"'
								ORDER BY r.RDB$RELATION_NAME;";
	
				OleDbConnection cn = new OleDbConnection(dbRoot.ConnectionString);
				cn.Open();
				cn.ChangeDatabase(this.Database.Name);

				OleDbDataAdapter adapter = new OleDbDataAdapter(select, cn);
				DataTable dataTable = new DataTable();

				adapter.Fill(dataTable);

				cn.Close();

				string entity = "";

				Table table;
				View view;

				foreach(DataRow row in dataTable.Rows)
				{
					entity = row["TABLE_NAME"] as string;

					// It might be a table or a view
					table = this.Database.Tables[entity] as Table;

					if(null != table)
					{
						// It's a table
						_tables.AddTable(table);
					}
					else
					{
						// Check for View
						view = this.Database.Views[entity] as View;

						if(null != view)
						{
							// It's a table
							_views.AddView(view);
						}
					}
				}
			}
			catch {}
		}
	}
}
